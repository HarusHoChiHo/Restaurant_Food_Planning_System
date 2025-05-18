use crate::AppState;
use crate::req_res_structs::unit::{CommonRequestUnit, CommonResponseUnit, DeleteResponseUnit};
use axum::extract::{Path, State};
use axum::http::StatusCode;
use axum::routing::{delete, get};
use axum::{Json, Router};
use entity::units::{ActiveModel, Entity as Units, Model as UnitsModel};
use sea_orm::{ActiveModelTrait, DeleteResult, EntityTrait, Set, TryIntoModel};
use utoipa::path as SwaggerAPIPath;
use utoipa_axum::router::OpenApiRouter;
use utoipa_axum::routes;

#[SwaggerAPIPath(
    get,
    path = "/",
    responses(
        (status=200, body=Vec<CommonResponseUnit>, description="Unit Object", example=json!({"id": 1, "name": "testing"})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[axum::debug_handler]
async fn read(
    State(state): State<AppState>,
) -> Result<Json<Vec<CommonResponseUnit>>, (StatusCode, String)> {
    let result = Units::find()
        .all(&state.env.database_connection)
        .await
        .map_err(|e| {
            eprintln!("Retrieving unit data error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?
        .iter()
        .map(|item| CommonResponseUnit {
            id: item.id,
            name: item.name.to_owned(),
        })
        .collect();

    Ok(Json(result))
}

#[SwaggerAPIPath(
    post,
    path = "/",
    request_body = CommonRequestUnit,
    responses(
        (status=200, body=CommonResponseUnit, description="Unit Object", example=json!({"id": 1, "name": "testing"})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[axum::debug_handler]
async fn creation(
    State(state): State<AppState>,
    Json(payload): Json<CommonRequestUnit>,
) -> Result<Json<CommonResponseUnit>, (StatusCode, String)> {
    let insert_result: UnitsModel = ActiveModel {
        name: Set(payload.name),
        ..Default::default()
    }
    .insert(&state.env.database_connection)
    .await
    .map_err(|e| {
        eprintln!("Database save error: {}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?
    .try_into_model()
    .map_err(|e| {
        eprintln!("Database save error: {}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    Ok(Json(CommonResponseUnit {
        id: insert_result.id,
        name: insert_result.name.to_owned(),
    }))
}

#[SwaggerAPIPath(
    put,
    path = "/",
    request_body = CommonRequestUnit,
    responses(
        (status=200, body=CommonResponseUnit, description="Unit Object", example=json!({"id": 1, "name": "testing"})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[axum::debug_handler]
async fn update(
    State(state): State<AppState>,
    Json(payload): Json<CommonRequestUnit>,
) -> Result<Json<CommonResponseUnit>, (StatusCode, String)> {
    if payload.id.is_none() {
        return Err((
            StatusCode::INTERNAL_SERVER_ERROR,
            "id cannot be null.".to_string(),
        ));
    }

    let active_model: ActiveModel = ActiveModel {
        id: Set(payload.id.unwrap()),
        name: Set(payload.name),
        ..Default::default()
    };

    let update_result: UnitsModel = active_model
        .save(&state.env.database_connection)
        .await
        .map_err(|e| {
            eprintln!("Database save error: {}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?
        .try_into_model()
        .map_err(|e| {
            eprintln!("Database convert error: {}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?;

    Ok(Json(CommonResponseUnit {
        id: update_result.id,
        name: update_result.name.to_owned(),
    }))
}

#[SwaggerAPIPath(
    delete,
    path = "/{id}",
    params(("id", Path, description = "The id of unit record")),
    responses(
        (status=200, body=CommonResponseUnit, description="Unit Object", example=json!({"rows": 1})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[axum::debug_handler]
async fn deletion(
    State(state): State<AppState>,
    Path(id): Path<i32>,
) -> Result<Json<DeleteResponseUnit>, (StatusCode, String)> {
    let result: DeleteResult = Units::delete_by_id(id)
        .exec(&state.env.database_connection)
        .await
        .unwrap();

    Ok(Json(DeleteResponseUnit {
        rows: result.rows_affected,
    }))
}

pub fn router() -> OpenApiRouter<AppState> {
    OpenApiRouter::new().routes(routes!(read, creation, update, deletion))
    // Router::new()
    //     .route("/", get(read).post(creation).put(update))
    //     .route("/{id}", delete(deletion))
}

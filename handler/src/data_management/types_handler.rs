use crate::AppState;
use crate::req_res_structs::types::{
    CommonRequestType, CommonResponseType, DeleteResponseType,
};
use axum::extract::{Path, State};
use axum::http::StatusCode;
use axum::{Json, debug_handler};
use entity::types::{ActiveModel as TypesActiveModel, Entity as TypesEntity, Model as TypesModel};
use sea_orm::{ActiveModelTrait, DeleteResult, EntityTrait, Set, TryIntoModel};
use tracing::{error, instrument};
use utoipa::path as SwaggerAPIPath;
use utoipa_axum::router::OpenApiRouter;
use utoipa_axum::routes;

#[SwaggerAPIPath(
    get,
    path = "/",
    tag = "Type Management",
    operation_id = "get_type",
    responses(
        (status=200, body=Vec<CommonResponseType>, description="Type Object", example=json!({"id": 1, "name": "testing"})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[instrument]
#[debug_handler]
async fn read(
    State(state): State<AppState>,
) -> Result<Json<Vec<CommonResponseType>>, (StatusCode, String)> {
    let result: Vec<CommonResponseType> = TypesEntity::find()
        .all(&state.env.database_connection)
        .await
        .map_err(|e| {
            error!("Retrieving types data error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?
        .iter()
        .map(|item| CommonResponseType {
            id: item.id,
            name: item.to_owned().name,
        })
        .collect();

    Ok(Json(result))
}

#[SwaggerAPIPath(
    post,
    path = "/",
    tag = "Type Management",
    operation_id = "create_type",
    request_body = CommonRequestType,
    responses(
        (status=200, body=CommonResponseType, description="Type Object", example=json!({"id": 1, "name": "testing"})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[instrument]
#[debug_handler]
async fn creation(
    State(state): State<AppState>,
    Json(payload): Json<CommonRequestType>,
) -> Result<Json<CommonResponseType>, (StatusCode, String)> {
    let types: TypesModel = TypesActiveModel {
        name: Set(payload.name),
        ..Default::default()
    }
    .insert(&state.env.database_connection)
    .await
    .map_err(|e| {
        error!("Saving types data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?
    .try_into_model()
    .map_err(|e| {
        error!("Converting types data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    Ok(Json(CommonResponseType {
        id: types.id,
        name: types.to_owned().name,
    }))
}

#[SwaggerAPIPath(
    put,
    path = "/",
    tag = "Type Management",
    operation_id = "update_type",
    request_body = CommonRequestType,
    responses(
        (status=200, body=CommonResponseType, description="Type Object", example=json!({"id": 1, "name": "testing"})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[instrument]
#[debug_handler]
async fn update(
    State(state): State<AppState>,
    Json(payload): Json<CommonRequestType>,
) -> Result<Json<CommonResponseType>, (StatusCode, String)> {
    if payload.id.is_none() {
        return Err((
            StatusCode::INTERNAL_SERVER_ERROR,
            "id cannot be null.".to_string(),
        ));
    }

    let result: TypesModel = TypesActiveModel {
        id: Set(payload.id.unwrap()),
        name: Set(payload.name),
        ..Default::default()
    }
    .save(&state.env.database_connection)
    .await
    .map_err(|e| {
        error!("Update types data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?
    .try_into_model()
    .map_err(|e| {
        error!("Converting types data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    Ok(Json(CommonResponseType {
        id: result.id,
        name: result.to_owned().name,
    }))
}

#[SwaggerAPIPath(
    delete,
    path = "/{id}",
    tag = "Type Management",
    operation_id = "delete_type",
    params(("id", Path, description = "The id of type record")),
    responses(
        (status=200, body=DeleteResponseType, description="Unit Object", example=json!({"rows": 1})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[instrument]
#[debug_handler]
async fn deletion(
    State(state): State<AppState>,
    Path(id): Path<i32>,
) -> Result<Json<DeleteResponseType>, (StatusCode, String)> {
    let result: DeleteResult = TypesEntity::delete_by_id(id)
        .exec(&state.env.database_connection)
        .await
        .map_err(|e| {
            error!("Deleting types data by id error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?;

    Ok(Json(DeleteResponseType {
        rows: result.rows_affected,
    }))
}

pub fn router() -> OpenApiRouter<AppState> {
    // Router::new()
    //     .route("/", get(read).post(creation).put(update))
    //     .route("/{id}", delete(deletion))
    OpenApiRouter::new().routes(routes!(read, creation, update, deletion))
}

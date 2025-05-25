use crate::AppState;
use crate::req_res_structs::food_item::{
    CommonRequestFoodItem, CommonResponseFoodItem, DeleteResponseFoodItem,
};
use crate::req_res_structs::types::CommonResponseType;
use crate::req_res_structs::unit::CommonResponseUnit;
use axum::extract::{Path, State};
use axum::http::StatusCode;
use axum::{Json, debug_handler};
use entity::food_item::{
    ActiveModel as FoodItemActiveModel, Entity as FoodItemEntity, Model as FoodItemModel,
};
use entity::types::{Entity as TypeEntity, Model as TypeModel};
use entity::units::{Entity as UnitEntity, Model as UnitModel};
use sea_orm::{ActiveModelTrait, DeleteResult, EntityTrait, ModelTrait, Set, TryIntoModel};
use tracing::{error, instrument};
use utoipa::path as SwaggerAPIPath;
use utoipa_axum::router::OpenApiRouter;
use utoipa_axum::routes;

#[SwaggerAPIPath(
    get,
    path = "/",
    tag = "Food Item Management",
    operation_id = "get_food_item",
    responses(
        (status=200, body=Vec<CommonResponseFoodItem>, description="Food Item Object", example=json!({"id": 1, "name": "testing", "quantity": 1, "type":{"id": 1, "name": "type"}, "unit":{"id": 1, "name": "unit"}})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[instrument]
#[debug_handler]
async fn read(
    State(state): State<AppState>,
) -> Result<Json<Vec<CommonResponseFoodItem>>, (StatusCode, String)> {
    let result: Vec<CommonResponseFoodItem> = FoodItemEntity::find()
        .find_also_related(TypeEntity)
        .find_also_related(UnitEntity)
        .all(&state.env.database_connection)
        .await
        .map_err(|e| {
            error!("Retrieving food item data error: {:?}", e);
            //eprintln!("Retrieving food item data error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?
        .iter()
        .map(|(food, type_opt, unit_opt)| {
            let type_data = type_opt.as_ref().ok_or_else(|| {
                error!("Missing type for food item ID {}", food.id);
                (
                    StatusCode::INTERNAL_SERVER_ERROR,
                    "Missing type".to_string(),
                )
            })?;

            let unit_data = unit_opt.as_ref().ok_or_else(|| {
                error!("Missing unit for food item ID {}", food.id);
                (
                    StatusCode::INTERNAL_SERVER_ERROR,
                    "Missing unit".to_string(),
                )
            })?;

            Ok(CommonResponseFoodItem {
                id: food.id,
                name: food.name.clone(),
                quantity: food.quantity.clone(),
                types: CommonResponseType {
                    id: type_data.id,
                    name: type_data.name.clone(),
                },
                units: CommonResponseUnit {
                    id: unit_data.id,
                    name: unit_data.name.clone(),
                },
            })
        })
        .collect::<Result<Vec<CommonResponseFoodItem>, (StatusCode, String)>>()?;

    Ok(Json(result))
}

#[SwaggerAPIPath(
    post,
    path = "/",
    tag = "Food Item Management",
    operation_id = "create_food_item",
    request_body = CommonRequestFoodItem,
    responses(
        (status=200, body=CommonResponseFoodItem, description="Food Item Object", example=json!({"id": 1, "name": "testing", "quantity": 1, "type":{"id": 1, "name": "type"}, "unit":{"id": 1, "name": "unit"}})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[instrument]
#[debug_handler]
async fn creation(
    State(state): State<AppState>,
    Json(payload): Json<CommonRequestFoodItem>,
) -> Result<Json<CommonResponseFoodItem>, (StatusCode, String)> {
    let result = FoodItemActiveModel {
        name: Set(payload.name),
        quantity: Set(payload.quantity),
        type_id: Set(payload.type_id),
        unit_id: Set(payload.unit_id),
        ..Default::default()
    }
    .insert(&state.env.database_connection)
    .await
    .map_err(|e| {
        error!("Creating food item data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?
    .try_into_model()
    .map_err(|e| {
        error!("Converting food item data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    let types_model: TypeModel = result
        .find_related(TypeEntity)
        .one(&state.env.database_connection)
        .await
        .map_err(|e| {
            error!("Find type related to food item data error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?
        .ok_or_else(|| {
            error!("No related type found for food item ID {}", result.id);
            (StatusCode::NOT_FOUND, "Type not found".to_string())
        })?
        .to_owned();

    let units_model: UnitModel = result
        .find_related(UnitEntity)
        .one(&state.env.database_connection)
        .await
        .map_err(|e| {
            error!("Find unit related to food item data error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?
        .ok_or_else(|| {
            error!("No related unit found for food item ID {}", result.id);
            (StatusCode::NOT_FOUND, "Type not found".to_string())
        })?
        .to_owned();

    let response = CommonResponseFoodItem {
        id: result.id,
        name: result.name,
        quantity: result.quantity,
        types: CommonResponseType {
            id: types_model.id,
            name: types_model.name,
        },
        units: CommonResponseUnit {
            id: units_model.id,
            name: units_model.name,
        },
    };

    Ok(Json(response))
}

#[SwaggerAPIPath(
    put,
    path = "/",
    tag = "Food Item Management",
    operation_id = "update_food_item",
    request_body = CommonRequestFoodItem,
    responses(
        (status=200, body=CommonResponseFoodItem, description="Food Item Object", example=json!({"id": 1, "name": "testing", "quantity": 1, "type":{"id": 1, "name": "type"}, "unit":{"id": 1, "name": "unit"}})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[instrument]
#[debug_handler]
async fn update(
    State(state): State<AppState>,
    Json(payload): Json<CommonRequestFoodItem>,
) -> Result<Json<CommonResponseFoodItem>, (StatusCode, String)> {
    let id = payload.id.ok_or_else(|| {
        error!("ID cannot be null.");
        (
            StatusCode::INTERNAL_SERVER_ERROR,
            "ID cannot be null.".to_string(),
        )
    })?;

    let result: FoodItemModel = FoodItemActiveModel {
        id: Set(id),
        name: Set(payload.name),
        quantity: Set(payload.quantity),
        type_id: Set(payload.type_id),
        unit_id: Set(payload.unit_id),
        ..Default::default()
    }
    .save(&state.env.database_connection)
    .await
    .map_err(|e| {
        error!("Updating food item data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?
    .try_into_model()
    .map_err(|e| {
        error!("Converting food item data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    let types_model: TypeModel = result
        .find_related(TypeEntity)
        .one(&state.env.database_connection)
        .await
        .map_err(|e| {
            error!("Find type related to food item data error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?
        .ok_or_else(|| {
            error!("No related type found for food item ID {}", result.id);
            (
                StatusCode::INTERNAL_SERVER_ERROR,
                "Type not found".to_string(),
            )
        })?
        .to_owned();

    let units_model: UnitModel = result
        .find_related(UnitEntity)
        .one(&state.env.database_connection)
        .await
        .map_err(|e| {
            error!("Find unit related to food item data error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?
        .ok_or_else(|| {
            error!("No related unit found for food item ID {}", result.id);
            (
                StatusCode::INTERNAL_SERVER_ERROR,
                "Unit not found".to_string(),
            )
        })?
        .to_owned();

    let response = CommonResponseFoodItem {
        id: result.id,
        name: result.name,
        quantity: result.quantity,
        types: CommonResponseType {
            id: types_model.id,
            name: types_model.name.clone(),
        },
        units: CommonResponseUnit {
            id: units_model.id,
            name: units_model.name.clone(),
        },
    };

    Ok(Json(response))
}

#[SwaggerAPIPath(
    delete,
    path = "/{id}",
    tag = "Food Item Management",
    operation_id = "delete_food_item",
    params(("id", Path, description = "The id of food item record")),
    responses(
        (status=200, body=DeleteResponseFoodItem, description="Unit Object", example=json!({"rows": 1})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[instrument]
#[debug_handler]
async fn deletion(
    State(state): State<AppState>,
    Path(id): Path<i32>,
) -> Result<Json<DeleteResponseFoodItem>, (StatusCode, String)> {
    let result: DeleteResult = FoodItemEntity::delete_by_id(id)
        .exec(&state.env.database_connection)
        .await
        .map_err(|e| {
            error!("Deleting food item data error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?;

    Ok(Json(DeleteResponseFoodItem {
        rows: result.rows_affected,
    }))
}

pub fn router() -> OpenApiRouter<AppState> {
    // Router::new()
    //     .route("/", get(read).post(creation).put(update))
    //     .route("/{id}", delete(deletion))

    OpenApiRouter::new().routes(routes!(read, creation, update, deletion))
}

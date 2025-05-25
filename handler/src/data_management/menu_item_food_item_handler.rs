use crate::AppState;
use crate::req_res_structs::food_item::FoodItemModel;
use crate::req_res_structs::menu_item::CommonResponseMi;
use crate::req_res_structs::menu_item_food_item::{
    CommonRequestMiFi, CommonResponseMiFi, DeleteResponseMiFi,
};
use axum::extract::{Path, State};
use axum::http::StatusCode;
use axum::{Json, debug_handler};
use entity::food_item::Entity as FoodItemEntity;
use entity::menu_item::Entity as MenuItemEntity;
use entity::menu_item_food_item::{
    ActiveModel as MenuItemFoodItemActiveModel, Entity as MenuItemFoodItemEntity,
    Model as MenuItemFoodItemModel,
};
use sea_orm::{ActiveModelTrait, DeleteResult, EntityTrait, Set, TryIntoModel};
use tracing::{error, instrument};
use utoipa::path as SwaggerAPIPath;
use utoipa_axum::router::OpenApiRouter;
use utoipa_axum::routes;

#[SwaggerAPIPath(
    get,
    path = "/",
    tag = "Menu & Food Item Management",
    operation_id = "get_menu_item_food_item",
    responses(
        (status=200, body=Vec<CommonResponseMiFi>, description="Menu & Food Item Object", example=json!({"menu_item_id": 1, "food_item_id": 1, "consumption": 1})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[instrument]
#[debug_handler]
async fn read(
    State(state): State<AppState>,
) -> Result<Json<Vec<CommonResponseMiFi>>, (StatusCode, String)> {
    let result = MenuItemFoodItemEntity::find()
        .find_also_related(MenuItemEntity)
        .find_also_related(FoodItemEntity)
        .all(&state.env.database_connection)
        .await
        .map_err(|e| {
            error!("Retrieving menu item food item data error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?
        .iter()
        .map(|(mifi, mi, fi)| {
            let mi_opt = mi.as_ref().ok_or_else(|| {
                error!(
                    "No menu item found for id of food item: {:?} and id of menu item: {:?}",
                    mifi.food_item_id, mifi.menu_item_id
                );
                (
                    StatusCode::INTERNAL_SERVER_ERROR,
                    "No menu item found".to_string(),
                )
            })?;

            let fi_opt = fi.as_ref().ok_or_else(|| {
                error!(
                    "No food item found for id of food item: {:?} and id of menu item: {:?}",
                    mifi.food_item_id, mifi.menu_item_id
                );
                (
                    StatusCode::INTERNAL_SERVER_ERROR,
                    "No food item found".to_string(),
                )
            })?;

            Ok(CommonResponseMiFi {
                menu_item: CommonResponseMi {
                    id: mi_opt.id,
                    name: mi_opt.name.clone(),
                },
                food_item: FoodItemModel {
                    id: fi_opt.id,
                    name: fi_opt.name.clone(),
                    quantity: fi_opt.quantity,
                    types: fi_opt.type_id,
                    units: fi_opt.unit_id,
                },
                consumption: mifi.consumption,
            })
        })
        .collect::<Result<Vec<CommonResponseMiFi>, (StatusCode, String)>>()?;

    Ok(Json(result))
}

#[SwaggerAPIPath(
    post,
    path = "/",
    tag = "Menu & Food Item Management",
    operation_id = "create_menu_item_food_item",
    request_body = CommonRequestMiFi,
    responses(
        (status=200, body=CommonResponseMiFi, description="Menu & Food Item Object", example=json!({"menu_item_id": 1, "food_item_id": 1, "consumption": 1})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[instrument]
#[debug_handler]
pub async fn creation(
    State(state): State<AppState>,
    Json(payload): Json<CommonRequestMiFi>,
) -> Result<Json<CommonResponseMiFi>, (StatusCode, String)> {
    let result: MenuItemFoodItemModel = MenuItemFoodItemActiveModel {
        consumption: Set(payload.consumption),
        menu_item_id: Set(payload.menu_item_id),
        food_item_id: Set(payload.food_item_id),
        ..Default::default()
    }
    .insert(&state.env.database_connection)
    .await
    .map_err(|e| {
        error!("Creating menu item food item data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?
    .try_into_model()
    .map_err(|e| {
        error!("Converting menu item food item data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    let (mifi, fi, mi) =
        MenuItemFoodItemEntity::find_by_id((result.menu_item_id, result.food_item_id))
            .find_also_related(FoodItemEntity)
            .find_also_related(MenuItemEntity)
            .one(&state.env.database_connection)
            .await
            .map_err(|e| {
                error!("Retrieving menu item food item data error: {:?}", e);
                (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
            })?
            .ok_or_else(|| {
                error!(
                    "No menu item food item data found: food_item_id: {:?}, menu_item_id: {:?}",
                    result.food_item_id, result.menu_item_id
                );
                (
                    StatusCode::INTERNAL_SERVER_ERROR,
                    "No menu item food item data found".to_string(),
                )
            })?;

    let fi_data = fi.ok_or_else(|| {
        error!(
            "No food item data found: food_item_id: {:?}",
            result.food_item_id
        );
        (
            StatusCode::INTERNAL_SERVER_ERROR,
            "No food item data found".to_string(),
        )
    })?;

    let mi_data = mi.ok_or_else(|| {
        error!(
            "No menu item data found: menu_item_id: {:?}",
            result.menu_item_id
        );
        (
            StatusCode::INTERNAL_SERVER_ERROR,
            "No menu item data found".to_string(),
        )
    })?;

    Ok(Json(CommonResponseMiFi {
        food_item: FoodItemModel {
            id: fi_data.id,
            quantity: fi_data.quantity,
            name: fi_data.name,
            types: fi_data.type_id,
            units: fi_data.unit_id,
        },
        menu_item: CommonResponseMi {
            id: mi_data.id,
            name: mi_data.name,
        },
        consumption: mifi.consumption,
    }))
}

#[SwaggerAPIPath(
    put,
    path = "/",
    tag = "Menu & Food Item Management",
    operation_id = "update_menu_item_food_item",
    request_body = CommonRequestMiFi,
    responses(
        (status=200, body=CommonResponseMiFi, description="Menu & Food Item Object", example=json!({"menu_item_id": 1, "food_item_id": 1, "consumption": 1})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[instrument]
#[debug_handler]
pub async fn update(
    State(state): State<AppState>,
    Json(payload): Json<CommonRequestMiFi>,
) -> Result<Json<CommonResponseMiFi>, (StatusCode, String)> {
    let result: MenuItemFoodItemActiveModel = MenuItemFoodItemActiveModel {
        menu_item_id: Set(payload.menu_item_id),
        food_item_id: Set(payload.food_item_id),
        consumption: Set(payload.consumption),
    }
    .save(&state.env.database_connection)
    .await
    .map_err(|e| {
        error!("Updating menu item food item data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    let (mifi, fi, mi) =
        MenuItemFoodItemEntity::find_by_id((payload.food_item_id, payload.menu_item_id))
            .find_also_related(FoodItemEntity)
            .find_also_related(MenuItemEntity)
            .one(&state.env.database_connection)
            .await
            .map_err(|e| {
                error!("Retrieving menu item food item data error: {:?}", e);
                (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
            })?
            .unwrap();

    let fi_data = fi.ok_or_else(|| {
        error!(
            "No food item data found: food_item_id: {:?}",
            result.food_item_id
        );
        (
            StatusCode::INTERNAL_SERVER_ERROR,
            "No food item data found".to_string(),
        )
    })?;

    let mi_data = mi.ok_or_else(|| {
        error!(
            "No menu item data found: menu_item_id: {:?}",
            result.menu_item_id
        );
        (
            StatusCode::INTERNAL_SERVER_ERROR,
            "No menu item data found".to_string(),
        )
    })?;

    Ok(Json(CommonResponseMiFi {
        consumption: mifi.consumption,
        menu_item: CommonResponseMi {
            id: mi_data.id,
            name: mi_data.name,
        },
        food_item: FoodItemModel {
            id: fi_data.id,
            name: fi_data.name,
            quantity: fi_data.quantity,
            types: fi_data.type_id,
            units: fi_data.unit_id,
        },
    }))
}

#[SwaggerAPIPath(
    delete,
    path = "/{id}",
    tag = "Menu & Food Item Management",
    operation_id = "delete_menu_item_food_item",
    params(("id", Path, description = "The id of menu item food item record")),
    responses(
        (status=200, body=DeleteResponseMiFi, description="Menu & Food Item Object", example=json!({"rows": 1})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[instrument]
#[debug_handler]
pub async fn deletion(
    State(state): State<AppState>,
    Path((menu_item_id, food_item_id)): Path<(i32, i32)>,
) -> Result<Json<DeleteResponseMiFi>, (StatusCode, String)> {
    let result: DeleteResult = MenuItemFoodItemEntity::delete_by_id((menu_item_id, food_item_id))
        .exec(&state.env.database_connection)
        .await
        .map_err(|e| {
            error!("Deleting menu item food item data error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?;

    Ok(Json(DeleteResponseMiFi {
        rows: result.rows_affected,
    }))
}

pub fn router() -> OpenApiRouter<AppState> {
    // Router::new()
    //     .route("/", get(read).post(creation).put(update))
    //     .route("/{mid}/{fid}", delete(deletion))

    OpenApiRouter::new().routes(routes!(read, creation, update, deletion))
}

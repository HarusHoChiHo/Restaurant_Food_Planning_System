use axum::{serve};
use handler::data_management::{food_item_handler, menu_handler, menu_item_food_item_handler, menu_item_handler, order_handler, order_item_handler, types_handler, units_handler};
use tokio::net::TcpListener;
use utoipa_axum::router::OpenApiRouter;
use utoipa_swagger_ui::SwaggerUi;



#[tokio::main]
async fn main() {
    tracing_subscriber::fmt()
        .with_max_level(tracing::Level::DEBUG)
        .with_test_writer()
        .init();

    let (router, api) = OpenApiRouter::new()
        .nest("/unit", units_handler::router())
        .nest("/type", types_handler::router())
        .nest(
            "/food_item",
            food_item_handler::router(),
        )
        .nest(
            "/menu_item_food_item",
            menu_item_food_item_handler::router(),
        )
        .nest(
            "/menu_item",
           menu_item_handler::router(),
        )
        .nest("/order", order_handler::router())
        .nest("/menu", menu_handler::router())
        .nest("/order_item", order_item_handler::router())
        .with_state(handler::init_db_con().await)
        .split_for_parts();

    let app = router
        .merge(SwaggerUi::new("/swagger-ui").url("/api/openapi.json", api))
        .into_make_service();

    let listener = TcpListener::bind("0.0.0.0:8000").await.unwrap();
    serve(listener, app).await.unwrap();
}

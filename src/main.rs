use axum::serve;
use handler::data_management::{
    food_item_handler, menu_handler, menu_item_food_item_handler, menu_item_handler, order_handler,
    order_item_handler, types_handler, units_handler,
};
use tokio::net::TcpListener;
use tracing::{info, instrument};
use utoipa::openapi::{ContactBuilder};
use utoipa_axum::router::OpenApiRouter;
use utoipa_swagger_ui::SwaggerUi;
use handler::health::health_check_handler;

#[tokio::main]
#[instrument]
async fn main() {
    tracing_subscriber::fmt()
        .with_max_level(tracing::Level::DEBUG)
        .with_test_writer()
        .init();

    let (router, mut api) = OpenApiRouter::new()
        .nest("/unit", units_handler::router())
        .nest("/type", types_handler::router())
        .nest("/food_item", food_item_handler::router())
        .nest(
            "/menu_item_food_item",
            menu_item_food_item_handler::router(),
        )
        .nest("/menu_item", menu_item_handler::router())
        .nest("/order", order_handler::router())
        .nest("/menu", menu_handler::router())
        .nest("/order_item", order_item_handler::router())
        .nest("/health", health_check_handler::router())
        .with_state(handler::init_db_con().await)
        .split_for_parts();

    api.info.title = "Restaurant Food Planning System".to_string();
    let contact = ContactBuilder::new()
        .name(Some("Harus Ho".to_string()))
        .email(Some("harusho@singlebit.slmail.me".to_string()));
    api.info.contact = Option::from(contact.build());
    api.info.version = "1.0.0".to_string();
    api.info.description = Some("This is an example of backend application built in Rust".to_string());

    let app = router
        .merge(SwaggerUi::new("/swagger-ui").url("/api/openapi.json", api))
        .into_make_service();
    
    info!("Server is running.");
    
    let listener = TcpListener::bind("0.0.0.0:8000").await.unwrap();
    serve(listener, app).await.unwrap();
}

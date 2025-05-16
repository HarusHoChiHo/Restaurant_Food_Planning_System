use axum::{serve, Router};
use tokio::net::TcpListener;
use handler::data_management::{types_handler, units_handler, food_item_handler, menu_item_food_item_handler, menu_item_handler, menu_handler, order_item_handler, order_handler};

#[tokio::main]
async fn main() {
    let app = Router::new()
        .nest("/unit", units_handler::router())
        .nest("/type", types_handler::router())
        .nest("/food_item", food_item_handler::router())
        // .nest("/menu_item_food_item", menu_item_food_item_handler::router())
        // .nest("/menu_item", menu_item_handler::router())
        // .nest("/menu", menu_handler::router())
        // .nest("/order_item", order_item_handler::router())
        // .nest("/order", order_handler::router())
        .with_state(handler::init_db_con().await);

    let listener = TcpListener::bind("0.0.0.0:8000").await.unwrap();
    serve(listener, app).await.unwrap();
    
    tracing_subscriber::fmt()
        .with_max_level(tracing::Level::DEBUG)
        .with_test_writer()
        .init();
    
}

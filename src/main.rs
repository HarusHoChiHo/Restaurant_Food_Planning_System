use axum::{serve, Router};
use tokio::net::TcpListener;
use handler::data_management::{types_handler, units_handler};

#[tokio::main]
async fn main() {
    let app = Router::new()
        .nest("/unit", units_handler::router())
        .nest("/type", types_handler::router());

    let listener = TcpListener::bind("0.0.0.0:8000").await.unwrap();
    serve(listener, app).await.unwrap();
    
    tracing_subscriber::fmt()
        .with_max_level(tracing::Level::DEBUG)
        .with_test_writer()
        .init();
    
}

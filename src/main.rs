use axum::{serve, Router};
use tokio::net::TcpListener;



#[tokio::main]
async fn main() {
    let app = Router::new().nest("/unit", handler::data_management::units_handler::router());

    let listener = TcpListener::bind("0.0.0.0:8000").await.unwrap();
    serve(listener, app).await.unwrap();
    
    tracing_subscriber::fmt()
        .with_max_level(tracing::Level::DEBUG)
        .with_test_writer()
        .init();
    
}

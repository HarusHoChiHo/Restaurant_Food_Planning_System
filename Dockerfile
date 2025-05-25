FROM rust:latest

WORKDIR /usr/src/RFPS-Rust
COPY . .
RUN cargo install --path .

EXPOSE 8000

CMD ["RFPS-Rust"]
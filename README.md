# E-Commerce API

REST API for an e-commerce platform built with Clean Architecture.

## Tech Stack

- ASP.NET Core 8
- PostgreSQL + Entity Framework Core
- Redis (cart storage)
- Docker + Docker Compose
- JWT + Refresh Token authentication
- Role-based access control (Admin, Seller, Delivery, User)
- xUnit + Moq (unit tests)

## Getting Started

### Prerequisites
- Docker Desktop

### Run the project

git clone https://github.com/abjorayev/E-Commerce.git
cd E-Commerce
docker-compose up --build

API will be available at: http://localhost:5000/swagger

### Default Admin credentials
- Username: admin
- Password: Admin123!

## Architecture

The project follows Clean Architecture principles:

- E-Commerce.Domain — Entities, Enums
- E-Commerce.Application — Services, DTOs, Business logic
- E-Commerce.Repository — Generic repository, IQueryable pattern
- ECommerce.Infrastructure — DbContext, Migrations
- E-Commerce — Controllers, Middleware, DI configuration
- E-Commerce.Tests — Unit tests (xUnit + Moq)

## API Endpoints

### Auth
- POST /api/auth/register
- POST /api/auth/login
- POST /api/auth/refresh
- POST /api/auth/revoke

### Products
- GET /api/products — with pagination and filters
- POST /api/products — Admin, Seller only
- PUT /api/products/{id} — Admin, Seller only
- DELETE /api/products/{id} — Admin only

### Cart
- GET /api/cart
- POST /api/cart
- PUT /api/cart/increase
- PUT /api/cart/decrement
- DELETE /api/cart/{productId}

### Orders
- POST /api/orders
- GET /api/orders
- GET /api/orders/{id}
- PUT /api/orders/{id}/status — Admin, Delivery only

### Roles
- POST /api/roles/create — Admin only
- POST /api/roles/assign — Admin only

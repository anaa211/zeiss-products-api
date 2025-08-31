# Products API

A simple **.NET Web API** for managing products, categories, and stock operations.  
Built with **ASP.NET Core**, **Entity Framework Core**, and tested using **xUnit + Moq**.

---

## Features

- CRUD operations for **Products**
- Manage product stock (increment/decrement)
- **Validation** for input data
- **Seeded categories** to ensure valid `CategoryId`s
- Centralized **exception handling middleware**
- **Logging** for errors, stock changes, and product deletions
- Unit tests for managers and controllers

---

## Tech Stack

- **Backend:** ASP.NET Core Web API  
- **Database:** SQL Server (EF Core Migrations)  
- **ORM:** Entity Framework Core  
- **Testing:** xUnit, Moq, InMemoryDatabase  
- **Mapping:** AutoMapper  

---

## Database & Seed Data

The database is seeded with categories during migration:

| Id | Name             | Description                                  |
|----|------------------|----------------------------------------------|
| 1  | Kitchenware      | Items used in kitchens, e.g., utensils, pans |
| 2  | Clothing         | Apparel for men, women, and children         |
| 3  | Books            | Fiction, non-fiction, educational, and more  |
| 4  | Home Appliances  | Household appliances like fridges, mixers    |
| 5  | Electronics      | Gadgets and electronic devices               |

This ensures products can only be created with valid `CategoryId`s.

---

## API Endpoints

### Products
- **GET** `/api/products` -> Get all products
- **GET** `/api/products/{id}` -> Get product by Id
- **POST** `/api/products` -> Add product(s)
- **PUT** `/api/products/{id}` -> Update product
- **DELETE** `/api/products/{id}` -> Delete product

### Stock Management
- **PUT** `/api/products/decrement-stock/{id}/{quantity}` -> Decrement stock
- **PUT** `/api/products/add-to-stock/{id}/{quantity}` -> Increment stock

---

## Validation Rules

- **Name**: required, max 100 chars  
- **Description**: optional, max 500 chars  
- **CategoryId**: must exist in DB  
- **Stock**: must be non-negative  
- **Price**: must be greater than 0  

---

## Error Handling

- **400 Bad Request** -> Invalid input or validation error  
- **404 Not Found** -> Product/Category not found  
- **500 Internal Server Error** -> Unexpected exception  

Error response structure:

```json
{
  "statusCode": 400,
  "message": "Validation failed.",
  "errors": ["Product name cannot exceed 100 characters."]
}
```

---

## Logging

- Errors logged with exception details
- Information logs for:
	- Stock updates (increment/decrement)
	- Product deletions


---

## Testing

- Written with xUnit + Moq
- Covers:
	- Product CRUD operations
	- Stock management
	- Validation errors
	- Exception handling
	- Logger calls (e.g., stock changes, errors)
- Uses InMemoryDatabase to isolate tests from the real DB
- Mocked ILogger<T> with custom VerifyLog extension to validate logging


---

## Future Improvements

- Authentication & Authorization (JWT)
- Role-based product management (admin vs user)
- Caching for product queries
- CI/CD pipeline for automated builds & tests
- Docker support


---

## How To Run

- git clone https://github.com/anaa211/zeiss-products-api.git
- cd zeiss-products-api
- dotnet restore
- dotnet ef database update
- dotnet run --project src/Products.Api
- https://localhost:7148/swagger



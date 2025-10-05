# ProductsFunctions (Azure Functions) Documentation

## OpenAPI Specification (Summary)

**Base Path:** `/products`
**Consumes:** `application/json`
**Produces:** `application/json`

### Functions

#### POST `/products`
- **Summary:** Create a new product
- **Request Body:**
	- `name` (string, required)
	- `price` (decimal, required)
	- `storeId` (Guid, required)
- **Responses:**
	- `201 Created`: Returns `ProductResponseDto`
	- `400 Bad Request`: Validation error

#### GET `/products`
- **Summary:** Get all products
- **Responses:**
	- `200 OK`: Returns array of `ProductResponseDto`

#### GET `/products/{id}`
- **Summary:** Get a product by ID
- **Parameters:**
	- `id` (Guid, path, required)
- **Responses:**
	- `200 OK`: Returns `ProductResponseDto`
	- `404 Not Found`: Product not found

## Overview
Azure Functions for product management.

## Functions

### CreateProduct
- **Trigger:** HTTP POST `/products`
- **Authorization:** Anonymous
- **Request Body:** `CreateProductRequestDto`
- **Response:** `201 Created` or error

### GetProducts
- **Trigger:** HTTP GET `/products`
- **Authorization:** Anonymous
- **Response:** `200 OK`, returns list of products

### GetProduct
- **Trigger:** HTTP GET `/products/{id}`
- **Authorization:** Anonymous
- **Route Parameter:** `id`
- **Response:** `200 OK`, returns product details

## Data Contracts
- `CreateProductRequestDto`: See `src/Functions/Dtos/CreateProductRequestDto.cs`
- `ProductResponseDto`: See `src/Functions/Dtos/ProductResponseDto.cs`
- `UpdateProductRequestDto`: See `src/Functions/Dtos/UpdateProductRequestDto.cs`

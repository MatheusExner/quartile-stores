# StoreController API Documentation

## OpenAPI Specification (Summary)

**Base Path:** `/api/v1/Store`
**Consumes:** `application/json`
**Produces:** `application/json`

### Endpoints

#### POST `/api/v1/Store`
- **Summary:** Create a new store
- **Request Body:**
	- `name` (string, required)
	- `companyId` (Guid, required)
	- `address` (string, required)
- **Responses:**
	- `201 Created`: Returns `StoreDto`
	- `400 Bad Request`: Validation error

#### PUT `/api/v1/Store/{id}`
- **Summary:** Update an existing store
- **Parameters:**
	- `id` (Guid, path, required)
- **Request Body:**
	- `name` (string, required)
	- `address` (string, required)
- **Responses:**
	- `200 OK`: Returns `StoreDto`
	- `404 Not Found`: Store not found

#### GET `/api/v1/Store/{id}`
- **Summary:** Get a store by ID
- **Parameters:**
	- `id` (Guid, path, required)
- **Responses:**
	- `200 OK`: Returns `DetailedStoreDto`
	- `404 Not Found`: Store not found

#### GET `/api/v1/Store`
- **Summary:** Get all stores
- **Responses:**
	- `200 OK`: Returns array of `StoreDto`

#### DELETE `/api/v1/Store/{id}`
- **Summary:** Delete a store by ID
- **Parameters:**
	- `id` (Guid, path, required)
- **Responses:**
	- `204 No Content`: Deleted
	- `404 Not Found`: Store not found

## Overview
Handles CRUD operations for stores.

## Endpoints

### POST `/api/v1/Store`
- **Description:** Creates a new store.
- **Request Body:** `CreateStoreCommand`
- **Response:** `201 Created`, returns `StoreDto`

### PUT `/api/v1/Store/{id}`
- **Description:** Updates an existing store.
- **Route Parameter:** `id` (Guid)
- **Request Body:** `UpdateStoreCommand`
- **Response:** `200 OK`, returns `StoreDto`

### GET `/api/v1/Store/{id}`
- **Description:** Gets a store by its ID.
- **Route Parameter:** `id` (Guid)
- **Response:** `200 OK`, returns `DetailedStoreDto`

### GET `/api/v1/Store`
- **Description:** Gets all stores.
- **Response:** `200 OK`, returns `IEnumerable<StoreDto>`

### DELETE `/api/v1/Store/{id}`
- **Description:** Deletes a store by its ID.
- **Route Parameter:** `id` (Guid)
- **Response:** `204 No Content`

## Data Contracts
- `StoreDto`: See `src/Application/Dtos/StoreDto.cs`
- `DetailedStoreDto`: See `src/Application/Dtos/DetailedStoreDto.cs`
- `CreateStoreCommand`: See `src/Application/Features/Stores/CreateStore/CreateStoreCommand.cs`
- `UpdateStoreCommand`: See `src/Application/Features/Stores/UpdateStore/UpdateStoreCommand.cs`

# CompanyController API Documentation

## OpenAPI Specification (Summary)

**Base Path:** `/api/v1/Company`
**Consumes:** `application/json`
**Produces:** `application/json`

### Endpoints

#### POST `/api/v1/Company`
- **Summary:** Create a new company
- **Request Body:**
	- `name` (string, required)
	- `country` (string, required)
- **Responses:**
	- `201 Created`: Returns `CompanyDto`
	- `400 Bad Request`: Validation error

#### PUT `/api/v1/Company/{id}`
- **Summary:** Update an existing company
- **Parameters:**
	- `id` (Guid, path, required)
- **Request Body:**
	- `name` (string, required)
	- `country` (string, required)
- **Responses:**
	- `200 OK`: Returns `CompanyDto`
	- `404 Not Found`: Company not found

#### GET `/api/v1/Company/{id}`
- **Summary:** Get a company by ID
- **Parameters:**
	- `id` (Guid, path, required)
- **Responses:**
	- `200 OK`: Returns `CompanyDto`
	- `404 Not Found`: Company not found

#### GET `/api/v1/Company`
- **Summary:** Get all companies
- **Responses:**
	- `200 OK`: Returns array of `CompanyDto`

#### DELETE `/api/v1/Company/{id}`
- **Summary:** Delete a company by ID
- **Parameters:**
	- `id` (Guid, path, required)
- **Responses:**
	- `204 No Content`: Deleted
	- `404 Not Found`: Company not found

## Overview
Handles CRUD operations for companies.

## Endpoints

### POST `/api/v1/Company`
- **Description:** Creates a new company.
- **Request Body:** `CreateCompanyCommand`
- **Response:** `201 Created`, returns `CompanyDto`

### PUT `/api/v1/Company/{id}`
- **Description:** Updates an existing company.
- **Route Parameter:** `id` (Guid)
- **Request Body:** `UpdateCompanyCommand`
- **Response:** `200 OK`, returns `CompanyDto`

### GET `/api/v1/Company/{id}`
- **Description:** Gets a company by its ID.
- **Route Parameter:** `id` (Guid)
- **Response:** `200 OK`, returns `CompanyDto`

### GET `/api/v1/Company`
- **Description:** Gets all companies.
- **Response:** `200 OK`, returns `IEnumerable<CompanyDto>`

### DELETE `/api/v1/Company/{id}`
- **Description:** Deletes a company by its ID.
- **Route Parameter:** `id` (Guid)
- **Response:** `204 No Content`

## Data Contracts
- `CompanyDto`: See `src/Application/Dtos/CompanyDto.cs`
- `CreateCompanyCommand`: See `src/Application/Features/Companies/CreateCompany/CreateCompanyCommand.cs`
- `UpdateCompanyCommand`: See `src/Application/Features/Companies/UpdateCompany/UpdateCompanyCommand.cs`

# Distributed Logging System

## Overview
The Distributed Logging System is a unified application designed to handle log entries for a distributed system. It provides APIs to store and retrieve log entries across multiple backends, such as:

1. Amazon S3-Compatible Storage 
2. Database Table
3. Local File System
4. Message Queue 

The system also includes an Angular front-end to enable efficient log management and visualization.

---

## Features

### Back-End (API Service)
- **Store Log Entries:**
  - **Endpoint:** `POST /v1/logs`
  - **Request Format:**
    ```json
    {
        "service": "name_of_the_service",
        "level": "info | warning | error",
        "message": "The log message",
        "timestamp": "2023-01-22T21:37:55Z"
    }
    ```
  - Validates fields and dynamically selects the storage backend.

- **Retrieve Log Entries:**
  - **Endpoint:** `GET /v1/logs`
  - **Query Parameters:**
    - `service` (optional)
    - `level` (optional)
    - `start_time` (optional)
    - `end_time` (optional)
  - **Response Format:**
    ```json
    [
        {
            "service": "name_of_the_service",
            "level": "info",
            "message": "The log message",
            "timestamp": "2023-01-22T21:37:55Z"
        }
    ]
    ```

- **Authentication:**
  - Bearer token authentication secures all API endpoints.

- **Dynamic Backend Configuration:**
  - Seamlessly switch between storage backends (e.g., S3, database, local file system).

- **Metadata Tracking:**
  - Store and query metadata such as `service`, `level`, `timestamp`, and backend type.

---

### Front-End (Angular Application)
- **Log List View:**
  - Displays log entries in a tabular format.
  - Columns include `service`, `level`, `message`, and `timestamp`.

- **Filtering Options:**
  - Filters logs by `service`, `level`, `start_time`, and `end_time`.

- **Details View:**
  - Shows detailed information for individual log entries.

- **Pagination:**
  - Handles large log datasets efficiently.

- **Error Handling:**
  - Displays user-friendly error messages for failed requests or invalid inputs.

---

## Project Structure

### Back-End
- **Endpoints:**
  - `POST /v1/logs`: Stores logs.
  - `GET /v1/logs`: Retrieves logs.
- **Backends Supported:**
  1. **Amazon S3-Compatible Storage:** Uses HTTP client for integration.
  2. **Database Storage:** Scalable and ensures data integrity.
  3. **Local File System:** Configurable path for log storage.
  4. **Message Queue (Optional):** Allows distributed processing of logs.

### Front-End
- Built with Angular.
- Provides a dynamic user interface for managing logs.

---

## Setup Instructions

### Back-End
1. Clone the repository.
2. Install dependencies:
   ```bash
   npm install
   ```
3. Set up environment variables for backend configurations.
4. Run the application:
   ```bash
   npm start
   ```

### Front-End
1. Navigate to the `frontend` directory.
2. Install Angular dependencies:
   ```bash
   npm install
   ```
3. Run the Angular application:
   ```bash
   ng serve
   ```

## License
This project is licensed under the MIT License.

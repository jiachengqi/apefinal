# APE demo

This repository contains the demo of internal assessment tool built with .NET 8 and Blazor Server. The application allows product and platform owner to create and submit assessments, cyber build team to manage and review the assessments . It supports local development with environment-specific configuration and containerized deployment using Docker Compose.

---
## Features

- Create, edit, and submit assessments
- Reviewer workflows for assessment review
- Image uploads with draggable pins for solution design evidence


## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/get-started)
- PostgreSQL database

## Configuration

Local configuration is managed in the `appsettings.Development.yaml` file. Ensure you have a YAML configuration provider installed. Adjust the connection string and logging settings as needed.
`appsettings.yaml` file for Docker compose run environment  
## Running Locally

1. Restore dependencies and run the application:

    ```bash
    dotnet restore
    dotnet run
    ```

2. Access the application at `https://localhost:5288`.

## Running with Docker
 Docker Compose:
    ```
    docker compose up --build
    ```

1. Docker Compose:

    ```bash
    docker compose up --build
    ```

2. Access the application at `https://localhost:5001`.


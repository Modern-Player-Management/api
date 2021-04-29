# Modern Player Management API

[![Build Status](https://travis-ci.com/Modern-Player-Management/api.svg?token=xgUivpRDAhwRrR1iUzX4&branch=master)](https://travis-ci.com/Modern-Player-Management/api)

Backend WebAPI for the Modern Player Management App.

## Environnement variable used

| Name | Description | Exemple Value |
| ---- | ----------- | ------------- |
| `DATABASE_URL` | Connection string to the PostgresSQL database | postgres://user:password@host:port/database |

The swagger API docs can be found at `/index.html`.

## Run

Check that you have the .NET 3.1+ SDK installed.

```bash
https://github.com/Modern-Player-Management
cd https://github.com/Modern-Player-Management
dotnet run
```

## Build with

- .NET 3.1
- ASP .NET Core
- Entity Framework Core
- Swagger
- PostgreSQL

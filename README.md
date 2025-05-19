# Preparatoria IIM - Sistema de Gestión

Sistema de gestión integral para la Preparatoria IIM

[![Estado del despliegue](https://github.com/taquion/prepaiim/actions/workflows/deploy-backend.yml/badge.svg)](https://github.com/taquion/prepaiim/actions/workflows/deploy-backend.yml)

## 🚀 Características principales

- Gestión de estudiantes y personal
- Control académico
- Administración de pagos
- Generación de reportes
- API RESTful

## 🏗️ Estructura del proyecto

- `src/PreparatoriaIIM.API` - API principal (ASP.NET Core 8.0)
- `src/PreparatoriaIIM.Application` - Lógica de negocio
- `src/PreparatoriaIIM.Domain` - Entidades y contratos
- `src/PreparatoriaIIM.Infrastructure` - Implementaciones de infraestructura

## 🛠️ Requisitos

- .NET 8.0 SDK
- SQL Server 2019+ o Azure SQL
- Visual Studio 2022 o VS Code
- Azure CLI (solo para despliegue)

## ⚙️ Configuración local

1. Clona el repositorio
2. Configura el archivo `appsettings.Development.json`
3. Ejecuta las migraciones:
   ```bash
   cd src/PreparatoriaIIM.API
   dotnet ef database update
   ```
4. Inicia la aplicación:
   ```bash
   dotnet run
   ```

## 🌐 Despliegue

El despliegue automático está configurado con GitHub Actions. Los cambios en la rama `main` se despliegan automáticamente a Azure App Service.

### URL de producción

- API: https://app-iim-backend-api-prod-3386.azurewebsites.net
- Health Check: https://app-iim-backend-api-prod-3386.azurewebsites.net/health
- Swagger: https://app-iim-backend-api-prod-3386.azurewebsites.net/swagger

## 📚 Documentación

- [Guía de despliegue](DEPLOYMENT.md)
- [Documentación de la API](https://app-iim-backend-api-prod-3386.azurewebsites.net/swagger)

## 📝 Licencia

Este proyecto está bajo la Licencia MIT.

# PrepaIIM

Sistema de gestión para la Preparatoria IIM

## Estado del despliegue

[![.NET](https://github.com/taquion/prepaiim/actions/workflows/deploy-backend.yml/badge.svg)](https://github.com/taquion/prepaiim/actions/workflows/deploy-backend.yml)

## Estructura del proyecto

- `src/PreparatoriaIIM.API`: API principal del sistema
- `src/PreparatoriaIIM.Domain`: Lógica de negocio y entidades
- `src/PreparatoriaIIM.Infrastructure`: Acceso a datos e implementaciones de infraestructura

## Requisitos

- .NET 9.0
- SQL Server 2019 o superior
- Azure CLI (para despliegue)

## Configuración

1. Clona el repositorio
2. Configura las variables de entorno necesarias
3. Ejecuta las migraciones de la base de datos
4. Inicia la aplicación

## Despliegue

El despliegue automático está configurado a través de GitHub Actions. Los cambios en la rama `main` se despliegan automáticamente a Azure App Service.

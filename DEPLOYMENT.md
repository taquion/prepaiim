# Instrucciones de Despliegue

## Requisitos Previos

- Una suscripción de Azure activa
- Permisos para crear recursos en Azure
- Azure CLI instalado localmente (opcional, para pruebas locales)
- GitHub Actions configurado con acceso a Azure

## Configuración de Azure

1. **Crear un App Service en Azure**
   - Ve a [Azure Portal](https://portal.azure.com)
   - Crea un nuevo recurso "Web App"
   - Configura los detalles básicos:
     - Suscripción: Tu suscripción
     - Grupo de recursos: Crea uno nuevo o usa uno existente
     - Nombre: `app-iim-backend-api-prod-3386`
     - Publicar: Código
     - Pila de tiempo de ejecución: .NET 8.0
     - Sistema operativo: Windows (o Linux según prefieras)
     - Región: Cercana a tu ubicación
   - Revisa y crea el recurso

2. **Configurar el perfil de publicación**
   - Ve a tu App Service recién creada
   - En el menú izquierdo, ve a "Deployment Center"
   - Selecciona "GitHub" como fuente
   - Configura la autenticación con tu cuenta de GitHub
   - Selecciona la organización, repositorio y rama (`main`)
   - Guarda la configuración

3. **Configurar secretos en GitHub**
   - Ve a la configuración de tu repositorio en GitHub
   - Navega a "Settings" > "Secrets and variables" > "Actions"
   - Agrega un nuevo secreto llamado `AZURE_WEBAPP_PUBLISH_PROFILE`
   - Para obtener el valor del perfil de publicación:
     - Ve a tu App Service en Azure Portal
     - En "Deployment Center", haz clic en "Get publish profile"
     - Copia TODO el contenido del archivo XML descargado
     - Pega este contenido como valor del secreto en GitHub

## Configuración de la Aplicación

1. **Variables de entorno**
   - Configura las siguientes variables de entorno en la configuración de tu App Service:
     - `ASPNETCORE_ENVIRONMENT`: Production
     - `ConnectionStrings__DefaultConnection`: Tu cadena de conexión a la base de datos
     - `AzureStorage__ConnectionString`: Tu cadena de conexión a Azure Storage
     - `Jwt__Key`: Tu clave secreta para JWT
     - `Jwt__Issuer`: El emisor del token JWT
     - `Jwt__Audience`: La audiencia del token JWT

2. **Configuración de CORS**
   - En la configuración de tu App Service, ve a "CORS"
   - Agrega los orígenes permitidos (URL de tu frontend)
   - O usa `*` para desarrollo (no recomendado para producción)

## Despliegue Automático

El despliegue automático está configurado mediante GitHub Actions. Cada vez que hagas push a la rama `main`, se ejecutará el flujo de trabajo definido en `.github/workflows/deploy-backend.yml`.

Para desplegar manualmente:
1. Haz push a la rama `main`
2. O ve a "Actions" en GitHub, selecciona el workflow de despliegue y haz clic en "Run workflow"

## Verificación del Despliegue

Después del despliegue, puedes verificar que todo funcione correctamente:

1. Ve a la URL de tu App Service: `https://app-iim-backend-api-prod-3386.azurewebsites.net`
2. Verifica el health check: `https://app-iim-backend-api-prod-3386.azurewebsites.net/health`
3. Verifica Swagger (si está habilitado): `https://app-iim-backend-api-prod-3386.azurewebsites.net/swagger`

## Solución de Problemas

- **Error de conexión a la base de datos**: Verifica que la cadena de conexión sea correcta y que el servidor de base de datos permita conexiones desde Azure App Service.
- **Error 500 en producción**: Revisa los registros de la aplicación en Azure Portal > App Service > Logs
- **El despliegue falla**: Revisa la pestaña "Actions" en GitHub para ver los logs de error

## Seguridad

- **No** subas claves o secretos al control de versiones
- Usa siempre HTTPS
- Configura reglas de firewall según sea necesario
- Mantén actualizadas las dependencias de seguridad

---

Para más información, consulta la [documentación de Azure App Service](https://docs.microsoft.com/es-es/azure/app-service/).

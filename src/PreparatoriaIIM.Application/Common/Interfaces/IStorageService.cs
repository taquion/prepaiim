using System;
using System.IO;
using System.Threading.Tasks;

namespace PreparatoriaIIM.Application.Common.Interfaces
{
    public interface IStorageService
    {
        /// <summary>
        /// Sube un archivo al almacenamiento
        /// </summary>
        /// <param name="containerName">Nombre del contenedor</param>
        /// <param name="fileName">Nombre del archivo</param>
        /// <param name="fileStream">Stream con el contenido del archivo</param>
        /// <returns>URI del archivo subido</returns>
        Task<Uri> UploadFileAsync(string containerName, string fileName, Stream fileStream);

        /// <summary>
        /// Descarga un archivo del almacenamiento
        /// </summary>
        /// <param name="containerName">Nombre del contenedor</param>
        /// <param name="fileName">Nombre del archivo</param>
        /// <returns>Stream con el contenido del archivo</returns>
        Task<Stream> DownloadFileAsync(string containerName, string fileName);

        /// <summary>
        /// Elimina un archivo del almacenamiento
        /// </summary>
        /// <param name="containerName">Nombre del contenedor</param>
        /// <param name="fileName">Nombre del archivo</param>
        /// <returns>True si se eliminó correctamente, false en caso contrario</returns>
        Task<bool> DeleteFileAsync(string containerName, string fileName);

        /// <summary>
        /// Obtiene la URL de un archivo para acceso directo
        /// </summary>
        /// <param name="containerName">Nombre del contenedor</param>
        /// <param name="fileName">Nombre del archivo</param>
        /// <param name="expiryTime">Tiempo de expiración del enlace</param>
        /// <returns>URL del archivo con token de SAS</returns>
        Task<Uri> GetFileUrlAsync(string containerName, string fileName, TimeSpan? expiryTime = null);
    }
}

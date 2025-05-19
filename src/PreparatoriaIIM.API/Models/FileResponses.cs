using System;

namespace PreparatoriaIIM.API.Models
{
    /// <summary>
    /// Respuesta al subir un archivo
    /// </summary>
    public class FileUploadResponse
    {
        /// <summary>
        /// Nombre del archivo generado
        /// </summary>
        public string FileName { get; set; }
        
        /// <summary>
        /// Nombre original del archivo
        /// </summary>
        public string OriginalFileName { get; set; }
        
        /// <summary>
        /// URL completa del archivo
        /// </summary>
        public string FileUrl { get; set; }
        
        /// <summary>
        /// Tamaño del archivo en bytes
        /// </summary>
        public long FileSizeInBytes { get; set; }
        
        /// <summary>
        /// Tipo de contenido del archivo
        /// </summary>
        public string ContentType { get; set; }
    }
    
    /// <summary>
    /// Respuesta con la URL de un archivo
    /// </summary>
    public class FileUrlResponse
    {
        /// <summary>
        /// Nombre del archivo
        /// </summary>
        public string FileName { get; set; }
        
        /// <summary>
        /// URL del archivo con token SAS
        /// </summary>
        public string FileUrl { get; set; }
        
        /// <summary>
        /// Fecha y hora de expiración del enlace
        /// </summary>
        public DateTime ExpiresAt { get; set; }
    }
}

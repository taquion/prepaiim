using System;
using System.IO;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.Extensions.Logging;
using PreparatoriaIIM.Application.Common.Interfaces;

namespace PreparatoriaIIM.Infrastructure.Services
{
    public class PdfReportService : IPdfReportService
    {
        private readonly ILogger<PdfReportService> _logger;

        public PdfReportService(ILogger<PdfReportService> logger)
        {
            _logger = logger;
        }

        public async Task<byte[]> GenerateStudentReportAsync(StudentReportData data)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                using (var writer = new PdfWriter(memoryStream))
                using (var pdf = new PdfDocument(writer))
                using (var document = new Document(pdf))
                {
                    // Título del documento
                    var title = new Paragraph("Informe Académico del Estudiante")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(20);
                    document.Add(title);

                    // Información del estudiante
                    document.Add(new Paragraph("\n"));
                    document.Add(new Paragraph("Información del Estudiante").SetBold());
                    document.Add(new Paragraph($"Nombre: {data.FullName}"));
                    document.Add(new Paragraph($"Matrícula: {data.StudentId}"));
                    document.Add(new Paragraph($"Grado: {data.Grade}"));
                    document.Add(new Paragraph($"Grupo: {data.Group}"));

                    // Calificaciones
                    document.Add(new Paragraph("\n"));
                    document.Add(new Paragraph("Calificaciones").SetBold());
                    
                    // Tabla de calificaciones
                    var table = new Table(3);
                    table.SetWidth(UnitValue.CreatePercentValue(100));
                    
                    // Encabezados de la tabla
                    table.AddHeaderCell("Materia");
                    table.AddHeaderCell("Calificación");
                    table.AddHeaderCell("Estado");
                    
                    // Filas de la tabla
                    foreach (var subject in data.Subjects)
                    {
                        table.AddCell(subject.Name);
                        table.AddCell(subject.Grade.ToString("0.0"));
                        table.AddCell(subject.Approved ? "Aprobado" : "No Aprobado");
                    }
                    
                    document.Add(table);
                    
                    // Promedio general
                    document.Add(new Paragraph($"\nPromedio General: {data.Average:0.00}"));
                    
                    // Fecha de generación
                    document.Add(new Paragraph($"\nGenerado el: {DateTime.Now:dd/MM/yyyy HH:mm}"));
                    
                    document.Close();
                    
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar el informe PDF");
                throw;
            }
        }

        public async Task<byte[]> GeneratePaymentReceiptAsync(PaymentReceiptData data)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                using (var writer = new PdfWriter(memoryStream))
                using (var pdf = new PdfDocument(writer))
                using (var document = new Document(pdf))
                {
                    // Título del documento
                    var title = new Paragraph("Recibo de Pago")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(20);
                    document.Add(title);

                    // Información del recibo
                    document.Add(new Paragraph("\n"));
                    document.Add(new Paragraph($"Número de Recibo: {data.ReceiptNumber}"));
                    document.Add(new Paragraph($"Fecha de Pago: {data.PaymentDate:dd/MM/yyyy}"));
                    document.Add(new Paragraph($"Concepto: {data.Concept}"));
                    document.Add(new Paragraph($"Monto: {data.Amount:C2}"));
                    document.Add(new Paragraph($"Método de Pago: {data.PaymentMethod}"));
                    document.Add(new Paragraph($"Referencia: {data.Reference}"));

                    // Información del estudiante
                    document.Add(new Paragraph("\n"));
                    document.Add(new Paragraph("Datos del Estudiante").SetBold());
                    document.Add(new Paragraph($"Nombre: {data.StudentName}"));
                    document.Add(new Paragraph($"Matrícula: {data.StudentId}"));
                    document.Add(new Paragraph($"Grado: {data.Grade}"));
                    document.Add(new Paragraph($"Grupo: {data.Group}"));

                    // Detalles adicionales
                    document.Add(new Paragraph("\n"));
                    document.Add(new Paragraph("Detalles Adicionales").SetBold());
                    document.Add(new Paragraph(data.AdditionalDetails ?? "Sin detalles adicionales"));

                    // Pie de página
                    document.Add(new Paragraph("\n\n"));
                    document.Add(new Paragraph("________________________________________"));
                    document.Add(new Paragraph("Firma del Responsable"));
                    document.Add(new Paragraph($"\nGenerado el: {DateTime.Now:dd/MM/yyyy HH:mm}"));
                    
                    document.Close();
                    
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar el recibo de pago en PDF");
                throw;
            }
        }
    }

    public class StudentReportData
    {
        public string FullName { get; set; }
        public string StudentId { get; set; }
        public string Grade { get; set; }
        public string Group { get; set; }
        public SubjectGrade[] Subjects { get; set; }
        public decimal Average { get; set; }
    }

    public class SubjectGrade
    {
        public string Name { get; set; }
        public decimal Grade { get; set; }
        public bool Approved { get; set; }
    }

    public class PaymentReceiptData
    {
        public string ReceiptNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Concept { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string Reference { get; set; }
        public string StudentName { get; set; }
        public string StudentId { get; set; }
        public string Grade { get; set; }
        public string Group { get; set; }
        public string AdditionalDetails { get; set; }
    }
}

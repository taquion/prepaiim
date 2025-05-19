using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PreparatoriaIIM.Application.Common.Interfaces;

namespace PreparatoriaIIM.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : BaseApiController
    {
        private readonly IPdfReportService _pdfReportService;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(
            IPdfReportService pdfReportService,
            ILogger<ReportsController> logger)
        {
            _pdfReportService = pdfReportService;
            _logger = logger;
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GenerateStudentReport(Guid studentId)
        {
            try
            {
                // En una implementación real, obtendrías estos datos de tu base de datos
                var reportData = new StudentReportData
                {
                    FullName = "Juan Pérez López",
                    StudentId = studentId.ToString(),
                    Grade = "3ro",
                    Group = "B",
                    Subjects = new[]
                    {
                        new SubjectGrade { Name = "Matemáticas", Grade = 9.5m, Approved = true },
                        new SubjectGrade { Name = "Español", Grade = 8.7m, Approved = true },
                        new SubjectGrade { Name = "Ciencias", Grade = 7.2m, Approved = true },
                        new SubjectGrade { Name = "Historia", Grade = 5.9m, Approved = false },
                    },
                    Average = 7.8m
                };

                var pdfBytes = await _pdfReportService.GenerateStudentReportAsync(reportData);
                
                return File(pdfBytes, "application/pdf", $"Informe_Estudiante_{studentId}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al generar el informe del estudiante {studentId}");
                return StatusCode(500, "Error al generar el informe del estudiante");
            }
        }

        [HttpGet("payment/{paymentId}")]
        public async Task<IActionResult> GeneratePaymentReceipt(Guid paymentId)
        {
            try
            {
                // En una implementación real, obtendrías estos datos de tu base de datos
                var receiptData = new PaymentReceiptData
                {
                    ReceiptNumber = $"R-{DateTime.Now:yyyyMMdd}-{paymentId.ToString().Substring(0, 4)}",
                    PaymentDate = DateTime.Now,
                    Concept = "Colegiatura mes de junio 2024",
                    Amount = 2500.00m,
                    PaymentMethod = "Transferencia bancaria",
                    Reference = $"TRF-{DateTime.Now:yyyyMMdd}",
                    StudentName = "Juan Pérez López",
                    StudentId = "A12345678",
                    Grade = "3ro",
                    Group = "B",
                    AdditionalDetails = "Pago realizado a través de la plataforma en línea."
                };

                var pdfBytes = await _pdfReportService.GeneratePaymentReceiptAsync(receiptData);
                
                return File(pdfBytes, "application/pdf", $"Recibo_Pago_{receiptData.ReceiptNumber}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al generar el recibo de pago {paymentId}");
                return StatusCode(500, "Error al generar el recibo de pago");
            }
        }
    }

    // Clases de modelo para los informes
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

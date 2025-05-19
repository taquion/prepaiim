using System.Threading;
using System.Threading.Tasks;

namespace PreparatoriaIIM.Application.Common.Interfaces
{
    public interface IPdfReportService
    {
        Task<byte[]> GenerateStudentReportAsync(StudentReportData data);
        Task<byte[]> GeneratePaymentReceiptAsync(PaymentReceiptData data);
    }

    public class StudentReportData
    {
        public string StudentName { get; set; }
        public string StudentId { get; set; }
        public string Program { get; set; }
        public string Semester { get; set; }
        public decimal AverageGrade { get; set; }
        public List<SubjectGrade> Subjects { get; set; }
    }

    public class SubjectGrade
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public decimal Grade { get; set; }
        public string Status { get; set; }
    }

    public class PaymentReceiptData
    {
        public string ReceiptNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public string StudentName { get; set; }
        public string StudentId { get; set; }
        public string PaymentConcept { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string ReferenceNumber { get; set; }
        public string Status { get; set; }
    }
}

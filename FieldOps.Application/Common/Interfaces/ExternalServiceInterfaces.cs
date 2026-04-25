namespace FieldOps.Application.Common.Interfaces;

public interface IBlobStorageService
{
    Task<string> UploadAsync(string tenantId, string path, Stream stream, string contentType, CancellationToken cancellationToken);
    Task DeleteAsync(string blobUrl, CancellationToken cancellationToken);
    string GenerateReadSasUrl(string blobUrl, TimeSpan expiry);
}

public interface INotificationService
{
    Task SendWhatsAppAsync(string phoneNumber, string message, CancellationToken cancellationToken);
    Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken);
}

public interface IPdfService
{
    Task<byte[]> GenerateInvoicePdfAsync(int invoiceId, CancellationToken cancellationToken);
    Task<byte[]> GenerateServiceReportPdfAsync(int workOrderId, CancellationToken cancellationToken);
}

public interface IEmailService
{
    Task SendAsync(string toEmail, string subject, string body, IReadOnlyCollection<(string FileName, byte[] Content)>? attachments, CancellationToken cancellationToken);
}

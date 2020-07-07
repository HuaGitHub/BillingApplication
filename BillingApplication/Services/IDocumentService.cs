namespace BillingApplication.Services
{
    public interface IDocumentService
    {
        bool UploadDocument(string fileName, string email);
    }
}
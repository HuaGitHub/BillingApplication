using BillingApplication.Data.Classes;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BillingApplication.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly ILogger<DocumentService> _logger;
        private readonly IHostEnvironment _hosting;
        private readonly IDatabaseService _databaseService;
        private const string CMD = @"C:\Windows\System32\cmd.exe";
        private readonly string Directory;
        public DocumentService(ILogger<DocumentService> logger, IHostEnvironment hosting, IDatabaseService dbService)
        {
            _logger = logger;
            _hosting = hosting;
            _databaseService = dbService;
            Directory = @$"{_hosting.ContentRootPath}\wwwroot\invoices";
        }
        /// <summary>
        /// Add document to database
        /// </summary>
        /// <param name="fileName">File Name</param>
        /// <param name="email">Email</param>
        /// <returns>Boolean</returns>
        public bool UploadDocument(string fileName, string email)
        {
            //Convert Pdf to Text
            var txtFile = ConvertPdfToText(fileName);
            if (!string.IsNullOrEmpty(txtFile))
            {
                //Extra invoice
                var invoice = ExtractInvoice(txtFile, email);

                //Add new invoice to database
                if (invoice != null)
                {

                    if (_databaseService.AddInvoice(invoice)) { return true; }
                }
                return false;

            }
            _logger.LogError($"Upload document unsuccessful.");
            return false;
        }
        //Extract invoice from text file
        private Invoice ExtractInvoice(string fileName, string email)
        {
            try
            {
                int val;
                decimal amount;
                var invoice = new Invoice();
                _logger.LogInformation("Extra data from text file");

                var lines = File.ReadLines(@$"{Directory}\{fileName}", Encoding.GetEncoding("iso-8859-1")).Where(line => line != "").ToList();
                for (int i = 0; i < lines.Count(); i++)
                {
                    //Get InvoiceDate & VendorName
                    if (invoice.InvoiceDate == null && lines[i].Contains("Invoice"))
                    {
                        var temp = lines[i].Trim().Split(" ");
                        if (int.TryParse(temp[1], out val))
                        {
                            int len = temp.Length;
                            invoice.InvoiceDate = $"{temp[len - 3]} {temp[len - 2]} {temp[len - 1]}";
                            invoice.VendorName = lines[++i].Trim();
                        }
                    }
                    //Get TotalAmountDue & Currency
                    else if (invoice.TotalAmountDue == 0 && lines[i].Contains("Total Due"))
                    {
                        var temp = lines[i].Trim().Split(" ");
                        if (decimal.TryParse(ParseAmount(temp[temp.Length - 1]), out amount))
                        {
                            invoice.TotalAmountDue = amount;
                            invoice.Currency = lines[++i].Trim();
                        }
                    }
                    //Get TotalAmount
                    else if (invoice.TotalAmount == 0 && lines[i].Contains("Total"))
                    {
                        var temp = lines[i].Trim().Split(" ");
                        if (decimal.TryParse(ParseAmount(temp[temp.Length - 1]), out amount))
                        {
                            invoice.TotalAmount = amount;
                        }
                    }
                    //Get TaxAmount
                    else if (invoice.TaxAmount == 0 && ( lines[i].Contains("Tax") || lines[i].Contains("GST")))
                    {
                        var temp = lines[i].Trim().Split(" ");
                        if (decimal.TryParse(ParseAmount(temp[temp.Length - 1]), out amount))
                        {
                            invoice.TaxAmount = amount;
                        }
                    }
                }
                invoice.UploadedBy = email;
                invoice.UploadTimestamp = DateTime.UtcNow;
                invoice.FileSize = new FileInfo(@$"{Directory}\{fileName}").Length;
                return invoice;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Extract data from text file failed, {ex}");
                return null;
            }
        }
        // Convert PDF file to text file
        private string ConvertPdfToText(string fileName)
        {
            try
            {
                _logger.LogInformation($"Convert {fileName} to text");

                //Pdf file exist?
                if (File.Exists(@$"{Directory}\{fileName}"))
                {
                    string txtFile = fileName.Split(".")[0] + ".txt";
                    //Text file exist?
                    if (File.Exists(@$"{Directory}\{txtFile}"))
                    {
                        _logger.LogInformation("File was already converted to text");
                    }
                    else
                    {
                        //Setup Command Prompt 
                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                        startInfo.FileName = CMD;
                        startInfo.WorkingDirectory = Directory;
                        startInfo.CreateNoWindow = true;
                        startInfo.RedirectStandardInput = true;
                        startInfo.RedirectStandardOutput = true;
                        process.StartInfo = startInfo;
                        process.Start();

                        //Run the pdftotext command
                        process.StandardInput.WriteLine($"pdftotext -table {fileName}");
                        process.StandardInput.Flush();
                        process.StandardInput.Close();
                        process.WaitForExit();
                    }
                    return txtFile;
                }
                else
                {
                    _logger.LogError("File does not exist.");
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"Convert pdf to text failed, {ex}");
            }
            return string.Empty;
        }

        //Remove any currency symbols from amount
        private string ParseAmount(string val)
        {
            return Regex.Replace(val, @"[^\d.-]", "");

        }
    }
}

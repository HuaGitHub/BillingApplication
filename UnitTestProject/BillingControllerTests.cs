using BillingApplication.Controllers;
using BillingApplication.Data.Classes;
using BillingApplication.Services;
using BillingApplication.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace UnitTestProject
{
    public class BillingControllerTests
    {
        private BillingController _billingController;
        private Mock<IDocumentService> _documentMockService;
        private Mock<IDatabaseService> _databaseMockService;
        private Mock<ILogger<BillingController>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            //Setup Mock services
            _documentMockService = new Mock<IDocumentService>();
            _databaseMockService = new Mock<IDatabaseService>();
            _loggerMock = new Mock<ILogger<BillingController>>();
            _billingController = new BillingController(_loggerMock.Object, _documentMockService.Object, _databaseMockService.Object);
        }

        [Test]
        public void When_GetAllInvoicesIsSuccessful_Then_Status200WillBeReturned()
        {
            //Arrange 
            _databaseMockService.Setup(x => x.GetAllInvoice()).Returns(It.IsAny<IEnumerable<Invoice>>());

            //Act
            var result = (ObjectResult)_billingController.Get();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        [TestCase(1)]
        public void When_InvoiceIsFoundInDatabase_Then_Status200WillBeReturned(int id)
        {
            //Arrange
            var invoiceResult = new Invoice()
            {
                Id = 1,
                UploadedBy = "newUser@gmail.com",
                UploadTimestamp = System.DateTime.UtcNow,
                FileSize = 1708,
                VendorName = "UK Company",
                InvoiceDate = "February 22, 2019",
                TotalAmount = 22.50M,
                TotalAmountDue = 0.00M,
                Currency = "GBP",
                TaxAmount = 0.00M,
            };
            _databaseMockService.Setup(x => x.GetInvoice(It.IsAny<int>())).Returns(invoiceResult);

            //Act
            var result = (ObjectResult)_billingController.Get(id);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        [TestCase(100)]
        public void When_InvoiceIsNotFoundInDatabase_Then_Status404WillBeReturned(int id)
        {
            //Arrange
            _databaseMockService.Setup(x => x.GetInvoice(It.IsAny<int>())).Returns(It.IsAny<Invoice>());

            //Act
            var result = (ObjectResult)_billingController.Get(id);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.AreEqual("Invoice not found", result.Value);
        }

        [Test]
        public void When_StatsAreFoundInDatabase_Then_Status200WillBeReturned()
        {
            //Arrange
            var statsResult = new List<Statistic>()
            {
                new Statistic()
                {
                    Id=1,
                    UploadedBy="jch@gmail.com",
                    FileCount=1,
                    TotalFileSize=1231,
                    TotalAmountDueUSD=0.00M,
                    TotalAmountUSD=34.00M
                }
            };
            _databaseMockService.Setup(x => x.GetStats()).Returns(statsResult);

            //Act
            var result = (ObjectResult)_billingController.GetStats();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public void When_GetStatsReturnNull_Then_Status404WillBeReturned()
        {
            //Arrange
            _databaseMockService.Setup(x => x.GetStats()).Returns(It.IsAny<IEnumerable<Statistic>>());

            //Act
            var result = (ObjectResult)_billingController.GetStats();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.AreEqual("Statistics not found", result.Value);
        }

        [Test]
        public void When_UploadInvoiceIsSuccessful_Then_Status201WillBeReturned()
        {
            //Arrange
            var model = new UploadViewModel()
            {
                File = "HubdocInvoice2.pdf",
                Email = "newuser@gmail.com"
            };
            _documentMockService.Setup(x => x.UploadDocument(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            //Act
            var result = (ObjectResult)_billingController.Post(model);
            
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(201, result.StatusCode);
            Assert.IsTrue((bool)result.Value);
        }
        [Test]
        public void When_UploadInvoiceIsUnsuccessful_Then_Status400WillBeReturned()
        {
            //Arrange
            var model = new UploadViewModel()
            {
                File = "InvalidPDF.pdf",
                Email = "newuser@gmail.com"
            };
            _documentMockService.Setup(x => x.UploadDocument(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            //Act
            var result = (ObjectResult)_billingController.Post(model);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Invalid request data", result.Value);
        }
    }
}
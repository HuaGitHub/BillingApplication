using System;
using System.Collections.Generic;
using System.Linq;
using BillingApplication.Services;
using BillingApplication.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BillingApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillingController : ControllerBase
    {
        private readonly ILogger<BillingController> _logger;
        private readonly IDocumentService _documentService;
        private readonly IDatabaseService _databaseService;

        public BillingController(ILogger<BillingController> logger, IDocumentService documentService, IDatabaseService databaseService)
        {
            _logger = logger;
            _documentService = documentService;
            _databaseService = databaseService;
        }
        // GET: api/Billing
        [HttpGet]
        [Route("/document")]
        public IActionResult Get()
        {
            try
            {
                _logger.LogInformation($"GET all invoices");
                var invoices = _databaseService.GetAllInvoice();
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET all invoice failed, {ex}");
                return BadRequest("GET all invoice failed");
            }
        }

        // GET: /document/{id}
        [HttpGet]
        [Route("/document/{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                _logger.LogInformation($"GET invoice - Id: {id}");
                var invoice = _databaseService.GetInvoice(id);
                if (invoice == null)
                {
                    _logger.LogInformation($"Invoice {id} not found");
                    return NotFound("Invoice not found");
                }
                return Ok(invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET invoice failed, {ex}");
                return BadRequest("GET invoice failed");
            }
        }

        // GET: /stats
        [HttpGet]
        [Route("/stats")]
        public IActionResult GetStats()
        {
            try
            {
                _logger.LogInformation("GET stats");
                var stats = _databaseService.GetStats();
                if (stats == null)
                {
                    _logger.LogInformation("Statistics not found");
                    return NotFound("Statistics not found");
                }
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET statistics failed, {ex}");
                return BadRequest("GET statistics failed");
            }
        }

        // POST: /upload
        [HttpPost]
        [Route("/upload")]
        public IActionResult Post([FromBody] UploadViewModel model)
        {
            try
            {
                _logger.LogInformation($"POST invoice: {model.File}");
                bool success = _documentService.UploadDocument(model.File, model.Email);
                if (success) { 
                    return Created("/upload", success);
                }
                return BadRequest("Invalid request data");
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST invoice failed, {ex}");
                return BadRequest("POST invoice failed");
            }
        }
    }
}

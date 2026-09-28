using FreeLanceTracker.Data;
using FreeLanceTracker.Services.InvoiceService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeLanceTracker.Controllers;

[ApiController]
[Route("api/invoice")]
[Authorize]
public class InvoiceController(IInvoiceService invoiceServiceService) : ControllerBase
{
    [HttpGet("{invoiceId:int}")]
    public async Task<ActionResult<Invoice>> GetByIdAsync(int invoiceId)
    {
        var invoice = await invoiceServiceService.GetByIdAsync(invoiceId);
        return invoice is null ? NotFound() : Ok(invoice);
    }
    
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<Invoice>>> GetAllAsync(int clientId)
    {
        var invoiceList = await invoiceServiceService.GetByClientIdAsync(clientId);
        return Ok(invoiceList);
    }

    [HttpPost]
    public async Task<ActionResult<Invoice>> GenerateInvoiceFromUnbilledTimeAsync(int clientId,
        IEnumerable<int> projectIds, DateTime dueDate)
    {
        var invoice = await invoiceServiceService.GenerateInvoiceFromUnbilledTimeAsync(clientId, projectIds, dueDate);
        return Ok(invoice);
    }
    
    [HttpPatch("{invoiceId:int}/status")]
    public async Task<ActionResult> UpdateStatusAsync(int invoiceId, InvoiceStatus newStatus)
    {
        await invoiceServiceService.UpdateStatusAsync(invoiceId, newStatus);
        return Ok();
    }

    [HttpPatch("{invoiceId:int}/update")]
    public async Task<ActionResult> UpdateLineItemAsync(InvoiceLineItem lineItem, int invoiceLineItemId)
    {
        await invoiceServiceService.UpdateLineItemAsync(lineItem, invoiceLineItemId);
        return Ok();
        
    }
    
    [HttpPatch("{invoiceId:int}/add-line-item/{lineItemId:int}")]
    public async Task<ActionResult<InvoiceLineItem>> AddLineItemAsync(int invoiceId, InvoiceLineItem lineItem)
    {
        var invoiceLineItem = await invoiceServiceService.AddLineItemAsync(lineItem, invoiceId);
        return Ok(invoiceLineItem);
    }
    
    [HttpDelete("{invoiceLineItemId:int}")]
    public async Task<ActionResult> DeleteLineItemAsync(int invoiceLineItemId)
    {
        await invoiceServiceService.DeleteLineItemAsync(invoiceLineItemId);
        return Ok();
    }
    
    [HttpPost("{invoiceId:int}/invoice-total")]
    public async Task<ActionResult<decimal> > GetInvoiceTotalAsync(int invoiceId)
    {
        var total = await invoiceServiceService.GetTotalAsync(invoiceId);
        return Ok(total);
    }
    
    
    
    
    
    
    
}
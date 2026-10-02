using System.Security.Claims;
using FreeLanceTracker.Data;
using FreeLanceTracker.Services.InvoiceService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeLanceTracker.Controllers;

[ApiController]
[Route("api/invoice")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class InvoiceController(IInvoiceService invoiceService) : ControllerBase
{
    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();
    }

    [HttpGet("{invoiceId:int}")]
    public async Task<ActionResult<Invoice>> GetByIdAsync(int invoiceId)
    {
        var invoice = await invoiceService.GetByIdAsync(invoiceId, GetUserId());
        return invoice is null ? NotFound() : Ok(invoice);
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<Invoice>>> GetAllAsync(int clientId)
    {
        var invoiceList = await invoiceService.GetByClientIdAsync(clientId, GetUserId());
        return Ok(invoiceList);
    }

    [HttpPost]
    public async Task<ActionResult<Invoice>> GenerateInvoiceFromUnbilledTimeAsync(int clientId,
        IEnumerable<int> projectIds, DateTime dueDate)
    {
        var invoice =
            await invoiceService.GenerateInvoiceFromUnbilledTimeAsync(clientId, projectIds, dueDate, GetUserId());
        return Ok(invoice);
    }

    [HttpPatch("{invoiceId:int}/status")]
    public async Task<ActionResult> UpdateStatusAsync(int invoiceId, InvoiceStatus newStatus)
    {
        await invoiceService.UpdateStatusAsync(invoiceId, newStatus, GetUserId());
        return Ok();
    }

    [HttpPatch("{invoiceId:int}/line-items/{invoiceLineItemId:int}")]
    public async Task<ActionResult> UpdateLineItemAsync(int invoiceLineItemId, InvoiceLineItem lineItem)
    {
        await invoiceService.UpdateLineItemAsync(lineItem, invoiceLineItemId, GetUserId());
        return Ok();
    }

    [HttpPost("{invoiceId:int}/line-items")]
    public async Task<ActionResult<InvoiceLineItem>> AddLineItemAsync(int invoiceId, InvoiceLineItem lineItem)
    {
        var invoiceLineItem = await invoiceService.AddLineItemAsync(lineItem, invoiceId, GetUserId());
        return Ok(invoiceLineItem);
    }

    [HttpDelete("{invoiceId:int}/line-items/{invoiceLineItemId:int}")]
    public async Task<ActionResult> DeleteLineItemAsync(int invoiceLineItemId)
    {
        await invoiceService.DeleteLineItemAsync(invoiceLineItemId, GetUserId());
        return Ok();
    }

    [HttpGet("{invoiceId:int}/total")]
    public async Task<ActionResult<decimal>> GetInvoiceTotalAsync(int invoiceId)
    {
        var total = await invoiceService.GetTotalAsync(invoiceId, GetUserId());
        return Ok(total);
    }
}
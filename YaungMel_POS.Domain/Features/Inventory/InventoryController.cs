using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using YaungMel_POS.Domain.DTOs;
using YaungMel_POS.Shared.Responses;

namespace YaungMel_POS.Domain.Features.Inventory
{
    [Route("api/inventory")]
    [ApiController]
    [Authorize(Roles = "Admin,Staff")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _service;

        public InventoryController(IInventoryService service)
        {
            _service = service;
        }

        [HttpGet("low-stock")]
        public async Task<IActionResult> LowStock([FromQuery] int lowStock = 5)
        {
            var result = await _service.GetLowStockAlertsAsync(lowStock);
            return Ok(result);
        }
    }
}

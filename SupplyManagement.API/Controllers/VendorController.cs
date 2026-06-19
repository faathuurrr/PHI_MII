using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplyManagement.API.DTO;
using SupplyManagement.API.Service.Interfaces;

namespace SupplyManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorController : ControllerBase
    {
        private readonly IVendorService _vendorService;

        public VendorController(IVendorService vendorService)
        {
            _vendorService = vendorService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterCompanyDto dto)
        {
            try
            {
                var result = await _vendorService.RegisterCompanyAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, new
                {
                    message = "Registration submitted. Waiting for admin approval.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,LogisticManager")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _vendorService.GetAllCompaniesAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _vendorService.GetCompanyByIdAsync(id);
            if (result == null) return NotFound(new { message = "Company not found." });
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateCompanyDto dto)
        {
            try
            {
                var result = await _vendorService.UpdateCompanyAsync(id, dto);
                return Ok(new { message = "Company updated.", data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _vendorService.DeleteCompanyAsync(id);
                return Ok(new { message = "Company deleted." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("{id:guid}/complete-profile")]
        public async Task<IActionResult> CompleteProfile(Guid id, [FromBody] CompleteVendorProfileDto dto)
        {
            try
            {
                var result = await _vendorService.CompleteVendorProfileAsync(id, dto);
                return Ok(new { message = "Profile submitted. Waiting for manager approval.", data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("submit-tender")]
        [Authorize(Roles = "Admin,LogisticManager")]
        public async Task<IActionResult> SubmitTender([FromBody] SubmitTenderDto dto)
        {
            try
            {
                await _vendorService.SubmitTenderAsync(dto);
                return Ok(new { message = "Tender submitted successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}

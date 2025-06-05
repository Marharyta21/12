using Microsoft.AspNetCore.Mvc;
using Tutorial__12.Services;

namespace Tutorial__12.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }
        
        [HttpDelete("{idClient}")]
        public async Task<IActionResult> DeleteClient(int idClient)
        {
            try
            {
                if (!await _clientService.ClientExistsAsync(idClient))
                    return NotFound(new { message = "Client not found" });

                await _clientService.DeleteClientAsync(idClient);
                return Ok(new { message = "Client successfully deleted" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }
    }
}
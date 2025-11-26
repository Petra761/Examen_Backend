using Microsoft.AspNetCore.Mvc;

namespace ExamenFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly HttpClient context;
        private readonly string api = "https://programacionweb2examen3-production.up.railway.app/api/Clientes";
        public ClientesController(HttpClient context)
        {
            this.context = context;
        }

        // GET: api/Clientes
        [HttpGet]
        public async Task<IActionResult> GetCliente()
        {
            try
            {
                var response = await context.GetAsync($"{api}/Listar");

                if (response.IsSuccessStatusCode)
                {
                    var clientes = await response.Content.ReadAsStringAsync();
                    var clientesJson = System.Text.Json.JsonSerializer.Deserialize<object>(clientes);
                    return Ok(clientesJson);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound("No se encontraron clientes.");
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error en la api");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error interno del servidor.", details = ex.Message });
            }
        }


        [HttpGet("{ci}")]
        public async Task<IActionResult> GetClienteByCi(string ci)
        {
            try
            {
                var response = await context.GetAsync($"{api}/{ci}");
                if (response.IsSuccessStatusCode)
                {
                    var cliente = await response.Content.ReadAsStringAsync();
                    var clienteJson = System.Text.Json.JsonSerializer.Deserialize<object>(cliente);
                    return Ok(clienteJson);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound("Cliente no encontrado.");
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error en la api");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error interno del servidor.", details = ex.Message });
            }
        }
    }
}

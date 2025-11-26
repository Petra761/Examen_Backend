using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExamenFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditosController : ControllerBase
    {
        private readonly HttpClient context;
        private readonly string api = "https://programacionweb2examen3-production.up.railway.app/api/Creditos";

        public CreditosController(HttpClient context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCreditos()
        {
            try
            {
                var response = await context.GetAsync($"{api}/Listar");

                if (response.IsSuccessStatusCode)
                {
                    var creditos = await response.Content.ReadAsStringAsync();
                    var creditosJson = System.Text.Json.JsonSerializer.Deserialize<object>(creditos);
                    return Ok(creditosJson);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound("No se encontraron créditos.");
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCreditoById(string id)
        {
            try
            {
                var response = await context.GetAsync($"{api}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var credito = await response.Content.ReadAsStringAsync();
                    var creditoJson = System.Text.Json.JsonSerializer.Deserialize<object>(credito);
                    return Ok(creditoJson);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound("Crédito no encontrado.");
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

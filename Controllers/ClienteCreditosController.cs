using ExamenFinal.Entidades;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ExamenFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteCreditosController : ControllerBase
    {
        private readonly HttpClient context;
        private readonly ExamenContext dbContext;
        private readonly string apiClientes = "https://programacionweb2examen3-production.up.railway.app/api/Clientes";
        private readonly string apiCreditos = "https://programacionweb2examen3-production.up.railway.app/api/Creditos";

        public ClienteCreditosController(HttpClient context, ExamenContext dbContext)
        {
            this.context = context;
            this.dbContext = dbContext;
        }

        [HttpGet("{ci}")]
        public async Task<IActionResult> GetClienteCreditoByCi(string ci)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var clienteResponse = await context.GetAsync($"{apiClientes}/{ci}");

                if (!clienteResponse.IsSuccessStatusCode)
                {
                    if (clienteResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return NotFound($"El cliente con CI {ci} no fue encontrado en la API externa.");
                    }
                    return StatusCode((int)clienteResponse.StatusCode, "Error al conectar con la API de Clientes.");
                }

                var jsonCliente = await clienteResponse.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(jsonCliente))
                {
                    return NotFound("La API de clientes devolvió una respuesta vacía.");
                }

                var cliente = JsonSerializer.Deserialize<ClienteDTO>(jsonCliente, options);


                var creditosResponse = await context.GetAsync($"{apiCreditos}/Listar");

                if (!creditosResponse.IsSuccessStatusCode)
                {
                    return StatusCode((int)creditosResponse.StatusCode, "Error al conectar con la API de Créditos.");
                }

                var jsonCreditos = await creditosResponse.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(jsonCreditos))
                {
                    return StatusCode(500, "La API de créditos devolvió una respuesta vacía.");
                }

                var creditos = JsonSerializer.Deserialize<List<CreditoDTO>>(jsonCreditos, options);


                // --- 3. LÓGICA DE NEGOCIO ---
                // Nota: Asegúrate que el tipo de dato de clienteCi coincida (string vs int/long)
                var credito = creditos?.FirstOrDefault(c => c.clienteCi.ToString() == ci);

                if (credito == null)
                {
                    return NotFound($"El cliente {cliente.nombre} existe, pero no tiene créditos asignados.");
                }

                ClienteCredito clienteCredito = new ClienteCredito
                {
                    ci = cliente.ci,
                    nombre = cliente.nombre,
                    limiteCredito = credito.limiteCredito,
                    saldoUsado = credito.saldoUsado
                };

                return Ok(clienteCredito);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
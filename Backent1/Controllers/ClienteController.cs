using Backent1.Data;
using Backent1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backent1.Controllers
{
    [Route("api/v1/Cliente")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ClienteController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetActivo()
        {
            var clientes = await _context.clientes
                .Include(c => c.User)
                .Where(c => c.Estado == true && c.User.Estado == true)
                .ToListAsync();

            return Ok(clientes);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetById(int id)
        {
            var cliente = await _context.clientes
                .Include(c => c.User)
                .FirstOrDefaultAsync(c =>
                    c.Id == id &&
                    c.Estado &&
                    c.User.Estado
                );

            if (cliente == null)
            {
                return NotFound("Cliente no encontrado o inactivo");
            }

            return Ok(cliente);
        }
        [HttpPost]
        public async Task<ActionResult<Cliente>> Create(Cliente cliente)
        {
            // 🔹 1. Validar que el User exista
            var user = await _context.users.FindAsync(cliente.UserId);

            if (user == null)
            {
                return BadRequest("El usuario no existe");
            }

            // 🔹 2. Validar que el User esté activo
            if (!user.Estado)
            {
                return BadRequest("El usuario está inactivo");
            }

            // 🔹 3. Generar CodigoCliente automático
            var ultimoCliente = await _context.clientes
                .OrderByDescending(c => c.Id)
                .FirstOrDefaultAsync();

            int nuevoNumero = 1;

            if (ultimoCliente != null)
            {
                // extraer número del ultimo
                var numeroActual = int.Parse(ultimoCliente.CodigoCliente.Split('-')[1]);
                nuevoNumero = numeroActual + 1;
            }
            cliente.CodigoCliente = $"CLI-{nuevoNumero.ToString("D3")}";
            cliente.Estado = true;
            _context.clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return Ok(cliente);
        }
        [HttpPut("{Id}")]
        public async Task<ActionResult<Cliente>> Update(int Id, Cliente cliente)
        {
            // 🔹 1. Verificar que el cliente exista
            var clienteExiste = await _context.clientes.FindAsync(Id);

            if (clienteExiste == null)
            {
                return NotFound("Cliente no existe");
            }

            // 🔹 2. Verificar que el nuevo User exista
            var user = await _context.users.FindAsync(cliente.UserId);

            if (user == null)
            {
                return BadRequest("El usuario no existe");
            }

            // 🔹 3. Verificar que el User esté activo
            if (!user.Estado)
            {
                return BadRequest("El usuario está inactivo");
            }

            // 🔹 4. Actualizar SOLO lo permitido
            clienteExiste.UserId = cliente.UserId;

            // ⚠️ opcional: permitir cambiar estado
            clienteExiste.Estado = cliente.Estado;

            // ❌ NO tocar CodigoCliente (se mantiene)
            // ❌ NO tocar relaciones directamente

            await _context.SaveChangesAsync();

            return Ok(clienteExiste);

        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var cliente = await _context.clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound("Cliente no existe");
            }

            if (!cliente.Estado)
            {
                return BadRequest("El cliente ya está inactivo");
            }

            cliente.Estado = false;

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}


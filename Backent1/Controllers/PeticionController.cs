using Backent1.Data;
using Backent1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backent1.Controllers
{
    [Route("api/v1/Peticion")]
    [ApiController]
    public class PeticionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PeticionController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Peticion>>> GetActivo()
        {
            var Peticiones = await _context.peticiones
                .Include(c => c.Producto)
                .Include(c => c.Cliente)
                .Where(c => c.Estado == true && c.Cliente.Estado == true)
                .ToListAsync();

            return Ok(Peticiones);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Peticion>> GetById(int id)
        {
            var peticion = await _context.peticiones
                .Include(p => p.Cliente)
                .Include(p => p.Producto)
                .FirstOrDefaultAsync(p => p.Id == id && p.Estado && p.Cliente.Estado == true);

            if (peticion == null)
            {
                return NotFound("Petición no encontrada o inactiva");
            }

            return Ok(peticion);
        }
        [HttpPost]
        public async Task<ActionResult<Peticion>> Create(Peticion peticion)
        {
            // 🔹 Validar cliente
            var cliente = await _context.clientes.FindAsync(peticion.ClienteId);

            if (cliente == null || !cliente.Estado)
            {
                return BadRequest("Cliente no válido o inactivo");
            }

            // 🔹 Validar producto
            var producto = await _context.productos.FindAsync(peticion.ProductoId);

            if (producto == null || !producto.Estado)
            {
                return BadRequest("Producto no válido o inactivo");
            }

            // 🔹 Validaciones básicas
            if (string.IsNullOrWhiteSpace(peticion.Altura) ||
                string.IsNullOrWhiteSpace(peticion.Ancho))
            {
                return BadRequest("Altura y Ancho son obligatorios");
            }

            if (peticion.cantidad <= 0)
            {
                return BadRequest("La cantidad debe ser mayor a 0");
            }

            // 🔹 Estado automático
            peticion.Estado = true;

            _context.peticiones.Add(peticion);
            await _context.SaveChangesAsync();

            return Ok(peticion);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<Peticion>> Update(int id, Peticion peticion)
        {
            var peticionExiste = await _context.peticiones.FindAsync(id);

            if (peticionExiste == null)
            {
                return NotFound("Petición no existe");
            }

            // 🔹 Validar cliente
            var cliente = await _context.clientes.FindAsync(peticion.ClienteId);
            if (cliente == null || !cliente.Estado)
            {
                return BadRequest("Cliente inválido o inactivo");
            }

            // 🔹 Validar producto
            var producto = await _context.productos.FindAsync(peticion.ProductoId);
            if (producto == null || !producto.Estado)
            {
                return BadRequest("Producto inválido o inactivo");
            }

            // 🔹 Validaciones básicas
            if (peticion.cantidad <= 0)
            {
                return BadRequest("Cantidad inválida");
            }

            // 🔹 Actualizar
            peticionExiste.ClienteId = peticion.ClienteId;
            peticionExiste.ProductoId = peticion.ProductoId;
            peticionExiste.Altura = peticion.Altura;
            peticionExiste.Ancho = peticion.Ancho;
            peticionExiste.cantidad = peticion.cantidad;
            peticionExiste.Descripcion = peticion.Descripcion;
            peticionExiste.Estado = peticion.Estado;

            await _context.SaveChangesAsync();

            return Ok(peticionExiste);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var peticion = await _context.peticiones.FindAsync(id);

            if (peticion == null)
            {
                return NotFound("Petición no existe");
            }

            if (!peticion.Estado)
            {
                return BadRequest("La petición ya está inactiva");
            }

            peticion.Estado = false;

            await _context.SaveChangesAsync();

            return Ok("Petición eliminada correctamente");
        }

    }
}

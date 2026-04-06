using Backent1.Data;
using Backent1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backent1.Controllers
{
    [Route("api/v1/Trabajador")]
    [ApiController]
    public class TrabajadorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public TrabajadorController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Trabajador>>> GetActivo()
        {
            var Trabajadores = await _context.trabajadores
                .Include(c => c.User).Include(t => t.Area)
                .Where(c => c.Estado == true && c.User.Estado == true)
                .ToListAsync();

            return Ok(Trabajadores);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Trabajador>> GetById(int id)
        {
            var trabajador = await _context.trabajadores
                .Include(c => c.User)
                .FirstOrDefaultAsync(c =>
                    c.Id == id &&
                    c.Estado &&
                    c.User.Estado
                );

            if (trabajador == null)
            {
                return NotFound("Trabajador no encontrado o inactivo");
            }

            return Ok(trabajador);
        }
        [HttpPost]
        public async Task<ActionResult<Trabajador>> Create(Trabajador trabajador)
        {
            // 🔹 1. Validar USER
            var user = await _context.users.FindAsync(trabajador.UserId);

            if (user == null)
            {
                return BadRequest("El usuario no existe");
            }

            if (!user.Estado)
            {
                return BadRequest("El usuario está inactivo");
            }

            // 🔹 2. Validar AREA
            var area = await _context.areas.FindAsync(trabajador.AreaId);

            if (area == null)
            {
                return BadRequest("El área no existe");
            }

            // 🔹 3. Validar que no exista otro trabajador con ese UserId
            var existeTrabajador = await _context.trabajadores
                .AnyAsync(t => t.UserId == trabajador.UserId);

            if (existeTrabajador)
            {
                return BadRequest("Este usuario ya está asignado a un trabajador");
            }

            // 🔹 4. Generar Código Seguro
            var ultimo = await _context.trabajadores.OrderByDescending(t => t.Id).FirstOrDefaultAsync();

            int nuevoNumero = 1; // 🔥 valor por defecto SI está vacío

            if (ultimo != null && !string.IsNullOrEmpty(ultimo.CodigoTrabajador))
            {
                var partes = ultimo.CodigoTrabajador.Split('-');

                if (partes.Length == 2 && int.TryParse(partes[1], out int numeroActual))
                {
                    nuevoNumero = numeroActual + 1;
                }
            }

            // 🔥 Generar código final
            trabajador.CodigoTrabajador = $"CTD-{nuevoNumero:D3}";

            // 🔹 5. Estado automático
            trabajador.Estado = true;

            // 🔹 6. Guardar
            _context.trabajadores.Add(trabajador);
            await _context.SaveChangesAsync();

            return Ok(trabajador);
        }
        [HttpPut("{Id}")]
        public async Task<ActionResult<Trabajador>> Update(int Id, Trabajador trabajador)
        {
            var trabajadorExiste = await _context.trabajadores.FindAsync(Id);

            if (trabajadorExiste == null)
            {
                return NotFound("Trabajador no existe");
            }
            var user = await _context.users.FindAsync(trabajador.UserId);

            if (user == null)
            {
                return BadRequest("El usuario no existe");
            }

            if (!user.Estado)
            {
                return BadRequest("El usuario está inactivo");
            }

            trabajadorExiste.UserId = trabajador.UserId;
            trabajadorExiste.AreaId = trabajador.AreaId;
            trabajadorExiste.HorarioLaboral = trabajador.HorarioLaboral;
            trabajadorExiste.Estado = trabajador.Estado;
            await _context.SaveChangesAsync();

            return Ok(trabajadorExiste);

        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var trabajador = await _context.trabajadores.FindAsync(id);

            if (trabajador == null)
            {
                return NotFound("Cliente no existe");
            }

            if (!trabajador.Estado)
            {
                return BadRequest("El cliente ya está inactivo");
            }

            trabajador.Estado = false;

            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}

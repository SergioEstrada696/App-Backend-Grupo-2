using Backent1.Data;
using Backent1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backent1.Controllers
{
    [Route("api/v1/Area")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public AreaController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Area>>> GetActivo()
        {
            var areas = await _context.areas.Where(a => a.Estado).Include(t => t.trabajadors).ToListAsync();
            return Ok(areas);
        }
        [HttpGet("{Id}")]
        public async Task<ActionResult<Area>> GetId(int Id)
        {
            var areas = await _context.areas.Include(t => t.trabajadors).FirstOrDefaultAsync(u => u.Id == Id && u.Estado == true);
            if (areas == null)
            {
                return NotFound("Usuario no se encontro");
            }
            return Ok(areas);
        }
        [HttpPost]
        public async Task<ActionResult<Area>> Create(Area area)
        {
            // 🔹 1. Validar nombre
            if (string.IsNullOrWhiteSpace(area.Nombre))
            {
                return BadRequest("El nombre es obligatorio");
            }

            // 🔹 2. Validar descripción
            if (string.IsNullOrWhiteSpace(area.Descripcion))
            {
                return BadRequest("La descripción es obligatoria");
            }

            var nombre = area.Nombre.Trim().ToUpper();

            // 🔹 3. Validar duplicado
            var existe = await _context.areas
                .AnyAsync(a => a.Nombre.ToUpper() == nombre);

            if (existe)
            {
                return BadRequest("El área ya existe");
            }

            // 🔹 4. Normalizar datos
            area.Nombre = area.Nombre.Trim();
            area.Descripcion = area.Descripcion.Trim();

            // 🔹 5. Estado automático
            area.Estado = true;

            // 🔹 6. Guardar
            _context.areas.Add(area);
            await _context.SaveChangesAsync();

            return Ok(area);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<Area>> Update(int id, Area area)
        {
            var areaExiste = await _context.areas.FindAsync(id);

            if (areaExiste == null)
            {
                return NotFound("Área no existe");
            }

            if (string.IsNullOrWhiteSpace(area.Nombre))
            {
                return BadRequest("El nombre es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(area.Descripcion))
            {
                return BadRequest("La descripción es obligatoria");
            }

            var nombre = area.Nombre.Trim().ToUpper();

            var existe = await _context.areas
                .AnyAsync(a => a.Id != id && a.Nombre.ToUpper() == nombre);

            if (existe)
            {
                return BadRequest("Ya existe un área con ese nombre");
            }

            areaExiste.Nombre = area.Nombre.Trim();
            areaExiste.Descripcion = area.Descripcion.Trim();
            areaExiste.Estado = area.Estado;

            await _context.SaveChangesAsync();

            return Ok(areaExiste);
        }
        [HttpDelete("{Id}")]
        public async Task<ActionResult<Area>> Delete(int Id)
        {
            var areas = await _context.areas.FindAsync(Id);
            if (areas == null)
            {
                return NotFound();
            }
            if (areas.Estado == false)
            {
                return NotFound();
            }
            areas.Estado = false;
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}

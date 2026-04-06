using Backent1.Data;
using Backent1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backent1.Controllers
{
    [Route("api/v1/Categoria")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CategoriaController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ActionResult<IEnumerable<Categoria>>> GetActivo()
        {
            var caracteristicas = await _context.categorias.Include(t => t.productos).Where(s => s.Estado == true).ToListAsync();
            return Ok(caracteristicas);
        }
        [HttpGet("{Id}")]
        public async Task<ActionResult<Categoria>> GetId(int Id)
        {
            var caracteristicas = await _context.categorias.Include(t => t.productos).FirstOrDefaultAsync(u => u.Id == Id && u.Estado == true);
            if (caracteristicas == null)
            {
                return NotFound("Usuario no se encontro");
            }
            return Ok(caracteristicas);
        }
        [HttpPost]
        public async Task<ActionResult<Categoria>> Create(Categoria categoria)
        {
            // 🔹 2. Validar que no exista (ignorando mayúsculas/minúsculas)
            var existe = await _context.categorias
                .AnyAsync(c => c.Nombre.ToLower() == categoria.Nombre.ToLower());

            if (existe)
            {
                return BadRequest("La categoría ya existe");
            }
            categoria.Estado = true;
            _context.categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return Ok(categoria);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<Categoria>> Update(int id, Categoria categoria)
        {
            // 🔹 1. Validar que exista
            var categoriaExiste = await _context.categorias.FindAsync(id);

            if (categoriaExiste == null)
            {
                return NotFound("Categoría no existe");
            }

            var nombre = categoria.Nombre.Trim().ToUpper();

            // 🔹 3. Validar duplicado (EXCLUYENDO el mismo ID)
            var existe = await _context.categorias
                .AnyAsync(c => c.Id != id && c.Nombre.ToUpper() == nombre);

            if (existe)
            {
                return BadRequest("Ya existe una categoría con ese nombre");
            }

            // 🔹 4. Actualizar datos
            categoriaExiste.Nombre = categoria.Nombre.Trim();

            categoriaExiste.Estado = categoria.Estado;

            await _context.SaveChangesAsync();

            return Ok(categoriaExiste);
        }
        [HttpDelete("{Id}")]
        public async Task<ActionResult<Categoria>> Delete(int Id)
        {
            var categoria = await _context.categorias.FindAsync(Id);
            if (categoria == null)
            {
                return NotFound();
            }
            if (categoria.Estado == false)
            {
                return NotFound();
            }
            categoria.Estado = false;
            await _context.SaveChangesAsync();
            return Ok();
        }

    }
}

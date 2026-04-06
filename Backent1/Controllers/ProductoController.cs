using Backent1.Data;
using Backent1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backent1.Controllers
{
    [Route("api/v1/Producto")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ProductoController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetActivo()
        {
            var Categoria = await _context.productos
                .Include(c => c.Categoria)
                .Where(c => c.Estado == true && c.Categoria.Estado == true)
                .ToListAsync();

            return Ok(Categoria);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetById(int id)
        {
            var cliente = await _context.productos
                .Include(c => c.Categoria)
                .FirstOrDefaultAsync(c =>
                    c.Id == id &&
                    c.Estado &&
                    c.Categoria.Estado
                );

            if (cliente == null)
            {
                return NotFound("`Producto no encontrado o inactivo");
            }

            return Ok(cliente);
        }
        [HttpPost]
        public async Task<ActionResult<Producto>> Create(Producto producto)
        {
            var categoria = await _context.categorias.FindAsync(producto.CategoriaId);

            if (categoria == null)
            {
                return BadRequest("La categoría no existe");
            }
            if (!categoria.Estado)
            {
                return BadRequest("La categoría está inactiva");
            }
            var nombre = producto.Nombre.Trim().ToUpper();

            var existe = await _context.productos
                .AnyAsync(p => p.Nombre.ToUpper() == nombre);
            if (existe)
            {
                return BadRequest("El producto ya existe");
            }
            producto.Estado = true;
            _context.productos.Add(producto);
            await _context.SaveChangesAsync();

            return Ok(producto);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<Producto>> Update(int id, Producto producto)
        {
            var productoExiste = await _context.productos.FindAsync(id);

            if (productoExiste == null)
            {
                return NotFound("Producto no existe");
            }

            // 🔹 2. Validar nombre
            if (string.IsNullOrWhiteSpace(producto.Nombre))
            {
                return BadRequest("El nombre es obligatorio");
            }

            // 🔹 3. Validar descripción
            if (string.IsNullOrWhiteSpace(producto.Descripcion))
            {
                return BadRequest("La descripción es obligatoria");
            }

            // 🔹 4. Validar precio
            if (string.IsNullOrWhiteSpace(producto.Precio))
            {
                return BadRequest("El precio es obligatorio");
            }

            // 🔹 5. Validar categoría
            var categoria = await _context.categorias.FindAsync(producto.CategoriaId);

            if (categoria == null)
            {
                return BadRequest("La categoría no existe");
            }

            if (!categoria.Estado)
            {
                return BadRequest("La categoría está inactiva");
            }

            // 🔹 6. Validar duplicado (EXCLUYENDO el mismo ID)
            var nombre = producto.Nombre.Trim().ToUpper();

            var existe = await _context.productos
                .AnyAsync(p => p.Id != id && p.Nombre.ToUpper() == nombre);

            if (existe)
            {
                return BadRequest("Ya existe un producto con ese nombre");
            }

            // 🔹 7. Actualizar datos
            productoExiste.Nombre = producto.Nombre.Trim();
            productoExiste.Descripcion = producto.Descripcion.Trim();
            productoExiste.Precio = producto.Precio;
            productoExiste.Imagen = producto.Imagen;
            productoExiste.CategoriaId = producto.CategoriaId;

            // ⚠️ opcional
            productoExiste.Estado = producto.Estado;

            await _context.SaveChangesAsync();

            return Ok(productoExiste);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var producto = await _context.productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound("Producto no existe");
            }

            if (!producto.Estado)
            {
                return BadRequest("El producto ya está inactivo");
            }

            producto.Estado = false;

            await _context.SaveChangesAsync();

            return Ok("Producto eliminado correctamente");
        }
    }
}

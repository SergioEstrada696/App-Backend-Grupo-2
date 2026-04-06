using Backent1.Data;
using Backent1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backent1.Controllers
{
    [Route("api/v1/User")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetActivo() 
        {
            var users = await _context.users.Include(t => t.Clientes).Include(t => t.trabajadors).Where(s => s.Estado == true).ToListAsync();
            return Ok(users);
        }
        [HttpGet("{Id}")]
        public async Task<ActionResult<User>> GetId(int Id)
        {
            var user = await _context.users.Include(t => t.Clientes).Include(t => t.trabajadors).FirstOrDefaultAsync(u => u.Id == Id && u.Estado == true);
            if (user == null) 
            {
                return NotFound("Usuario no se encontro");
            }
            return Ok(user);
        }
        [HttpPost]
        public async Task<ActionResult<User>> Create(User user)
        {
            var correo = user.Email.Trim().ToUpper();
            var existe = await _context.users
                .AnyAsync(u => u.Email.ToUpper() == correo);
            user.Estado = true;
            user.FechaRegistro = DateTime.Now;
            _context.users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }
        [HttpPut("{Id}")]
        public async Task<ActionResult<User>> Update(int Id, User user)
        {
            var userExistente = await _context.users.FindAsync(Id);

            if (userExistente == null)
            {
                return NotFound("Usuario no encontrado");
            }
            var correo = user.Email.Trim().ToUpper();
            var existe = await _context.users
                .AnyAsync(u => u.Id != Id && u.Email.ToUpper() == correo);

            if (existe)
            {
                return BadRequest("El correo ya está registrado en otro usuario");
            }
            userExistente.Nombre = user.Nombre;
            userExistente.Apellido = user.Apellido;
            userExistente.Email = user.Email;
            userExistente.Password = user.Password;
            userExistente.Telefono = user.Telefono;
            userExistente.Estado = user.Estado;
            await _context.SaveChangesAsync();
            return Ok(userExistente);
        }
        [HttpDelete("{Id}")]
        public async Task<ActionResult<User>> Delete(int Id) 
        {
            var user = await _context.users.FindAsync(Id);
            if (user == null)
            {
                return NotFound();
            }
            if (user.Estado == false)
            {
                return NotFound();
            }
            user.Estado = false;
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}

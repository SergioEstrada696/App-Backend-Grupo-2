using Backent1.Models;
using Microsoft.EntityFrameworkCore;

namespace Backent1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<User> users { get; set; }
        public DbSet<Area> areas { get; set; }
        public DbSet<Categoria> categorias { get; set; }
        public DbSet<Cliente> clientes { get; set; }
        public DbSet<Trabajador> trabajadores { get; set; }
        public DbSet<Producto> productos { get; set; }
        public DbSet<Peticion> peticiones { get; set; }
    }
}

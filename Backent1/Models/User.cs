namespace Backent1.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Telefono { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Estado { get; set; }
        public List<Cliente>? Clientes { get; set; }
        public List<Trabajador>? trabajadors { get; set; }

    }
}

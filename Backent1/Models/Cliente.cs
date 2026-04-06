namespace Backent1.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public string? CodigoCliente { get; set; }
        public bool Estado { get; set; }
        public List<Peticion>? peticions { get; set; }
    }
}

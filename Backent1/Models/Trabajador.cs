namespace Backent1.Models
{
    public class Trabajador
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int AreaId { get; set; }
        public Area? Area { get; set; }
        public string? CodigoTrabajador { get; set; }
        public string? HorarioLaboral { get; set; }
        public bool Estado { get; set; }
    }
}

namespace Backent1.Models
{
    public class Peticion
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }
        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }
        public string Altura { get; set; }
        public string Ancho { get; set; }
        public int cantidad { get; set; }
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }
    }
}

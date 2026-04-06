namespace Backent1.Models
{
    public class Producto
    {
        public int Id { get; set; }

        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }
        public string Nombre { get; set; }
        public string? Imagen { get; set; }
        public string Descripcion { get; set; }
        public string Precio { get; set; }
        public bool Estado { get; set; }
        public List<Peticion>? peticions { get; set; }
    }
}

namespace Backent1.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Estado { get; set; }
        public List<Producto>? productos { get; set; }
    }
}

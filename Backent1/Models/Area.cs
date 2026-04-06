namespace Backent1.Models
{
    public class Area
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Estado { get; set; }
        public List<Trabajador>? trabajadors { get; set; }

        
    }
}

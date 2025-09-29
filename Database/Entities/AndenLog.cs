namespace Entities
{
    public class AndenLog
    {
        public int Id { get; set; }
        public int AndenId { get; set; }
        public Anden Anden { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }
    }
}

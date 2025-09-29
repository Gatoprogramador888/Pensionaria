namespace Entities
{
    public class PensionLog
    {
        public int Id { get; set; }
        public int PensionId { get; set; }
        public Pension Pension { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }
    }
}

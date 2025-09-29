namespace Entities
{
    public class Camion
    {
        public int Id { get; set; }
        public string Placa { get; set; }
        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; }
    }
}

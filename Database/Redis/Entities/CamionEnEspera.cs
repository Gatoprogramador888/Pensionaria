using Redis.Enums;
using System.Text.Json.Serialization;

namespace Redis.Entities
{
    public class CamionEnEspera
    {
        public string IdTemporalRedis { get; set; }
        public string Placa { get; set; }
        public string Conductor { get; set; }
        public string TipoCamion { get; set; }
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ServicioTipo Servicio { get; set; }
        
        public DateTime HoraLlegada { get; set; }
        public string? Empresa { get; set; }
        public string? Observaciones { get; set; }

        public CamionEnEspera()
        {
            IdTemporalRedis = Guid.NewGuid().ToString();
            HoraLlegada = DateTime.Now;
        }
    }
}
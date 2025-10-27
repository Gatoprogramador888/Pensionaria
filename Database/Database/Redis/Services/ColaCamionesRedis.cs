using StackExchange.Redis;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Database.Database.Redis;
using Database.Database.Redis.Entities;

namespace Database.Database.Redis.Services
{
    public class ColaCamionesRedis
    {
        private readonly IDatabase _db;
        private readonly ILogger<ColaCamionesRedis> _logger;
        private readonly string _queueKey = "cola:camiones:sinCita";
        private readonly string _hashPrefix = "camion:espera:";
        private readonly double time = 5; // horas

        public ColaCamionesRedis(conexionRedis conexion, ILogger<ColaCamionesRedis> logger)
        {
            _db = conexion.Database;
            _logger = logger;
        }

        /// <summary>
        /// Encola un camión sin cita en Redis
        /// </summary>
        public string EncolarCamion(CamionEnEspera camion)
        {
            try
            {
                // Generar ID temporal si no existe
                if (string.IsNullOrEmpty(camion.IdTemporalRedis))
                {
                    camion.IdTemporalRedis = Guid.NewGuid().ToString();
                }

                // Guardar los datos del camión en un Hash de Redis
                string hashKey = $"{_hashPrefix}{camion.IdTemporalRedis}";
                var camionJson = JsonSerializer.Serialize(camion);

                // Guardar el objeto completo en Redis Hash
                _db.StringSet(hashKey, camionJson);

                // Agregar el ID a la cola (List) para mantener el orden FIFO
                _db.ListRightPush(_queueKey, camion.IdTemporalRedis);

                // Opcional: Establecer expiración de 24 horas por seguridad
                _db.KeyExpire(hashKey, TimeSpan.FromHours(time));

                _logger.LogInformation("Camion {Placa} encolado con ID: {Id}", camion.Placa, camion.IdTemporalRedis);
                return camion.IdTemporalRedis;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al encolar camion");
                throw;
            }
        }

        /// <summary>
        /// Desencola el siguiente camión en espera (FIFO)
        /// </summary>
        public CamionEnEspera? DesencolarCamion()
        {
            try
            {
                // Obtener el primer ID de la cola
                var idTemporal = _db.ListLeftPop(_queueKey);
                
                if (!idTemporal.HasValue)
                {
                    _logger.LogInformation("Cola vacia, no hay camiones en espera");
                    return null;
                }

                // Recuperar los datos del camión usando el ID
                string hashKey = $"{_hashPrefix}{idTemporal}";
                var camionJson = _db.StringGet(hashKey);

                if (!camionJson.HasValue)
                {
                    _logger.LogWarning("Datos del camion {Id} no encontrados en Redis", idTemporal);
                    return null;
                }

                var camion = JsonSerializer.Deserialize<CamionEnEspera>(camionJson.ToString());

                // Eliminar el hash del camión de Redis
                _db.KeyDelete(hashKey);

                _logger.LogInformation("Camion {Placa} desencolado", camion?.Placa);
                return camion;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desencolar camion");
                throw;
            }
        }

        /// <summary>
        /// Ver el siguiente camión sin desencolarlo (Peek)
        /// </summary>
        public CamionEnEspera? VerSiguienteCamion()
        {
            try
            {
                var ids = _db.ListRange(_queueKey, 0, 0);
                if (ids.Length == 0) return null;

                string hashKey = $"{_hashPrefix}{ids[0]}";
                var camionJson = _db.StringGet(hashKey);

                if (!camionJson.HasValue) return null;

                return JsonSerializer.Deserialize<CamionEnEspera>(camionJson.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al ver siguiente camion");
                return null;
            }
        }

        /// <summary>
        /// Obtiene toda la cola actual de camiones en espera
        /// </summary>
        public List<CamionEnEspera> ObtenerColaCompleta()
        {
            try
            {
                var ids = _db.ListRange(_queueKey);
                var camiones = new List<CamionEnEspera>();

                foreach (var id in ids)
                {
                    string hashKey = $"{_hashPrefix}{id}";
                    var camionJson = _db.StringGet(hashKey);

                    if (camionJson.HasValue)
                    {
                        var camion = JsonSerializer.Deserialize<CamionEnEspera>(camionJson.ToString());
                        if (camion != null)
                        {
                            camiones.Add(camion);
                        }
                    }
                }

                return camiones;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cola completa");
                throw;
            }
        }

        /// <summary>
        /// Obtiene la cantidad de camiones en espera
        /// </summary>
        public long CantidadEnCola()
        {
            return _db.ListLength(_queueKey);
        }

        /// <summary>
        /// Busca un camión específico en la cola por placa
        /// </summary>
        public CamionEnEspera? BuscarPorPlaca(string placa)
        {
            try
            {
                var camiones = ObtenerColaCompleta();
                return camiones.FirstOrDefault(c => 
                    c.Placa.Equals(placa, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar camion por placa");
                return null;
            }
        }

        /// <summary>
        /// Elimina un camión específico de la cola por su ID temporal
        /// </summary>
        public bool EliminarCamionPorId(string idTemporal)
        {
            try
            {
                // Remover de la lista (cola)
                var removed = _db.ListRemove(_queueKey, idTemporal);
                
                // Eliminar el hash con los datos
                string hashKey = $"{_hashPrefix}{idTemporal}";
                _db.KeyDelete(hashKey);

                if (removed > 0)
                {
                    _logger.LogInformation("Camion con ID {Id} eliminado de la cola", idTemporal);
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar camion");
                return false;
            }
        }

        /// <summary>
        /// Limpia completamente la cola de camiones
        /// </summary>
        public void LimpiarCola()
        {
            try
            {
                // Obtener todos los IDs antes de limpiar
                var ids = _db.ListRange(_queueKey);

                // Eliminar todos los hashes
                foreach (var id in ids)
                {
                    string hashKey = $"{_hashPrefix}{id}";
                    _db.KeyDelete(hashKey);
                }

                // Limpiar la cola
                _db.KeyDelete(_queueKey);

                _logger.LogInformation("Cola de camiones limpiada completamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al limpiar cola");
                throw;
            }
        }

        /// <summary>
        /// Obtiene estadísticas de la cola
        /// </summary>
        public string ObtenerEstadisticas()
        {
            var camiones = ObtenerColaCompleta();
            var total = camiones.Count;

            if (total == 0)
                return "Cola vacia - No hay camiones en espera";

            var porServicio = camiones.GroupBy(c => c.Servicio)
                .Select(g => $"{g.Key}: {g.Count()}")
                .ToList();

            var tiempoEsperaMayor = camiones
                .Max(c => (DateTime.Now - c.HoraLlegada).TotalMinutes);

            return $@"
Estadisticas de Cola:
   Total en espera: {total}
   Por servicio: {string.Join(", ", porServicio)}
   Tiempo espera mayor: {tiempoEsperaMayor:F0} minutos
   Primer camion: {camiones.First().Placa} (llego {camiones.First().HoraLlegada:HH:mm})
            ";
        }
    }
}
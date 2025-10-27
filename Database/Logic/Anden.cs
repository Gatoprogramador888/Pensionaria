using Database.Database.MySQL.Entities;
using Database.Database.Redis;
using Microsoft.EntityFrameworkCore;
using Database.Database.MySQL.Contexto;
using Database.Database.Redis.Services;


namespace Pensionaria.Logic
{
    public class Anden
    {
        private readonly DBContexto _context;
        private readonly ColaCamionesRedis _redisContext;

        public Anden(DBContexto context, conexionRedis conexionRedis, ILogger<ColaCamionesRedis> log)
        {
          _context = context;
          _redisContext = new ColaCamionesRedis(conexionRedis, log);
        }

        private void Calculo(int anden)
        {
      // Implementation for calculations related to the platform
        }

        public async Task<bool> AgendarCita(Camion camion, DateTime entrada, DateTime salida, int id_anden)
        {
              try
              {
               // Verificar si el andén está disponible
                            var andenLog = await _context.AndenLogs
                 .FirstOrDefaultAsync(a => a.HoraEntrada != entrada);

                  if (andenLog != null)
                     {
                        throw new Exception("Anden no disponible en esa hora seleccion otra.");
                     }

                // Crear el registro de cita en AndenLog
                     var nuevaCita = new AndenLog
                     {
                      NumeroAnden = id_anden,
                      Placa = camion.Placa,
                        Conductor = camion.Conductor,
                         HoraEntrada = entrada,
                         HoraSalida = salida,
                            Estado = EstadoLog.Proceso
                     };


                 // Guardar los cambios
                            _context.AndenLogs.Add(nuevaCita);
               await _context.SaveChangesAsync();

                   return true;
                }
                catch (Exception ex)
                {
                // Aquí podrías agregar logging
                throw new Exception($"Error al agendar cita: {ex.Message}");
                }
        }

        public async Task<List<AndenLog>> CitasPendientesEnAnden(int anden)
        {
            var citas = await _context.AndenLogs
                .Where(a => a.NumeroAnden == anden && a.Estado == EstadoLog.Proceso)
                .ToListAsync();

            return citas;
        }

        public async Task<List<AndenLog>> CitasPendientes()
        {
            var citas = await _context.AndenLogs
                .Where(a => a.Estado == EstadoLog.Proceso)
                .ToListAsync();
            return citas;
        }

        public async Task<bool> AndenOcupado(int anden)
        {
            var ocupado = await _context.Andenes
                .AnyAsync(a => a.NumeroAnden == anden && a.Estado != EstadoGeneral.Disponible);

            return ocupado;
        }

        public void LiberarAndenConCita(int anden, Camion camion)
        {
            var andenEntity = _context.Andenes.FirstOrDefault(a => a.NumeroAnden == anden);

            if (andenEntity == null)
            {
                throw new Exception("Anden no encontrado.");
            }

            andenEntity.Estado = EstadoGeneral.Disponible;
            _context.SaveChanges();
        }

        public void LiberarAndenSinCita(int anden, DateTime entrada, DateTime salida)
        {
            var andenEntity = _context.Andenes.FirstOrDefault(a => a.NumeroAnden == anden);

            if (andenEntity == null)
            {
                throw new Exception("Anden no encontrado.");
            }

            andenEntity.Estado = EstadoGeneral.Disponible;
            _context.SaveChanges();
        }

        /*En caso de que el anden con cita no este disponible se avisara y ya el empleado actuara
         Pero si el anden esta disponible se procedera a ingresar el vehiculo
         */
        public void IngresarVehiculoConCita(int id_anden, Camion vehiculo)
        {
            var anden = _context.Andenes.FirstOrDefault(a => a.NumeroAnden == id_anden);
            if (anden == null)
            {
                throw new Exception("Anden no encontrado.");
            }

            if (anden.Estado != EstadoGeneral.Disponible)
            {
                throw new Exception("Anden no disponible.");
            }

            anden.Estado = EstadoGeneral.Ocupado;
            _context.SaveChanges();

        }

        public void IngresarVehiculoSinCita(int id_anden, Camion vehiculo)
        {
            var anden = _context.Andenes.FirstOrDefault(a => a.NumeroAnden == id_anden);
            if (anden == null)
            {
                throw new Exception("Anden no encontrado.");
            }

            if (anden.Estado != EstadoGeneral.Disponible)
            {
                throw new Exception("Anden no disponible.");
            }

            //Liberar un lugar en el redis de vehiculos pendientes
            _redisContext.DesencolarCamion();
            anden.Estado = EstadoGeneral.Ocupado;
            _context.SaveChanges();
        }

        public void ColaVehiculosPendientes()
        {
            _redisContext.DesencolarCamion();
        }
    }
}

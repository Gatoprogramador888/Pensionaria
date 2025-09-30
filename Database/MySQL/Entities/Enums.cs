namespace Database.MySQL.Entities
{
    public enum ServicioTipo
    {
        Carga,
        Descarga,
        CD, // c/d
        Pension
    }

    public enum EstadoGeneral
    {
        Disponible,
        Ocupado,
        Mantenimiento
    }

    public enum EstadoLog
    {
        Proceso,
        Finalizado
    }

    public enum EstadoPensionLog
    {
        Disponible,
        Ocupado,
        Finalizado,
        Excedido
    }

    public enum TipoEvento
    {
        Error,
        Advertencia,
        Accion
    }
}
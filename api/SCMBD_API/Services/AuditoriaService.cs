namespace SCMBD_API.Services;

public class AuditoriaService
{
    private readonly ILogger<AuditoriaService> _logger;

    public AuditoriaService(
        ILogger<AuditoriaService> logger)
    {
        _logger = logger;
    }

    public void Info(
        string mensaje)
    {
        _logger.LogInformation(
            mensaje);
    }

    public void Warning(
        string mensaje)
    {
        _logger.LogWarning(
            mensaje);
    }

    public void Error(
        string mensaje)
    {
        _logger.LogError(
            mensaje);
    }

    public void Error(
        Exception ex)
    {
        _logger.LogError(
            ex,
            ex.Message);
    }

    public void Error(
        Exception ex,
        string mensaje)
    {
        _logger.LogError(
            ex,
            mensaje);
    }

    public void Consulta(
        string baseDatos,
        string sql)
    {
        _logger.LogInformation(
            "BD={BaseDatos} SQL={Sql}",
            baseDatos,
            sql);
    }

    public void Tiempo(
        string baseDatos,
        double milisegundos)
    {
        _logger.LogInformation(
            "BD={BaseDatos} Tiempo={Tiempo} ms",
            baseDatos,
            milisegundos);
    }
}
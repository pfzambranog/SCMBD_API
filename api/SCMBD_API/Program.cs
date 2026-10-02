using System.Data;
using System.Text;
using System.Xml.Linq;
using SCMBD_API.Models;
using SCMBD_API.Services;

const int COMMAND_TIMEOUT = 300;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();

builder.Services.AddScoped<ParametrosService>();
builder.Services.AddScoped<GestorCredenciales>();
builder.Services.AddScoped<AuditoriaService>();

builder.Services.AddCors(o =>
{
    o.AddPolicy("Todo", p =>
    {
        p.AllowAnyOrigin()
         .AllowAnyMethod()
         .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("Todo");

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/xml";

        await context.Response.WriteAsync(
            XmlResponse.Error(
                999,
                ex.Message),
            Encoding.UTF8);
    }
});

app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        estado = "OK",
        version = "2.0",
        cacheSP = !string.IsNullOrWhiteSpace(
            AppCache.SpDecode)
                ? "ACTIVA"
                : "VACIA",
        fecha = DateTime.Now.ToString("o")
    });
});

app.MapPost("/consulta",
async (
    HttpContext context,
    ParametrosService parametrosService,
    GestorCredenciales gestorCredenciales,
    AuditoriaService auditoria) =>
{
    try
    {
        using var reader =
            new StreamReader(
                context.Request.Body);

        string json =
            await reader.ReadToEndAsync();

        if (string.IsNullOrWhiteSpace(json))
        {
            return Results.Content(
                XmlResponse.Error(
                    900,
                    "JSON vacío"),
                "application/xml");
        }

        List<Solicitud> solicitudes =
            Solicitud.Deserializar(json);

        XElement resultados =
            new XElement("Resultados");

        foreach (var solicitud in solicitudes)
        {
            XElement resultado =
                await ProcesarSolicitud(
                    solicitud,
                    parametrosService,
                    gestorCredenciales,
                    auditoria);

            resultados.Add(resultado);
        }

        XElement respuesta =
            new XElement(
                "RespuestaGlobal",
                resultados);

        return Results.Content(
            respuesta.ToString(),
            "application/xml");
    }
    catch (Exception ex)
    {
        auditoria.Error(
            ex,
            "Error general");

        return Results.Content(
            XmlResponse.Error(
                999,
                ex.Message),
            "application/xml");
    }
});

Console.WriteLine("====================================");
Console.WriteLine(" SCMBD API v2.0 ");
Console.WriteLine(" API Lista Para Recibir Solicitudes ");
Console.WriteLine("====================================");

app.Run();

static async Task<XElement> ProcesarSolicitud(
    Solicitud solicitud,
    ParametrosService parametrosService,
    GestorCredenciales gestorCredenciales,
    AuditoriaService auditoria)
{
    var inicio = DateTime.Now;

    try
    {
        ValidadorSolicitud.Validar(
            solicitud);

        ValidarSql(
            solicitud.Sql);

        string passwordControl =
            gestorCredenciales
                .ObtenerPasswordControl(
                    solicitud.Control.PasswordCifrada);

        var controlDestino =
            new Destino
            {
                Servidor =
                    solicitud.Control.Servidor,

                Puerto =
                    solicitud.Control.Puerto,

                BaseDatos =
                    solicitud.Control.BaseDatos,

                Usuario =
                    solicitud.Control.Usuario
            };

        using var cnControl =
            ConexionFactory.Crear(
                controlDestino,
                passwordControl);

        await cnControl.OpenAsync();

        string passwordDestino =
            gestorCredenciales
                .ObtenerPasswordDestino(
                    cnControl,
                    solicitud.Destino.PasswordCifrada);

        using var cnDestino =
            ConexionFactory.Crear(
                solicitud.Destino,
                passwordDestino);

        await cnDestino.OpenAsync();

        using var cmd =
            cnDestino.CreateCommand();

        cmd.CommandText =
            solicitud.Sql;

        cmd.CommandTimeout =
            solicitud.Timeout > 0
                ? solicitud.Timeout
                : COMMAND_TIMEOUT;

        switch (solicitud.Tipo)
        {
            case 1:
                {
                    using var dr =
                        await cmd.ExecuteReaderAsync();

                    return XmlResponse
                        .GenerarConsulta(dr);
                }

            case 2:
                {
                    await cmd.ExecuteNonQueryAsync();

                    return XmlResponse
                        .ResultadoOk();
                }

            default:
                {
                    return XmlResponse
                        .ResultadoError(
                            998,
                            $"Tipo [{solicitud.Tipo}] no soportado.");
                }
        }
    }
    catch (Exception ex)
    {
        auditoria.Error(
            ex,
            $"Error Base={solicitud.Destino.BaseDatos}");

        return XmlResponse.ResultadoError(
            999,
            ex.Message);
    }
    finally
    {
        auditoria.Info(
            $"Base={solicitud.Destino.BaseDatos} Tiempo={DateTime.Now.Subtract(inicio).TotalMilliseconds} ms");
    }
}

static void ValidarSql(
    string sql)
{
    string texto =
        sql.ToUpperInvariant();

    string[] prohibidas =
    {
        "DROP ",
        "TRUNCATE ",
        "ALTER ",
        "SHUTDOWN ",
        "XP_CMDSHELL",
        "SP_CONFIGURE"
    };

    foreach (var palabra in prohibidas)
    {
        if (texto.Contains(palabra))
        {
            throw new Exception(
                $"Instrucción no permitida: {palabra}");
        }
    }
}
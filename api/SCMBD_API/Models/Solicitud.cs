using System.Text.Json;

namespace SCMBD_API.Models;

public class Solicitud
{
    public Control Control { get; set; } = new();

    public Destino Destino { get; set; } = new();

    public string Sql { get; set; } = string.Empty;

    public int Tipo { get; set; } = 1;

    public int Timeout { get; set; } = 300;

    public static List<Solicitud> Deserializar(
        string json)
    {
        return JsonSerializer.Deserialize<List<Solicitud>>(
                   json,
                   new JsonSerializerOptions
                   {
                       PropertyNameCaseInsensitive = true
                   })
               ?? new List<Solicitud>();
    }
}
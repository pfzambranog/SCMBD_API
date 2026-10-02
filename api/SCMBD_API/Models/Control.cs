namespace SCMBD_API.Models;

public class Control
{
    public string Servidor { get; set; } = "";

    public string Puerto { get; set; } = "";

    public string BaseDatos { get; set; } = "";

    public string Usuario { get; set; } = "";

    public string PasswordCifrada { get; set; } = "";
}
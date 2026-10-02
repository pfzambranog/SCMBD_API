using Microsoft.Data.SqlClient;
using SCMBD_API.Models;

namespace SCMBD_API.Services;

public static class ConexionFactory
{
    public static SqlConnection Crear(
        Destino destino,
        string password)
    {
        SqlConnectionStringBuilder cs =
            new();

        cs.DataSource =
            $"{destino.Servidor},{destino.Puerto}";

        cs.InitialCatalog =
            destino.BaseDatos;

        cs.UserID =
            destino.Usuario;

        cs.Password =
            password;

        cs.TrustServerCertificate =
            true;

        cs.ConnectTimeout =
            30;

        cs.MultipleActiveResultSets =
            true;

        return new SqlConnection(
            cs.ConnectionString);
    }
}
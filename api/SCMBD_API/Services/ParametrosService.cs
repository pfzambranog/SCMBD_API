using Microsoft.Data.SqlClient;

namespace SCMBD_API.Services;

public class ParametrosService
{
    public string ObtenerNombreSpDecode(
        SqlConnection cn)
    {
        bool cacheActivo =
            !string.IsNullOrWhiteSpace(
                AppCache.SpDecode)
            &&
            DateTime.Now <
            AppCache.SpDecodeFecha
                .AddMinutes(15);

        if (cacheActivo)
            return AppCache.SpDecode!;

        using SqlCommand cmd =
            cn.CreateCommand();

        cmd.CommandText =
        """
        SELECT parametroChar
        FROM dbo.conParametrosGralesTbl
        WHERE idParametroGral = 41
        """;

        AppCache.SpDecode =
            Convert.ToString(
                cmd.ExecuteScalar());

        AppCache.SpDecodeFecha =
            DateTime.Now;

        return AppCache.SpDecode!;
    }
}
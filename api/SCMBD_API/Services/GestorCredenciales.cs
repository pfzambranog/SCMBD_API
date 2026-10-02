using System.Data;
using System.Security.Cryptography;
using Microsoft.Data.SqlClient;

namespace SCMBD_API.Services;

public class GestorCredenciales
{
    private readonly ParametrosService _parametros;

    public GestorCredenciales(
        ParametrosService parametros)
    {
        _parametros = parametros;
    }

    private static readonly byte[] ClaveMaestra =
    {
        0x53, 0x43, 0x4D, 0x42, 0x44, 0x2D, 0x41, 0x50,
        0x49, 0x53, 0x45, 0x47, 0x2D, 0x32, 0x30, 0x32,
        0x34, 0x2D, 0x43, 0x4C, 0x41, 0x56, 0x45, 0x2D,
        0x53, 0x45, 0x47, 0x55, 0x52, 0x41, 0x31, 0x32
    };

    public string ObtenerPasswordControl(
        string passwordCifrada)
    {
        return DescifrarAES(
            passwordCifrada);
    }

    public string ObtenerPasswordDestino(
        SqlConnection conexionControl,
        string passwordCifrada)
    {
        string nombreSp =
            _parametros.ObtenerNombreSpDecode(
                conexionControl);

        using SqlCommand cmd =
            conexionControl.CreateCommand();

        cmd.CommandText =
            nombreSp;

        cmd.CommandType =
            CommandType.StoredProcedure;

        cmd.CommandTimeout = 30;

        cmd.Parameters.Add(
            new SqlParameter(
                "@PsInputBase64",
                passwordCifrada));

        cmd.Parameters.Add(
            new SqlParameter(
                "@PsOutputClaro",
                SqlDbType.NVarChar,
                4000)
            {
                Direction =
                    ParameterDirection.Output
            });

        cmd.Parameters.Add(
            new SqlParameter(
                "@PnEstatus",
                SqlDbType.Int)
            {
                Direction =
                    ParameterDirection.Output
            });

        cmd.Parameters.Add(
            new SqlParameter(
                "@PsMensaje",
                SqlDbType.VarChar,
                4000)
            {
                Direction =
                    ParameterDirection.Output
            });

        cmd.ExecuteNonQuery();

        int estatus =
            Convert.ToInt32(
                cmd.Parameters["@PnEstatus"].Value);

        string mensaje =
            Convert.ToString(
                cmd.Parameters["@PsMensaje"].Value)
            ?? "Error desconocido";

        if (estatus != 0)
        {
            throw new Exception(
                $"Error al descifrar credencial destino: {mensaje}");
        }

        return Convert.ToString(
                   cmd.Parameters["@PsOutputClaro"].Value)
               ?? string.Empty;
    }

    private string DescifrarAES(
        string valorCifrado)
    {
        if (string.IsNullOrWhiteSpace(valorCifrado))
            throw new ArgumentNullException(
                nameof(valorCifrado));

        byte[] datos =
            Convert.FromBase64String(
                valorCifrado);

        using var aes =
            Aes.Create();

        aes.Key =
            ClaveMaestra;

        byte[] iv =
            new byte[16];

        Array.Copy(
            datos,
            0,
            iv,
            0,
            16);

        aes.IV = iv;

        byte[] cifrado =
            new byte[
                datos.Length - 16];

        Array.Copy(
            datos,
            16,
            cifrado,
            0,
            cifrado.Length);

        using var ms =
            new MemoryStream(cifrado);

        using var cs =
            new CryptoStream(
                ms,
                aes.CreateDecryptor(),
                CryptoStreamMode.Read);

        using var sr =
            new StreamReader(cs);

        return sr.ReadToEnd();
    }
}
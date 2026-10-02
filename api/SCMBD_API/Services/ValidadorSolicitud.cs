using SCMBD_API.Models;

namespace SCMBD_API.Services;

public static class ValidadorSolicitud
{
    public static void Validar(
        Solicitud solicitud)
    {
        if (solicitud == null)
            throw new Exception(
                "Solicitud no recibida.");

        if (solicitud.Control == null)
            throw new Exception(
                "Bloque Control requerido.");

        if (solicitud.Destino == null)
            throw new Exception(
                "Bloque Destino requerido.");

        if (string.IsNullOrWhiteSpace(
            solicitud.Sql))
        {
            throw new Exception(
                "SQL vacío.");
        }

        if (string.IsNullOrWhiteSpace(
            solicitud.Control.Servidor))
        {
            throw new Exception(
                "Servidor Control requerido.");
        }

        if (string.IsNullOrWhiteSpace(
            solicitud.Control.BaseDatos))
        {
            throw new Exception(
                "BaseDatos Control requerida.");
        }

        if (string.IsNullOrWhiteSpace(
            solicitud.Control.Usuario))
        {
            throw new Exception(
                "Usuario Control requerido.");
        }

        if (string.IsNullOrWhiteSpace(
            solicitud.Control.PasswordCifrada))
        {
            throw new Exception(
                "Password Control requerida.");
        }

        if (string.IsNullOrWhiteSpace(
            solicitud.Destino.Servidor))
        {
            throw new Exception(
                "Servidor Destino requerido.");
        }

        if (string.IsNullOrWhiteSpace(
            solicitud.Destino.BaseDatos))
        {
            throw new Exception(
                "BaseDatos Destino requerida.");
        }

        if (string.IsNullOrWhiteSpace(
            solicitud.Destino.Usuario))
        {
            throw new Exception(
                "Usuario Destino requerido.");
        }

        if (string.IsNullOrWhiteSpace(
            solicitud.Destino.PasswordCifrada))
        {
            throw new Exception(
                "Password Destino requerida.");
        }

        if (solicitud.Tipo != 1 &&
            solicitud.Tipo != 2)
        {
            throw new Exception(
                "Tipo debe ser 1 o 2.");
        }
    }
}
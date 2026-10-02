using System.Data;
using System.Xml.Linq;

namespace SCMBD_API.Services;

public static class XmlResponse
{
    public static XElement ResultadoOk()
    {
        return new XElement("Ejecucion",
            new XElement("PnEstatus", 0),
            new XElement("PsMensaje",
                "Operación ejecutada correctamente"));
    }

    public static XElement ResultadoOk(
        XElement consulta)
    {
        return consulta;
    }

    public static XElement ResultadoError(
        int codigo,
        string mensaje)
    {
        return new XElement("Ejecucion",
            new XElement("PnEstatus", codigo),
            new XElement("PsMensaje", mensaje));
    }

    public static string Error(
        int codigo,
        string mensaje)
    {
        return new XElement("RespuestaGlobal",
            new XElement("Resultados",
                ResultadoError(
                    codigo,
                    mensaje)))
            .ToString();
    }

    public static XElement GenerarConsulta(
        IDataReader reader)
    {
        XElement consulta =
            new XElement("Consulta");

        XElement columnas =
            new XElement("Columnas");

        for (int i = 0; i < reader.FieldCount; i++)
        {
            columnas.Add(
                new XElement("Columna",
                    new XAttribute(
                        "nombre",
                        reader.GetName(i)),
                    new XAttribute(
                        "tipo",
                        reader.GetFieldType(i)?.FullName
                        ?? "System.String"),
                    new XAttribute(
                        "longitud",
                        0)));
        }

        consulta.Add(columnas);

        XElement filas =
            new XElement("Filas");

        while (reader.Read())
        {
            XElement fila =
                new XElement("Fila");

            for (int i = 0; i < reader.FieldCount; i++)
            {
                fila.Add(
                    new XElement(
                        reader.GetName(i),
                        reader.IsDBNull(i)
                            ? string.Empty
                            : Convert.ToString(
                                reader.GetValue(i))));
            }

            filas.Add(fila);
        }

        consulta.Add(filas);

        return consulta;
    }

    public static XElement GenerarConsulta(
        DataTable tabla)
    {
        XElement consulta =
            new XElement("Consulta");

        XElement columnas =
            new XElement("Columnas");

        foreach (DataColumn columna in tabla.Columns)
        {
            columnas.Add(
                new XElement("Columna",
                    new XAttribute(
                        "nombre",
                        columna.ColumnName),
                    new XAttribute(
                        "tipo",
                        columna.DataType.FullName
                        ?? "System.String"),
                    new XAttribute(
                        "longitud",
                        0)));
        }

        consulta.Add(columnas);

        XElement filas =
            new XElement("Filas");

        foreach (DataRow row in tabla.Rows)
        {
            XElement fila =
                new XElement("Fila");

            foreach (DataColumn columna in tabla.Columns)
            {
                fila.Add(
                    new XElement(
                        columna.ColumnName,
                        row[columna] == DBNull.Value
                            ? string.Empty
                            : row[columna]?.ToString()));
            }

            filas.Add(fila);
        }

        consulta.Add(filas);

        return consulta;
    }

    public static XElement RespuestaGlobal(
        IEnumerable<XElement> resultados)
    {
        return new XElement(
            "RespuestaGlobal",
            new XElement(
                "Resultados",
                resultados));
    }
}
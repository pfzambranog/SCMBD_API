# SCMBD API V2

## Estado

✅ Solución creada

✅ Proyecto compila correctamente

✅ Models implementados

✅ Services implementados

✅ XmlResponse implementado

✅ GestorCredenciales implementado

✅ Docker funcional

✅ HTTP funcional

✅ HTTPS funcional

✅ Consulta SQL validada

---

## Entradas

### JSON Control

Responsable de conectarse a SCMBD.

Incluye:

- Servidor
- Puerto
- BaseDatos
- Usuario
- PasswordCifrada

### JSON Destino

Responsable de conectarse a la base de datos destino.

Incluye:

- Servidor
- Puerto
- BaseDatos
- Usuario
- PasswordCifrada
- Manejador

---

## Flujo

JSON
↓
Control.PasswordCifrada (AES)
↓
ObtenerPasswordControl()
↓
Conexión SCMBD
↓
Parámetro 41
↓
Sp_DecodeBase64
↓
Destino.PasswordCifrada
↓
ObtenerPasswordDestino()
↓
Conexión Destino
↓
SQL
↓
XML

---

## Control

- BaseDatos dinámica desde JSON
- Obtiene parámetro 41
- Ejecuta Sp_DecodeBase64

---

## Credenciales

### Control

AES-256 local.

### Destino

Base64 mediante SP definido en el parámetro 41.

---

## XML

```xml
<RespuestaGlobal>
  <Resultados>
     <Consulta />
     <Ejecucion />
  </Resultados>
</RespuestaGlobal>

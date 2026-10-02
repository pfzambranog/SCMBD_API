FROM mcr.microsoft.com/mssql/server:2025-latest

USER root

# Instalar herramientas
RUN apt-get update && apt-get install -y --no-install-recommends \
    ca-certificates \
    openssl \
    curl \
    net-tools \
    iputils-ping \
    && rm -rf /var/lib/apt/lists/*

# Copiar certificado público de la API
WORKDIR /tmp

COPY secrets/scmbd-api-public.crt .

# Instalar certificado en el almacén de confianza
RUN cp scmbd-api-public.crt /usr/local/share/ca-certificates/scmbd-api-public.crt \
    && update-ca-certificates

# Certificados confiables para SQL Server
RUN mkdir -p /var/opt/mssql/security/ca-certificates/ \
    && cp scmbd-api-public.crt /var/opt/mssql/security/ca-certificates/ \
    && chmod 644 /var/opt/mssql/security/ca-certificates/scmbd-api-public.crt \
    && chown -R mssql:mssql /var/opt/mssql/security

# Limpiar temporales
RUN rm -rf /tmp/*

USER mssql

EXPOSE 1433
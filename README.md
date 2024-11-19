# Sis-Pa-Co: Sistema de Partidas Contables

### Este proyecto consiste en la gestión financiera con partidas contables para facilitar el registro de transacciones, ofrececiendo un catálogo de cuentas estructurado, automatizando la actualización de saldos de cuentas, con el objetivo de proporcionar una herramienta eficiente y precisa para manejar la contabilidad de una empresa.

## Características

- Crear y dar de baja partidas contables.

- Crear, activar o desactivar cuentas del catálogo.

- Enlistar partidas, cuentas, saldos y registros con opción de busqueda.

- Sistema de autenticación y autorización.

## Tecnologías Utilizadas

![C#](https://img.shields.io/badge/Language-C%23-blue)
![ASP.NET Core](https://img.shields.io/badge/Framework-ASP.NET%20Core-blue)
![JavaScript](https://img.shields.io/badge/Language-JavaScript-darkgreen)
![React](https://img.shields.io/badge/Framework-React-darkgreen)
![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-orange)

## Instalación

1. Clona este repositorio:
   ```bash
   git clone https://github.com/AnthonyEMF/sis-pa-co.git
   ```
2. Instala las dependencias:

   ```bash
   npm install
   ```

3. Corre el proyecto:

   ```bash
   npm run dev
   ```

4. Migración de la base de datos transaccional:

   ```bash
   add-migration Init -context SisPaCoContext
   update-database -context SisPaCoContext
   ```

5. Migración de la base de datos de registros:

   ```bash
   add-migration Init -context SisPaCoLogsContext
   update-database -context SisPaCoLogsContext
   ```

## Estado del Proyecto

Actualmente en desarrollo.  
Se están mejorando funcionalidades como proporcionar datos más detallados en los registros.  
Evitar que se cierre la sesión cuando se actualiza la página.  
Agregar la posibilidad de editar el nombre de las cuentas.

## Autores

- Anthony Miranda - [@AnthonyEMF](https://github.com/AnthonyEMF)
- Danilo Vides - [@IsaacV04](https://github.com/IsaacV04)

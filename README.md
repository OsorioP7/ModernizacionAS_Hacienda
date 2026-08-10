HaciendaSoft — Refactorización SOLID

Descripción

Este repositorio contiene el código fuente del sistema HaciendaSoft, su versión rediseñada aplicando principios SOLID, el programa principal y los casos de caracterización utilizados para comprobar la preservación del comportamiento observable del sistema.

Arquitecto de dominio, Thomas Osorio Passos, Identificación de responsabilidades y límites de cada clase (SRP), modelo del dominio, jerarquías de herencia y su validez frente a LSP.

Arquitecto de dependencias, Isabella Gutierrez Montoya, Mapa de dependencias, abstracciones (interfaces), inversión e inyección de dependencias, composition root (DIP, ISP).

Ingeniero de comportamiento, Gloria Yuliana Pena, Pruebas de caracterización, evidencia de que la conducta observable se preservó y escenarios de ejecución del programa principal.

Integrador y evidencia, Juan Sebastian Jaramillo, Consistencia diagrama–código, estructura del entregable, bitácora de uso de IA y métricas antes/después.

Requisitos de ejecución

.NET SDK compatible con la solución.

Visual Studio o una terminal con acceso al comando dotnet.

Clonar o descargar el repositorio completo antes de ejecutar.

Ejecución del sistema

Desde la carpeta raíz del repositorio:

dotnet restore
dotnet build

Para ejecutar el programa principal:

dotnet run --project p_mvcHacienda

Una vez iniciado, abrir en el navegador la dirección indicada por la consola.

Video de sustentación

Enlace al video:

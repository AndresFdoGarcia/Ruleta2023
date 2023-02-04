# Ruleta2023
Este proyecto tiene como propósito dejar evidencia de algunas de las aptitudes que tengo sobre el desarrollo de aplicaciones basadas en .Net ( C# ). Hace parte de un ejercicio propuesto como prueba técnica para el cargo de Ing de desarrollo jr. De momento se tiene el desarollo para ejecutarse en local.

Se desea implementar un API que represente una ruleta de apuestas online que cumpla las siguientes características:
a) tenga los siguientes endpoints :
1. Endpoint de creación de nuevas ruletas que devuelva el id de la nueva ruleta creada
2. Endpoint de apertura de ruleta (el input es un id de ruleta) que permita las 
posteriores peticiones de apuestas, este debe devolver simplemente un estado que 
confirme que la operación fue exitosa o denegada.
3. Endpoint de apuesta a un número (los números válidos para apostar son del 0 al 36)
o color (negro o rojo) de la ruleta una cantidad determinada de dinero  a una ruleta abierta.
nota: este enpoint recibe además de los parámetros de la apuesta, un id de ruleta
en los HEADERS.
4. Endpoint de cierre apuestas dado un id de ruleta, este endpoint debe devolver el
resultado de las apuestas hechas desde su apertura hasta el cierre.
El número ganador se debe seleccionar automáticamente por la aplicación al cerrar
la ruleta y para las apuestas de tipo numérico se debe entregar 5 veces el dinero
apostado si atinan al número ganador, para las apuestas de color se debe entrega 1.8
veces el dinero apostado, todos los demás perderán el dinero apostado.
nota: para seleccionar el color ganador se debe tener en cuenta que los números
pares son rojos y los impares son negros.
5. Endpoint de listado de ruletas creadas con sus estados (abierta o cerrada):
  a) El desarrollo debe estar pensado para escalar horizontalmente (se sugiere el uso de Redis
como persistencia de datos y herramienta para resolver posibles problemas de concurrencia
pero queda abierto a libre escogencia)
  b) Se debe cumplir las reglas de Clean Code, el objetivo principal de este ejercicio es
precisamente revisar clean code.

El desarrollo ahora mismo se construye en capas, tiene implementado bases de datos : 
<li> MongoDB : Gestor de usuarios y ruletas
<li> Redis : Usada en la recepción y evaluación de apuestas, asi como para el menejo en timepo real de el dinero de cada usuario
 
 <p> En proximas versiones se hará la impementación de colas para el manejo de las apuestas y un worker que las procese para quitar responsabilidades a la API y hacerla algo más cercano a la realidad de un servicio en produción.Se agregara un docker compose para levantar los servicios de mongo, redis y rabbitMq.
   

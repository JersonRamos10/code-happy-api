# Suite contractual de `client-api-migration` (T15)

Este directorio contendrá la suite HTTP de regresión de la migración del backend.
Su propósito es comprobar el contrato de la API local real, no reemplazar las
pruebas unitarias futuras.

## Decisión cerrada

La suite se implementará como un script PowerShell versionado en este directorio.
`codeHappy.Api/codeHappy.Api.http` permanece para exploración manual desde Rider
o VS Code, pero no es la fuente de evidencia de regresión. No se crea todavía
un proyecto de pruebas .NET ni se agregan paquetes de testing.

## Instrucciones para el agente que implemente la suite

Crear un único script PowerShell legible y comentado dentro de este directorio.
Debe ejecutarse contra `http://localhost:5104` y cumplir estas reglas:

- La API debe estar iniciada previamente. El script primero comprobará
  `GET /api/health` y fallará con un mensaje claro si no responde 200.
- Debe ejecutar `dotnet build CodeHappy.slnx --no-restore` antes de las pruebas
  HTTP y marcar la ejecución como fallida si el build falla.
- Recibirá los dos JWT reales mediante las variables de entorno `OWNER_TOKEN` y
  `OTHER_USER_TOKEN`. Nunca debe incluir tokens, contraseñas, connection strings
  ni secretos en el repositorio o en la salida.
- Debe crear sus propios recursos de prueba, registrar sus IDs en memoria y
  eliminar únicamente esos recursos. La limpieza debe correr incluso si una
  verificación falla.
- Debe usar nombres con un prefijo identificable, por ejemplo `T15-`, para que
  cualquier recurso temporal sea reconocible.
- Debe mostrar cada verificación como aprobada o fallida, imprimir al final el
  total de casos, aprobados y fallidos, y finalizar con un código distinto de
  cero si hay fallas.
- No debe depender de recursos, shares o datos que ya existan en Supabase.
- Cada assertion debe comprobar tanto el estado HTTP como los campos relevantes
  del body cuando el contrato lo requiera.

La forma de ejecución y las variables requeridas deben quedar documentadas al
inicio del script y en este archivo. El agente puede usar cmdlets nativos de
PowerShell; no debe exigir Bash, `jq`, Bruno, Scalar ni extensiones del editor.

## Cobertura contractual mínima

1. Ambiente y autenticación: build, health, CORS permitido y denegado, 401 sin
   JWT, 401 con JWT inválido, y sync de perfil con 200 y 400.
2. Spaces y Groups: CRUD relevante, 400 para GUID malformado, 404 para recurso
   inexistente, 403 para recurso ajeno, `spaceId` en Group y 404 para IDs
   cruzados.
3. Snippets: create, lista, detalle, update y delete; description y topics;
   paginación incluida página vacía; enums camelCase; entrada enum inválida,
   numérica y JSON malformado como 400; la lista no expone `blocks` y el detalle
   sí.
4. GUID malformado: 400 en GET, PUT, DELETE, favorite, copy y move de Snippets.
5. Move: body ausente, `null`, `{}` y `groupId` sin `spaceId` devuelven 400;
   movimiento válido, descategorización, 404 de Space o Group inexistente o
   cruzado, 403 ajeno, no-op sin cambio de `updatedAt`, y preservación de Blocks
   y contadores.
6. Favorite y copy: reglas de owner, recurso ajeno, público, privado y 404.
7. Blocks y Comments: operaciones CRUD esenciales, annotations y ownership.
8. Shares: creación según visibilidad, vista pública, expiración, ownership y
   D4. Al pasar un Snippet a `private`, el enlace anterior devuelve 404,
   desaparece de la lista del creador y no se reactiva al volver a `public`.
9. Search: confirmar que no existe un endpoint Search, conforme a T14.

## Fuera de esta suite

Cloudinary e imágenes y el reorder de Blocks quedan documentados como cobertura
manual pendiente. No se deben ocultar como casos verdes ni convertirlos en
dependencias de esta suite sin una nueva tarea.

El reorder de **Groups** sí está automatizado: la suite ejecuta
`PUT /api/spaces/{spaceId}/groups/reorder` con dos Groups y verifica la
`position` persistida. Se incorporó al cubrir el CRUD completo de Groups.

## Ejecución

El script es `Invoke-ContractSuite.ps1`, en este mismo directorio.

Con la API ya iniciada y los dos tokens cargados en la sesión de PowerShell:

```powershell
$env:OWNER_TOKEN = '<jwt del usuario propietario>'
$env:OTHER_USER_TOKEN = '<jwt de un segundo usuario distinto>'
powershell.exe -ExecutionPolicy Bypass -File .\tests\contract\Invoke-ContractSuite.ps1
```

### Compatibilidad e intérprete

La suite corre en **Windows PowerShell 5.1**, que es el intérprete disponible en
el equipo de desarrollo, y también en PowerShell 7 sin cambios.

Concretamente, no usa `-SkipHttpErrorCheck`: ese parámetro solo existe en
PowerShell 6+ y su uso haría que el script fallara por completo en 5.1. En su
lugar, los códigos 4xx y 5xx se capturan por excepción: `WebException` en 5.1 y
`HttpResponseException` en 7. En ambas ediciones el objeto de respuesta cuelga de
`$_.Exception.Response`, de donde sale el código de estado.

El **cuerpo** del error, en cambio, no se obtiene igual en las dos. PowerShell 7
rellena `$_.ErrorDetails.Message`, pero Windows PowerShell 5.1 con frecuencia lo
deja vacío. Por eso `Invoke-Api` intenta primero `ErrorDetails.Message` y, si
viene vacío y el objeto de respuesta expone `GetResponseStream`, lee el stream
directamente. Sin ese fallback, bajo 5.1 todo Problem Details llegaría vacío y
las afirmaciones sobre el cuerpo darían falsos negativos.

Las llamadas usan `-UseBasicParsing` para no depender del motor de Internet
Explorer en 5.1, y el body se envía como bytes UTF-8 porque 5.1 puede recodificar
un body string según el charset negociado.

Si la política de ejecución del equipo bloquea scripts, se invoca con
`-ExecutionPolicy Bypass` como en el ejemplo de arriba. No hace falta cambiar la
política del sistema.

El archivo se guarda como UTF-8 **con BOM**: sin BOM, Windows PowerShell 5.1
interpreta el script como ANSI y corrompe los acentos. Si se edita con una
herramienta que quite el BOM, hay que volver a agregarlo.

Parámetros opcionales:

- `-BaseUrl` - URL base de la API. Por defecto `http://localhost:5104`.
- `-SkipBuild` - omite `dotnet build`. Solo para iteración rápida; la ejecución
  de referencia debe incluir el build.

Códigos de salida: `0` si todas las verificaciones pasan; `1` si hay al menos
una falla o si falta un requisito previo (API caída, build fallido o variables
de entorno ausentes).

### Nota sobre el build en Windows

`dotnet build` falla por bloqueo de archivos si la API en ejecución fue
compilada desde fuentes distintas a las actuales. El script detecta ese caso y
lo explica en la salida: hay que detener la API, recompilar y volver a
iniciarla. Si las fuentes no cambiaron, el build es incremental y no interfiere
con la API en ejecución.

## Última ejecución de referencia

2026-08-03 - **215 casos, 215 aprobados, 0 fallidos, 0 fallos de limpieza**,
código de salida 0, con `dotnet build` en 0 errores, contra
`http://localhost:5104`, ejecutada con Windows PowerShell 5.1.

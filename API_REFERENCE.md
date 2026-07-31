# CodeHappy .NET API — Referencia de implementación actual

_Generado el 2026-07-31. Documenta lo que el backend .NET (`codeHappy.Api`/`codeHappy.Business`/`codeHappy.Data`) tiene **realmente implementado** a esta fecha, para que el frontend (a adaptar más adelante) sepa contra qué contrato integrar. No es un documento de diseño (eso vive en `.dev-notes/spec/`) ni de proceso (`.dev-notes/status/`) — es un snapshot del contrato HTTP actual._

Todas las rutas están bajo el prefijo `api/` salvo las anotadas como excepción. Todos los endpoints requieren JWT (`RequireAuthorization()`) salvo `GET /shared/{shareId}`, explícitamente anónimo.

Autenticación: JWT emitido por Supabase Auth, validado vía JWKS. El `userId` se extrae de los claims (`ICurrentUserService`), no hay cookies de sesión.

Manejo de errores global (`GlobalExceptionHandler`): toda excepción no controlada se traduce a un `ProblemDetails`:

| Excepción | Status |
|---|---|
| `NotFoundException` | 404 |
| `ForbiddenException` | 403 |
| `BadRequestException` | 400 |
| `ExternalServiceException` | 502 |
| (cualquier otra) | 500 |

Los 400 de validación de input (FluentValidation) devuelven `Results.ValidationProblem` (shape distinto, con diccionario `errors` por campo) — ver nota en `.dev-notes/status/comments/notas.md` (auditoría B2) sobre por qué esa diferencia de forma es intencional.

---

## 1. Auth

Base: `api/auth`

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/sync` | Sincroniza los claims del JWT de Supabase (`userId`, `email`, `userName`, `displayName`) contra la tabla `profiles` local. Crea el perfil si no existe. Devuelve el `UserProfileResponse` actual. |

**`UserProfileResponse`**: `Id`, `UserName`, `Email`, `AvatarUrl?`

---

## 2. Spaces

Base: `api/spaces`

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/` | Crea un space. 403 si no es dueño no aplica (recurso nuevo). |
| GET | `/` | Lista los spaces del usuario autenticado. |
| PUT | `/{id}` | Actualiza `Name`/`Icon`. 403 si no es dueño. |
| DELETE | `/{id}` | Borra el space. 403 si no es dueño. |
| PATCH | `/{id}/touch` | Actualiza `LastAccessedAt` (para "recientes"). |

**`CreateSpaceRequest`** / **`UpdateSpaceRequest`**: `Name: string`, `Icon?: string`
**`SpaceResponse`**: `Id`, `Name`, `Icon?`, `CreatedAt`, `UpdatedAt`, `LastAccessedAt?`

---

## 3. Groups

Base: `api/spaces/{spaceId}/groups`

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/` | Lista los groups del space, ordenados por `Position`. |
| POST | `/` | Crea un group (`Position` se calcula automáticamente). |
| PUT | `/{id}` | Renombra el group. |
| DELETE | `/{id}` | Borra el group. |
| PUT | `/reorder` | Actualiza posiciones en bloque — body `List<ReorderGroupRequest>`. |

**`CreateGroupRequest`** / **`UpdateGroupRequest`**: `Name: string`
**`GroupResponse`**: `Id`, `Name`, `Position`, `CreatedAt`
**`ReorderGroupRequest`**: `Id`, `Position`

---

## 4. Snippets

Base: `api/snippets`

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/` | Lista paginada. Query params (`[AsParameters] SnippetParamsRequest`): `SpaceId?`, `GroupId?`, `IsFavorite?`, `PageNumber=1`, `PageSize=20`. Scope: dueño o `Visibility=Public`. |
| GET | `/{id}` | Detalle de un snippet. 403 si no es dueño ni público. Incrementa `ViewCount`. |
| POST | `/` | Crea un snippet con sus blocks embebidos (`CreateBlockRequest[]`). |
| PUT | `/{id}` | **Reemplazo destructivo**: borra todos los blocks existentes y los recrea desde el request. |
| DELETE | `/{id}` | Borra el snippet (cascade sobre blocks/comments) y sus imágenes en Cloudinary (best-effort). |
| PATCH | `/{id}/favorite` | Alterna `IsFavorite`. **Sin chequeo de `userId`/ownership en el endpoint** — cualquier usuario autenticado puede togglear el favorito de cualquier snippet por id. Revisar antes de exponer al frontend. |
| POST | `/{id}/copy` | Incrementa `CopyCount`. **Mismo caso**: sin chequeo de ownership en el endpoint. |

**`SnippetParamsRequest`**: `SpaceId?`, `GroupId?`, `IsFavorite?`, `PageNumber=1`, `PageSize=20`
**`CreateSnippetRequest`** / **`UpdateSnippetRequest`**: `Title`, `Description?`, `Visibility` (enum), `Blocks: CreateBlockRequest[]`, `SpaceId?`, `GroupId?`, `Topics?: string` (nota: `string`, no `List<string>`, en el request — asimetría con la respuesta)
**`SnippetResponse`**: `Id`, `Title`, `Description?`, `Visibility`, `Topics?: List<string>`, `Blocks: BlocksResponse[]`, `SpaceId?`, `GroupId?`
**`SnippetVisibility` (enum)**: `Private`, `Shared`, `Public`
**`PagedResponse<T>`**: `Items`, `PageNumber`, `PageSize`, `TotalItems`, `TotalPages`

---

## 5. Blocks

Base: `api/snippets/{snippetId}/blocks`

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/` | Crea un block dentro del snippet. |
| PUT | `/{blockId}` | Actualiza `Title`/`Content`/`Language` — semántica PATCH (campos `null` no se tocan) aunque el verbo HTTP es PUT. |
| PUT | `/{blockId}/annotations` | Reemplaza todas las anotaciones. Body `null`/vacío las borra. |
| DELETE | `/{blockId}` | Borra el block. |
| PUT | `/reorder` | Actualiza posiciones en bloque. |

**`CreateBlockRequest`**: `Title?`, `Content`, `Type` (enum), `Position`, `Language?`, `PublicId?`, `Width?`, `Height?`, `Format?`, `Bytes?` (estos 5 últimos solo aplican si `Type=Image`, vienen del upload previo a Cloudinary), `Annotations?: CreateAnnotationRequest[]`
**`UpdateBlockRequest`**: `Title?`, `Content?`, `Language?`
**`BlocksResponse`**: `Id`, `Title?`, `Content`, `Language?`, `Type`, `Annotations?: AnnotationResponse[]`, `Position`, `ImageMetadata?: ImageMetadataResponse`, `CreatedAt`, `UpdateAt`
**`BlockType` (enum)**: `Code`, `Text`, `Image`
**`AnnotationResponse`**: `Id`, `LineNumber`, `Text`
**`ImageMetadataResponse`**: `PublicId`, `SecureUrl`, `Width?`, `Height?`, `Format?`, `Bytes?`, `Alt?`, `BucketPath?`
**`ReorderBlockRequest`**: `Id`, `Position`

---

## 6. Images

Rutas mixtas — upload anidado bajo snippet, delete plano:

| Método | Ruta | Descripción |
|---|---|---|
| POST | `api/snippets/{snippetId}/images/upload` | `multipart/form-data` (`IFormFile file`). Sube a Cloudinary con `public_id` firmado y devuelve `ImageMetadataResponse`. |
| DELETE | `api/images/{**publicId}` | Borra el asset de Cloudinary por `publicId` (catch-all, admite `/` en el valor). |

---

## 7. Shares

Base (⚠️ sin prefijo `api/` todavía en esta rama — ver nota abajo): `/snippets/{snippetId}/shares`, `/shares`, `/shared`

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/snippets/{snippetId}/shares` | Crea un link de share. **400 si `Snippet.Visibility == Private`** (no genera el link). 403 si no es dueño del snippet. |
| GET | `/shares` | Lista los shares creados por el usuario autenticado. |
| DELETE | `/shares/{shareId}` | Revoca (borra) un share. 403 si no es quien lo creó. |
| GET | `/shared/{shareId}` | **Anónimo** (sin JWT) — vista pública del snippet compartido. 404 si el share no existe o expiró. No valida `Visibility` del snippet en este punto (ver nota). |

**`CreateShareRequest`**: `SnippetId`, `ExpiresAt?`
**`ShareResponse`**: `Id`, `SnippetId`, `ExpiresAt?`, `CreatedAt`
**`SharedSnippetResponse`**: `Id`, `Title`, `Description?`, `Visibility`, `Topics?`, `Blocks: BlocksResponse[]`

**⚠️ Nota de sincronización de ramas:** `ShareEndpoints.cs` en `feature/comments-service` todavía expone las rutas **sin** el prefijo `api/` (`/snippets/{snippetId}/shares` en vez de `api/snippets/{snippetId}/shares`). El fix ya existe en la rama `feature/shares-service` pero no está mergeado acá — confirmar el prefijo real antes de que el frontend apunte a esta ruta. Ver `.dev-notes/status/share/task.md` (T2).

**⚠️ Nota de caso borde aceptada:** un link de share creado mientras el snippet era `Public`/`Shared` sigue siendo válido aunque el dueño cambie después la `Visibility` a `Private` — no hay revocación automática, el link vive hasta que expire o se borre manualmente (decisión cerrada en auditoría de Comments B2).

---

## 8. Comments

Base: `api/snippets/{snippetId}/comments`

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/` | Crea un comentario. Query param opcional `shareId` (para autorizar sobre snippets `Shared`). |
| GET | `/` | Lista los comentarios del snippet, ordenados por `CreatedAt`. Mismo query param `shareId`. |
| PUT | `/{commentId}` | Edita el texto. Solo el autor del comentario (ni siquiera el dueño del snippet puede editar ajenos). |
| DELETE | `/{commentId}` | Borra el comentario. Autor del comentario **o** dueño del snippet. |

**Autorización combinada para crear/leer** (todas las rutas excepto DELETE, que no depende de esto): dueño del snippet, o `Visibility=Public`, o `shareId` válido/no expirado para ese snippet.

**`CreateCommentRequest`** / **`UpdateCommentRequest`**: `Text: string` (no vacío, máx. 10.000 caracteres)
**`CommentResponse`**: `Id`, `SnippetId`, `Text`, `CreatedAt`, `UpdatedAt`, `OwnerId`, `Profile: UserProfileResponse`

---

## Módulos explícitamente NO migrados

- **Search** (`/api/search`, `/api/search/suggestions` en el proyecto original): descartado. El endpoint original nunca tuvo un consumidor real — el frontend original filtraba en memoria sobre los snippets ya cargados. Ver `.dev-notes/spec/search.md`.

## Hallazgo transversal sobre el proyecto original

El frontend original (`E:\Playground\CodeHappy`) usa **Firestore directo** (SDK cliente) para todo — no consume el backend Express/Supabase (`apps/api`) en ningún store. Ese backend Express es una migración a Supabase que quedó sin conectar al producto real. El inventario completo de colecciones Firestore reales (`profiles`, `snippets`, `blocks`, `comments`, `groups`, `shares`, `spaces`) está cubierto 1:1 por los módulos de este documento — no queda ningún módulo del original sin representar acá.

## Deuda técnica transversal conocida (no bloqueante, ver `.dev-notes/status/infraestructura/`)

- Consistencia de `CancellationToken` en todos los métodos de servicio/endpoints.
- Consistencia de atributos de binding (`[FromRoute]`/`[FromQuery]`/`[FromForm]`) en todos los endpoints.
- Evaluar la incorporación de `ILogger` en servicios/endpoints (hoy no se usa en ningún lado).

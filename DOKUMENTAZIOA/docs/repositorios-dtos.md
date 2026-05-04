# Repositorios y DTOs actualizados

## Repositorios

### ZerbitzuaRepository

Gestiona el ciclo completo de los pedidos de mesa sobre las tablas `zerbitzua`, `eskaerak`, `platerak_has_eskaerak`, `produktuak` y `produktuak_has_platerak`.

- `Create(dto)`: crea un servicio nuevo y registra sus lineas. Si la linea es un producto, descuenta una unidad de `produktuak.stock`. Si la linea es un plato (`IsPlatera = true`), descuenta una unidad de cada ingrediente asociado en `produktuak_has_platerak` y guarda la relacion en `platerak_has_eskaerak`.
- `GetByMahai(mahaiaId)`: devuelve los ultimos servicios de una mesa con sus lineas, imagen normalizada (`/irudiak/...`) y marca `IsPlatera` para que TPV y movil puedan reconstruir el pedido correctamente.
- `Update(id, dto)`: no permite editar servicios pagados. Primero devuelve al stock las lineas anteriores, despues valida y descuenta el nuevo contenido. Todo ocurre en transaccion para evitar stock incoherente.
- `Ordaindu(id)`: marca el servicio como pagado y crea la factura si todavia no existe.

### ProduktuaRepository

Expone los productos de la base de datos centralizada con su precio, categoria, stock e imagen. TPV, movil y gerente consumen estos datos para evitar catalogos locales divergentes.

### PlateraRepository

Expone los platos con su precio, tipo, imagen publica (`ArgazkiaUrl`) y lista de ingredientes. La lista de ingredientes incluye stock actual, por lo que las aplicaciones cliente pueden saber si un plato se puede pedir.

## DTOs

### ZerbitzuaSortuDto

DTO de entrada para crear o actualizar un servicio:

- `PrezioTotala`: total del pedido.
- `Data`: fecha del servicio.
- `ErreserbaId`: reserva asociada, opcional.
- `MahaiakId`: mesa asociada.
- `Eskaerak`: lineas del pedido.

### EskaeraSortuDto

DTO de entrada para cada linea:

- `ProduktuaId`: ID del producto o del plato, segun `IsPlatera`.
- `Izena`: nombre visible de la linea.
- `Prezioa`: precio de la linea.
- `Data`: fecha de la linea.
- `Egoera`: estado de cocina/pago.
- `IsPlatera`: `true` cuando la linea corresponde a `platerak`; `false` cuando corresponde a `produktuak`.

### ZerbitzuaMahaiDTO y ZerbitzuaEskaeraDTO

DTOs de salida para consultar pedidos por mesa. `ZerbitzuaEskaeraDTO` devuelve `ProduktuaId`, `Izena`, `Irudia`, `Prezioa`, `Egoera` e `IsPlatera`, por lo que TPV y movil pueden mostrar imagenes y editar lineas sin confundir platos con productos.

### ProduktuaDTO

Representa un producto inventariable: `id`, `izena`, `prezioa`, `kategoria_id` y `stock_aktuala`.

### PlateraDTO y PlateraOsagaiaDTO

`PlateraDTO` representa un plato con `Id`, `Izena`, `Mota`, `Prezioa`, `Argazkia`, `ArgazkiaUrl` y `Osagaiak`. Cada `PlateraOsagaiaDTO` contiene `Id`, `Izena` y `Stock`.

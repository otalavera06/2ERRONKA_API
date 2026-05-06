# Kontrollerrak

Kontrollerrak APIaren sarrera-puntuak dira. Bezeroaren eskaera jasotzen dute, gutxieneko balidazioak egiten dituzte, dagokion repositorioari lana eskatzen diote eta HTTP erantzun egokia bueltatzen dute.

## `LangileakController`

- `GET /api/langileak`: langile guztiak itzultzen ditu, Odoo edo beste bezero batzuek langileen zerrenda sinkronizatzeko.
- `POST /api/langileak/login`: erabiltzaile izena eta pasahitza jasotzen ditu. Datuak zuzenak badira langilearen informazioa eta `chatBaimena` itzultzen ditu; okerrak badira `401 Unauthorized`.
- `GET /api/langileak/{id}/txat-baimena`: langile jakin batek txata erabiltzeko baimena duen kontsultatzen du. Baimena datu-basetik irakurtzen da, ez login eskaeratik.

## `MahaiakController`

- `GET /api/mahaiak/libre`: une horretan libre dauden mahaiak itzultzen ditu.
- `POST /api/mahaiak/login`: mahaiaren erabiltzaile izena eta pasahitza egiaztatzen ditu, Mugikorra aplikazioak mahaia identifikatzeko.
- `GET /api/mahaiak/{id}/txat-baimena`: mahai batek txata erabiltzeko baimena duen itzultzen du.

## `ZerbitzuaController`

- `POST /api/zerbitzua`: mahai edo barrarako zerbitzu berria sortzen du. Mahai horretan ordaindu gabeko zerbitzua badago, repositorioak zerbitzu bera berrerabili dezake.
- `GET /api/zerbitzua/mahaia/{mahaiaId}`: mahai edo barraren azken zerbitzuak itzultzen ditu, TPVk azken eskaerak erakusteko.
- `PUT /api/zerbitzua/{id}`: ordaindu gabeko zerbitzu baten produktuak ordezkatzen ditu. Ordainduta badago, ezin da editatu.
- `POST /api/zerbitzua/{id}/ordaindu`: zerbitzua ordainduta markatzen du.

## `EskaeraKontrollerra`

- `GET /api/eskaerak`: eskaera guztiak itzultzen ditu.
- `GET /api/eskaerak/{id}`: eskaera jakin bat bilatzen du.
- `POST /api/eskaerak`: eskaera berria sortzeko datuak jasotzen ditu eta produktuen existentzia eta stocka balidatzen ditu.
- `PUT /api/eskaerak/{id}`: eskaera baten datuak eguneratzen ditu.
- `DELETE /api/eskaerak/{id}`: eskaera bat ezabatzen du.
- `PATCH /api/eskaerak/{id}/sukaldea-egoera`: sukaldeko egoera aldatzen du.

## `ProduktuKontrollerra` eta `PlaterakController`

- `GET /api/produktuak`: produktuen katalogoa itzultzen du, stock eta irudiekin.
- `GET /api/produktuak/kategoria/{kategoriaId}`: kategoria bateko produktuak itzultzen ditu.
- `GET /api/platerak`: plateren katalogoa itzultzen du, argazkiaren URLarekin eta osagaiekin.

## `KategoriaKontrollerra`

- `GET /api/kategoriak`: produktuen kategoriak itzultzen ditu.

## `ErreserbakController`

- `GET /api/erreserbak`: egun eta txanda bateko erreserbak itzultzen ditu.
- `POST /api/erreserbak`: erreserba berria sortzen du.
- `PUT /api/erreserbak/mahaia/{mahaiaId}`: mahai eta data baten erreserba eguneratzen du.
- `DELETE /api/erreserbak/mahaia/{mahaiaId}`: mahai eta data baten erreserba ezabatzen du.

## `FakturaKontrollerra`

- Fakturak sortzeko eta ordaindu gabeko eskaerak kontsultatzeko erabiltzen da. Zerbitzuaren edo eskaeraren datuekin faktura prestatzen du eta bezeroari behar den informazioa itzultzen dio.

## `Login` eta `Log`

- `Login`: login zaharragoaren endpoint-a mantentzen du bateragarritasunagatik. `LoginDTO` bidez erabiltzaile izena eta pasahitza bakarrik jasotzen ditu.
- `Log`: aplikazioko ekintzak fitxategian erregistratzeko endpoint-a da.

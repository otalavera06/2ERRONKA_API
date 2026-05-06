# DTOak

DTOak APIaren sarrera eta irteera datuak garraiatzeko erabiltzen dira. Klase hauek ez dute negozio-logikarik gordetzen; kontroladoreen, bezeroen eta erantzun estandarren arteko kontratua definitzen dute.

## Eskaerak eta zerbitzuak

- `EskaeraSortuDTO`: eskaera berri bat sortzeko datuak, mahaiaren IDa, erabiltzailearen IDa, komensalak eta produktuen zerrenda barne.
- `EskaeraProduktuaSortuDTO`: sortzen ari den eskaerako produktu bakoitzaren IDa, kantitatea eta prezioa.
- `EskaeraEguneratuDTO`: eskaera baten komensalak eta produktuak eguneratzeko datuak.
- `EskaeraProduktuaEditatuDTO`: produktu baten kantitatea editatzeko datuak.
- `EskaeraDTO`: eskaera baten oinarrizko informazioa.
- `EskaeraLortuDTO`: eskaera baten produktuen xehetasunak itzultzeko egitura.
- `EskaeraProduktuaDTO`: eskaera bati lotutako produktuaren IDa, prezioa eta kantitatea.
- `EskaeraSukaldeaEgoeraDTO`: sukaldeko egoera aldatzeko datua.
- `ZerbitzuaMahaiDTO`: mahai bateko zerbitzuaren laburpena, ordainketa egoera eta eskaeren zerrenda.
- `ZerbitzuaEskaeraDTO`: zerbitzu baten barruan dagoen produktu edo plater bakoitzaren informazioa.

## Produktuak eta platerak

- `ProduktuaDTO`: produktuaren IDa, izena, prezioa, kategoria, stocka eta irudia.
- `PlateraDTO`: plateraren IDa, izena, mota, prezioa, argazkia, argazkiaren URL osoa eta osagaiak.
- `PlateraOsagaiaDTO`: plater bati lotutako osagaiaren IDa, izena eta stocka.
- `KategoriaDTO`: produktuen kategoriaren IDa eta izena.

## Erabiltzaileak eta erantzunak

- `LangileaDTO`: langilearen datu nagusiak, baimenak, mahai lotura eta txat baimena.
- `LoginDTO`: saioa hasteko erabiltzaile izena eta pasahitza. Txat baimena ez da bezeroak bidaltzen; APIak datu-baseko erabiltzailearen informaziotik itzultzen du.
- `LogDTO`: erabiltzaile batek egindako ekintza erregistratzeko datuak.
- `ErantzunaDTO<T>`: APIaren erantzun estandarra, kodea, mezua eta datuen zerrenda generikoa biltzen dituena.

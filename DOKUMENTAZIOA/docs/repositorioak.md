# Repositorioak

Repositorioek kontroladoreen eta datu-basearen arteko sarbidea zentralizatzen dute. Gehienek NHibernate saio bat irekitzen dute, behar den kontsulta edo transakzioa exekutatzen dute, eta kontroladoreek behar dituzten modeloak edo DTOak itzultzen dituzte.

## Repositorio nagusiak

- `ZerbitzuaRepository`: zerbitzuak sortu, mahai baten zerbitzuak kontsultatu, zerbitzuak editatu eta ordainduta markatzen ditu.
- `EskaeraRepository`: eskaerak sortu, eguneratu, ezabatu, sukaldeko egoera aldatu eta ordaintzeko dauden eskaerak kontsultatzen ditu.
- `ProduktuaRepository`: produktuen kontsultak eta stockarekin lotutako datuak kudeatzen ditu.
- `PlateraRepository`: platerak, argazkien URLak eta osagaiak SQL kontsulta bateratu baten bidez lortzen ditu.
- `KategoriaRepository`: kategoriak eta kategoriak DTO formatuan itzultzen ditu.
- `MahaiaRepository`: mahaiak lortu, eguneratu, ezabatu eta libre dauden mahaiak kontsultatzen ditu.
- `ErabiltzaileaRepository`: erabiltzaileen eta langileen datuak lortzen ditu, baita login egiaztapena ere.
- `ErreserbaRepository`: egun eta txanda baten erreserbak lortu, sortu, eguneratu eta ezabatzen ditu.
- `EskaeraProduktuakRepository`: eskaera bati lotutako produktuak lortu, eguneratu eta ezabatzen ditu.
- `EskaeraMahaiakRepository`: eskaera eta mahai arteko loturak ezabatzen ditu.

## Portaera garrantzitsuak

- Zerbitzu berria sortzean, mahai horretan ordaindu gabeko zerbitzua badago, `ZerbitzuaRepository`k zerbitzu bera berrerabiltzen du eta prezio totala gehitzen du.
- Produktu arrunt bat gehitzean, stocka unitateka deskontatzen da.
- Platerak produktu tekniko baten bidez gordetzen dira, eta `IsPlatera` erabiliz bereizten dira.
- Zerbitzu bat editatzean, aurreko produktuen stocka leheneratu eta eskaera berriak berriro sartzen dira.
- Ordainduta dagoen zerbitzua ezin da editatu.

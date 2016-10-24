Scena MachineBehaviourTest pozwala pobawić się przygotowanymi już maszynkami.
Katalog Stolen zawiera rzeczy pochodzące z innej gry, kompletnie zbędne dla samej maszyny, jednak zapewniające możliwość testowania (kamera, możliwość przenoszenia rzeczy przez drag & drop).
Ostatecznie nie służą one w żaden sposób grze.

Sam model maszyny jest ubogi, ale jest jedynie placeholderem. Kod działa ładnie i jest uniwersalny

Składniki zastępują w przykładzie kolorowe kule. Poza IngredientBehaviour korzystają z DraggablePhysicsObject z Stolen, by można nimi ruszać.

Sterowanie w przykładzie:
wsad, ctrl i spacja - ruszanie kamerą
prawy myszy - obrót kamery
lewy myszy - złapanie kulki
ruszanie myszą i WASD trzymając kulkę - ruszanie kulką
kliknięcie maszyny - włączenie na niej ProcessIngredients ("zakręcenie korbką")

W kodzie sporo komentarzy.

Instrukcja składania maszyny:
- Posklecaj dowolną konfigurację gameobjectów (z rigidbody, colliderami etc), jaką uważasz za sensowną maszynę
- Opcjonalnie - umieść lampki zawierające LampBehaviour
- Wrzuć do korzenia tej struktury 0-2 triggery, umieść je w miejscach, gdzie maszyna ma przyjmować składniki
- Podepnij skrypt MachineBehaviour
- przeciągając, czy jakkolwiek wolisz w tablicy Inputs ustaw referencje na utworzone triggery
- w tablicy Accepted Inputs wybierz, jaki rodzaj składnika jest akceptowany na każdym wejściu
- Jeśli masz lampki z LampBehaviour, to referencje do nich w kolejności czerwony, żółty, zielony umieść w tablicy Lights
- Ustaw czas w sekundach, jaki zajmuje maszynie utworzenie nowego produktu
- Ustaw prefab, który maszyna ma tworzyć
- Ustaw miejsce, gdzie będą pojawiać się nowe produkty (Output Area Size, Output Area Center)

EWENTUALNY PROBLEM:
OnTriggerEnter nie pozwala nam (maszynie) się dowiedzieć, który z naszych triggerów dostał. Stąd musimy prosić, by to pączek po trafieniu na trigger, powiedział nam, na który - a my musimy wtedy dopiąć go do odpowiedniego zbioru.

Jeśli w przyszłości będziemy chcieli (a na pewno) podnosić, nosić, rzucać itd. innymi rzeczami, niż składniki, to wszystkie powinny dziedziczyć po jakiejś wspólnej PickUpAble, która to powinna implementowac OnTriggerCollider tak, jak teraz robi to składnik (który sam by po niej dziedziczył), aby maszyna mogła nam błysnąć na czerwono, że próbujemy ją nakarmić młotkiem.

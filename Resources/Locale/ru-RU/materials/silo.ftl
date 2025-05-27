ore-silo-ui-title = Склад Материалов
ore-silo-ui-label-clients = Машины
ore-silo-ui-label-mats = Материалы
ore-silo-ui-itemlist-entry = {$linked ->
    [true] {"[Сопряжено] "}
    *[False] {""}
} {$name} ({$beacon}) {$inRange ->
    [true] {""}
    *[false] (Вне Зоны Действия)
}

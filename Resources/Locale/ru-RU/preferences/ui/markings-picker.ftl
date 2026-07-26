markings-search = Поиск
-markings-selection = { $selectable ->
    [0] Вы больше не можете выбрать черт персонажа.
    [one] Вы можете выбрать ещё одну черту.
   *[other] Вы можете выбрать ещё { $selectable } черт.
}
markings-limits = { $required ->
    [true] { $count ->
        [-1] Выберите хотя бы одну черту.
        [0] Вы не можете выбрать ни одной черты, но, каким то образом, вы должны это сделать? Это баг.
        [one] Выберите одну черту.
       *[other] Выберите хотя бы одну черту и вплоть до {$count} черт. { -markings-selection(selectable: $selectable) }
    }
   *[false] { $count ->
        [-1] Выберите сколько угодно черт.
        [0] Вы не можете выбрать ни одной черты.
        [one] Выберите вплоть до одной черты.
       *[other] Выберите вплоть до {$count} черт. { -markings-selection(selectable: $selectable) }
    }
}
markings-reorder = Перераспределить черты

humanoid-marking-modifier-respect-limits = Уважать ограничения
humanoid-marking-modifier-respect-group-sex = Уважать ограничения по группе и полу
humanoid-marking-modifier-base-layers = Базовые слои
humanoid-marking-modifier-enable = Включить
humanoid-marking-modifier-prototype-id = ID прототипа:

# Categories

markings-organ-Torso = Торс
markings-organ-Head = Голова
markings-organ-ArmLeft = Левая рука
markings-organ-ArmRight = Правая рука
markings-organ-HandRight = Правая ладонь
markings-organ-HandLeft = Левая ладонь
markings-organ-LegLeft = Левая нога
markings-organ-LegRight = Правая нога
markings-organ-FootLeft = Левая ступня
markings-organ-FootRight = Правая ступня
markings-organ-Eyes = Глаза

markings-layer-Special = Особые
markings-layer-Tail = Хвост
markings-layer-Tail-Moth = Крылья
markings-layer-Hair = Волосы
markings-layer-FacialHair = Лицевая растительность
markings-layer-UndergarmentTop = Нижнее бельё (верх)
markings-layer-UndergarmentBottom = Нижнее бельё (низ)
markings-layer-Chest = Грудь
markings-layer-Head = Голова
markings-layer-Snout = Морда
markings-layer-SnoutCover = Морда (покрытие)
markings-layer-HeadSide = Голва (бока)
markings-layer-HeadTop = Голова (верх)
markings-layer-Eyes = Глаза
markings-layer-RArm = Правая рука
markings-layer-LArm = Левая рука
markings-layer-RHand = Правая ладонь
markings-layer-LHand = Левая ладонь
markings-layer-RLeg = Правая нога
markings-layer-LLeg = Левая нога
markings-layer-RFoot = Правая ступня
markings-layer-LFoot = Левая ступня
markings-layer-Overlay = Оверлей
markings-layer-TailOverlay = Оверлей


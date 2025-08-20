shared-solution-container-component-on-examine-main-text = Содержит { $chemCount ->
    [1] вещество.
   *[other] смесь веществ.
    }
    На вид [color={$color}]{$desc}[/color].

examinable-solution-has-recognizable-chemicals = Вы можете распознать {$recognizedString} в этом растворе.
examinable-solution-recognized = [color={$color}]{$chemical}[/color]

examinable-solution-on-examine-volume = Ёмкость { $fillLevel ->
    [exact] заполнена на [color=white]{$current}/{$max}ед[/color].
   *[other] [bold]{ -solution-vague-fill-level(fillLevel: $fillLevel) }[/bold].
}

examinable-solution-on-examine-volume-no-max = Ёмкость { $fillLevel ->
    [exact] заполнена на [color=white]{$current}ед[/color].
   *[other] [bold]{ -solution-vague-fill-level(fillLevel: $fillLevel) }[/bold].
}

examinable-solution-on-examine-volume-puddle = Лужа { $fillLevel ->
    [exact] содержит [color=white]{$current}ед[/color].
    [full] огромная и растекается!
    [mostlyfull] огромная и растекается!
    [halffull] глубокая и немного протекает.
    [halfempty] глубокая.
   *[mostlyempty] объединяется в одну.
    [empty] формирует множество мелких лужиц.
}

-solution-vague-fill-level =
    { $fillLevel ->
        [full] [color=white]заполнена[/color]
        [mostlyfull] [color=#DFDFDF]почти заполнена[/color]
        [halffull] [color=#C8C8C8]заполнена наполовину[/color]
        [halfempty] [color=#C8C8C8]наполовину пуста[/color]
        [mostlyempty] [color=#A4A4A4]почти пуста[/color]
       *[empty] [color=gray]пуста[/color]
    }

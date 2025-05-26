analysis-console-menu-title = Аналитическая Консоль Широкого Спектра 2-й Ревизии
analysis-console-server-list-button = Сервер
analysis-console-extract-button = Извлечь О.И.

analysis-console-info-no-scanner = Анализатор не подключён! Пожалуйста, подключите его при помощи мультитула.
analysis-console-info-no-artifact = Артефакт не найден! Поставьте артефакт на пластину сканера для просмотра информации.
analysis-console-info-ready = Системы в норме. Готово к сканированию.

analysis-console-no-node = Выберите узел для просмотра
analysis-console-info-id = [font="Monospace" size=11]ID:[/font]
analysis-console-info-id-value = [font="Monospace" size=11][color=yellow]{$id}[/color][/font]
analysis-console-info-class = [font="Monospace" size=11]Класс:[/font]
analysis-console-info-class-value = [font="Monospace" size=11]{$class}[/font]
analysis-console-info-locked = [font="Monospace" size=11]Статус:[/font]
analysis-console-info-locked-value = [font="Monospace" size=11][color={ $state ->
    [0] red]Закрыт
    [1] lime]Открыт
    *[2] plum]Активен
}[/color][/font]
analysis-console-info-durability = [font="Monospace" size=11]Прочность:[/font]
analysis-console-info-durability-value = [font="Monospace" size=11][color={$color}]{$current}/{$max}[/color][/font]
analysis-console-info-effect = [font="Monospace" size=11]Эффект:[/font]
analysis-console-info-effect-value = [font="Monospace" size=11][color=gray]{ $state ->
    [true] {$info}
    *[false] Разблокируйте узлы для просмотра информации
}[/color][/font]
analysis-console-info-trigger = [font="Monospace" size=11]Триггер:[/font]
analysis-console-info-triggered-value = [font="Monospace" size=11][color=gray]{$triggers}[/color][/font]
analysis-console-info-scanner = Сканирование...
analysis-console-info-scanner-paused = Приостановлено.
analysis-console-progress-text = {$seconds ->
    [one] T-{$seconds} секунда
    [few] T-{$seconds} секунды
    *[other] T-{$seconds} секунд
}

analysis-console-extract-value = [font="Monospace" size=11][color=orange]Узел {$id} (+{$value})[/color][/font]
analysis-console-extract-none = [font="Monospace" size=11][color=orange] У разблокированных узлов нет очков, которые можно извлечь [/color][/font]
analysis-console-extract-sum = [font="Monospace" size=11][color=orange]Total Research: {$value}[/color][/font]

analyzer-artifact-extract-popup = Поверхность артефакта сияет энергией!

delivery-recipient-examine = Адресат: {$recipient}, {$job}.
delivery-already-opened-examine = Уже вскрыто.
delivery-earnings-examine = Доставка принесёт станции [color=yellow]{$spesos}[/color] кредитов.
delivery-recipient-no-name = Безымянный
delivery-recipient-no-job = Неизвестно

delivery-unlocked-self = Вы разблокируете {$delivery} своим отпечатком пальца.
delivery-opened-self = Вы открываете {$delivery}.
delivery-unlocked-others = {CAPITALIZE($recipient)} разблокирует {$delivery} своим отпечатком пальца.
delivery-opened-others = {CAPITALIZE($recipient)} открывает {$delivery}.

delivery-unlock-verb = Разблокировать
delivery-open-verb = Открыть
delivery-slice-verb = Вскрыть

delivery-teleporter-amount-examine =
    { $amount ->
        [one] Он содержит [color=yellow]{$amount}[/color] посылку.
        [few] Он содержит [color=yellow]{$amount}[/color] посылки.
        *[other] Он содержит [color=yellow]{$amount}[/color] посылок.
    }
delivery-teleporter-empty = { CAPITALIZE($entity) } пуст.
delivery-teleporter-empty-verb = Взять почту


# modifiers
delivery-priority-examine = Это [color=orange]{$type} с пометкой "Срочное"[/color]. У вас есть [color=orange]{$time}[/color] на доставку, чтобы получить бонус.
delivery-priority-delivered-examine = Это [color=orange]{$type} с пометкой "Срочное"[/color]. Доставлено вовремя.
delivery-priority-expired-examine = Это [color=orange]{$type} с пометкой "Срочное"[/color]. Время истекло.

delivery-fragile-examine = Это [color=red]{$type} с пометкой "Хрупкое"[/color]. Доставьте в целости, чтобы получить бонус.
delivery-fragile-broken-examine = Это [color=red]{$type} с пометкой "Хрупкое"[/color]. Имеет сильные повреждения.

delivery-bomb-examine = Это [color=purple]{$type} с бомбой[/color]. О нет.
delivery-bomb-primed-examine = Это [color=purple]{$type} с бомбой [/color]. Читая это, вы тратите своё время впустую.

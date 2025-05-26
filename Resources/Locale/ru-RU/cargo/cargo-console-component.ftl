## UI
cargo-console-menu-title = Консоль запросов снабжения
cargo-console-menu-account-name-label = Счёт:{" "}
cargo-console-menu-account-name-none-text = Нет
cargo-console-menu-account-name-format = [bold][color={$color}]{$name}[/color][/bold] [font="Monospace"]\[{$code}\][/font]
cargo-console-menu-shuttle-name-label = Имя шаттла:{" "}
cargo-console-menu-shuttle-name-none-text = Нет
cargo-console-menu-points-label = Баланс:{" "}
cargo-console-menu-points-amount = ${$amount}
cargo-console-menu-shuttle-status-label = Статус шаттла:{" "}
cargo-console-menu-shuttle-status-away-text = Удалён
cargo-console-menu-order-capacity-label = Вместимость заказов:{" "}
cargo-console-menu-call-shuttle-button = Активировать телепад
cargo-console-menu-permissions-button = Разрешения
cargo-console-menu-categories-label = Категория:{" "}
cargo-console-menu-search-bar-placeholder = Поиск
cargo-console-menu-requests-label = Запросы
cargo-console-menu-orders-label = Заказы
cargo-console-menu-order-reason-description = Причины: {$reason}
cargo-console-menu-populate-categories-all-text = Все
cargo-console-menu-populate-orders-cargo-order-row-product-name-text = {$productName} (x{$orderAmount}) от {$orderRequester} из [color={$accountColor}]{$account}[/color]
cargo-console-menu-cargo-order-row-approve-button = Одобрить
cargo-console-menu-cargo-order-row-cancel-button = Отменить
cargo-console-menu-tab-title-orders = Заказы
cargo-console-menu-tab-title-funds = Переводы
cargo-console-menu-account-action-transfer-limit = [bold]Лимит переводов:[/bold] ${$limit}
cargo-console-menu-account-action-transfer-limit-unlimited-notifier = [color=gold](Безлимитный)[/color]
cargo-console-menu-account-action-select = [bold]Действие со счётом:[/bold]
cargo-console-menu-account-action-amount = [bold]Количество:[/bold] $
cargo-console-menu-account-action-button = Перевести
cargo-console-menu-toggle-account-lock-button = Переключить Лимит Переводов
cargo-console-menu-account-action-option-withdraw = Вывести Средства
cargo-console-menu-account-action-option-transfer = Перевести Средства {$code}

# Orders
cargo-console-order-not-allowed = Доступ запрещён
cargo-console-station-not-found = Нет доступной станции
cargo-console-invalid-product = Неверный ID продукта
cargo-console-too-many = Слишком много одобренных заказов
cargo-console-snip-snip = Заказ обрезан до вместимости
cargo-console-insufficient-funds = Недостаточно средств (требуется {$cost})
cargo-console-unfulfilled = Нет места для выполнения заказа
cargo-console-trade-station = Отправлено в {$destination}
cargo-console-unlock-approved-order-broadcast = Заказ на [bold]{$productName} x{$orderAmount}[/bold], стоимостью [bold]{$cost}[/bold], одобрен [bold]{$approver}[/bold]
cargo-console-fund-withdraw-broadcast = [bold]{$name} вывел {$amount} кредитов из {$name1} \[{$code1}\]
cargo-console-fund-transfer-broadcast = [bold]{$name} перевёл {$amount} кредитов {$name1} \[{$code1}\] в {$name2} \[{$code2}\][/bold]
cargo-console-fund-transfer-user-unknown = Неизвестный

cargo-console-paper-reason-default = Нет
cargo-console-paper-approver-default = Сам
cargo-console-paper-print-name = Заказ #{$orderNumber}
cargo-console-paper-print-text = [head=2]Заказ #{$orderNumber}[/head]
    {"[bold]Предмет:[/bold]"} {$itemName} (x{$orderQuantity})
    {"[bold]Запрошено:[/bold]"} {$requester}

    {"[head=3]Информация о заказе[/head]"}
    {"[bold]Плательщик[/bold]:"} {$account} [font="Monospace"]\[{$accountcode}\][/font]
    {"[bold]Одобрено:[/bold]"} {$approver}
    {"[bold]Причина:[/bold]"} {$reason}

# Cargo shuttle console
cargo-shuttle-console-menu-title = Консоль шаттла снабжения
cargo-shuttle-console-station-unknown = Неизвестно
cargo-shuttle-console-shuttle-not-found = Не найден
cargo-shuttle-console-organics = На шаттле замечены органический формы жизни.
cargo-no-shuttle = Шаттл снабжения не найден!

# Funding allocation console
cargo-funding-alloc-console-menu-title = Консоль Распределения Бюджета
cargo-funding-alloc-console-label-account = [bold]Счёт[/bold]
cargo-funding-alloc-console-label-code = [bold] Код [/bold]
cargo-funding-alloc-console-label-balance = [bold] Баланс [/bold]
cargo-funding-alloc-console-label-cut = [bold] Разделение Доходов (%) [/bold]

cargo-funding-alloc-console-label-primary-cut = Доля отдела снабжения из источников, не являющимися ящиками с замком (%):
cargo-funding-alloc-console-label-lockbox-cut = Доля отдела снабжения от продажи ящиков с замком (%):

cargo-funding-alloc-console-label-help-non-adjustible = Отдел снабжения получает {$percent}% доходов из источников, не являющимися ящиками с замком. Остаток разделяется как указано ниже:
cargo-funding-alloc-console-label-help-adjustible = Оставшиеся средства из источников, не являющимися ящиками с замком распределяются как указано ниже:
cargo-funding-alloc-console-button-save = Сохранить Изменения
cargo-funding-alloc-console-label-save-fail = [bold]Неверное Разделение Доходов![/bold] [color=red]({$pos ->
    [1] +
    *[-1] -
}{$val}%)[/color]

# Slip template
cargo-acquisition-slip-body = [head=3]Детали Заказа[/head]
    {"[bold]Продукт:[/bold]"} {$product}
    {"[bold]Описание:[/bold]"} {$description}
    {"[bold]Стоимость единицы:[/bold"}] ${$unit}
    {"[bold]Количество:[/bold]"} {$amount}
    {"[bold]Итого:[/bold]"} ${$cost}

    {"[head=3]Детали Покупки[/head]"}
    {"[bold]Заказчик:[/bold]"} {$orderer}
    {"[bold]Причина:[/bold]"} {$reason}

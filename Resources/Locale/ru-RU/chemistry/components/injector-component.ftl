## UI

injector-volume-transfer-label = Объём: [color=white]{$currentVolume}/{$totalVolume}ед[/color]
    Режим: [color=white]{$modeString}[/color] ([color=white]{$transferVolume}ед[/color])
injector-volume-label = Volume: [color=white]{$currentVolume}/{$totalVolume}ед[/color]
    Режим: [color=white]{$modeString}[/color]
injector-toggle-verb-text = Переключить Режим Инъектора

## Entity

injector-component-inject-mode-name = введение
injector-component-draw-mode-name = набор
injector-component-dynamic-mode-name = динамический
injector-component-mode-changed-text = Теперь {$mode}
injector-component-transfer-success-message = Вы переводите {$amount}ед в {$target}.
injector-component-transfer-success-message-self = Вы переводите {$amount}ед себя.
injector-component-inject-success-message = Вы вводите {$amount}ед в {$target}!
injector-component-inject-success-message-self = Вы вводите {$amount}ед в себя!
injector-component-draw-success-message = Вы набираете {$amount}ед из {$target}.
injector-component-draw-success-message-self = Вы набираете {$amount}ед из себя.

## Fail Messages

injector-component-target-already-full-message = {CAPITALIZE($target)} уже полон!
injector-component-target-already-full-message-self = Вы уже переполнены!
injector-component-target-is-empty-message = {CAPITALIZE($target)} пуст!
injector-component-target-is-empty-message-self = Вы пусты!
injector-component-cannot-toggle-draw-message = Слишком заполнено для набора!
injector-component-cannot-toggle-inject-message = Нечего вводить!
injector-component-cannot-toggle-dynamic-message = Нельзя переключить динамический!
injector-component-empty-message = {CAPITALIZE($injector)} пуст!
injector-component-blocked-user = Защитное снаряжение заблокировало вашу инъекцию!
injector-component-blocked-other = Броня {CAPITALIZE($target)} заблокировала инъекцию {$user}!
injector-component-cannot-transfer-message = Вы не можете перевести в {$target}!
injector-component-cannot-transfer-message-self = Вы не можете перевести в себя!
injector-component-cannot-inject-message = Вы не можете ввести в {$target}!
injector-component-cannot-inject-message-self = Вы не можете ввести в себя!
injector-component-cannot-draw-message = Вы не можете набрать из {$target}!
injector-component-cannot-draw-message-self = Вы не можете набрать из себя!
injector-component-ignore-mobs = Этот инъектор может взаимодействовать только с контейнерами!

## mob-inject doafter messages

injector-component-needle-injecting-user = Вы начинаете вводить иглу.
injector-component-needle-injecting-target = {CAPITALIZE($user)} пытается ввести в вас иглу!
injector-component-needle-drawing-user = Вы начинаете набирать через иглу.
injector-component-needle-drawing-target = {CAPITALIZE($user)} пытается использовать иглу, чтобы набрать из вас что-то!
injector-component-spray-injecting-user = Вы начинаете подготавливать сопло спрея.
injector-component-spray-injecting-target = {CAPITALIZE($user)} пытается поставить сопло спрея на вас!

## Target Popup Success messages
injector-component-feel-prick-message = Вы чувствуете слабый укольчик!

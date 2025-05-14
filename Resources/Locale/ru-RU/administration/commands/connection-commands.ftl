## Strings for the "grant_connect_bypass" command.

cmd-grant_connect_bypass-desc = Временно позволяет пользователю обходить проверки подключения.
cmd-grant_connect_bypass-help =
    Использзование: grant_connect_bypass <пользователь> [время в минутах]
    Временно позволяет пользователю обходить обычные ограничения подключения.
    Обход работает только на этом сервере и будет закончен через (по умолчанию) 1 час.
    Они смогут подключаться вне зависимости от бункера, белого списка или ограничения числа игроков.
cmd-grant_connect_bypass-arg-user = <пользователь>
cmd-grant_connect_bypass-arg-duration = [время в минутах]
cmd-grant_connect_bypass-invalid-args = Ожидается 1 или 2 аргумента
cmd-grant_connect_bypass-unknown-user = Невозможно найти пользователя '{ $user }'
cmd-grant_connect_bypass-invalid-duration = Неправильное значение: '{ $duration }'
cmd-grant_connect_bypass-success = Успешно добавлен обход для пользователя '{ $user }'

### Interaction Messages

# System

## When trying to ingest without the required utensil... but you gotta hold it
ingestion-you-need-to-hold-utensil = Вам нужно держать {$utensil}, чтобы есть это!

ingestion-try-use-is-empty = {CAPITALIZE($entity)} пуст!
ingestion-try-use-wrong-utensil = Вы не можете {$verb} {$food} при помощи {$utensil}.

ingestion-remove-mask = You need to take off the {$entity} first.

## Failed Ingestion

ingestion-you-cannot-ingest-any-more = В вас больше не лезет!
ingestion-other-cannot-ingest-any-more = {CAPITALIZE(SUBJECT($target))} не может {$verb} больше!

ingestion-cant-digest = Вы не можете переварить {$entity}!
ingestion-cant-digest-other = {CAPITALIZE(SUBJECT($target))} не может переварить {$entity}!

## Action Verbs, not to be confused with Verbs

ingestion-verb-food = Съесть
ingestion-verb-drink = Выпить

# Edible Component

edible-nom = Ням. {$flavors}
edible-nom-other = Ням.
edible-slurp = Слёрп. {$flavors}
edible-slurp-other = Слёрп.
edible-swallow = Вы глотаете { $food }
edible-gulp = Гулп. {$flavors}
edible-gulp-other = Гулп.

edible-has-used-storage = Вы не можете {$verb} { $food } с помещённым внутрь предметом.

## Nouns

edible-noun-edible = нечто съедобное
edible-noun-food = еду
edible-noun-drink = напиток
edible-noun-pill = таблетку

## Verbs

edible-verb-edible = усваивоить
edible-verb-food = съесть
edible-verb-drink = выпить
edible-verb-pill = проглотить

## Force feeding

edible-force-feed = {CAPITALIZE($user)} пытается заставить вас {$verb} что-то!
edible-force-feed-success = {CAPITALIZE($user)} заставляет вас {$verb} что-то! {$flavors}
edible-force-feed-success-user = Вы успешно накормили {$target}

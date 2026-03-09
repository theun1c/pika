# Текущий статус проекта

Проект: Pika

Стадия разработки: ранняя исследовательская.

---

# Что уже сделано

- создан Git repository
- написан README
- создан базовый Avalonia Desktop проект
- описана предварительная архитектура
- добавлены AI docs
- создан тестовый проект-песочница Pika.Sandbox
- произведено исследование источников
- выполнены базовые HTTP запросы
- сохранены примеры ответов
---

# Текущий подход разработки

Разработка начинается с **исследования источников**, а не с полной архитектуры.

Причина:

структура данных источников пока неизвестна.

Необходимо сначала понять:

- какие данные доступны
- как устроены страницы
- есть ли API
- какие поля можно получить
- как выглядят episodes и playback options

---

# План разработки (текущий)

1. Изучить сайты и API источников
2. Найти закономерности в данных
3. Сохранять примеры ответов и HTML в `docs/samples`
4. Создать sandbox проект
5. Реализовать базовый парсинг
6. Протестировать все в Pika.Sandbox
7. После исследования выделить финальную архитектуру и модели

---

# Sandbox

Для исследования используется отдельный проект:

Pika.Sandbox

Это временный Web API проект.

В нем будут находиться:

- экспериментальные модели
- черновая бизнес-логика
- тестовый парсинг
- HTTP запросы к источникам

Sandbox может содержать код, который **не соответствует финальной архитектуре**.

Его задача — исследование.

---

# После Sandbox

Когда станет понятно:

- какие сущности реально нужны
- какие данные доступны
- какие структуры используются

будет выполнен рефакторинг:

Sandbox код будет разделен на:

Core / Domain  
Infrastructure  
Connectors  
API  
Worker

---

# Следующая задача разработки

Начать: 

- разработку чернового парсинга одной конкретной страницы 
- доказать, что можно получить m3u8 и прочие provider (kodik) metadata
- зафиксировать выводы


# Ход мыслей (Roadmap на бумаге)

Я перехожу на страницу с нужным мне тайтлом https://animego.me/anime/naruto-uragannye-hroniki-103

В плеере есть sidebar с выбором озвучки: 
```html
<div class="list-group pt-2 position-absolute w-100">
    <button class="align-items-center d-flex gap-2 list-group-item list-group-item-action mb-1 active" role="button" data-translation="9"><span class="text-truncate">2x2</span> <span class="error__player d-none text-danger small text-nowrap">(ошибка)</span></button>
    <button class="align-items-center d-flex gap-2 list-group-item list-group-item-action mb-1" role="button" data-translation="1"><span class="text-truncate">AniDUB</span> <span class="error__player d-none text-danger small text-nowrap">(ошибка)</span></button>
    <button class="align-items-center d-flex gap-2 list-group-item list-group-item-action mb-1" role="button" data-translation="4"><span class="text-truncate">SHIZA Project</span> <span class="error__player d-none text-danger small text-nowrap">(ошибка)</span></button>
</div>
```

Его я могу парсить для того, чтобы собрать в metadata информацию об озвучке. 

Также в дальнейшем при нажатии на одну из кнопок ссылка на m3u8 файл будет меняться, что также необходимо учитывать, для правильного вывода озвучки 

После мне нужно будет имитировать нажатие на кнопку play:
```html
<path d="M85.5 0h-83A2.5 2.5 0 0 0 0 2.5v83A2.5 2.5 0 0 0 2.5 88h83a2.5 2.5 0 0 0 2.5-2.5v-83A2.5 2.5 0 0 0 85.5 0zM61 45.1L33.66 60.31a.75.75 0 0 1-.82.06.81.81 0 0 1-.34-.74V29.29a.84.84 0 0 1 .34-.75.79.79 0 0 1 .82.06L61 43.82c.32.18.48.39.48.62a.76.76 0 0 1-.48.66z" class="hover fill"></path>
```

После чего в Network вкладке браузера (F12) появится подобный GET запрос:

``` 
https://p92.kodik.info/s/m/aHR0cHM6Ly9jbG91ZC5rb2Rpay1zdG9yYWdlLmNvbS91c2VydXBsb2Fkcy8wMWY2MjE1Yi0wYTg5LTQ4NDgtOGYxYi0zZGRjMGVkZDhkYWI/f3f72f41d3b4c47efc09416ba7fc84a15c08c5879e21b3e30985ad241164dfe5:2026030906/480.mp4:hls:manifest.m3u8
```

Это нужный мне m3u8 файл, который я потом смогу вставлять внутрь VLS Avalonia

ВАЖНО :
- ссылка на файл не вечная
- 1 файл соответствует 1 качеству видео (их обычно около 3х)

После получения одной ссылки одного качества, нужно получить ссылки на видео с остальным качеством (720, 360)

Чтобы понять какое качество вообще есть на данном тайтле, можно проследить в запросе внутри Network следующее: 
``` 
POST Запрос: https://kodik.info/ftor
```

Он возвращает следующий response:
``` 
{"advert_script":"","domain":"animego.me","default":360,"links":{"360":[{"src":"iPZ0kPU6Tg9eVhoci29siEaciE5ujg9hT20dGCpAUOVQBBHUmBtyGsk5UDxLVFRqUtReGFsfmuZPWFtHD2ZaBO1WluRBWBNrUtH5HNpKk2QgZubrmBp3BDlHUs1yZBNHiBJ3EDZvVCfCCBZWZOl0B0lHmNtxUPxiZ1RyBClEi1xMiObHD0sdHrVuVhRuVLNsU2Q0GhY3HEHrULs0UBHqGBluGho0GBM1GhI4GhC4VhttUrNqU2ChULs4VENsUrYfUBG0HOHtVBwgULQ2ULUeWBI2ThU2UK5bkLY6iOfhWu1pjutuHFV0Tu0hlBo","type":"application/x-mpegURL"}],"480":[{"src":"iPZ0kPU6Tg9eUBQci29siEaciE5ujg9hT20dGCpAUOVQBBHUmBtyGsk5UDxLVFRqUtReGFsfmuZPWFtHD2ZaBO1WluRBWBNrUtH5HNpKk2QgZubrmBp3BDlHUs1yZBNHiBJ3EDZvVCfCCBZWZOl0B0lHmNtxUPxiZ1RyBClEi1xMiObHD0sdHrVuVhRuVLNsU2Q0GhY3HEHrULs0UBHqGBluGho0GBM1GhI4GhC4VhttUrNqU2ChULs4VENsUrYfUBG0HOHtVBwgULQ2ULUeWBI2ThY4UK5bkLY6iOfhWu1pjutuHFV0Tu0hlBo","type":"application/x-mpegURL"}],"720":[{"src":"iPZ0kPU6Tg9eVhoci29siEaciE5ujg9hT20dGCpAUOVQBBHUmBtyGsk5UDxLVFRqUtReGFsfmuZPWFtHD2ZaBO1WluRBWBNrUtH5HNpKk2QgZubrmBp3BDlHUs1yZBNHiBJ3EDZvVCfCCBZWZOl0B0lHmNtxUPxiZ1RyBClEi1xMiObHD0sdHrVuVhRuVLNsU2Q0GhY3HEHrULs0UBHqGBluGho0GBM1GhI4GhC4VhttUrNqU2ChULs4VENsUrYfUBG0HOHtVBwgULQ2ULUeWBI2ThY4UK5bkLY6iOfhWu1pjutuHFV0Tu0hlBo","type":"application/x-mpegURL"}]},"vast":[{"title_small":"bu-t","src":"https://exchange.buzzoola.com/ad/1285159","hide_interface":true,"async_load":true,"vpaid_target_event":"AdImpression","vpaid_max_load_time":5,"vpaid_max_start_time":10},{"title_small":"scnd","timer":5,"src":"https://pet.umwiba.com/vast/second/manifest.xml"}],"reserve_vast":[{"title_small":"bu-m","src":"https://exchange.buzzoola.com/ad/1267453","hide_interface":true,"async_load":true,"vpaid_target_event":"AdImpression","vpaid_max_load_time":5,"vpaid_max_start_time":10},{"title_small":"mp_c","src":"https://wrapper.bracdn.online/wrapper2.php?vast=//yandex.ru/ads/adfox/12359998/getCode?p1=dgbygamp;p2=jjsh","async_load":true,"max_length":40,"vpaid_target_event":"AdImpression","vpaid_max_load_time":5,"vpaid_max_start_time":10,"hide_interface":true},{"title_small":"adp","src":"https://a.snsv.ru/vast/62cab44c2577409d95e12bfb","max_length":60,"vpaid_max_load_time":10,"vpaid_max_start_time":20,"vpaid_start_event":"AdVideoStart","vpaid_impression_event":"adp_ads_viewed","vpaid_target_event":"adp_ads_viewed","hide_interface":true,"async_load":true},{"title_small":"bu-a","src":"https://exchange.buzzoola.com/ad/1217814","hide_interface":true,"async_load":true,"vpaid_target_event":"AdImpression","vpaid_max_load_time":5,"vpaid_max_start_time":10},{"title_small":"pyr","src":"https://srv.marketer.lv/vast?z=105830&tid1=1,2,3,4,5&num=1&noad=1","max_length":40,"vpaid_max_load_time":5,"vpaid_max_start_time":10,"vpaid_target_event":"AdVideoStart","send_stat_method":"image","advert_id":"pyramid"},{"title_small":"adp_u","src":"https://a.snsv.ru/vast/65f9b797a27aa6f0b7f6b69f","max_length":60,"vpaid_max_load_time":5,"vpaid_max_start_time":10,"vpaid_start_event":"AdVideoStart","vpaid_impression_event":"adp_ads_viewed","vpaid_target_event":"adp_ads_viewed","hide_interface":true,"async_load":true},{"title_small":"iz","async_load":true,"timer":15,"src":"https://code.21wiz.com/ovpaid.php?v=a13cf7df8058e05a3ee2f5479b694d8b&position=pre","max_length":60,"vpaid_target_event":"AdImpression","vpaid_max_load_time":5,"vpaid_max_start_time":10,"hide_interface":true},{"title_small":"iz","async_load":true,"timer":15,"src":"https://code.21wiz.com/ovpaid.php?v=a13cf7df8058e05a3ee2f5479b694d8b&position=pre","max_length":60,"vpaid_target_event":"AdImpression","vpaid_max_load_time":5,"vpaid_max_start_time":10,"hide_interface":true},{"title_small":"mv_t","src":"https://code.moviead55.ru/ovpaid.php?v=d90808d9b555b246706da7b29cf0b8aa&position=pre&cp.referer=https%3A%2F%2Fanimego.me%2F","vpaid_max_load_time":10,"vpaid_max_start_time":10,"max_length":60,"vpaid_target_event":"AdImpression","vpaid_start_event":["AdImpression","AdVideoStart"],"async_load":true,"send_stat_method":"image","timer":15},{"title_small":"adp_t","src":"https://a.snsv.ru/vast/65168a386caa69d54ab8d07c","max_length":60,"vpaid_max_load_time":5,"vpaid_max_start_time":10,"vpaid_start_event":"AdVideoStart","vpaid_impression_event":"adp_ads_viewed","vpaid_target_event":"adp_ads_viewed","hide_interface":true,"async_load":true},{"title_small":"mp_c_2","src":"https://wrapper.bracdn.online/wrapper2.php?vast=//yandex.ru/ads/adfox/12359998/getCode?p1=dgbyfamp;p2=jjsh","async_load":true,"max_length":40,"vpaid_target_event":"AdImpression","vpaid_max_load_time":5,"vpaid_max_start_time":10,"hide_interface":true},{"title_small":"mp_uzaz_2","src":"https://adx.stack.bidster.net/vast/ca2437da-3e84-4d17-842d-581f8edcf8d2?domain=animego.me","async_load":true,"max_length":60,"vpaid_target_event":"AdImpression","vpaid_max_load_time":5,"vpaid_max_start_time":10,"hide_interface":true},{"title_small":"vib-a-f","src":"https://vidalak.com/api/vpaid?userId=314261&format=4&sig=cf5e8f98c44764eb","async_load":true,"hide_interface":true,"vpaid_target_event":"AdImpression","vpaid_max_load_time":10,"vpaid_max_start_time":20},{"title_small":"adp_m","src":"https://a.snsv.ru/vast/658089f07412859dd2802be9","max_length":60,"vpaid_max_load_time":5,"vpaid_max_start_time":10,"vpaid_start_event":"AdVideoStart","vpaid_impression_event":"adp_ads_viewed","vpaid_target_event":"adp_ads_viewed","hide_interface":true,"async_load":true},{"title_small":"wis","src":"https://franecki.net/assets/vendor/6f8f1160c2220180e0e939b54e85d950.xml?v=3.0&external_subid=animego.me","max_length":60,"vpaid_max_load_time":5,"vpaid_max_start_time":10,"send_stat_method":"image","async_load":true,"hide_interface":true,"timer":15},{"title_small":"wis_2","src":"https://franecki.net/assets/vendor/f13a12b0332574ba83884b6da4b86bab.xml?v=3.0&external_subid=animego.me","max_length":60,"vpaid_max_load_time":5,"vpaid_max_start_time":10,"send_stat_method":"image","async_load":true,"hide_interface":true,"timer":15},{"title_small":"abt-o","src":"https://z.cdn.traffmovie.com/load?o=v&z=2100321148&random=[random]#Kdf5wxwO9my","max_length":30,"vpaid_max_load_time":10,"vpaid_max_start_time":20,"vpaid_target_event":"AdImpression","vpaid_ad_skippable_state":false,"send_stat_method":"image","timer":15},{"title_small":"mp","src":"https://yandex.ru/ads/adfox/12359998/getCode?p1=dgbye&p2=jjsh","async_load":true,"max_length":40,"vpaid_target_event":"AdImpression","vpaid_max_load_time":10,"vpaid_max_start_time":20,"hide_interface":true},{"title_small":"mkach","src":"https://spylees.com/vast.php?hash=S7TDs6f3IC4FX7kO","async_load":true,"max_length":30,"vpaid_max_load_time":10,"vpaid_max_start_time":20,"vpaid_target_event":"AdImpression","hide_interface":true,"timer":15},{"title_small":"wip","async_load":true,"timer":15,"src":"https://flyroll.ru/vast/vpaid.xml?host=animego.me&rnd=144232","max_length":40,"vpaid_target_event":"AdImpression","vpaid_max_load_time":5,"vpaid_max_start_time":10,"send_stat_method":"image","hide_interface":true},{"title_small":"mp2","src":"https://yandex.ru/ads/adfox/12359998/getCode?p1=dgbyd&p2=jjsh","async_load":true,"max_length":40,"vpaid_target_event":"AdImpression","vpaid_max_load_time":10,"vpaid_max_start_time":20,"hide_interface":true},{"title_small":"mp_uzaz","src":"https://adx.stack.bidster.net/vast/a2995884-f8cf-4fa4-bd76-ca460bd687f1?domain=animego.me","async_load":true,"max_length":60,"vpaid_target_event":"AdImpression","vpaid_max_load_time":5,"vpaid_max_start_time":10,"hide_interface":true},{"title_small":"moe_res","src":"https://ad.moe.video/vast?pid=10187&vpt=middle&advertCount=1&vt=vpaid&rnd=16598&referer=https%3A%2F%2Fanimego.me%2F","async_load":true,"max_length":40,"vpaid_start_event":"AdVideoStart","vpaid_timer_start_event":"AdVideoStart","vpaid_target_event":"AdImpression","vpaid_max_load_time":20,"vpaid_max_start_time":120,"hide_interface":true,"advert_id":"moevideo_ult","start_muted":true},{"title_small":"moe_res","src":"https://ad.moe.video/vast?pid=10187&vpt=middle&advertCount=1&vt=vpaid&rnd=69030&referer=https%3A%2F%2Fanimego.me%2F","async_load":true,"max_length":40,"vpaid_start_event":"AdVideoStart","vpaid_timer_start_event":"AdVideoStart","vpaid_target_event":"AdImpression","vpaid_max_load_time":20,"vpaid_max_start_time":120,"hide_interface":true,"advert_id":"moevideo_ult","start_muted":true},{"title_small":"adp","src":"https://xcec.ru/vpaids/vpaid50B.php?id=2353","max_length":60,"vpaid_max_load_time":10,"vpaid_max_start_time":20,"vpaid_start_event":"AdVideoStart","vpaid_impression_event":"adp_ads_viewed","vpaid_target_event":"adp_ads_viewed","hide_interface":true,"async_load":true},{"title_small":"mkach","src":"https://spylees.com/vast.php?hash=apd3wU1P0MkSaxmR","async_load":true,"max_length":30,"vpaid_max_load_time":10,"vpaid_max_start_time":20,"vpaid_target_event":"AdImpression","hide_interface":true,"timer":15},{"title_small":"seed-n","src":"https://ssp.adseedtech.com/vast/e60f8a3a-ccdc-4d5f-b2eb-26268f630605?domain=animego.me&page=https%3A%2F%2Fanimego.me%2F","async_load":true,"max_length":30,"vpaid_start_event":"AdImpression","vpaid_target_event":"AdImpression","vpaid_max_load_time":5,"vpaid_max_start_time":10,"timer":15},{"title_small":"rock-a","src":"https://vast.ufouxbwn.com/vast.php?partner_id=5639530&ad_type=native&tip=white,gray,black&ref=https%3A%2F%2Fanimego.me%2F&real_source=https%3A%2F%2Fanimego.me%2F&cid=281884,tt0988824#BQSBKm02X5w","vpaid_max_load_time":10,"vpaid_max_start_time":20,"vpaid_start_event":"AdStarted","vpaid_target_event":"AdImpression","hide_interface":true,"async_load":true,"start_muted":false,"timer":20,"stop_timer_on_pause":true,"send_stat_method":"image"},{"title_small":"view_ru","async_load":true,"timer":10,"src":"https://r5.adstag0102.xyz/code/video-steam/?id=15&vast=3&channel1=animego.me","max_length":40,"vpaid_target_event":"AdImpression","vpaid_max_load_time":10,"vpaid_max_start_time":20},{"title_small":"adp","src":"https://xcec.ru/vpaids/vpaid50B.php?id=2175","max_length":60,"vpaid_max_load_time":10,"vpaid_max_start_time":20,"vpaid_start_event":"AdVideoStart","vpaid_impression_event":"adp_ads_viewed","vpaid_target_event":"adp_ads_viewed","hide_interface":true,"async_load":true}],"ip":"94.140.115.97"}
```

Тут можно выделить один объект links:
``` 
{
	"links": {
		"360": [
			{
				"src": "iPZ0kPU6Tg9eVhoci29siEaciE5ujg9hT20dGCpAUOVQBBHUmBtyGsk5UDxLVFRqUtReGFsfmuZPWFtHD2ZaBO1WluRBWBNrUtH5HNpKk2QgZubrmBp3BDlHUs1yZBNHiBJ3EDZvVCfCCBZWZOl0B0lHmNtxUPxiZ1RyBClEi1xMiObHD0sdHrVuVhRuVLNsU2Q0GhY3HEHrULs0UBHqGBluGho0GBM1GhI4GhC4VhttUrNqU2ChULs4VENsUrYfUBG0HOHtVBwgULQ2ULUeWBI2ThU2UK5bkLY6iOfhWu1pjutuHFV0Tu0hlBo",
				"type": "application/x-mpegURL"
			}
		],
		"480": [
			{
				"src": "iPZ0kPU6Tg9eUBQci29siEaciE5ujg9hT20dGCpAUOVQBBHUmBtyGsk5UDxLVFRqUtReGFsfmuZPWFtHD2ZaBO1WluRBWBNrUtH5HNpKk2QgZubrmBp3BDlHUs1yZBNHiBJ3EDZvVCfCCBZWZOl0B0lHmNtxUPxiZ1RyBClEi1xMiObHD0sdHrVuVhRuVLNsU2Q0GhY3HEHrULs0UBHqGBluGho0GBM1GhI4GhC4VhttUrNqU2ChULs4VENsUrYfUBG0HOHtVBwgULQ2ULUeWBI2ThY4UK5bkLY6iOfhWu1pjutuHFV0Tu0hlBo",
				"type": "application/x-mpegURL"
			}
		],
		"720": [
			{
				"src": "iPZ0kPU6Tg9eVhoci29siEaciE5ujg9hT20dGCpAUOVQBBHUmBtyGsk5UDxLVFRqUtReGFsfmuZPWFtHD2ZaBO1WluRBWBNrUtH5HNpKk2QgZubrmBp3BDlHUs1yZBNHiBJ3EDZvVCfCCBZWZOl0B0lHmNtxUPxiZ1RyBClEi1xMiObHD0sdHrVuVhRuVLNsU2Q0GhY3HEHrULs0UBHqGBluGho0GBM1GhI4GhC4VhttUrNqU2ChULs4VENsUrYfUBG0HOHtVBwgULQ2ULUeWBI2ThY4UK5bkLY6iOfhWu1pjutuHFV0Tu0hlBo",
				"type": "application/x-mpegURL"
			}
		]
	}
}
```

В данном объекте сопоставляются закодированные ссылки на m3u8 файлы с качеством файла. Это означает то, что из этого файла мы можем получить информацию о доступных качествах

Также после нажатия на:

``` html
<svg xmlns="http://www.w3.org/2000/svg" width="9" height="9" viewBox="0 0 451.847 451.847"><path d="M226 354.7c-8.2 0-16.3-3-22.4-9.3L9.3 151.2C-3 138.8-3 118.8 9.3 106.4 21.6 94 41.7 94 54 106.4l172 172 171.8-172c12.4-12.3 32.4-12.3 44.8 0 12.3 12.4 12.3 32.4 0 44.8L248.3 345.4c-6.2 6.2-14.3 9.3-22.4 9.3z" class="fp-fill"></path></svg>
```

Появляется следующее: 
```html
<div class="items"><div class="hd" data-quality="720">720p</div><div class="current" data-quality="480">480p</div><div class="" data-quality="360">360p</div></div>
```

При нажатии на элемент, меняется ссылка на видео и мы получаем m3u8 с нужным качеством.

Для начала в Pika.Sandbox будем получать m3u8 с дефолтным качеством, у одной из серий любого тайтла для тестирования возможности


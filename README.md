# Бекенд веб-редактора REAL.NET

### Сервисы

#### Gateway

Сервис для перенаправления запросов в бекенд по конкретным сервисам. Там же реализована проверка токена авторизации для любого запроса, кроме как на авторизацию. Используется библиотека `Ocelot` для описания прех перенаправлений в одном JSON-файле.

По умолчанию стартует на `8000` порту

#### Auth

Сервис авторизации. Имеет данные об аккаунтах, поддерживает регистрацию. Авторизация происходит выписыванием токена JWT, который затем проверяется на Gateway

Стартует на `8002` порту, требует файл БД. Путь `/api/auth`


#### Repo

Основной сервис управления языком. Представляет собой API обертку над репозиторием оригинального REAL.NET. 
Для справки и тестирования запросов можно использовать `Swagger` (`api/repo/swagger`).

Стартует на `8004` порту. Путь `/api/repo`


#### Storage

Сервис хранения данных о моделях. Хранит информацию для каждого пользователя о сохраненных репозиториях. Сохраняет и загружает репозитории. 
Сохранение и загрузку стоит осуществлять через этот сервис, а не напрямую через `Repo`.
Для справки и тестирования запросов можно использовать `Swagger` (`api/storage/swagger`).

Стартует на `8006` порту. Путь `/api/storage`


#### Test 

Ничего не умеет, никому не нужен

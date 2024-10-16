# TestReactForum

## Документация
Чтобы ознакомится с документацией перейдите по [ссылке](https://german322vdk.github.io/TestForumServer/api/index.html)

[![Testing](https://github.com/German322VDK/TestForumServer/actions/workflows/Testing.yml/badge.svg)](https://github.com/German322VDK/TestForumServer/actions/workflows/Testing.yml)

# TestReactForum

## Описание проекта
TestReactForum — это проект, который включает в себя API на ASP.NET Core, клиентскую часть на React JS, сервер Nginx и контейнеризацию с помощью Docker. Проект предоставляет функциональность для управления форумом, включая создание и взаимодействие с постами, комментариями и темами обсуждения.

## Структура проекта
```
TestForumServer
├── Program
├── Startup
├── TestForumServer.Controllers
│   ├── CommentController
│   ├── PostController
│   ├── TradController
│   └── UsersController
├── TestForumServer.Database.Context
│   └── TestForumDbContext
├── TestForumServer.Domain.Entities.ForumEntities
│   ├── Base
│   │   └── Entity
│   ├── Contents
│   │   ├── CommentEntity
│   │   ├── ContentEntity
│   │   ├── PostEntity
│   │   └── TradEntity
│   ├── Likes
│   │   ├── CommentLikeEntity
│   │   ├── LikeEntity
│   │   ├── PostLikeEntity
│   │   └── TradLikeEntity
│   ├── Identity
│   │   ├── RoleEntity
│   │   ├── UserEntity
│   │   └── UserStatus
├── TestForumServer.Domain.ViewModels.ForumViewModels
│   ├── Contents
│   │   ├── Base
│   │   │   ├── CommentViewModel
│   │   │   ├── ContentViewModel
│   │   │   ├── ImageViewModel
│   │   │   ├── PostViewModel
│   │   │   ├── TradShortViewModel
│   │   │   └── TradViewModel
│   │   ├── FromView
│   │   │   ├── CommentFromView
│   │   │   ├── ContentFromView
│   │   │   ├── PostFromView
│   │   │   └── TradFromView
│   ├── Identities
│   │   ├── LoginViewModel
│   │   ├── RegisterViewModel
│   │   ├── UserContentViewModel
│   │   ├── UserInfoViewModel
│   │   ├── UserLongModel
│   │   └── UserRefViewModel
├── TestForumServer.Infrastructure.Initializers
│   └── TestForumDbInitializer
├── TestForumServer.Infrastructure.Mapping
│   ├── ContentMapper
│   └── UserMapper
├── TestForumServer.Infrastructure.Services.Identity
│   └── UserManagerExtensions
├── TestForumServer.Infrastructure.Services.Stores.Contents
│   ├── CommentStore
│   ├── ContentStoreBase
│   ├── IStore
│   ├── PostStore
│   └── TradStore
├── TestForumServer.Infrastructure.StaticData
│   └── Init
├── TestForumServer.WebInfrastructure.FileManagement.Images
│   ├── IImageManager
│   ├── ImageManager
│   └── ImageUploadResult
├── TestForumServer.WebInfrastructure.Middlewares
│   ├── BanMiddleware
│   └── ErrorHandlingMiddleware
└── TestForumServer.WebInfrastructure.Security
    ├── CustomTokenOptions
    ├── ITokenService
    ├── JwtSettings
    └── TokenService

```


## Установка и запуск
1. Убедитесь, что у вас установлены [Docker](https://www.docker.com/get-started) и [Docker Compose](https://docs.docker.com/compose/).
2. Клонируйте репозиторий:
   ```bash
   git clone https://github.com/German322VDK/TestForumServer.git
   cd TestForumServer
3. Соберите и запустите проект с помощью Docker Compose:
   ```bash
   docker-compose up --build

## Тестирование
Проект включает в себя модульные тесты для CommentStore, PostStore и TradStore. Чтобы запустить тесты, используйте GitHub Actions или выполните тесты локально с помощью следующей команды:
 ```bash
  dotnet test
 ```
[![Testing](https://github.com/German322VDK/TestForumServer/actions/workflows/Testing.yml/badge.svg)](https://github.com/German322VDK/TestForumServer/actions/workflows/Testing.yml)

Документация
Чтобы ознакомиться с документацией, перейдите по ссылке.

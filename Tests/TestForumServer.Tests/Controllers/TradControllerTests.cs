using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Newtonsoft.Json;
using TestForumServer.Controllers;
using TestForumServer.Domian.Entities.ForumEntities.Contents;
using TestForumServer.Domian.Entities.Identity;
using TestForumServer.Infrastructure.Services.Stores.Contents;
using TestForumServer.WebInfrastructure.FileManagement.Images;
using TestForumServer.Domian.ViewModels.ForumViewModels.Contents.Base;
using TestForumServer.Infrastructure.Mapping;

namespace TestForumServer.Tests.Controllers
{
    public class TradControllerTests
    {
        private readonly Mock<IStore<TradEntity>> _mockTradStore;
        private readonly Mock<UserManager<UserEntity>> _mockUserManager;
        private readonly Mock<IWebHostEnvironment> _mockEnv;
        private readonly Mock<IImageManager> _mockImageManager;
        private readonly Mock<ILogger<TradController>> _mockLogger;
        private readonly TradController _controller;
        private readonly List<UserEntity> _users;
        private readonly UserEntity _user;
        private readonly Mock<HttpContext> _mockHttpContext;

        public TradControllerTests()
        {
            _user = new UserEntity
            {
                UserName = "testuser",
                Id = 1
            };

            _users = new List<UserEntity> { _user };

            _mockTradStore = new Mock<IStore<TradEntity>>();
            _mockUserManager = MockUserManager(_users);
            _mockEnv = new Mock<IWebHostEnvironment>();
            _mockImageManager = new Mock<IImageManager>();
            _mockLogger = new Mock<ILogger<TradController>>();
            _mockHttpContext = new Mock<HttpContext>();


            // Настройка HttpContext и User
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Name, _user.UserName),
            new Claim(ClaimTypes.NameIdentifier, _user.Id.ToString())
            }, "mock"));

            _mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);

            // Привязываем HttpContext к контроллеру
            _controller = new TradController(
                _mockTradStore.Object,
                _mockUserManager.Object,
                _mockEnv.Object,
                _mockImageManager.Object,
                _mockLogger.Object
            )
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object
                }
            };

            _mockEnv.Setup(m => m.WebRootPath).Returns("wwwroot");
        }

        public static Mock<UserManager<UserEntity>> MockUserManager(List<UserEntity> users)
        {
            var store = new Mock<IUserStore<UserEntity>>();
            var mockUserManager = new Mock<UserManager<UserEntity>>(store.Object, null, null, null, null, null, null, null, null);

            mockUserManager.Object.UserValidators.Add(new UserValidator<UserEntity>());
            mockUserManager.Object.PasswordValidators.Add(new PasswordValidator<UserEntity>());

            mockUserManager.Setup(x => x.DeleteAsync(It.IsAny<UserEntity>())).ReturnsAsync(IdentityResult.Success);
            mockUserManager.Setup(x => x.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<UserEntity, string>((x, y) => users.Add(x));
            mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<UserEntity>())).ReturnsAsync(IdentityResult.Success);

            // Настройка FindByNameAsync для работы с UserEntity
            mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((string userName) => users.SingleOrDefault(u => u.UserName == userName));

            return mockUserManager;
        }

        #region Get

        [Fact]
        public async Task GetAllShort_ShouldReturnOk_WithTrads()
        {
            // Arrange
            var trads = new List<TradEntity>
            {
                new TradEntity { Id = 1, Content = "Test content 1" },
                new TradEntity { Id = 2, Content = "Test content 2" }
            };

            // Настройка метода GetAll для возвращения trads
            _mockTradStore.Setup(m => m.GetAll()).Returns(trads.AsQueryable());
            // Act
            var result = await _controller.GetAllShort();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<IEnumerable<TradShortViewModel>>(okResult.Value);

            // Сериализация результата в JSON
            var jsonResult = JsonConvert.SerializeObject(returnValue);

            // Десериализация JSON обратно в объекты
            var deserializedResult = JsonConvert.DeserializeObject<List<TradShortViewModel>>(jsonResult);

            // Проверяем, что возвращаемые объекты содержат правильные данные
            Assert.Equal(2, deserializedResult.Count);
            Assert.Equal(1, deserializedResult[0].Id);
            Assert.Equal("Test content 1", deserializedResult[0].Content);
            Assert.Equal(2, deserializedResult[1].Id);
            Assert.Equal("Test content 2", deserializedResult[1].Content);
        }

        [Fact]
        public async Task GetAllShort_ShouldReturnNotFound_WhenUserNotFound()
        {
            // Arrange
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((UserEntity)null); // Пользователь не найден

            // Act
            var result = await _controller.GetAllShort();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var value = JsonConvert.DeserializeObject<Dictionary<string, string>>((string)notFoundResult.Value);
            Assert.Equal("Пользователь не найден.", value["message"]);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WithTrads()
        {
            // Arrange
            var trads = new List<TradEntity> { new TradEntity { Id = 1, Content = "Test content" } };
            _mockTradStore.Setup(m => m.GetAll()).Returns(trads.AsQueryable());

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetAll_ShouldReturnNotFound_WhenUserNotFound()
        {
            // Arrange
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((UserEntity)null);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            var json = JsonConvert.SerializeObject(notFoundResult.Value); // Сериализуем в JSON
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json); // Десериализуем в словарь

            string message = dict["message"]; 
            Assert.Equal("Пользователь не найден.", message);
        }

        [Fact]
        public void GetAllRef_ShouldReturnOk_WithTrads()
        {
            // Arrange
            var trads = new List<TradEntity>
            {
                new TradEntity { Id = 1, Content = "Test content 1" },
                new TradEntity { Id = 2, Content = "Test content 2" }
            };

            _mockTradStore.Setup(m => m.GetAll()).Returns(trads.AsQueryable());

            // Act
            var result = _controller.GetAllRef();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);

            var jsonResult = JsonConvert.SerializeObject(returnValue);

            // Преобразование сериализованного JSON обратно в объекты
            var deserializedResult = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonResult);

            // Проверяем, что возвращаемые объекты содержат правильные данные
            Assert.Equal(2, deserializedResult.Count);
            Assert.Equal(1, Convert.ToInt32(deserializedResult[0]["Id"]));
            Assert.Equal("Test content 1", Convert.ToString(deserializedResult[0]["Title"]));
            Assert.Equal(2, Convert.ToInt32(deserializedResult[1]["Id"]));
            Assert.Equal("Test content 2", Convert.ToString(deserializedResult[1]["Title"]));
        }
    }

    #endregion

}

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestForumServer.Database.Context;
using TestForumServer.Infrastructure.Services.Stores.Contents;
using TestForumServer.Domian.Entities.Identity;
using TestForumServer.Domian.Entities.ForumEntities.Contents;
using TestForumServer.Domian.Entities.ForumEntities.Likes;
using Moq;
using Microsoft.Data.Sqlite;

namespace TestFroumServer.Infrastructure.Tests.Services.Stores.Contents
{
    public class TradStoreTests
    {
        private readonly DbContextOptions<TestForumDbContext> _options;
        private readonly Mock<ILogger<TradStore>> _mockLogger;

        private UserEntity _user;
        private UserEntity _user2;

        public TradStoreTests()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            _options = new DbContextOptionsBuilder<TestForumDbContext>()
                .UseSqlite(connection)
                .UseLazyLoadingProxies()
                .Options;

            _mockLogger = new Mock<ILogger<TradStore>>();

            Init();
        }

        public async void Init()
        {
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            var userStore = new UserStore<UserEntity, RoleEntity, TestForumDbContext, int>(context);
            var userManager = new UserManager<UserEntity>(
                userStore,
                null,
                new PasswordHasher<UserEntity>(),
                null,
                null,
                null,
                null,
                null,
                null
            );


            _user = new UserEntity
            {
                NickName = "Главный",
                UserName = "InitData.MAIN_USER_NAME",
                ProfilePicturePath = "InitData.MAIN_USER_PICTURE",
            };

            _user2 = new UserEntity
            {
                NickName = "2222222",
                UserName = "InitData.MAIN_USER_NAME2222222222222222",
                ProfilePicturePath = "InitData.MAIN_USER_PICTURE",
            };

            var result = await userManager.CreateAsync(_user, "InitData.MAIN_USER_PASS");
            var result2 = await userManager.CreateAsync(_user2, "InitData.MAIN_USER_PASS2");


        }

        #region Add

        [Theory]
        [InlineData("1", "14142")]
        [InlineData("", "")]
        public async Task AddTrad_ShouldAddTradToDatabase(string content, string image)
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            TradStore tradStore = new TradStore(context, _mockLogger.Object);
            TradEntity trad = new TradEntity
            {
                UserId = _user.Id,
                Content = content,
                ProfilePicturePath = image
            };
            // Act
            TradEntity addedTrad = await tradStore.AddAsync(trad);
            // Assert
            Assert.NotNull(addedTrad);
            TradEntity? storedTrad = context.Trads.FirstOrDefault(c => c.Id == addedTrad.Id);
            Assert.NotNull(storedTrad);
            Assert.Equal(addedTrad.Id, storedTrad.Id);
            Assert.Equal(addedTrad.Content, storedTrad.Content);
            Assert.Equal(addedTrad.ProfilePicturePath, storedTrad.ProfilePicturePath);
        }

        [Fact]
        public async Task AddNullTrad_ShouldThrowException()
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            TradStore tradStore = new TradStore(context, _mockLogger.Object);
            TradEntity nullTrad = null;
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await tradStore.AddAsync(nullTrad));
            // Assert
            Assert.Equal("Параметр item=null", exception.Message);
        }

        [Fact]
        public async Task AddNullUserTrad_ShouldThrowException()
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            TradStore tradStore = new TradStore(context, _mockLogger.Object);
            TradEntity nullTrad = new TradEntity { User = null };
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await tradStore.AddAsync(nullTrad));
            // Assert
            Assert.Equal("user == null", exception.Message);
        }


        #endregion


        #region Delete

        [Theory]
        [InlineData("1", "14142")]
        public async Task DeletePost_ShouldRemovePostFromDatabase(string content, string image)
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            TradStore tradStore = new TradStore(context, _mockLogger.Object);
            TradEntity trad = new TradEntity
            {
                UserId = _user.Id,
                Content = content,
                ProfilePicturePath = image
            };
            TradEntity addedTrad = await tradStore.AddAsync(trad);
            // Act

            bool result = await tradStore.DeleteAsync(addedTrad.Id);
            context.SaveChanges();
            // Assert
            Assert.True(result);
            TradEntity? deletedTrad = context.Trads.FirstOrDefault(c => c.Id == addedTrad.Id);
            Assert.Null(deletedTrad);
        }

        [Fact]
        public async Task DeleteIsNotExsistingPost_ShouldNotRemovePostFromDatabase()
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            TradStore tradStore = new TradStore(context, _mockLogger.Object);
            // Act

            bool result = await tradStore.DeleteAsync(2);
            context.SaveChanges();

            Assert.False(result);
        }

        #endregion


        #region Update

        [Theory]
        [InlineData("1", "14142", "2", "21241")]
        public async Task UpdateTrad_ShouldUpdateTradFromDatabase(string oldContent, string oldImage,
            string newContent, string newImage)
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            TradStore tradStore = new TradStore(context, _mockLogger.Object);
            TradEntity trad = new TradEntity
            {
                UserId = _user.Id,
                Content = oldContent,
                ProfilePicturePath = oldImage
            };
            TradEntity addedTrad = await tradStore.AddAsync(trad);
            addedTrad.Content = newContent;
            addedTrad.ProfilePicturePath = newImage;
            // Act
            TradEntity? updatedTrad = await tradStore.UpdateAsync(addedTrad);
            // Assert
            Assert.NotNull(updatedTrad);
            Assert.Equal(updatedTrad.Content, newContent);
            Assert.Equal(updatedTrad.ProfilePicturePath, newImage);
        }

        [Fact]
        public async Task UpdateNullTrad_ShouldThrowException()
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            TradStore tradStore = new TradStore(context, _mockLogger.Object);
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await tradStore.UpdateAsync(null));
            // Assert
            Assert.Equal("Параметр item=null", exception.Message);
        }

        [Fact]
        public async Task UpdateNullUserTrad_ShouldThrowException()
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            TradStore tradStore = new TradStore(context, _mockLogger.Object);
            TradEntity trad = new TradEntity
            {
                UserId = _user.Id,
                Content = "oldContent",
                ProfilePicturePath = "oldImage"
            };
            TradEntity addedTrad = await tradStore.AddAsync(trad);
            addedTrad.UserId = 0;
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await tradStore.UpdateAsync(addedTrad));
            // Assert
            Assert.Equal("user == null", exception.Message);
        }

        [Fact]
        public async Task UpdateUserTrad_ShouldThrowException()
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            TradStore tradStore = new TradStore(context, _mockLogger.Object);
            TradEntity trad = new TradEntity
            {
                UserId = _user.Id,
                Content = "oldContent",
                ProfilePicturePath = "oldImage"
            };
            TradEntity addedTrad = await tradStore.AddAsync(trad);
            addedTrad.UserId = _user2.Id;
            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await tradStore.UpdateAsync(addedTrad));
            // Assert
            Assert.Equal("Нельзя менять User", exception.Message);
        }


        [Theory]
        [InlineData("1", "14142", "2")]
        public async Task UpdateContentTrad_ShouldUpdateTradFromDatabase(string content, string image, string newContent)
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            TradStore tradStore = new TradStore(context, _mockLogger.Object);
            TradEntity trad = new TradEntity
            {
                UserId = _user.Id,
                Content = content,
                ProfilePicturePath = image
            };
            TradEntity addedTrad = await tradStore.AddAsync(trad);
            // Act
            TradEntity? updatedTrad = await tradStore.UpdateContentAsync(addedTrad.Id, newContent);
            // Assert
            Assert.NotNull(updatedTrad);
            Assert.Equal(updatedTrad.Content, newContent);
        }

        [Theory]
        [InlineData("1", "14142", "214142")]
        public async Task UpdateImageTrad_ShouldUpdateTradFromDatabase(string content, string image, string newImage)
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            TradStore tradStore = new TradStore(context, _mockLogger.Object);
            TradEntity trad = new TradEntity
            {
                UserId = _user.Id,
                Content = content,
                ProfilePicturePath = image
            };
            TradEntity addedTrad = await tradStore.AddAsync(trad);
            // Act
            TradEntity? updatedTrad = await tradStore.UpdateImageAsync(addedTrad.Id, newImage);
            // Assert
            Assert.NotNull(updatedTrad);
            Assert.Equal(updatedTrad.ProfilePicturePath, newImage);
        }

        #endregion


        #region Like

        [Theory]
        [InlineData("1", "14142")]
        public async Task LikeTrad_ShouldLike(string content, string image)
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            TradStore tradStore = new TradStore(context, _mockLogger.Object);
            TradEntity trad = new TradEntity
            {
                UserId = _user.Id,
                Content = content,
                ProfilePicturePath = image
            };
            TradEntity addedTrad = await tradStore.AddAsync(trad);
            // Act
            bool result = await tradStore.LikeContentAsync(addedTrad.Id, _user.Id);
            // Assert
            TradLikeEntity? storedLike = context.TradLikes
                .FirstOrDefault(el => el.TradId == addedTrad.Id && el.UserId == _user.Id);
            Assert.True(result);
            Assert.NotNull(storedLike);
        }

        [Theory]
        [InlineData("1", "14142")]
        public async Task LikeTrad_ShouldNotLike(string content, string image)
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            TradStore tradStore = new TradStore(context, _mockLogger.Object);
            TradEntity trad = new TradEntity
            {
                UserId = _user.Id,
                Content = content,
                ProfilePicturePath = image
            };
            TradEntity addedTrad = await tradStore.AddAsync(trad);
            // Act
            await tradStore.LikeContentAsync(addedTrad.Id, _user.Id);
            bool result = await tradStore.LikeContentAsync(addedTrad.Id, _user.Id);
            // Assert
            TradLikeEntity? storedLike = context.TradLikes
                .FirstOrDefault(el => el.TradId == addedTrad.Id && el.UserId == _user.Id);
            Assert.False(result);
            Assert.NotNull(storedLike);
        }

        [Theory]
        [InlineData("1", "14142")]
        public async Task UnLikeTrad_ShouldUnLike(string content, string image)
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            TradStore tradStore = new TradStore(context, _mockLogger.Object);
            TradEntity trad = new TradEntity
            {
                UserId = _user.Id,
                Content = content,
                ProfilePicturePath = image
            };
            TradEntity addedTrad = await tradStore.AddAsync(trad);
            // Act
            bool likedResult = await tradStore.LikeContentAsync(addedTrad.Id, _user.Id);
            bool unLikedResult = await tradStore.UnLikeContentAsync(addedTrad.Id, _user.Id);
            // Assert
            TradLikeEntity? storedLike = context.TradLikes
                .FirstOrDefault(el => el.TradId == addedTrad.Id && el.UserId == _user.Id);
            Assert.True(likedResult);
            Assert.True(unLikedResult);
            Assert.Null(storedLike);
        }

        [Fact]
        public async Task UnLikeTrad_ShouldNotUnLike()
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            TradStore tradStore = new TradStore(context, _mockLogger.Object);
            // Act
            bool result = await tradStore.UnLikeContentAsync(2, _user.Id);
            // Assert
            TradLikeEntity? storedLike = context.TradLikes
                .FirstOrDefault(el => el.TradId == 2 && el.UserId == _user.Id);
            Assert.False(result);
            Assert.Null(storedLike);
        }

        #endregion


    }
}

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TestForumServer.Database.Context;
using TestForumServer.Infrastructure.Services.Stores.Contents;
using TestForumServer.Domian.Entities.ForumEntities.Contents;
using TestForumServer.Domian.Entities.ForumEntities.Likes;
using TestForumServer.Domian.Entities.Identity;

namespace TestFroumServer.Infrastructure.Tests.Services.Stores.Contents
{
    public class PostStoreTests
    {
        private readonly DbContextOptions<TestForumDbContext> _options;
        private readonly Mock<ILogger<PostStore>> _mockLogger;

        private UserEntity _user;
        private UserEntity _user2;
        private TradEntity _trad;

        public PostStoreTests()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            _options = new DbContextOptionsBuilder<TestForumDbContext>()
                .UseSqlite(connection)
                .UseLazyLoadingProxies()
                .Options;

            _mockLogger = new Mock<ILogger<PostStore>>();

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

            TradEntity trad = new TradEntity
            {
                Content = "InitData.MAIN_TRAD_CONTENT",
                UserId = _user.Id,
                ProfilePicturePath = "InitData.MAIN_TRAD_PICTURE",
            };
            var tResult = await context.Trads.AddAsync(trad);
            await context.SaveChangesAsync();
            _trad = tResult.Entity;

        }

        #region Add

        [Theory]
        [InlineData("1", "14142")]
        [InlineData("", "")]
        public async Task AddPost_ShouldAddPostToDatabase(string content, string image)
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            PostStore postStore = new PostStore(context, _mockLogger.Object);
            PostEntity post = new PostEntity
            {
                UserId = _user.Id,
                TradId = _trad.Id,
                Content = content,
                ProfilePicturePath = image
            };
            // Act
            PostEntity addedPost = await postStore.AddAsync(post);
            // Assert
            Assert.NotNull(addedPost);
            PostEntity? storedPost = context.Posts.FirstOrDefault(c => c.Id == addedPost.Id);
            Assert.NotNull(storedPost);
            Assert.Equal(addedPost.Id, storedPost.Id);
            Assert.Equal(addedPost.Content, storedPost.Content);
            Assert.Equal(addedPost.ProfilePicturePath, storedPost.ProfilePicturePath);
        }

        [Fact]
        public async Task AddNullPost_ShouldThrowException()
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            PostStore postStore = new PostStore(context, _mockLogger.Object);
            PostEntity nullPost = null;
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await postStore.AddAsync(nullPost));
            // Assert
            Assert.Equal("Параметр item=null", exception.Message);
        }

        [Fact]
        public async Task AddNullUserPost_ShouldThrowException()
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            PostStore postStore = new PostStore(context, _mockLogger.Object);
            PostEntity nullPost = new PostEntity { User = null };
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await postStore.AddAsync(nullPost));
            // Assert
            Assert.Equal("user == null", exception.Message);
        }

        [Fact]
        public async Task AddNullTradPost_ShouldThrowException()
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            PostStore postStore = new PostStore(context, _mockLogger.Object);
            PostEntity nullPost = new PostEntity { UserId = _user.Id };
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await postStore.AddAsync(nullPost));
            // Assert
            Assert.Equal("Параметр trad=null", exception.Message);
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
            PostStore postStore = new PostStore(context, _mockLogger.Object);
            PostEntity post = new PostEntity
            {
                UserId = _user.Id,
                TradId = _trad.Id,
                Content = content,
                ProfilePicturePath = image
            };
            PostEntity addedPost = await postStore.AddAsync(post);
            // Act

            bool result = await postStore.DeleteAsync(addedPost.Id);
            context.SaveChanges();
            // Assert
            Assert.True(result);
            PostEntity? deletedPost = context.Posts.FirstOrDefault(c => c.Id == addedPost.Id);
            Assert.Null(deletedPost);
        }

        [Fact]
        public async Task DeleteIsNotExsistingPost_ShouldNotRemovePostFromDatabase()
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            PostStore postStore = new PostStore(context, _mockLogger.Object);
            // Act

            bool result = await postStore.DeleteAsync(2);
            context.SaveChanges();

            Assert.False(result);
        }

        #endregion


        #region Update

        [Theory]
        [InlineData("1", "14142", "2", "21241")]
        public async Task UpdatePost_ShouldUpdatePostFromDatabase(string oldContent, string oldImage,
            string newContent, string newImage)
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            PostStore postStore = new PostStore(context, _mockLogger.Object);
            PostEntity post = new PostEntity
            {
                UserId = _user.Id,
                TradId = _trad.Id,
                Content = oldContent,
                ProfilePicturePath = oldImage
            };
            PostEntity addedPost = await postStore.AddAsync(post);
            addedPost.Content = newContent;
            addedPost.ProfilePicturePath = newImage;
            // Act
            PostEntity? updatedPost = await postStore.UpdateAsync(addedPost);
            // Assert
            Assert.NotNull(updatedPost);
            Assert.Equal(updatedPost.Content, newContent);
            Assert.Equal(updatedPost.ProfilePicturePath, newImage);
        }

        [Fact]
        public async Task UpdateNullPost_ShouldThrowException()
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            PostStore postStore = new PostStore(context, _mockLogger.Object);
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await postStore.UpdateAsync(null));
            // Assert
            Assert.Equal("Параметр item=null", exception.Message);
        }

        [Fact]
        public async Task UpdateNullUserPost_ShouldThrowException()
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            PostStore postStore = new PostStore(context, _mockLogger.Object);
            PostEntity post = new PostEntity
            {
                UserId = _user.Id,
                TradId = _trad.Id,
                Content = "oldContent",
                ProfilePicturePath = "oldImage"
            };
            PostEntity addedPost = await postStore.AddAsync(post);
            addedPost.UserId = 0;
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await postStore.UpdateAsync(addedPost));
            // Assert
            Assert.Equal("user == null", exception.Message);
        }

        [Fact]
        public async Task UpdateUserPost_ShouldThrowException()
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            PostStore postStore = new PostStore(context, _mockLogger.Object);
            PostEntity post = new PostEntity
            {
                UserId = _user.Id,
                TradId = _trad.Id,
                Content = "oldContent",
                ProfilePicturePath = "oldImage"
            };
            PostEntity addedPost = await postStore.AddAsync(post);
            addedPost.UserId = _user2.Id;
            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await postStore.UpdateAsync(addedPost));
            // Assert
            Assert.Equal("Нельзя менять User", exception.Message);
        }

        [Fact]
        public async Task UpdateNullTradPost_ShouldThrowException()
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            PostStore postStore = new PostStore(context, _mockLogger.Object);
            PostEntity post = new PostEntity
            {
                UserId = _user.Id,
                TradId = _trad.Id,
                Content = "oldContent",
                ProfilePicturePath = "oldImage"
            };
            PostEntity addedPost = await postStore.AddAsync(post);
            addedPost.TradId = 0;
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await postStore.UpdateAsync(addedPost));
            // Assert
            Assert.Equal("Параметр trad=null", exception.Message);
        }

        [Theory]
        [InlineData("1", "14142", "2")]
        public async Task UpdateContentPost_ShouldUpdatePostFromDatabase(string content, string image, string newContent)
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            PostStore postStore = new PostStore(context, _mockLogger.Object);
            PostEntity post = new PostEntity
            {
                UserId = _user.Id,
                TradId = _trad.Id,
                Content = content,
                ProfilePicturePath = image
            };
            PostEntity addedPost = await postStore.AddAsync(post);
            // Act
            PostEntity? updatedPost = await postStore.UpdateContentAsync(addedPost.Id, newContent);
            // Assert
            Assert.NotNull(updatedPost);
            Assert.Equal(updatedPost.Content, newContent);
        }

        [Theory]
        [InlineData("1", "14142", "214142")]
        public async Task UpdateImagePost_ShouldUpdatePostFromDatabase(string content, string image, string newImage)
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            PostStore postStore = new PostStore(context, _mockLogger.Object);
            PostEntity post = new PostEntity
            {
                UserId = _user.Id,
                TradId = _trad.Id,
                Content = content,
                ProfilePicturePath = image
            };
            PostEntity addedPost = await postStore.AddAsync(post);
            // Act
            PostEntity? updatedPost = await postStore.UpdateImageAsync(addedPost.Id, newImage);
            // Assert
            Assert.NotNull(updatedPost);
            Assert.Equal(updatedPost.ProfilePicturePath, newImage);
        }

        #endregion


        #region Like

        [Theory]
        [InlineData("1", "14142")]
        public async Task LikePost_ShouldLike(string content, string image)
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            PostStore postStore = new PostStore(context, _mockLogger.Object);
            PostEntity post = new PostEntity
            {
                UserId = _user.Id,
                TradId = _trad.Id,
                Content = content,
                ProfilePicturePath = image
            };
            PostEntity addedPost = await postStore.AddAsync(post);
            // Act
            bool result = await postStore.LikeContentAsync(addedPost.Id, _user.Id);
            // Assert
            PostLikeEntity? storedLike = context.PostLikes
                .FirstOrDefault(el => el.PostId == addedPost.Id && el.UserId == _user.Id);
            Assert.True(result);
            Assert.NotNull(storedLike);
        }

        [Theory]
        [InlineData("1", "14142")]
        public async Task LikePost_ShouldNotLike(string content, string image)
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            PostStore postStore = new PostStore(context, _mockLogger.Object);
            PostEntity post = new PostEntity
            {
                UserId = _user.Id,
                TradId = _trad.Id,
                Content = content,
                ProfilePicturePath = image
            };
            PostEntity addedPost = await postStore.AddAsync(post);
            // Act
            await postStore.LikeContentAsync(addedPost.Id, _user.Id);
            bool result = await postStore.LikeContentAsync(addedPost.Id, _user.Id);
            // Assert
            PostLikeEntity? storedLike = context.PostLikes
                .FirstOrDefault(el => el.PostId == addedPost.Id && el.UserId == _user.Id);
            Assert.False(result);
            Assert.NotNull(storedLike);
        }

        [Theory]
        [InlineData("1", "14142")]
        public async Task UnLikePost_ShouldUnLike(string content, string image)
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            PostStore postStore = new PostStore(context, _mockLogger.Object);
            PostEntity post = new PostEntity
            {
                UserId = _user.Id,
                TradId = _trad.Id,
                Content = content,
                ProfilePicturePath = image
            };
            PostEntity addedPost = await postStore.AddAsync(post);
            // Act
            bool likedResult = await postStore.LikeContentAsync(addedPost.Id, _user.Id);
            bool unLikedResult = await postStore.UnLikeContentAsync(addedPost.Id, _user.Id);
            // Assert
            PostLikeEntity? storedLike = context.PostLikes
                .FirstOrDefault(el => el.PostId == addedPost.Id && el.UserId == _user.Id);
            Assert.True(likedResult);
            Assert.True(unLikedResult);
            Assert.Null(storedLike);
        }

        [Fact]
        public async Task UnLikePost_ShouldNotUnLike()
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            PostStore postStore = new PostStore(context, _mockLogger.Object);
            // Act
            bool result = await postStore.UnLikeContentAsync(2, _user.Id);
            // Assert
            PostLikeEntity? storedLike = context.PostLikes
                .FirstOrDefault(el => el.PostId == 2 && el.UserId == _user.Id);
            Assert.False(result);
            Assert.Null(storedLike);
        }

        #endregion

    }
}

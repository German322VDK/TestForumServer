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
    public class CommentStoreTests
    {
        private readonly DbContextOptions<TestForumDbContext> _options;
        private readonly Mock<ILogger<CommentStore>> _mockLogger;

        private UserEntity _user;
        private UserEntity _user2;
        private TradEntity _trad;
        private PostEntity _post;

        public CommentStoreTests()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            _options = new DbContextOptionsBuilder<TestForumDbContext>()
                .UseSqlite(connection)
                .UseLazyLoadingProxies()
                .Options;

            _mockLogger = new Mock<ILogger<CommentStore>>();

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

            PostEntity post = new PostEntity
            {
                Content = "InitData.MAIN_POST_CONTENT",
                UserId = _user.Id,
                TradId = trad.Id,
                ProfilePicturePath = "InitData.MAIN_POST_PICTURE",
            };
            var pResult = await context.Posts.AddAsync(post);
            await context.SaveChangesAsync();
            _post = pResult.Entity;
        }

        #region Add

        [Theory]
        [InlineData("1", "14142")]
        [InlineData("", "")]
        public async Task AddComment_ShouldAddCommentToDatabase(string content, string image)
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            CommentStore commentStore = new CommentStore(context, _mockLogger.Object);
            CommentEntity nullComment = new CommentEntity
            {
                UserId = _user.Id,
                PostId = _post.Id,
                Content = content,
                ProfilePicturePath = image
            };
            // Act
            CommentEntity addedComment = await commentStore.AddAsync(nullComment);
            // Assert
            Assert.NotNull(addedComment);
            CommentEntity storedComment = context.Comments.FirstOrDefault(c => c.Id == addedComment.Id);
            Assert.NotNull(storedComment);
            Assert.Equal(addedComment.Id, storedComment.Id);
            Assert.Equal(addedComment.Content, storedComment.Content);
            Assert.Equal(addedComment.ProfilePicturePath, storedComment.ProfilePicturePath);
        }

        [Fact]
        public async Task AddNullComment_ShouldThrowException()
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            CommentStore commentStore = new CommentStore(context, _mockLogger.Object);
            CommentEntity nullComment = null;
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await commentStore.AddAsync(nullComment));
            // Assert
            Assert.Equal("Параметр item=null", exception.Message);
        }

        [Fact]
        public async Task AddNullUserComment_ShouldThrowException()
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            CommentStore commentStore = new CommentStore(context, _mockLogger.Object);
            CommentEntity nullComment = new CommentEntity { User = null };
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await commentStore.AddAsync(nullComment));
            // Assert
            Assert.Equal("user == null", exception.Message);
        }

        [Fact]
        public async Task AddNullPostComment_ShouldThrowException()
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            CommentStore commentStore = new CommentStore(context, _mockLogger.Object);
            CommentEntity nullComment = new CommentEntity { UserId = _user.Id };
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await commentStore.AddAsync(nullComment));
            // Assert
            Assert.Equal("Параметр post=null", exception.Message);
        }

        #endregion


        #region Delete

        [Theory]
        [InlineData("1", "14142")]
        public async Task DeleteComment_ShouldRemoveCommentFromDatabase(string content, string image)
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            CommentStore commentStore = new CommentStore(context, _mockLogger.Object);
            CommentEntity nullComment = new CommentEntity
            {
                UserId = _user.Id,
                PostId = _post.Id,
                Content = content,
                ProfilePicturePath = image
            };
            CommentEntity addedComment = await commentStore.AddAsync(nullComment);
            // Act

            bool result = await commentStore.DeleteAsync(addedComment.Id);
            context.SaveChanges();
            // Assert
            Assert.True(result);
            var deletedComment = context.Comments.FirstOrDefault(c => c.Id == addedComment.Id);
            Assert.Null(deletedComment);
        }

        [Fact]
        public async Task DeleteIsNotExsistingComment_ShouldNotRemoveCommentFromDatabase()
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            CommentStore commentStore = new CommentStore(context, _mockLogger.Object);
            // Act

            bool result = await commentStore.DeleteAsync(2);
            context.SaveChanges();

            Assert.False(result);
        }

        #endregion


        #region Update

        [Theory]
        [InlineData("1", "14142", "2", "21241")]
        public async Task UpdateComment_ShouldUpdateCommentFromDatabase(string oldContent, string oldImage,
            string newContent, string newImage)
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            CommentStore commentStore = new CommentStore(context, _mockLogger.Object);
            CommentEntity comment = new CommentEntity
            {
                UserId = _user.Id,
                PostId = _post.Id,
                Content = oldContent,
                ProfilePicturePath = oldImage
            };
            CommentEntity addedComment = await commentStore.AddAsync(comment);
            addedComment.Content = newContent;
            addedComment.ProfilePicturePath = newImage;
            // Act
            CommentEntity? updatedComment = await commentStore.UpdateAsync(addedComment);
            // Assert
            Assert.NotNull(updatedComment);
            Assert.Equal(updatedComment.Content, newContent);
            Assert.Equal(updatedComment.ProfilePicturePath, newImage);
        }

        [Fact]
        public async Task UpdateNullComment_ShouldThrowException()
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            CommentStore commentStore = new CommentStore(context, _mockLogger.Object);
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await commentStore.UpdateAsync(null));
            // Assert
            Assert.Equal("Параметр item=null", exception.Message);
        }

        [Fact]
        public async Task UpdateNullUserComment_ShouldThrowException()
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            CommentStore commentStore = new CommentStore(context, _mockLogger.Object);
            CommentEntity comment = new CommentEntity
            {
                UserId = _user.Id,
                PostId = _post.Id,
                Content = "oldContent",
                ProfilePicturePath = "oldImage"
            };
            CommentEntity addedComment = await commentStore.AddAsync(comment);
            addedComment.UserId = 0;
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await commentStore.UpdateAsync(addedComment));
            // Assert
            Assert.Equal("user == null", exception.Message);
        }

        [Fact]
        public async Task UpdateUserComment_ShouldThrowException()
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            CommentStore commentStore = new CommentStore(context, _mockLogger.Object);
            CommentEntity comment = new CommentEntity
            {
                UserId = _user.Id,
                PostId = _post.Id,
                Content = "oldContent",
                ProfilePicturePath = "oldImage"
            };
            CommentEntity addedComment = await commentStore.AddAsync(comment);
            addedComment.UserId = _user2.Id;
            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await commentStore.UpdateAsync(addedComment));
            // Assert
            Assert.Equal("Нельзя менять User", exception.Message);
        }

        [Fact]
        public async Task UpdateNullPostComment_ShouldThrowException()
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            CommentStore commentStore = new CommentStore(context, _mockLogger.Object);
            CommentEntity comment = new CommentEntity
            {
                UserId = _user.Id,
                PostId = _post.Id,
                Content = "oldContent",
                ProfilePicturePath = "oldImage"
            };
            CommentEntity addedComment = await commentStore.AddAsync(comment);
            addedComment.PostId = 0;
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await commentStore.UpdateAsync(addedComment));
            // Assert
            Assert.Equal("Параметр post=null", exception.Message);
        }

        [Theory]
        [InlineData("1", "14142", "2")]
        public async Task UpdateContentComment_ShouldUpdateCommentFromDatabase(string content, string image, string newContent)
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            CommentStore commentStore = new CommentStore(context, _mockLogger.Object);
            CommentEntity comment = new CommentEntity
            {
                UserId = _user.Id,
                PostId = _post.Id,
                Content = content,
                ProfilePicturePath = image
            };
            CommentEntity addedComment = await commentStore.AddAsync(comment);
            // Act
            CommentEntity? updatetComment = await commentStore.UpdateContentAsync(addedComment.Id, newContent);
            // Assert
            Assert.NotNull(updatetComment);
            Assert.Equal(updatetComment.Content, newContent);
        }

        [Theory]
        [InlineData("1", "14142", "214142")]
        public async Task UpdateImageComment_ShouldUpdateCommentFromDatabase(string content, string image, string newImage)
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            CommentStore commentStore = new CommentStore(context, _mockLogger.Object);
            CommentEntity comment = new CommentEntity
            {
                UserId = _user.Id,
                PostId = _post.Id,
                Content = content,
                ProfilePicturePath = image
            };
            CommentEntity addedComment = await commentStore.AddAsync(comment);
            // Act
            CommentEntity? updatetComment = await commentStore.UpdateImageAsync(addedComment.Id, newImage);
            // Assert
            Assert.NotNull(updatetComment);
            Assert.Equal(updatetComment.ProfilePicturePath, newImage);
        }

        #endregion

        #region Like

        [Theory]
        [InlineData("1", "14142")]
        public async Task LikeComment_ShouldLike(string content, string image)
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            CommentStore commentStore = new CommentStore(context, _mockLogger.Object);
            CommentEntity nullComment = new CommentEntity
            {
                UserId = _user.Id,
                PostId = _post.Id,
                Content = content,
                ProfilePicturePath = image
            };
            CommentEntity addedComment = await commentStore.AddAsync(nullComment);
            // Act
            bool result = await commentStore.LikeContentAsync(addedComment.Id, _user.Id);
            // Assert
            CommentLikeEntity? storedLike = context.CommentLikes
                .FirstOrDefault(el => el.CommentId == addedComment.Id && el.UserId == _user.Id);
            Assert.True(result);
            Assert.NotNull(storedLike);
        }

        [Theory]
        [InlineData("1", "14142")]
        public async Task LikeComment_ShouldNotLike(string content, string image)
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            CommentStore commentStore = new CommentStore(context, _mockLogger.Object);
            CommentEntity nullComment = new CommentEntity
            {
                UserId = _user.Id,
                PostId = _post.Id,
                Content = content,
                ProfilePicturePath = image
            };
            CommentEntity addedComment = await commentStore.AddAsync(nullComment);
            // Act
            await commentStore.LikeContentAsync(addedComment.Id, _user.Id);
            bool result = await commentStore.LikeContentAsync(addedComment.Id, _user.Id);
            // Assert
            CommentLikeEntity? storedLike = context.CommentLikes
                .FirstOrDefault(el => el.CommentId == addedComment.Id && el.UserId == _user.Id);
            Assert.False(result);
            Assert.NotNull(storedLike);
        }

        [Theory]
        [InlineData("1", "14142")]
        public async Task UnLikeComment_ShouldUnLike(string content, string image)
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            CommentStore commentStore = new CommentStore(context, _mockLogger.Object);
            CommentEntity nullComment = new CommentEntity
            {
                UserId = _user.Id,
                PostId = _post.Id,
                Content = content,
                ProfilePicturePath = image
            };
            CommentEntity addedComment = await commentStore.AddAsync(nullComment);
            // Act
            bool likedResult = await commentStore.LikeContentAsync(addedComment.Id, _user.Id);
            bool unLikedResult = await commentStore.UnLikeContentAsync(addedComment.Id, _user.Id);
            // Assert
            CommentLikeEntity? storedLike = context.CommentLikes
                .FirstOrDefault(el => el.CommentId == addedComment.Id && el.UserId == _user.Id);
            Assert.True(likedResult);
            Assert.True(unLikedResult);
            Assert.Null(storedLike);
        }

        [Fact]
        public async Task UnLikeComment_ShouldNotUnLike()
        {
            // Arrange
            using TestForumDbContext context = new TestForumDbContext(_options);
            context.Database.EnsureCreated();
            CommentStore commentStore = new CommentStore(context, _mockLogger.Object);
            // Act
            bool result = await commentStore.UnLikeContentAsync(2, _user.Id);
            // Assert
            CommentLikeEntity? storedLike = context.CommentLikes
                .FirstOrDefault(el => el.CommentId == 2 && el.UserId == _user.Id);
            Assert.False(result);
            Assert.Null(storedLike);
        }

        #endregion
    }
}

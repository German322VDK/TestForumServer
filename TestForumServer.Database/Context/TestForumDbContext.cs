using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TestForumServer.Domian.Entities.ForumEntities.Contents;
using TestForumServer.Domian.Entities.ForumEntities.Likes;
using TestForumServer.Domian.Entities.Identity;

namespace TestForumServer.Database.Context
{
    /// <summary>
    /// Database context for the forum, extending IdentityDbContext.
    /// Used to work with entities like users, roles, posts, comments, threads, likes, and their relationships.
    /// </summary>
    public class TestForumDbContext : IdentityDbContext<UserEntity, RoleEntity, int>
    {
        /// <summary>
        /// Constructor for initializing the database context with the provided options.
        /// </summary>
        /// <param name="options">The configuration options for the database context.</param>
        public TestForumDbContext(DbContextOptions<TestForumDbContext> options) : base(options) { }

        /// <summary>
        /// Data set for comment entities.
        /// </summary>
        public DbSet<CommentEntity> Comments { get; set; }

        /// <summary>
        /// Data set for post entities.
        /// </summary>
        public DbSet<PostEntity> Posts { get; set; }

        /// <summary>
        /// Data set for trad entities.
        /// </summary>
        public DbSet<TradEntity> Trads { get; set; }

        /// <summary>
        /// Data set for comment like entities.
        /// </summary>
        public DbSet<CommentLikeEntity> CommentLikes { get; set; }

        /// <summary>
        /// Data set for post like entities.
        /// </summary>
        public DbSet<PostLikeEntity> PostLikes { get; set; }

        /// <summary>
        /// Data set for trad like entities.
        /// </summary>
        public DbSet<TradLikeEntity> TradLikes { get; set; }

        /// <summary>
        /// Configures the model relationships between entities.
        /// </summary>
        /// <param name="modelBuilder">Model builder for configuring entities and their relationships.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка связей для лайков
            modelBuilder.Entity<PostLikeEntity>()
                .HasKey(pl => new { pl.UserId, pl.PostId }); // Композитный ключ

            modelBuilder.Entity<PostLikeEntity>()
                .HasOne(pl => pl.User)
                .WithMany() // Каждый пользователь может иметь много лайков
                .HasForeignKey(pl => pl.UserId);

            modelBuilder.Entity<PostLikeEntity>()
                .HasOne(pl => pl.Post)
                .WithMany(p => p.Likes) // Пост может иметь много лайков
                .HasForeignKey(pl => pl.PostId)
                .OnDelete(DeleteBehavior.Cascade); // Каскадное удаление лайков при удалении поста

            modelBuilder.Entity<CommentLikeEntity>()
                .HasKey(cl => new { cl.UserId, cl.CommentId }); // Композитный ключ

            modelBuilder.Entity<CommentLikeEntity>()
                .HasOne(cl => cl.User)
                .WithMany()
                .HasForeignKey(cl => cl.UserId);

            modelBuilder.Entity<CommentLikeEntity>()
                .HasOne(cl => cl.Comment)
                .WithMany(c => c.Likes)
                .HasForeignKey(cl => cl.CommentId)
                .OnDelete(DeleteBehavior.Cascade); // Каскадное удаление лайков при удалении комментария

            modelBuilder.Entity<TradLikeEntity>()
                .HasKey(tl => new { tl.UserId, tl.TradId }); // Композитный ключ

            modelBuilder.Entity<TradLikeEntity>()
                .HasOne(tl => tl.User)
                .WithMany()
                .HasForeignKey(tl => tl.UserId);

            modelBuilder.Entity<TradLikeEntity>()
                .HasOne(tl => tl.Trad)
                .WithMany(t => t.Likes)
                .HasForeignKey(tl => tl.TradId)
                .OnDelete(DeleteBehavior.Cascade); // Каскадное удаление лайков при удалении треда

            // Настройка связи между пользователем и контентом
            modelBuilder.Entity<PostEntity>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts) // Каждый пользователь может иметь много постов
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Удаление поста при удалении пользователя

            modelBuilder.Entity<PostEntity>()
               .HasOne(p => p.Trad)
               .WithMany(u => u.Posts) // Каждый трэд может иметь много постов
               .HasForeignKey(p => p.TradId)
               .OnDelete(DeleteBehavior.Cascade); // Удаление поста при удалении трэда

            modelBuilder.Entity<CommentEntity>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments) // Каждый пользователь может иметь много комментариев
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Удаление комментария при удалении пользователя

            modelBuilder.Entity<CommentEntity>()
                .HasOne(c => c.Post)
                .WithMany(u => u.Comments) // Каждый пост может иметь много комментариев
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade); // Удаление комментария при удалении поста

            modelBuilder.Entity<TradEntity>()
                .HasOne(t => t.User)
                .WithMany(u => u.Trads) // Каждый пользователь может иметь много тредов
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Удаление треда при удалении пользователя
        }
    }
}

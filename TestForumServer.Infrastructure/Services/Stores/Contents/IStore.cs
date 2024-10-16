using TestForumServer.Domian.Entities.ForumEntities.Contents;

namespace TestForumServer.Infrastructure.Services.Stores.Contents
{
    /// <summary>
    /// Represents a generic interface for managing CRUD operations on entities of type <typeparamref name="TEntity"/> in a database context.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity that extends <see cref="ContentEntity"/>.</typeparam>
    public interface IStore<TEntity> where TEntity : ContentEntity?
    {
        /// <summary>
        /// Asynchronously adds a new entity to the database.
        /// </summary>
        /// <param name="item">The entity to be added.</param>
        /// <returns>A task representing the asynchronous operation, containing the added entity.</returns>
        Task<TEntity> AddAsync(TEntity item);

        /// <summary>
        /// Asynchronously deletes an entity from the database by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to be deleted.</param>
        /// <returns>A task representing the asynchronous operation, containing a boolean indicating whether the deletion was successful.</returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Retrieves an entity from the database by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to retrieve.</param>
        /// <returns>The requested entity, or <c>null</c> if not found.</returns>
        TEntity? Get(int id);

        /// <summary>
        /// Retrieves all entities from the database.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TEntity}"/> representing the collection of all entities.</returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// Asynchronously updates an existing entity in the database.
        /// </summary>
        /// <param name="item">The entity with updated values.</param>
        /// <returns>A task representing the asynchronous operation, containing the updated entity, or <c>null</c> if not found.</returns>
        Task<TEntity?> UpdateAsync(TEntity item);

        /// <summary>
        /// Asynchronously updates the image of an entity in the database.
        /// </summary>
        /// <param name="id">The identifier of the entity whose image is to be updated.</param>
        /// <param name="newImage">The new image path or URL.</param>
        /// <returns>A task representing the asynchronous operation, containing the updated entity, or <c>null</c> if not found.</returns>
        Task<TEntity?> UpdateImageAsync(int id, string newImage);

        /// <summary>
        /// Asynchronously updates the content of an entity in the database.
        /// </summary>
        /// <param name="id">The identifier of the entity whose content is to be updated.</param>
        /// <param name="content">The new content.</param>
        /// <returns>A task representing the asynchronous operation, containing the updated entity, or <c>null</c> if not found.</returns>
        Task<TEntity?> UpdateContentAsync(int id, string content);

        /// <summary>
        /// Asynchronously allows a user to like an entity in the database.
        /// </summary>
        /// <param name="id">The identifier of the entity to like.</param>
        /// <param name="userId">The identifier of the user liking the entity.</param>
        /// <returns>A task representing the asynchronous operation, containing a boolean indicating whether the like action was successful.</returns>
        Task<bool> LikeContentAsync(int id, int userId);

        /// <summary>
        /// Asynchronously allows a user to unlike an entity in the database.
        /// </summary>
        /// <param name="id">The identifier of the entity to unlike.</param>
        /// <param name="userId">The identifier of the user unliking the entity.</param>
        /// <returns>A task representing the asynchronous operation, containing a boolean indicating whether the unlike action was successful.</returns>
        Task<bool> UnLikeContentAsync(int id, int userId);

        /// <summary>
        /// Asynchronously toggles the like status of an entity for a user in the database.
        /// </summary>
        /// <param name="id">The identifier of the entity to toggle like status.</param>
        /// <param name="userId">The identifier of the user toggling the like status.</param>
        /// <returns>A task representing the asynchronous operation, containing a boolean indicating whether the action was successful.</returns>
        Task<bool> ToggleLikeContentAsync(int id, int userId);
    }
}

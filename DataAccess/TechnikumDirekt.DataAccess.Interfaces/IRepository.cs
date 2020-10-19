using System.Collections.Generic;

namespace TechnikumDirekt.DataAccess.Interfaces
{
    public interface IRepository<T>
    {
        /// <summary>
        /// Retrieves a single entity identified by its ID.
        /// </summary>
        /// <param name="entity">Primary key (id) of the desired entity</param>
        /// <returns>
        ///    T: Entity with Id = given id param
        ///    null: Entity was not found
        /// </returns>
        T Get(T entity);
    
        /// <summary>
        /// Retrieves an enumerable collection of all entities.
        /// </summary>
        /// <returns>Enumerable of all entities</returns>
        IEnumerable<T> GetAll();
    
        /// <summary>
        /// Add an entity to the collection.
        /// </summary>
        /// <param name="entity">Entity to add</param>
        /// <returns>Added entity with adjusted primary key (ID)</returns>
        T Add(T entity);
    
        /// <summary>
        /// Updates an entity in the collection.
        /// </summary>
        /// <param name="entity">Entity to update with the given entities properties (searches collection by comparing ID)</param>
        void Update(T entity);
    
        /// <summary>
        /// Deletes an entity from the collection.
        /// </summary>
        /// <param name="entity">Entity to be deleted (searches collection by comparing ID)</param>
        void Delete(T entity);
    }
}
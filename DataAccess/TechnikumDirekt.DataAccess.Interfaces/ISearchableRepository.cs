using System.Collections.Generic;

namespace TechnikumDirekt.DataAccess.Interfaces
{
    /// <summary>
    /// Implements IRepository and extends it by adding a find entity/find many entities method.
    /// </summary>
    /// <typeparam name="T">Db entity</typeparam>
    public interface ISearchableRepository<T> : IRepository<T>
    {
        /// <summary>
        /// Finds an entity in the collection.
        /// </summary>
        /// <param name="entity">Entity to search for (searches collection by comparing ID)</param>
        /// <returns>
        ///     T: Entity from the collection
        ///     null: No entity found
        /// </returns>
        T Find(T entity);
        
        /// <summary>
        /// Finds many entities in the collection.
        /// </summary>
        /// <param name="entity">Entity with properties to search for (searches collection by comparing properties)</param>
        /// <returns>Enumerable of search results</returns>
        IEnumerable<T> FindMany(T entity);
    }
}
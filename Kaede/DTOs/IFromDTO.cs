using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.DTOs
{
    /// <summary>
    /// Defines a contract for converting a Data Transfer Object (DTO) of type <typeparamref name="T"/>
    /// into a corresponding entity of type <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="T">The type of the Data Transfer Object.</typeparam>
    /// <typeparam name="TEntity">The type of the entity to be created from the DTO.</typeparam>
    public interface IFromDTO<T, TEntity>
    {
        /// <summary>
        /// Converts the specified DTO instance into an entity of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="dto">The DTO instance to convert.</param>
        /// <returns>An entity of type <typeparamref name="TEntity"/> constructed from the DTO.</returns>
        static abstract TEntity FromDTO(T dto);
    }
}

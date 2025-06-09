using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.DTOs
{
    /// <summary>
    /// Defines a contract for attempting to convert a Data Transfer Object (DTO) of type <typeparamref name="T"/>
    /// into a corresponding entity of type <typeparamref name="TEntity"/> with success indication.
    /// </summary>
    /// <typeparam name="T">The type of the Data Transfer Object.</typeparam>
    /// <typeparam name="TEntity">The type of the entity to be created from the DTO.</typeparam>
    /// <remarks>
    /// This interface is similar to <see cref="IFromDTO{T, TEntity}"/>, but provides a safe conversion method
    /// that returns a <see cref="bool"/> indicating whether the conversion was successful, and outputs the entity if successful.
    /// </remarks>
    public interface ITryFromDTO<T, TEntity>
    {
        /// <summary>
        /// Attempts to convert the specified DTO instance into an entity of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="dto">The DTO instance to convert.</param>
        /// <param name="entity">When this method returns, contains the converted entity if the conversion succeeded; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the conversion was successful; otherwise, <c>false</c>.</returns>
        static abstract bool TryFromDTO(T dto, out TEntity? entity);
    }


    // If this is thrown it is just unrecoverable and
    // it means we have fucked up the UI validation side
    public class InvalidDTOException : Exception
    {
        public InvalidDTOException(string message) : base(message) { }
    }
}

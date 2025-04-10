using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.DTOs
{
    public interface ITryFromDTO<T, TEntity> 
    {
        static abstract bool TryFromDTO(T dto, out TEntity? entity);
    }

    // If this is thrown it is just unrecoverable and
    // it means we have fucked up the UI validation side
    public class InvalidDTOException : Exception
    {
        public InvalidDTOException(string message) : base(message) { }
    }
}

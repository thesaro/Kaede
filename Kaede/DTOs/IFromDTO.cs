using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.DTOs
{
    public interface IFromDTO<T, TEntity> 
    {
        static abstract TEntity FromDTO(T dto);
    }
}

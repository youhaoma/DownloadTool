using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPan.Infrastructure.Interfaces
{
    public interface IRepository<in TKey, TEntity>
    {
        /// <summary>
        ///  Returns the first or defaul element in the collection
        /// </summary>
        /// <returns>he first or default element, will return null if there are nothing in the collection</returns>
        TEntity FirstOrDefault();



        /// <summary>
        /// Get Element By Id
        /// </summary>
        /// <returns></returns>
        TEntity FindById(TKey id);


        /// <summary>
        ///  Get All Elements
        /// </summary>
        /// <returns> </returns>
        IEnumerable<TEntity> GetAll();


        /// <summary>
        ///  Whether element is in collection or not
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Contains(TKey id);


        /// <summary>
        /// Persistences the entity parameter
        /// </summary>
        void Save();
    }
}

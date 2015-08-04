using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MNIT_Communication.Domain;

namespace MNIT_Communication.Services
{
    public interface IRepository
    {
        /// <summary>
        /// Gets all instances of the Type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<IList<T>> Get<T>() where T: BaseEntity;
        
        /// <summary>
        /// Gets only the instance represented by the id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns>Instance found, or null if not found</returns>
        Task<T> Get<T>(Guid id) where T : BaseEntity;

        /// <summary>
        /// Gets only instances that match the supplied predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">Lambda to filter with</param>
        /// <returns>Zero or more instances</returns>
        Task<IList<T>> Get<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity;

        /// <summary>
        /// If the object represented by the id exists, update it, otherwise insert it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<T> Upsert<T>(T item) where T : BaseEntity;

    }
}

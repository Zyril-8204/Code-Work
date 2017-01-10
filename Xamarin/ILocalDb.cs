using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XamarinApp.LocalDb
{
    public interface ILocalDb
    {
        Task<bool> UpdateDatabase();
        //bool AddRecord<T>(object record);

        /// <summary>Convenience method to call the type-specific overload
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        //bool AddRecord(object record);
        bool AddRecord<T>(T record) where T : class, new();
        List<object> GetRecords<T>(int max);

        /// <summary>Each VM will have its own data types and own criteria for removal. 
        /// Removal will have to be implemented on each VM and not based up to base.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="record"></param>
        /// <returns></returns>
        bool RemoveRecord<T>(T record);
       //bool  RemoveRecord(object record);
    }
}

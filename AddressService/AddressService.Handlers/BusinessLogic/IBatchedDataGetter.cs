using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AddressService.Handlers.BusinessLogic
{
    /// <summary>
    /// Gets data in asynchronous concurrent batches for speed (e.g. getting 1,000,000 rows from a database might be slow, but getting 10x 100,000 concurrently could be much faster
    /// </summary>
    public interface IBatchedDataGetter
    {
        /// <summary>
        /// Get data in asynchronous concurrent batches for speed
        /// </summary>
        /// <typeparam name="T">Individual data item</typeparam>
        /// <param name="dataGetter">Delegate that gets the data between a from and to ID</param>
        /// <param name="fromId">From ID</param>
        /// <param name="toId">To ID</param>
        /// <param name="batchSize">Size of batches</param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAsync<T>(Func<int, int, Task<IEnumerable<T>>> dataGetter, int fromId, int toId, int batchSize);
    }
}
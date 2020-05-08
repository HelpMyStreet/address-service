using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AddressService.Handlers.BusinessLogic
{
    /// <inheritdoc/>
    public class BatchedDataGetter : IBatchedDataGetter
    {
        /// <inheritdoc/>
        public async Task<IEnumerable<T>> GetAsync<T>(Func<int, int, Task<IEnumerable<T>>> dataGetter, int fromId, int toId, int batchSize)
        {
            List<Task<IEnumerable<T>>> dataInBatchTasks = new List<Task<IEnumerable<T>>>();

            int from = fromId;
            int to = from + batchSize - 1;

            while (from <= toId)
            {
                Task<IEnumerable<T>> postcodesInBatchTask = dataGetter.Invoke(from, to);
                dataInBatchTasks.Add(postcodesInBatchTask);

                from += batchSize;
                to += batchSize;
                if (to > toId)
                {
                    to = toId;
                }
            }

            List<T> data = new List<T>();

            while (dataInBatchTasks.Count > 0)
            {
                Task<IEnumerable<T>> finishedTask = await Task.WhenAny(dataInBatchTasks);
                dataInBatchTasks.Remove(finishedTask);

                IEnumerable<T> finishedBatch = await finishedTask;

                data.AddRange(finishedBatch);
            }

            return data;
        }
    }
}

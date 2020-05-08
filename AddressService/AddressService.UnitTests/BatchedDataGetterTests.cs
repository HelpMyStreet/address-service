using AddressService.Handlers.BusinessLogic;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AddressService.UnitTests
{
    public class BatchedDataGetterTests
    {
        private Func<int, int, Task<IEnumerable<string>>> _dataGetter;

        private List<Tuple<int, int>> _idsRequested;

        [SetUp]
        public void SetUp()
        {
            _idsRequested = new List<Tuple<int, int>>();
            _dataGetter = async (fromId, toId) =>
            {
                _idsRequested.Add(new Tuple<int, int>(fromId, toId));
                List<string> items = new List<string>();

                for (int j = fromId; j <= toId; j++)
                {
                    items.Add(j.ToString());
                }
                return await Task.FromResult(items);
            };
        }

        [Test]
        public async Task GetDataAsync()
        {
            int fromId = 5;
            int toId = 155;

            BatchedDataGetter batchedDataGetter = new BatchedDataGetter();

            IEnumerable<string> result = await batchedDataGetter.GetAsync(_dataGetter, fromId, toId, 100);

            Assert.AreEqual(151, result.Count());
            Assert.AreEqual(2, _idsRequested.Count());

            Assert.IsTrue(_idsRequested.Any(x => x.Item1 == 5 && x.Item2 == 104));
            Assert.IsTrue(_idsRequested.Any(x => x.Item1 == 105 && x.Item2 == 155));
        }
    }
}

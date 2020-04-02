using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddressService.Core.Dto;
using AddressService.Mappers;
using NUnit.Framework;

namespace AddressService.UnitTests
{
    public class QasMapperTests
    {
        [TestCase("https://api.edq.com/capture/address/v2/format?country=GBR&id=aWQ9NTUwMTgxNTZ-Zm9ybWF0aWQ9NTA3Y2Y5YmItNzA0MC00NGVhLWJiZmItNzE0ZDhmNWIxOWJhfnFsPTZ-Z2VvPTA", "aWQ9NTUwMTgxNTZ-Zm9ybWF0aWQ9NTA3Y2Y5YmItNzA0MC00NGVhLWJiZmItNzE0ZDhmNWIxOWJhfnFsPTZ-Z2VvPTA")]
        public void GetFormatIdFromUri(string uri, string expectedFormatId)
        {
            QasMapper qasMapper = new QasMapper();

            QasSearchRootResponse qasSearchRootResponse = new QasSearchRootResponse()
            {
                Results = new List<QasSearchResponse>()
                {
                    new QasSearchResponse()
                    {
                        Format = uri
                    }
                }
            };

            string result = qasMapper.GetFormatIds(new List<QasSearchRootResponse>(){ qasSearchRootResponse }).SelectMany(x=>x).FirstOrDefault();
            Assert.AreEqual(expectedFormatId, result);
        }
    }
}

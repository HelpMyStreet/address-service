using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using AddressService.Core.Dto;
using AddressService.Core.Utils;
using AddressService.Mappers;
using NUnit.Framework;

namespace AddressService.UnitTests
{
    public class QasMapperTests
    {
        [Test]
        public void GetFormatIdFromUri()
        {
            QasMapper qasMapper = new QasMapper();

            IEnumerable<QasSearchRootResponse> qasSearchRootResponses = new List<QasSearchRootResponse>()
            {
                new QasSearchRootResponse()
                {
                    Results = new List<QasSearchResponse>()
                    {
                        new QasSearchResponse()
                        {
                            Format = "https://api.edq.com/capture/address/v2/format?country=GBR&id=aWQ9NTUwMTgxNTZ-Zm9ybWF0aWQ9NTA3Y2Y5YmItNzA0MC00NGVhLWJiZmItNzE0ZDhmNWIxOWJhfnFsPTZ-Z2VvPTA"
                        }
                    }
                    },
                new QasSearchRootResponse()
                {
                    Results = new List<QasSearchResponse>()
                    {
                        new QasSearchResponse()
                        {
                            Format = ""
                        }
                    }
                },
                new QasSearchRootResponse()
                {
                    Results = new List<QasSearchResponse>()
                    {
                        new QasSearchResponse()
                        {
                            Format =null
                        }
                    }
                },
            };


            var result = qasMapper.GetFormatIds(qasSearchRootResponses);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("aWQ9NTUwMTgxNTZ-Zm9ybWF0aWQ9NTA3Y2Y5YmItNzA0MC00NGVhLWJiZmItNzE0ZDhmNWIxOWJhfnFsPTZ-Z2VvPTA", result.FirstOrDefault().FirstOrDefault());
        }

        [Test]
        public void MapToPostcodeDto()
        {
            QasMapper qasMapper = new QasMapper();

            var postCode = "ng1 5fs";
            var expectedPostcode = PostcodeCleaner.CleanPostcode(postCode);

            IEnumerable<QasFormatRootResponse> qasFormatRootResponses = new List<QasFormatRootResponse>()
            {
                new QasFormatRootResponse()
                {
                    Address = new List<QasFormatAddressReponse>()
                    {
                        new QasFormatAddressReponse()
                        {
                            PostalCode = postCode
                        },
                        new QasFormatAddressReponse()
                        {
                            AddressLine1 = "line1"
                        },
                        new QasFormatAddressReponse()
                        {
                            AddressLine2 = "line2"
                        },
                        new QasFormatAddressReponse()
                        {
                            AddressLine3 = "line3"
                        },
                        new QasFormatAddressReponse()
                        {
                            Locality = "loc"
                        },
                    }
                },
                new QasFormatRootResponse()
                {
                    Address = new List<QasFormatAddressReponse>()
                    {
                        new QasFormatAddressReponse()
                        {
                            PostalCode = "FilterMeOut"
                        },
                        new QasFormatAddressReponse()
                        {
                            AddressLine1 = "line1"
                        },
                        new QasFormatAddressReponse()
                        {
                            AddressLine2 = "line2"
                        },
                        new QasFormatAddressReponse()
                        {
                            AddressLine3 = "line3"
                        },
                        new QasFormatAddressReponse()
                        {
                            Locality = "loc"
                        }
                    }
                },
                new QasFormatRootResponse()
                {
                    Address = new List<QasFormatAddressReponse>()
                    {
                        new QasFormatAddressReponse()
                        {
                            PostalCode = null
                        },
                        new QasFormatAddressReponse()
                        {
                            AddressLine1 = "line1"
                        },
                        new QasFormatAddressReponse()
                        {
                            AddressLine2 = "line2"
                        },
                        new QasFormatAddressReponse()
                        {
                            AddressLine3 = "line3"
                        },
                        new QasFormatAddressReponse()
                        {
                            Locality = "loc"
                        }
                    }
                }
            };

            PostcodeDto result = qasMapper.MapToPostcodeDto(postCode, qasFormatRootResponses);

            Assert.AreEqual(1, result.AddressDetails.Count);
            Assert.AreEqual(expectedPostcode, result.Postcode);
            Assert.AreNotEqual(DateTime.MinValue, result.LastUpdated);

            AddressDetailsDto addressResult = result.AddressDetails.FirstOrDefault();

            Assert.AreEqual(expectedPostcode, addressResult.Postcode);
            Assert.AreEqual("line1", addressResult.AddressLine1);
            Assert.AreEqual("line2", addressResult.AddressLine2);
            Assert.AreEqual("line3", addressResult.AddressLine3);
            Assert.AreEqual("loc", addressResult.Locality);
        }

    }
}

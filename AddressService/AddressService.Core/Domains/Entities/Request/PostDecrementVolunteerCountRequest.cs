using AddressService.Core.Domains.Entities.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AddressService.Core.Domains.Entities.Request
{
    public class PostDecrementVolunteerCountRequest : IRequest
    {
        public string PostCode { get; set; }
    }
}

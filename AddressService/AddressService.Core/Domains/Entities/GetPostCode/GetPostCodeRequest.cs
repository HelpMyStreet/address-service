using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AddressService.Core.Domains.Entities.GetPostCode
{
    public class GetPostCodeRequest : IRequest<GetPostCodeResponse>
    {
        public string Name { get; set; }
        public string UserName { get; set; }
    }
}

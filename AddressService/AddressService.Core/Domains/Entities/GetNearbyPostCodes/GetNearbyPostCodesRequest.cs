using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AddressService.Core.Domains.Entities.GetNearbyPostCodes
{
    public class GetNearbyPostCodesRequest : IRequest<GetNearbyPostCodesResponse>
    {
        public string Name { get; set; }
        public string UserName { get; set; }
    }
}

using AddressService.Core.Domains.Entities.Response;
using AddressService.Core.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AddressService.Core.Interfaces.Repositories
{
    public interface IRepository
    {
        PostCodeResponse GetPostCode(string postCode);
    }
}

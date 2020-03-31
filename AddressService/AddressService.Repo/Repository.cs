using AutoMapper;
using AddressService.Core.Dto;
using AddressService.Core.Interfaces.Repositories;
using AddressService.Repo.EntityFramework.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using AddressService.Core.Domains.Entities.Response;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AddressService.Repo
{
    public class Repository : IRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public Repository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public PostCodeResponse GetPostCode(string postCode)
        {
            PostCode data = _context.PostCode
                .Include(i => i.AddressDetails)
                .Where(x => x.PostalCode == postCode)
                .FirstOrDefault();

            PostCodeResponse result = new PostCodeResponse();

            if (data != null)
            {
                result.PostCode = postCode;
                result.Addresses = new List<AddressDetailsDTO>();
                foreach(var ad in data.AddressDetails)
                {
                    result.Addresses.Add(new AddressDetailsDTO()
                    {
                        Id = ad.Id,
                        HouseName = ad.HouseName,
                        HouseNumber = ad.HouseNumber,
                        City = ad.City,
                        County = ad.County,
                        Street = ad.Street
                    });
                    //result.Addresses.Add(_mapper.Map<AddressDetailsDTO>(ad));
                }
            }

            // List<AddressDetailsDTO> addresses = _mapper.Map<List<AddressDetailsDTO>>(data);

            return result;
        }
    }
}

using AutoMapper;
using AddressService.Core.Dto;
using AddressService.Core.Interfaces.Repositories;
using AddressService.Repo.EntityFramework.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using AddressService.Core.Domains.Entities.Response;
using Microsoft.EntityFrameworkCore;

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
    }
}

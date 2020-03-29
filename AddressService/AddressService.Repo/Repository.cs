﻿using AutoMapper;
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

        public Task<VolunteerCountResponse> GetVolunteerCount()
        {
            int volunteerCount = _context.PostCode.Sum(x => x.VolunteerCount);
            int championCount = _context.PostCode.Sum(x => x.ChampionCount);

            var result = new VolunteerCountResponse()
            {
                ChampionCount = championCount,
                VolunteerCount = volunteerCount
            };

            return Task.FromResult(result);
        }

        public async Task IncrementChampionCount(string postCode)
        {
            PostCode p = _context.PostCode.Where(x => x.PostalCode == postCode).First();

            if(p!=null)
            {
                p.ChampionCount += 1;
                _context.Attach(p).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task IncrementVolunteerCount(string postCode)
        {
            PostCode p = _context.PostCode.Where(x => x.PostalCode == postCode).First();

            if (p != null)
            {
                p.VolunteerCount += 1;
                _context.Attach(p).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }
    }
}

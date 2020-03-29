using AutoMapper;
using AddressService.Core.Dto;
using AddressService.Core.Interfaces.Repositories;
using AddressService.Repo.EntityFramework.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using AddressService.Core.Domains.Entities.Response;

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

        public async Task AddAddress(AddressDetailsDTO addressDetailsDTO)
        {
            var param = _mapper.Map<AddressDetails>(addressDetailsDTO);
            _context.Add(param);
            await _context.SaveChangesAsync();
        }

        public async Task AddPostCode(PostCodeDTO postCodeDTO)
        {
            try
            {
                var param = _mapper.Map<PostCode>(postCodeDTO);
                _context.Add(param);
                await _context.SaveChangesAsync();
            }
            catch (Exception exc)
            {
                string a = exc.ToString();
            }

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
    }
}

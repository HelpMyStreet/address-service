using AddressService.Core.Dto;
using AddressService.Core.Interfaces.Repositories;
using AddressService.Repo.EntityFramework.Entities.AddressService.Repo.EntityFramework.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        // todo test GetMissingPostcodes is fast enough without using TVPs
        public async Task<IEnumerable<PostcodeDto>> GetPostcodes(IEnumerable<string> postCodes)
        {
            List<PostcodeEntity> missingPostCodeEntities = await _context.PostCode.Where(x => !postCodes.Contains(x.Postcode)).Include(x => x.AddressDetails).ToListAsync();

            IEnumerable<PostcodeDto> missingPostCodes = _mapper.Map<IEnumerable<PostcodeEntity>, IEnumerable<PostcodeDto>>(missingPostCodeEntities);

            return missingPostCodes;
        }

        // todo test GetPostcodes is fast enough without using TVPs (the answer will be no...)
        public async Task SavePostcodes(IEnumerable<PostcodeDto> postCodes)
        {
            IEnumerable<PostcodeEntity> missingPostCodesEntities = _mapper.Map<IEnumerable<PostcodeDto>, IEnumerable<PostcodeEntity>>(postCodes);

             await _context.PostCode.AddRangeAsync(missingPostCodesEntities);
             await _context.SaveChangesAsync();
        }
    }
}

using AutoMapper;
using EntityFrameworkCore;

namespace Application
{
    public class BasicLogic
    {
        protected readonly RFPSDbContext _context;
        protected readonly IMapper       _mapper;

        public BasicLogic(RFPSDbContext context,
                          IMapper       mapper)
        {
            this._context = context;
            this._mapper  = mapper;
        }
    }
}
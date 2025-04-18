using Application.Dtos.Unit;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using EntityFrameworkCore;

namespace Application.BusinessLogic.UnitLogic
{
    public class UnitImp(
        RFPSDbContext context,
        IMapper       mapper) : BasicLogic(
                                           context,
                                           mapper), IUnit
    {
        public async Task<DbOperationResult<UnitResultDto>> Insert(UnitQueryDto unitQuery)
        {
            DbOperationResult<UnitResultDto> result = new DbOperationResult<UnitResultDto>();

            Unit unit = _mapper.Map<Unit>(unitQuery);

            _context.Unit.Add(unit);

            result.amount = await _context.SaveChangesAsync();

            result.resultDto = new List<UnitResultDto>()
                               {
                                   mapper.Map<UnitResultDto>(unit)
                               };

            return result;
        }

        public async Task<DbOperationResult<UnitResultDto>> Update(UnitQueryDto unitQuery)
        {
            DbOperationResult<UnitResultDto> result = new DbOperationResult<UnitResultDto>();

            Unit unit = _context
                        .Unit.Where(x => x.Id == unitQuery.Id)
                        .Select(
                                o => new Unit()
                                     {
                                         Id = o.Id,
                                         Name = unitQuery.Name ?? o.Name
                                     })
                        .SingleOrDefault() ?? throw new Exception("Cannot find type.");

            _context.Unit.Update(unit);

            result.amount = await _context.SaveChangesAsync();

            result.resultDto = new List<UnitResultDto>()
                               {
                                   mapper.Map<UnitResultDto>(unit)
                               };

            return result;
        }

        public async Task<DbOperationResult<UnitResultDto>> Read(UnitQueryDto unitQuery)
        {
            DbOperationResult<UnitResultDto> result = new DbOperationResult<UnitResultDto>();

            result.resultDto = _context
                               .Unit.Where(
                                           item => (item.Id   == unitQuery.Id   || unitQuery.Id   == null)
                                                && (item.Name == unitQuery.Name || unitQuery.Name == null))
                               .OrderBy(o => o.Id)
                               .ProjectTo<UnitResultDto>(_mapper.ConfigurationProvider)
                               .ToList();

            result.amount = result.resultDto.Count;

            return result;
        }

        public async Task<DbOperationResult<UnitResultDto>> Delete(int id)
        {
            DbOperationResult<UnitResultDto> result = new DbOperationResult<UnitResultDto>();
            Unit                             unit   = _context.Unit.First(item => item.Id == id);

            _context.Unit.Remove(unit);

            result.amount = await _context.SaveChangesAsync();

            result.resultDto = new List<UnitResultDto>()
                               {
                                   mapper.Map<UnitResultDto>(unit)
                               };

            return result;
        }
    }
}
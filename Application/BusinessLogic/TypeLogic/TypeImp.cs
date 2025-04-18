using Application.Dtos.Type;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using EntityFrameworkCore;
using Type = Domain.Type;

namespace Application.BusinessLogic.TypeLogic
{
    public class TypeImp(
        RFPSDbContext context,
        IMapper       mapper) : BasicLogic(
                                           context,
                                           mapper), IType
    {
        public async Task<DbOperationResult<TypeResultDto>> Insert(TypeQueryDto typeQuery)
        {
            DbOperationResult<TypeResultDto> result = new DbOperationResult<TypeResultDto>();

            Type type = _mapper.Map<Type>(typeQuery);

            _context.Type.Add(type);

            result.amount = await _context.SaveChangesAsync();

            result.resultDto = new List<TypeResultDto>()
                               {
                                   _mapper.Map<TypeResultDto>(type)
                               };

            return result;
        }

        public async Task<DbOperationResult<TypeResultDto>> Update(TypeQueryDto typeQuery)
        {
            DbOperationResult<TypeResultDto> result = new DbOperationResult<TypeResultDto>();

            Type type = _context
                        .Type
                        .Where(x => x.Id == typeQuery.Id)
                        .Select(
                                o => new Type()
                                     {
                                         Id   = o.Id,
                                         Name = typeQuery.Name ?? o.Name
                                     })
                        .SingleOrDefault() ?? throw new Exception("Cannot find type.");

            _context.Type.Update(type);

            result.amount = await _context.SaveChangesAsync();

            result.resultDto = new List<TypeResultDto>()
                               {
                                   _mapper.Map<TypeResultDto>(type)
                               };

            return result;
        }

        public async Task<DbOperationResult<TypeResultDto>> Read(TypeQueryDto typeQuery)
        {
            DbOperationResult<TypeResultDto> result = new DbOperationResult<TypeResultDto>();

            result.resultDto = _context
                               .Type.Where(
                                           item => (item.Id   == typeQuery.Id   || typeQuery.Id   == null)
                                                && (item.Name == typeQuery.Name || typeQuery.Name == null))
                               .OrderBy(o => o.Id)
                               .ProjectTo<TypeResultDto>(_mapper.ConfigurationProvider)
                               .ToList();

            result.amount = result.resultDto.Count;

            return result;
        }

        public async Task<DbOperationResult<TypeResultDto>> Delete(int id)
        {
            DbOperationResult<TypeResultDto> result = new DbOperationResult<TypeResultDto>();
            Type                             type   = _context.Type.First(item => item.Id == id);

            _context.Type.Remove(type);

            result.amount = await _context.SaveChangesAsync();

            result.resultDto = new List<TypeResultDto>()
                               {
                                   _mapper.Map<TypeResultDto>(type)
                               };

            return result;
        }
    }
}
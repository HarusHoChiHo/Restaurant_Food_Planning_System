using Application.Dtos.Type;
using Type = Domain.Type;

namespace Application.BusinessLogic.TypeLogic
{
    public interface IType
    {
        public Task<DbOperationResult<TypeResultDto>> Insert(TypeQueryDto typeQuery);

        public Task<DbOperationResult<TypeResultDto>> Update(TypeQueryDto typeQuery);

        public Task<DbOperationResult<TypeResultDto>> Read(TypeQueryDto typeQuery);

        public Task<DbOperationResult<TypeResultDto>> Delete(int id);
    }
}
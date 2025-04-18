using Application.Dtos.Unit;

namespace Application.BusinessLogic.UnitLogic
{
    public interface IUnit
    {
        public Task<DbOperationResult<UnitResultDto>> Insert(UnitQueryDto unitQuery);

        public Task<DbOperationResult<UnitResultDto>> Update(UnitQueryDto unitQuery);

        public Task<DbOperationResult<UnitResultDto>> Read(UnitQueryDto unitQuery);

        public Task<DbOperationResult<UnitResultDto>> Delete(int id);
    }
}
using Application.Dtos.Role;
using Application.Dtos.Unit;

namespace Application.BusinessLogic.RoleLogic;

public interface IRole
{
    public Task<DbOperationResult<RoleResultDto>> Insert(RoleBasicDto basicDto);

    public Task<DbOperationResult<RoleResultDto>> Update(RoleFullDto fullDto);

    public Task<DbOperationResult<RoleResultDto>> Read(RoleQueryDto fullDto);

    public Task<DbOperationResult<RoleResultDto>> Delete(int id);
}
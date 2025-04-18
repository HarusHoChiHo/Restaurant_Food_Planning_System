using Application.Dtos.User;
using Microsoft.AspNetCore.Identity;

namespace Application.BusinessLogic.UserLogic
{
    public interface IUser
    {
        public Task<DbOperationResult<UserResultDto>> Insert(UserQueryDto queryDto);

        public Task<DbOperationResult<UserResultDto>> Update(UserQueryDto queryDto);

        public Task<DbOperationResult<UserResultDto>> Read(int id);

        public Task<UserResultDto> Validate(UserBasicDto basicDto);

        public Task<DbOperationResult<UserResultDto>> Read();

        public Task<DbOperationResult<UserResultDto>> Delete(int id);

        public Task<IdentityResult> SaveToken(int    userId,
                                              string loginProvider,
                                              string name,
                                              string token);

        public Task<DbOperationResult<UserResultDto>> AssignRole(int    userId,
                                                                 string roleName);

        public Task<DbOperationResult<UserResultDto>> RemoveRole(int    userId,
                                                                 string roleName);
    }
}
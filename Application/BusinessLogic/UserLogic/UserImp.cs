using Application.Dtos.User;
using AutoMapper;
using EntityFrameworkCore;
using Domain;
using Microsoft.AspNetCore.Identity;

namespace Application.BusinessLogic.UserLogic
{
    public class UserImp(
        RFPSDbContext     context,
        IMapper           mapper,
        UserManager<User> userManager)
        : BasicLogic(
                     context,
                     mapper), IUser
    {
        public async Task<DbOperationResult<UserResultDto>> Insert(UserQueryDto queryDto)
        {
            DbOperationResult<UserResultDto> result = new DbOperationResult<UserResultDto>();

            User user = _mapper.Map<User>(queryDto);

            user.SecurityStamp = Guid
                                 .NewGuid()
                                 .ToString();

            IdentityResult createResult = await userManager.CreateAsync(user);

            if (!createResult.Succeeded)
            {
                throw new Exception(createResult.ToString());
            }

            if (queryDto.Role != null)
            {
                foreach (string role in queryDto.Role)
                {
                    if (!await userManager.IsInRoleAsync(
                                                         user,
                                                         role))
                    {
                        await userManager.AddToRoleAsync(
                                                         user,
                                                         role);
                    }
                }
            }

            result.amount = 1;

            result.resultDto = new List<UserResultDto>()
                               {
                                   _mapper.Map<UserResultDto>(await userManager.FindByIdAsync(user.Id.ToString()))
                               };

            result.resultDto.First()
                  .Role = await userManager.GetRolesAsync(user);

            return result;
        }

        public async Task<DbOperationResult<UserResultDto>> Update(UserQueryDto queryDto)
        {
            DbOperationResult<UserResultDto> result = new DbOperationResult<UserResultDto>();

            User user = await userManager.FindByIdAsync(queryDto.Id.ToString());

            user = _mapper.Map(
                               _mapper.Map<UserFullDto>(queryDto),
                               user);

            IdentityResult updateUserResult = await userManager.UpdateSecurityStampAsync(user);

            if (!updateUserResult.Succeeded)
            {
                throw new Exception(updateUserResult.ToString());
            }

            updateUserResult = await userManager.UpdateAsync(user);

            if (!updateUserResult.Succeeded)
            {
                throw new Exception(updateUserResult.ToString());
            }

            if (queryDto.Role != null)
            {
                IList<string> existingRoles = await userManager.GetRolesAsync(user);

                List<string> rolesToAdd = queryDto
                                          .Role.Except(existingRoles)
                                          .ToList();

                List<string> rolesToRemove = existingRoles
                                             .Except(queryDto.Role)
                                             .ToList();

                foreach (string role in rolesToRemove)
                {
                    updateUserResult = await userManager.RemoveFromRoleAsync(
                                                                             user,
                                                                             role);
                    if (!updateUserResult.Succeeded)
                    {
                        throw new Exception(updateUserResult.ToString());
                    }
                }

                foreach (string role in rolesToAdd)
                {
                    updateUserResult = await userManager.AddToRoleAsync(
                                                                        user,
                                                                        role);
                    if (!updateUserResult.Succeeded)
                    {
                        throw new Exception(updateUserResult.ToString());
                    }
                }
            }

            result.resultDto = new List<UserResultDto>()
                               {
                                   _mapper.Map<UserResultDto>(user)
                               };
            result.amount = 1;
            result.resultDto.First()
                  .Role = await userManager.GetRolesAsync(user);

            return result;
        }

        public async Task<DbOperationResult<UserResultDto>> Read(int id)
        {
            DbOperationResult<UserResultDto> result = new DbOperationResult<UserResultDto>();

            User? user = _context.Users.Find(id);

            if (user != null)
            {
                user.SecurityStamp = Guid
                                     .NewGuid()
                                     .ToString();

                result.resultDto = new List<UserResultDto>()
                                   {
                                       _mapper.Map<UserResultDto>(user)
                                   };

                result.resultDto.First()
                      .Role = await userManager.GetRolesAsync(user);

                result.amount = 1;
            }

            return result;
        }

        public async Task<UserResultDto> Validate(UserBasicDto basicDto)
        {
            User? user = _context.Users.FirstOrDefault(
                                                       x => x.UserName == basicDto.UserName
                                                         && x.PasswordHash
                                                         == basicDto.Password);

            if (user != null)
            {
                user.SecurityStamp = Guid
                                     .NewGuid()
                                     .ToString();
            }

            UserResultDto dto = _mapper.Map<UserResultDto>(user);

            dto.Role = await userManager.GetRolesAsync(user);

            return dto;
        }

        public async Task<DbOperationResult<UserResultDto>> Read()
        {
            DbOperationResult<UserResultDto> result = new DbOperationResult<UserResultDto>();
            List<User> users = userManager
                               .Users
                               .OrderBy(o => o.Id)
                               .ToList();

            result.resultDto = new List<UserResultDto>();

            UserResultDto dto;

            users.ForEach(
                          user =>
                          {
                              dto = _mapper.Map<UserResultDto>(user);
                              dto.Role = userManager.GetRolesAsync(user)
                                                    .Result;
                              result.resultDto.Add(dto);
                          });

            return result;
        }

        public async Task<DbOperationResult<UserResultDto>> Delete(int id)
        {
            DbOperationResult<UserResultDto> result = new DbOperationResult<UserResultDto>();

            User user = _context
                        .Users.First(user => user.Id == id);

            user.SecurityStamp = Guid
                                 .NewGuid()
                                 .ToString();

            _context.Users.Remove(user);

            result.amount = await _context.SaveChangesAsync();

            result.resultDto = new List<UserResultDto>()
                               {
                                   _mapper.Map<UserResultDto>(user)
                               };

            return result;
        }

        public async Task<IdentityResult> SaveToken(int    userId,
                                                    string loginProvider,
                                                    string name,
                                                    string token)
        {
            User user = await userManager.FindByIdAsync(userId.ToString());

            user.SecurityStamp = Guid
                                 .NewGuid()
                                 .ToString();

            return await userManager.SetAuthenticationTokenAsync(
                                                                 user,
                                                                 loginProvider,
                                                                 name,
                                                                 token);
        }

        public async Task<DbOperationResult<UserResultDto>> AssignRole(int    userId,
                                                                       string roleName)
        {
            DbOperationResult<UserResultDto> result = new DbOperationResult<UserResultDto>();


            User user = await userManager.FindByIdAsync(userId.ToString());

            user.SecurityStamp = Guid
                                 .NewGuid()
                                 .ToString();

            IdentityResult addRoleResult = await userManager.AddToRoleAsync(
                                                                            user,
                                                                            roleName);

            if (!addRoleResult.Succeeded)
            {
                throw new Exception(addRoleResult.ToString());
            }

            result.resultDto = new List<UserResultDto>()
                               {
                                   _mapper.Map<UserResultDto>(user)
                               };
            result.resultDto.First()
                  .Role = await userManager.GetRolesAsync(user);
            result.amount = 1;

            return result;
        }

        public async Task<DbOperationResult<UserResultDto>> RemoveRole(int    userId,
                                                                       string roleName)
        {
            DbOperationResult<UserResultDto> result = new DbOperationResult<UserResultDto>();

            User user = await userManager.FindByIdAsync(userId.ToString());

            user.SecurityStamp = Guid
                                 .NewGuid()
                                 .ToString();

            IdentityResult removeRoleResult = await userManager.RemoveFromRoleAsync(
                                                                                    user,
                                                                                    roleName);

            if (!removeRoleResult.Succeeded)
            {
                throw new Exception(removeRoleResult.ToString());
            }

            result.resultDto = new List<UserResultDto>()
                               {
                                   _mapper.Map<UserResultDto>(user)
                               };
            result.resultDto.First()
                  .Role = await userManager.GetRolesAsync(user);
            result.amount = 1;

            return result;
        }
    }
}
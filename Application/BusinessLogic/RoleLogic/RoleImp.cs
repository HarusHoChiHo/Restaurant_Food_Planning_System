using Application.Dtos.Role;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using Microsoft.AspNetCore.Identity;

namespace Application.BusinessLogic.RoleLogic;

public class RoleImp(
    RoleManager<Domain.Role> roleManager,
    IMapper                  mapper) : IRole
{
    public async Task<DbOperationResult<RoleResultDto>> Insert(RoleBasicDto basicDto)
    {
        DbOperationResult<RoleResultDto> result = new DbOperationResult<RoleResultDto>();

        Domain.Role role = mapper.Map<Domain.Role>(basicDto);

        role.CreatedDate = DateTime.Now;

        IdentityResult createResult = await roleManager.CreateAsync(role);

        if (!createResult.Succeeded)
        {
            throw new Exception(createResult.ToString());
        }

        result.amount = 1;
        result.resultDto = new List<RoleResultDto>()
                           {
                               mapper.Map<RoleResultDto>(role)
                           };

        return result;
    }

    public async Task<DbOperationResult<RoleResultDto>> Update(RoleFullDto fullDto)
    {
        DbOperationResult<RoleResultDto> result = new DbOperationResult<RoleResultDto>();

        Domain.Role role = await roleManager.FindByIdAsync(fullDto.Id.ToString());

        if (fullDto.Description != null)
        {
            role.Description = fullDto.Description;
        }

        if (fullDto.Name != null)
        {
            role.Name = fullDto.Name;
        }

        IdentityResult updateResult = await roleManager.UpdateAsync(role);

        if (!updateResult.Succeeded)
        {
            throw new Exception(updateResult.ToString());
        }

        result.amount = 1;
        result.resultDto = new List<RoleResultDto>()
                           {
                               mapper.Map<RoleResultDto>(role)
                           };

        return result;
    }

    public async Task<DbOperationResult<RoleResultDto>> Read(RoleQueryDto fullDto)
    {
        DbOperationResult<RoleResultDto> result = new DbOperationResult<RoleResultDto>();

        if (fullDto.CreatedDate != null)
        {
            result.resultDto = roleManager
                               .Roles.Where(
                                            item =>
                                                (item.Id   == fullDto.Id   || fullDto.Id   == null)
                                             && (item.Name == fullDto.Name || fullDto.Name == null)
                                             && ((item.CreatedDate.Day   == fullDto.CreatedDate.Value.Day
                                               && item.CreatedDate.Month == fullDto.CreatedDate.Value.Month
                                               && item.CreatedDate.Year  == fullDto.CreatedDate.Value.Year)
                                              || fullDto.CreatedDate == null)
                                             && (item.Description == fullDto.Description || fullDto.Description == null)
                                           )
                               .OrderBy(o => o.Id)
                               .ProjectTo<RoleResultDto>(mapper.ConfigurationProvider)
                               .ToList();
        }
        else
        {
            result.resultDto = roleManager
                               .Roles.Where(
                                            item =>
                                                (item.Id          == fullDto.Id          || fullDto.Id          == null)
                                             && (item.Name        == fullDto.Name        || fullDto.Name        == null)
                                             && (item.Description == fullDto.Description || fullDto.Description == null)
                                           )
                               .ProjectTo<RoleResultDto>(mapper.ConfigurationProvider)
                               .ToList();
        }

        result.amount = result.resultDto.Count;

        return result;
    }

    public async Task<DbOperationResult<RoleResultDto>> Delete(int id)
    {
        DbOperationResult<RoleResultDto> result = new DbOperationResult<RoleResultDto>();

        Domain.Role role = roleManager.Roles.First<Domain.Role>(item => item.Id == id);

        IdentityResult deleteResult = await roleManager.DeleteAsync(role);

        if (!deleteResult.Succeeded)
        {
            throw new Exception(deleteResult.ToString());
        }

        result.resultDto = new List<RoleResultDto>()
                           {
                               mapper.Map<RoleResultDto>(role)
                           };
        result.amount = 1;

        return result;
    }
}
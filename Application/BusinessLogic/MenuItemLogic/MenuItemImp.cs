using Application.Dtos.MenuItem;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


namespace Application.BusinessLogic.MenuItemLogic
{
    public class MenuItemImp(
        RFPSDbContext context,
        IMapper       mapper)
        : BasicLogic(
                     context,
                     mapper), IMenuItem
    {
        public async Task<DbOperationResult<MenuItemResultDto>> Insert(MenuItemQueryDto menuItemQuery)
        {
            DbOperationResult<MenuItemResultDto> result = new DbOperationResult<MenuItemResultDto>();

            bool exists = await _context.MenuItem.AnyAsync(m => m.Name.Equals(menuItemQuery.Name));

            if (exists)
            {
                throw new Exception("The name is not allowed to be duplicated.");
            }
            
            MenuItem menuItem = _mapper.Map<MenuItem>(menuItemQuery);
            
            _context.MenuItem.Add(menuItem);

            result.amount = await _context.SaveChangesAsync();
            result.resultDto = new List<MenuItemResultDto>()
                               {
                                   _mapper.Map<MenuItemResultDto>(menuItem)
                               };

            return result;
        }

        public async Task<DbOperationResult<MenuItemResultDto>> Update(MenuItemQueryDto menuItemQuery)
        {
            DbOperationResult<MenuItemResultDto> result = new DbOperationResult<MenuItemResultDto>();

            bool exists = await _context.MenuItem.AnyAsync(m => m.Name.Equals(menuItemQuery.Name));

            if (exists)
            {
                throw new Exception("The name is not allowed to be duplicated.");
            }
            
            MenuItem menuItem = _context
                                .MenuItem
                                .Where(x => x.Id == menuItemQuery.Id)
                                .Select(
                                        o =>
                                            new MenuItem()
                                            {
                                                Id   = o.Id,
                                                Name = menuItemQuery.Name ?? o.Name
                                            })
                                .SingleOrDefault() ?? throw new Exception("Cannot find menu item.");

            _context.MenuItem.Update(_mapper.Map<MenuItem>(menuItemQuery));

            result.amount = await _context.SaveChangesAsync();

            result.resultDto = new List<MenuItemResultDto>()
                               {
                                   _mapper.Map<MenuItemResultDto>(menuItem)
                               };

            return result;
        }

        public async Task<DbOperationResult<MenuItemResultDto>> Read(MenuItemQueryDto menuItemQuery)
        {
            DbOperationResult<MenuItemResultDto> result = new DbOperationResult<MenuItemResultDto>();

            result.resultDto = _context
                               .MenuItem.Where(
                                               x =>
                                                   (x.Id             == menuItemQuery.Id
                                                 || menuItemQuery.Id == null)
                                                && (x.Name             == menuItemQuery.Name
                                                 || menuItemQuery.Name == null)
                                              )
                               .Include(x => x.Menus)
                               .Include(x => x.OrderItems)
                               .Include(x => x.MenuItemFoodItems)
                               .OrderBy(o => o.Id)
                               .ProjectTo<MenuItemResultDto>(_mapper.ConfigurationProvider)
                               .ToList();

            result.amount = result.resultDto.Count;
            
            return result;
        }

        public async Task<DbOperationResult<MenuItemResultDto>> Delete(int id)
        {
            DbOperationResult<MenuItemResultDto> result   = new DbOperationResult<MenuItemResultDto>();
            MenuItem                             menuItem = await _context.MenuItem.FindAsync(id);

            _context.MenuItem.Remove(menuItem);

            result.amount = await _context.SaveChangesAsync();

            result.resultDto = new List<MenuItemResultDto>()
                               {
                                   _mapper.Map<MenuItemResultDto>(menuItem)
                               };

            return result;
        }
    }
}
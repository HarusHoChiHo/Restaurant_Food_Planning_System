using System.Text.Json;
using Application.Dtos.Menu;
using Application.Dtos.MenuItem;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Application.BusinessLogic.MenuLogic
{
    public class MenuImp(
        RFPSDbContext context,
        IMapper       mapper)
        : BasicLogic(
                     context,
                     mapper), IMenu
    {
        public async Task<DbOperationResult<MenuResultDto>> Insert(MenuQueryDto menuQuery)
        {
            DbOperationResult<MenuResultDto> result = new DbOperationResult<MenuResultDto>();

            Menu menu = _mapper.Map<Menu>(menuQuery);

            _context.Menu.Add(menu);

            result.amount = await _context.SaveChangesAsync();
            result.resultDto = new List<MenuResultDto>()
                               {
                                   _mapper.Map<MenuResultDto>(menu)
                               };

            return result;
        }

        public async Task<DbOperationResult<MenuResultDto>> Update(MenuQueryDto menuQuery)
        {
            DbOperationResult<MenuResultDto> result = new DbOperationResult<MenuResultDto>();

            Menu menu = _context
                        .Menu
                        .Where(x => x.Id == menuQuery.Id)
                        .Select(
                                o => new Menu()
                                     {
                                         Id = o.Id,
                                         MenuItem_Id = menuQuery.MenuItem_Id ?? o.MenuItem_Id,
                                         Date = menuQuery.Date ?? o.Date
                                     })
                        .OrderByDescending(o => o.Date)
                        .ThenBy(o => o.Id)
                        .SingleOrDefault() ?? throw new Exception("Cannot find Menu.");

            _context.Menu.Update(menu);

            result.amount = await _context.SaveChangesAsync();

            result.resultDto = _context
                               .Menu.Where(x => x.Id.Equals(menu.Id))
                               .Include(x => x.MenuItem)
                               .ProjectTo<MenuResultDto>(_mapper.ConfigurationProvider)
                               .ToList();

            return result;
        }

        public async Task<DbOperationResult<MenuResultDto>> Read(MenuQueryDto menuQuery)
        {
            DbOperationResult<MenuResultDto> result = new DbOperationResult<MenuResultDto>();
            if (menuQuery.Date != null)
            {
                result.resultDto = _context
                                   .Menu.Where(
                                               menu => (menu.Id == menuQuery.Id || menuQuery.Id == null)
                                                    && ((menu.Date.Day   == menuQuery.Date.Value.Day
                                                      && menu.Date.Month == menuQuery.Date.Value.Month
                                                      && menu.Date.Year  == menuQuery.Date.Value.Year)
                                                     || menuQuery.Date == null)
                                                    && (menu.MenuItem_Id      == menuQuery.MenuItem_Id
                                                     || menuQuery.MenuItem_Id == null)
                                              )
                                   .Include(menu => menu.MenuItem)
                                   .OrderByDescending(menu => menu.Date)
                                   .ThenBy(menu => menu.Id)
                                   .ProjectTo<MenuResultDto>(_mapper.ConfigurationProvider)
                                   .ToList();
            }
            else
            {
                result.resultDto = _context
                                   .Menu.Where(
                                               menu => (menu.Id == menuQuery.Id || menuQuery.Id == null)
                                                    && (menu.MenuItem_Id      == menuQuery.MenuItem_Id
                                                     || menuQuery.MenuItem_Id == null)
                                              )
                                   .Include(menu => menu.MenuItem)
                                   .OrderByDescending(menu => menu.Date)
                                   .ThenBy(menu => menu.Id)
                                   .ProjectTo<MenuResultDto>(_mapper.ConfigurationProvider)
                                   .ToList();
            }

            result.amount = result.resultDto.Count;

            return result;
        }


        public async Task<DbOperationResult<MenuResultDto>> Delete(int id)
        {
            DbOperationResult<MenuResultDto> result = new DbOperationResult<MenuResultDto>();

            Menu menu = _context.Menu.Find(id);

            _context.Menu.Remove(menu);

            result.amount = await _context.SaveChangesAsync();
            result.resultDto = new List<MenuResultDto>()
                               {
                                   _mapper.Map<MenuResultDto>(menu)
                               };

            return result;
        }
    }
}
using Application.Dtos.MenuItemFoodItem;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using EntityFrameworkCore;

namespace Application.BusinessLogic.MenuItemFoodItemLogic
{
    public class MenuItemFoodItemImp(
        RFPSDbContext context,
        IMapper       mapper)
        : BasicLogic(
                     context,
                     mapper), IMenuItemFoodItem
    {
        public async Task<DbOperationResult<MenuItemFoodItemResultDto>> Insert(
            MenuItemFoodItemQueryDto menuIteFoodItemQuery)
        {
            DbOperationResult<MenuItemFoodItemResultDto> result = new DbOperationResult<MenuItemFoodItemResultDto>();
            MenuItemFoodItem menuItemFoodItem = _mapper.Map<MenuItemFoodItem>(menuIteFoodItemQuery);

            _context.MenuItemFoodItem.Add(menuItemFoodItem);

            result.amount = await _context.SaveChangesAsync();
            result.resultDto = new List<MenuItemFoodItemResultDto>()
                               {
                                   _mapper.Map<MenuItemFoodItemResultDto>(menuItemFoodItem)
                               };
            return result;
        }

        public async Task<DbOperationResult<MenuItemFoodItemResultDto>> Update(
            MenuItemFoodItemQueryDto menuItemFoodItemQuery)
        {
            DbOperationResult<MenuItemFoodItemResultDto> result = new DbOperationResult<MenuItemFoodItemResultDto>();

            MenuItemFoodItem menuItemFoodItem =
                _context
                    .MenuItemFoodItem
                    .Where(
                           x => x.MenuItem_Id == menuItemFoodItemQuery.MenuItem_Id
                             && x.FoodItem_Id == menuItemFoodItemQuery.FoodItem_Id)
                    .Select(
                            o => new MenuItemFoodItem()
                                 {
                                     FoodItem_Id = o.FoodItem_Id,
                                     MenuItem_Id = o.MenuItem_Id,
                                     Consumption = menuItemFoodItemQuery.Consumption ?? o.Consumption
                                 })
                    .SingleOrDefault() ?? throw new Exception("Cannot find item with MenuItem_Id and FoodItem_Id.");

            _context.MenuItemFoodItem.Update(menuItemFoodItem);

            result.amount = await _context.SaveChangesAsync();
            result.resultDto = new List<MenuItemFoodItemResultDto>()
                               {
                                   _mapper.Map<MenuItemFoodItemResultDto>(menuItemFoodItem)
                               };

            return result;
        }

        public async Task<DbOperationResult<MenuItemFoodItemResultDto>> Read(
            MenuItemFoodItemQueryDto menuIteFoodItemQuery)
        {
            DbOperationResult<MenuItemFoodItemResultDto> result =
                new DbOperationResult<MenuItemFoodItemResultDto>();

            result.resultDto = _context
                               .MenuItemFoodItem.Where(
                                                       item => (item.MenuItem_Id == menuIteFoodItemQuery.MenuItem_Id
                                                             || menuIteFoodItemQuery.MenuItem_Id == null)
                                                            && (item.FoodItem_Id == menuIteFoodItemQuery.FoodItem_Id
                                                             || menuIteFoodItemQuery.FoodItem_Id == null)
                                                            && (item.Consumption == menuIteFoodItemQuery.Consumption
                                                             || menuIteFoodItemQuery.Consumption == null))
                               .OrderBy(o => o.MenuItem_Id)
                               .ThenBy(o => o.FoodItem_Id)
                               .ProjectTo<MenuItemFoodItemResultDto>(_mapper.ConfigurationProvider)
                               .ToList();

            result.amount = result.resultDto.Count;

            return result;
        }

        public async Task<DbOperationResult<MenuItemFoodItemResultDto>> Delete(
            MenuItemFoodItemQueryDto menuIteFoodItemQuery)
        {
            DbOperationResult<MenuItemFoodItemResultDto> result =
                new DbOperationResult<MenuItemFoodItemResultDto>();

            List<MenuItemFoodItem> items = _context
                                           .MenuItemFoodItem.Where(
                                                                   item => (item.MenuItem_Id
                                                                         == menuIteFoodItemQuery.MenuItem_Id
                                                                         || menuIteFoodItemQuery.MenuItem_Id == null)
                                                                        && (item.FoodItem_Id
                                                                         == menuIteFoodItemQuery.FoodItem_Id
                                                                         || menuIteFoodItemQuery.FoodItem_Id == null)
                                                                        && (item.Consumption
                                                                         == menuIteFoodItemQuery.Consumption
                                                                         || menuIteFoodItemQuery.Consumption == null))
                                           .ToList();
            _context.MenuItemFoodItem.RemoveRange(items);

            result.amount = await _context.SaveChangesAsync();

            result.resultDto = items
                               .AsQueryable()
                               .ProjectTo<MenuItemFoodItemResultDto>(_mapper.ConfigurationProvider)
                               .ToList();

            return result;
        }
    }
}
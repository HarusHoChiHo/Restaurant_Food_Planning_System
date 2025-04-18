using Application.Dtos.MenuItem;

namespace Application.BusinessLogic.MenuItemLogic
{
    public interface IMenuItem
    {
        public Task<DbOperationResult<MenuItemResultDto>> Insert(MenuItemQueryDto menuItemQuery);

        public Task<DbOperationResult<MenuItemResultDto>> Update(MenuItemQueryDto menuItemQuery);

        public Task<DbOperationResult<MenuItemResultDto>> Read(MenuItemQueryDto menuItemQuery);

        public Task<DbOperationResult<MenuItemResultDto>> Delete(int id);
    }
}
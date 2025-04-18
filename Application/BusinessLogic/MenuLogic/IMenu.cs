using Application.Dtos.Menu;

namespace Application.BusinessLogic.MenuLogic
{
    public interface IMenu
    {
        public Task<DbOperationResult<MenuResultDto>> Insert(MenuQueryDto menuQuery);

        public Task<DbOperationResult<MenuResultDto>> Update(MenuQueryDto menuQuery);

        public Task<DbOperationResult<MenuResultDto>> Read(MenuQueryDto menuQuery);

        public Task<DbOperationResult<MenuResultDto>> Delete(int id);
    }
}
using Application.Dtos.MenuItem;

namespace Application.Dtos.Menu
{
    public class MenuResultDto
    {
        public int               Id       { get; set; }
        public DateTime          Date     { get; set; }
        public MenuItemResultDto MenuItem { get; set; }
    }
}
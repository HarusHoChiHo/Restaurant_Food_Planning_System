using Application.Dtos.Type;
using Application.Dtos.Unit;

namespace Application.Dtos.FoodItem
{
    public class FoodItemResultDto
    {
        public int           Id       { get; set; }
        public String?       Name     { get; set; }
        public int           Quantity { get; set; }
        public UnitResultDto Unit     { get; set; }
        public TypeResultDto Type     { get; set; }
    }
}
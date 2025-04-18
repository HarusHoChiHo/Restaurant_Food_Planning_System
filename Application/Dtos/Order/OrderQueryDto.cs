namespace Application.Dtos.Order
{
    public class OrderQueryDto
    {
        public int?  Id         { get; set; }
        public bool? IsCanceled { get; set; }
    }

    public class OrderQueryPerPageDto : OrderQueryDto
    {
        public int? PageNumber { get; set; }
        public int? Limit      { get; set; }
    }
}
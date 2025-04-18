using Application.Dtos.OrderItem;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Application.BusinessLogic.OrderItemLogic
{
    public class OrderItemImp(
        RFPSDbContext context,
        IMapper       mapper) : BasicLogic(
                                           context,
                                           mapper), IOrderItem
    {
        public async Task<DbOperationResult<OrderItemResultDto>> Insert(OrderItemQueryDto orderItemQuery)
        {
            DbOperationResult<OrderItemResultDto> result    = new DbOperationResult<OrderItemResultDto>();
            OrderItem                             orderItem = _mapper.Map<OrderItem>(orderItemQuery);

            _context.OrderItem.Add(orderItem);

            result.amount = await _context.SaveChangesAsync();
            result.resultDto = new List<OrderItemResultDto>()
                               {
                                   _mapper.Map<OrderItemResultDto>(orderItem)
                               };

            return result;
        }

        public async Task<DbOperationResult<OrderItemResultDto>> Update(OrderItemQueryDto orderItemQuery)
        {
            DbOperationResult<OrderItemResultDto> result = new DbOperationResult<OrderItemResultDto>();
            OrderItem orderItem = _context
                                  .OrderItem
                                  .Where(x => x.Id == orderItemQuery.Id)
                                  .Include(o => o.MenuItem)
                                  .Select(
                                          o => new OrderItem()
                                               {
                                                   Id         = o.Id,
                                                   MenuItemId = orderItemQuery.MenuItemId ?? o.MenuItemId,
                                                   OrderId    = orderItemQuery.OrderId    ?? o.OrderId,
                                                   MenuItem   = o.MenuItem
                                               })
                                  .SingleOrDefault() ?? throw new Exception("Cannot find order item.");

            _context.OrderItem.Update(orderItem);

            result.amount = await _context.SaveChangesAsync();

            result.resultDto = new List<OrderItemResultDto>()
                               {
                                   _mapper.Map<OrderItemResultDto>(orderItem)
                               };

            return result;
        }

        public async Task<DbOperationResult<OrderItemResultDto>> Read(OrderItemQueryDto orderItemQuery)
        {
            DbOperationResult<OrderItemResultDto> result = new DbOperationResult<OrderItemResultDto>();

            List<OrderItemResultDto> orderItemResultDtos = _context
                                                           .OrderItem.Where(
                                                                            orderItem =>
                                                                                (orderItem.Id      == orderItemQuery.Id
                                                                              || orderItemQuery.Id == null)
                                                                             && (orderItem.OrderId
                                                                              == orderItemQuery.OrderId
                                                                              || orderItemQuery.OrderId == null)
                                                                             && (orderItem.MenuItemId
                                                                              == orderItemQuery.MenuItemId
                                                                              || orderItemQuery.MenuItemId == null))
                                                           .Include(orderItem => orderItem.MenuItem)
                                                           .Include(orderItem => orderItem.Order)
                                                           .OrderBy(o => o.OrderId)
                                                           .ThenBy(o => o.MenuItemId)
                                                           .ProjectTo<OrderItemResultDto>(_mapper.ConfigurationProvider)
                                                           .ToList();

            result.amount    = orderItemResultDtos.Count;
            result.resultDto = orderItemResultDtos;

            return result;
        }

        public async Task<DbOperationResult<OrderItemResultDto>> Delete(int id)
        {
            DbOperationResult<OrderItemResultDto> result = new DbOperationResult<OrderItemResultDto>();

            OrderItem orderItem = _context.OrderItem.Find(id);

            _context.OrderItem.Remove(orderItem);

            result.amount = await _context.SaveChangesAsync();

            result.resultDto = new List<OrderItemResultDto>()
                               {
                                   _mapper.Map<OrderItemResultDto>(orderItem)
                               };

            return result;
        }
    }
}
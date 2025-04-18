using Application.Dtos.Order;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace Application.BusinessLogic.OrderLogic
{
    public class OrderImp(
        RFPSDbContext context,
        IMapper       mapper) : BasicLogic(
                                           context,
                                           mapper), IOrder
    {
        public async Task<DbOperationResult<OrderResultDto>> Insert(OrderQueryDto orderQuery)
        {
            DbOperationResult<OrderResultDto> result = new DbOperationResult<OrderResultDto>();
            Order                             order  = _mapper.Map<Order>(orderQuery);

            _context.Order.Add(order);

            result.amount = await _context.SaveChangesAsync();

            orderQuery.Id = order.Id;

            result.resultDto = new List<OrderResultDto>()
                               {
                                   _mapper.Map<OrderResultDto>(order)
                               };

            return result;
        }

        public async Task<DbOperationResult<OrderResultDto>> Update(OrderQueryDto orderQuery)
        {
            DbOperationResult<OrderResultDto> result = new DbOperationResult<OrderResultDto>();
            Order order = _context
                          .Order
                          .Where(x => x.Id == orderQuery.Id)
                          .Include(x => x.OrderItems)
                          .ThenInclude(x => x.MenuItem)
                          .Select(
                                  o => new Order()
                                       {
                                           Id         = o.Id,
                                           IsCanceled = orderQuery.IsCanceled ?? o.IsCanceled,
                                           OrderItems = o.OrderItems
                                       })
                          .SingleOrDefault() ?? throw new Exception("Cannot find order.");

            _context.Order.Update(order);

            result.amount = await _context.SaveChangesAsync();
            result.resultDto = new List<OrderResultDto>()
                               {
                                   _mapper.Map<OrderResultDto>(order)
                               };

            return result;
        }

        public async Task<DbOperationResult<OrderResultDto>> Read(OrderQueryPerPageDto orderQuery)
        {
            DbOperationResult<OrderResultDto> result = new DbOperationResult<OrderResultDto>();

            List<OrderResultDto> orderResultDtos = new List<OrderResultDto>();

            int pageNumber = orderQuery.PageNumber ?? 0;
            int limit      = orderQuery.Limit      ?? 0;

            if (pageNumber == 0 && limit == 0)
            {
                orderResultDtos = _context
                                  .Order
                                  .Where(
                                         order =>
                                             (order.Id == orderQuery.Id || orderQuery.Id == null)
                                          && (order.IsCanceled      == orderQuery.IsCanceled
                                           || orderQuery.IsCanceled == null))
                                  .Include(order => order.OrderItems)
                                  .OrderBy(o => o.Id)
                                  .ProjectTo<OrderResultDto>(_mapper.ConfigurationProvider)
                                  .ToList();
                result.amount = orderResultDtos.Count;
            }
            else
            {
                IOrderedQueryable<Order> query = _context
                                                 .Order
                                                 .Where(
                                                        order =>
                                                            (order.Id == orderQuery.Id || orderQuery.Id == null)
                                                         && (order.IsCanceled      == orderQuery.IsCanceled
                                                          || orderQuery.IsCanceled == null))
                                                 .Include(order => order.OrderItems)
                                                 .OrderBy(o => o.Id);
                result.amount = query.Count();
                
                orderResultDtos = query
                                  .Skip((pageNumber - 1) * limit)
                                  .Take(limit)
                                  .ProjectTo<OrderResultDto>(_mapper.ConfigurationProvider)
                                  .ToList();
            }
            
            result.resultDto = orderResultDtos;

            return result;
        }

        public async Task<DbOperationResult<OrderResultDto>> Delete(int id)
        {
            DbOperationResult<OrderResultDto> result = new DbOperationResult<OrderResultDto>();

            Order order = _context.Order.Find(id);

            _context.Order.Remove(order);

            result.amount = await _context.SaveChangesAsync();

            result.resultDto = new List<OrderResultDto>()
                               {
                                   _mapper.Map<OrderResultDto>(order)
                               };

            return result;
        }
    }
}
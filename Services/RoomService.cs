using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Interfaces;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class RoomService : IRoomService
    {
        private readonly PrimaryDbContext _context;
        private readonly IMapper _mapper;

        public RoomService(PrimaryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<Room>> GetRoomsAsync(
            PagedParams pagedParams, 
            SortParams<Room, RoomEntity> sortParams,
            SearchParams<Room, RoomEntity> searchParams)
        {
            IQueryable<RoomEntity> query = _context.Rooms;
            
            query = searchParams.Apply(query);
            query = sortParams.Apply(query);

            var size = await query.CountAsync();

            var rooms = await query
                .Skip(pagedParams.Offset.Value)
                .Take(pagedParams.Limit.Value)
                .ProjectTo<Room>(_mapper.ConfigurationProvider)
                .ToArrayAsync();

            return new PagedResult<Room>
            {
                Items = rooms,
                TotalSize = size
            };
        }

        public async Task<Room> GetRoomAsync(Guid id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return null;
            }

            return _mapper.Map<Room>(room);
        }
    }
}

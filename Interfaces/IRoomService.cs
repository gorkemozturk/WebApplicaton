using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Interfaces
{
    public interface IRoomService
    {
        Task<PagedResult<Room>> GetRoomsAsync(
            PagedParams pagedParams, 
            SortParams<Room, RoomEntity> sortParams,
            SearchParams<Room, RoomEntity> searchParams);
        Task<Room> GetRoomAsync(Guid id);
    }
}

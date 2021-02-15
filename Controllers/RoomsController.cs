using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApplication.Interfaces;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly PagedParams _pagedParams;

        public RoomsController(IRoomService roomService, IOptions<PagedParams> pagedParams)
        {
            _roomService = roomService;
            _pagedParams = pagedParams.Value;
        }

        [HttpGet(Name = "GetRooms")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Collection<Room>>> GetRooms(
            [FromQuery] PagedParams pagedParams,
            [FromQuery] SortParams<Room, RoomEntity> sortParams,
            [FromQuery] SearchParams<Room, RoomEntity> searchParams)
        {
            pagedParams.Offset = pagedParams.Offset ?? _pagedParams.Offset;
            pagedParams.Limit = pagedParams.Limit ?? _pagedParams.Limit;

            var rooms = await _roomService.GetRoomsAsync(pagedParams, sortParams, searchParams);

            var collection = PagedCollection<Room>.Create(
                Link.ToCollection(nameof(GetRooms)),
                rooms.Items.ToArray(),
                rooms.TotalSize,
                pagedParams);

            return collection;
        }

        [HttpGet("{id}", Name = "GetRoom")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<Room>> GetRoom([FromRoute] Guid id)
        {
            var room = await _roomService.GetRoomAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            return room;
        }
    }
}
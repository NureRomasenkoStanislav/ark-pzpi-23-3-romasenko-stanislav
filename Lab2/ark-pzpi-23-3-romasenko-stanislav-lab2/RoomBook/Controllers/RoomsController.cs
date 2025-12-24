using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomBook.API.DTOs;
using RoomBook.Core.Entities;
using RoomBook.Core.Interfaces;

namespace RoomBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomRepository _roomRepository;

        public RoomsController(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Room>))]
        public async Task<IActionResult> GetAllRooms()
        {
            var rooms = await _roomRepository.GetAllRoomsAsync();
            return Ok(rooms);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Room))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRoomById(int id)
        {
            var room = await _roomRepository.GetRoomByIdAsync(id);
            if (room == null)
            {
                return NotFound($"Приміщення з ID {id} не знайдено.");
            }
            return Ok(room);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Room))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRoom([FromBody] RoomDto roomDto)
        {
            if (!ModelState.IsValid || roomDto.Capacity <= 0)
            {
                return BadRequest(ModelState);
            }

            var roomEntity = new Room
            {
                Name = roomDto.Name,
                Capacity = roomDto.Capacity,
                WorkingHoursStart = roomDto.WorkingHoursStart,
                WorkingHoursEnd = roomDto.WorkingHoursEnd,
                Description = roomDto.Description ?? "",
                IsArchived = false
            };

            var createdRoom = await _roomRepository.CreateRoomAsync(roomEntity);
            return CreatedAtAction(nameof(GetRoomById), new { id = createdRoom.RoomId }, createdRoom);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")] 
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRoom(int id, [FromBody] RoomDto roomDto)
        {
            var existingRoom = await _roomRepository.GetRoomByIdAsync(id);
            if (existingRoom == null)
            {
                return NotFound($"Приміщення з ID {id} не знайдено.");
            }

            existingRoom.Name = roomDto.Name;
            existingRoom.Capacity = roomDto.Capacity;
            existingRoom.WorkingHoursStart = roomDto.WorkingHoursStart;
            existingRoom.WorkingHoursEnd = roomDto.WorkingHoursEnd;
            existingRoom.Description = roomDto.Description;

            var result = await _roomRepository.UpdateRoomAsync(existingRoom);

            if (!result)
            {
                return BadRequest("Помилка при оновленні приміщення.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var deleted = await _roomRepository.DeleteRoomAsync(id);

            if (!deleted)
            {
                return NotFound($"Приміщення з ID {id} не знайдено.");
            }

            return NoContent();
        }
    }
}
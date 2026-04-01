using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Circulation.Reservation;
using library.Models.Entities;
using library.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace library.Controllers
{
    [ApiController]
    [Route("api/reservations")]
    [Authorize]
    public class ReservationController : ControllerBase
    {
        private readonly ICirculationService _circulationService;
        private readonly UserManager<User> _userManager;

        public ReservationController(ICirculationService circulationService, UserManager<User> userManager)
        {
            _circulationService = circulationService;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> GetAllReservationsAsync([FromQuery] ReservationQueryParams queryParams)
        {
            var result = await _circulationService.GetAllReservationsAsync(queryParams);
            return Ok(result);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyReservationsAsync([FromQuery] ReservationQueryParams queryParams)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();
            var result = await _circulationService.GetUserReservationsAsync(userId, queryParams);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservationByIdAsync(int id)
        {
            var result = await _circulationService.GetReservationByIdAsync(id);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservationAsync([FromBody] CreateReservationDto dto)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();
            dto.UserId = userId; // ← always use token userId, never trust body
            var result = await _circulationService.CreateReservationAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelReservationAsync(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();
            await _circulationService.CancelReservationAsync(userId, id);
            return Ok(new { message = "Reservation cancelled successfully" });
        }

        [HttpPost("{id}/fulfill")]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> FulfillReservationAsync(int id)
        {
            var result = await _circulationService.FulfillReservationAsync(id);
            return Ok(result);
        }
    }
}
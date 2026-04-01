using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Circulation.Fine;
using library.Models.Entities;
using library.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace library.Controllers
{
    [ApiController]
    [Route("api/fines")]
    [Authorize]
    public class FineController : ControllerBase
    {
        private readonly ICirculationService _circulationService;
        private readonly UserManager<User> _userManager;

        public FineController(ICirculationService circulationService, UserManager<User> userManager)
        {
            _circulationService = circulationService;
            _userManager = userManager;
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllFinesAsync([FromQuery] FineQueryParam queryParams)
        {
            var result = await _circulationService.GetAllFinesAsync(queryParams);
            return Ok(result);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyFinesAsync([FromQuery] FineQueryParam queryParams)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();
            var result = await _circulationService.GetUserFinesAsync(userId, queryParams);
            return Ok(result);
        }

        [HttpPost("{id}/pay")]
        public async Task<IActionResult> PayFineAsync(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();
            var result = await _circulationService.PayFineAsync(userId, id);
            return Ok(result);
        }
    }
}
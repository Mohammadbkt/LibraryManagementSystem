using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Circulation.Loan;
using library.Models.Entities;
using library.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace library.Controllers
{
    [ApiController]
    [Route("api/loans")]
    [Authorize]
    public class LoanController : ControllerBase
    {
        private readonly ICirculationService _circulationService;
        private readonly UserManager<User> _userManager;

        public LoanController(ICirculationService circulationService, UserManager<User> userManager)
        {
            _circulationService = circulationService;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllActiveLoansAsync([FromQuery] LoanQueryParam queryParams)
        {
            var result = await _circulationService.GetAllActiveLoansAsync(queryParams);
            return Ok(result);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyLoansAsync([FromQuery] LoanQueryParam queryParams)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();
            var result = await _circulationService.GetUserLoansAsync(userId, queryParams);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetLoanByIdAsync(int id)
        {
            var result = await _circulationService.GetLoanByIdAsync(id);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CheckoutAsync([FromBody] CheckoutDto dto)
        {
            var UserId = _userManager.GetUserId(User);
            if (UserId == null) return Unauthorized();

            var result = await _circulationService.CheckoutItemAsync(UserId, dto);
            return Ok(result);
        }

        [HttpPost("{id:int}/return")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReturnAsync(int id, [FromBody] ReturnDto dto)
        {
            var result = await _circulationService.ReturnItemAsync(id, dto);
            return Ok(result);
        }

        [HttpPost("{id:int}/extend")]
        public async Task<IActionResult> ExtendAsync(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();
            var result = await _circulationService.ExtendLoanAsync(id);
            return Ok(result);
        }
    }
}
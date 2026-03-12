// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Mvc;

// namespace library.Controllers
// {
//     public class AuthController
//     {
        
//     [ApiController]
//     [Route("api/auth")]
//     public class AuthController : ControllerBase
//     {
//         private readonly IAuthService _authService;

//         public AuthController(IAuthService authService)
//         {
//             _authService = authService;
//         }

//         [HttpPost("login")]
//         public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
//         {
//             try
//             {
//                 if (!ModelState.IsValid)
//                     return BadRequest(ModelState);

//                 var res = await _authService.Login(loginRequestDto);
//                 if (!res.Success)
//                     return BadRequest(res);

//                 return Ok(res);
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, new { message = "Login failed due to server error", detail = ex.Message });
//             }
//         }

//         [HttpPost("register")]
//         public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
//         {
//             try
//             {
//                 if (!ModelState.IsValid)
//                     return BadRequest(ModelState);

//                 var res = await _authService.Register(registerRequestDto);
//                 if (!res.Success)
//                     return BadRequest(res);

//                 return Ok(res);
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, new { message = "Registration failed due to server error", detail = ex.Message });
//             }
//         }

//     [HttpPost("verify-otp")]
//     public async Task<IActionResult> VerifyOtpAsync([FromBody] VerifyOtpRequestDto requestDto)
//     {
//         try
//         {
//             if (!ModelState.IsValid)
//                 return BadRequest(ModelState);

//             var result = await _authService.VerifyOtp(requestDto.UserId, requestDto.OtpCode);
//             if (!result.Success)
//                 return BadRequest(result);

//             return Ok(result);
//         }
//         catch (Exception ex)
//         {
//             return StatusCode(500, new { message = "OTP verification failed", detail = ex.Message });
//         }
//     }

//     [HttpPost("resend-otp")]
//     public async Task<IActionResult> ResendOtpAsync([FromBody] ResendOtpDto dto)
//     {
//         try
//         {
//             if (!ModelState.IsValid)
//                 return BadRequest(ModelState);

//             var result = await _authService.ResendOtpAsync(dto.UserId);
//             if (!result.Success)
//                 return BadRequest(result);

//             return Ok(result);
//         }
//         catch (Exception ex)
//         {
//             return StatusCode(500, new { message = "Error resending OTP", detail = ex.Message });
//         }
//     }
// }


// }
// }
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Circulation.User;
using library.Dtos.Common;

namespace library.Services.Interface
{
    public interface IUserService
    {

        Task<ToProfileDto?> GetProfileAsync(string userId);
        Task<ToProfileDto> UpdateProfileAsync(string userId, UpdateProfileDto dto);
        Task<bool> DeactivateAccountAsync(string userId);

        // Sessions
        Task<IEnumerable<ActiveSessionDto>> GetActiveSessionsAsync(string userId);
        Task RevokeSessionAsync(string userId, int refreshTokenId);

        // Admin
        Task<PagedResult<UserSummaryDto>> GetAllUsersAsync(UserQueryParams queryParams);
        Task<bool> DeactivateUserAsync(string adminId, string targetUserId);
        Task<bool> ActivateUserAsync(string adminId, string targetUserId);
        Task<bool> UpdateUserRolesAsync(string adminId, string targetUserId, List<string> roles);
        Task<IEnumerable<string>> GetUserRolesAsync(string userId);


        // // ===== Profile Management =====
        // Task<UserProfileDto?> GetProfileAsync(string userId);
        // Task<UserProfileDto> UpdateProfileAsync(string userId, UpdateProfileDto dto);
        // Task<UserProfileDto> UpdateProfilePictureAsync(string userId, IFormFile profilePicture);
        // Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto dto);
        // Task<bool> ChangeEmailAsync(string userId, ChangeEmailDto dto);
        // Task<bool> DeactivateAccountAsync(string userId);
        // Task<bool> ReactivateAccountAsync(string userId); // For reactivating deactivated accounts

        // // ===== Account Settings =====
        // Task<UserSettingsDto> GetUserSettingsAsync(string userId);
        // Task<UserSettingsDto> UpdateUserSettingsAsync(string userId, UpdateUserSettingsDto dto);

        // // ===== Session Management =====
        // Task<IEnumerable<ActiveSessionDto>> GetActiveSessionsAsync(string userId);
        // Task RevokeSessionAsync(string userId, int refreshTokenId);
        // Task RevokeAllSessionsExceptCurrentAsync(string userId, string currentRefreshToken);

        // // ===== Account Security =====
        // Task<IEnumerable<SecurityLogDto>> GetSecurityLogsAsync(string userId, SecurityLogQueryParams queryParams);
        // Task<bool> EnableTwoFactorAuthAsync(string userId);
        // Task<bool> DisableTwoFactorAuthAsync(string userId);
        // Task<bool> VerifyTwoFactorCodeAsync(string userId, string code);

        // // ===== User Statistics =====
        // Task<UserStatisticsDto> GetUserStatisticsAsync(string userId);

        // // ===== Admin Only =====
        // Task<PagedResult<UserSummaryDto>> GetAllUsersAsync(UserQueryParams queryParams);
        // Task<UserDetailDto?> GetUserDetailsForAdminAsync(string adminId, string targetUserId);
        // Task<bool> DeactivateUserAsync(string adminId, string targetUserId);
        // Task<bool> ActivateUserAsync(string adminId, string targetUserId);
        // Task<bool> DeleteUserPermanentlyAsync(string adminId, string targetUserId, string reason);
        // Task<bool> ImpersonateUserAsync(string adminId, string targetUserId);
        // Task<bool> StopImpersonationAsync(string adminId);
    }
}
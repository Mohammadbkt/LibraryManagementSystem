using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Data;
using library.Dtos.Circulation.User;
using library.Dtos.Common;
using library.Mappers;
using library.Models.Entities;
using library.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace library.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;

        public UserService(UserManager<User> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // ─── Profile ──────────────────────────────────────────────────────────
        public async Task<ToProfileDto?> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            var roles = await _userManager.GetRolesAsync(user);

            return user.ToProfileDto(roles);
        }

        public async Task<ToProfileDto> UpdateProfileAsync(string userId, UpdateProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new KeyNotFoundException("User not found");

            var roles = await _userManager.GetRolesAsync(user);

            if (dto.FirstName != null) user.FirstName = dto.FirstName;
            if (dto.LastName != null) user.LastName = dto.LastName;
            if (dto.PhoneNumber != null) user.PhoneNumber = dto.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    string.Join(", ", result.Errors.Select(e => e.Description)));

            return user.ToProfileDto(roles);
        }

        public async Task<bool> DeactivateAccountAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var hasActiveLoans = await _context.Loans.AnyAsync(l => l.UserId == userId && l.ReturnDate == null);
            if (hasActiveLoans)
                throw new InvalidOperationException("Cannot deactivate account with active loans");

            user.IsActive = false;
            await _userManager.UpdateAsync(user);

            var tokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
                .ToListAsync();

            tokens.ForEach(t => t.RevokedAt = DateTime.UtcNow);
            await _context.SaveChangesAsync();

            return true;
        }

        // ─── Sessions ─────────────────────────────────────────────────────────
        public async Task<IEnumerable<ActiveSessionDto>> GetActiveSessionsAsync(string userId)
        {
            var sessions = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
                .AsNoTracking()
                .ToListAsync();

            return sessions.Select(rt => rt.ToDto());
        }

        public async Task RevokeSessionAsync(string userId, int refreshTokenId)
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Id == refreshTokenId && rt.UserId == userId);

            if (token == null) throw new KeyNotFoundException("Session not found");
            if (token.RevokedAt != null) throw new InvalidOperationException("Session already revoked");

            token.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        // ─── Admin ────────────────────────────────────────────────────────────
        public async Task<PagedResult<UserSummaryDto>> GetAllUsersAsync(UserQueryParams queryParams)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryParams.Search))
                query = query.Where(u => u.Email!.Contains(queryParams.Search) ||
                                    u.FirstName!.Contains(queryParams.Search) || 
                                    u.LastName!.Contains(queryParams.Search));

            if (!string.IsNullOrWhiteSpace(queryParams.Role))
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(queryParams.Role);
                var userIds = usersInRole.Select(u => u.Id);
                query = query.Where(u => userIds.Contains(u.Id));
            }

            if (queryParams.IsActive.HasValue)
                query = query.Where(u => u.IsActive == queryParams.IsActive);

            var totalCount = await query.CountAsync();

            var users = await query
                .OrderBy(u => u.MemberSince)
                .Skip((queryParams.Page - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagedResult<UserSummaryDto>
            {
                Items = users.Select(u => u.ToSummaryDto()).ToList(),
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        }

        public async Task<bool> DeactivateUserAsync(string adminId, string targetUserId)
        {
            if (adminId == targetUserId)
                throw new InvalidOperationException("Admin cannot deactivate their own account");

            var user = await _userManager.FindByIdAsync(targetUserId);
            if (user == null) return false;

            user.IsActive = false;

            // Use UserManager to update - it has its own transaction
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Failed to deactivate user: {string.Join(", ", result.Errors)}");

            var tokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == targetUserId && rt.RevokedAt == null)
                .ToListAsync();

            if (tokens.Any())
            {
                tokens.ForEach(t => t.RevokedAt = DateTime.UtcNow);
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> ActivateUserAsync(string adminId, string targetUserId)
        {
            if (adminId == targetUserId)
                throw new InvalidOperationException("Admin cannot activate their own account");

            var user = await _userManager.FindByIdAsync(targetUserId);
            if (user == null) return false;

            user.IsActive = true;
            await _userManager.UpdateAsync(user);
            return true;
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new KeyNotFoundException();

            return await _userManager.GetRolesAsync(user);
        }

        public async Task<bool> UpdateUserRolesAsync(string adminId, string targetUserId, List<string> roles)
        {
            var admin = await _userManager.FindByIdAsync(adminId);
            if (admin == null)
                throw new KeyNotFoundException("Admin user not found");

            var user = await _userManager.FindByIdAsync(targetUserId);
            if (user == null) return false;

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRolesAsync(user, roles);

            return true;
        }
    }
}
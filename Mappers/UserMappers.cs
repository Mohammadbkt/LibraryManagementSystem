using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Circulation.User;
using library.Models.Entities;

namespace library.Mappers
{
    public static class UserMappers
    {
        public static ToProfileDto ToProfileDto(this User user, IEnumerable<string> roles)
        {
            return new ToProfileDto
            {
                Id = user.Id,
                Name = user.FullName,
                Email = user.Email ?? "",
                MemberSince = user.MemberSince,
                LastLoginDate = user.LastLoginDate,
                Role = roles,
                IsActive = user.IsActive
            };
        }

        public static UserSummaryDto ToSummaryDto(this User user)
        {
            return new UserSummaryDto
            {
                Id = user.Id,
                Name = user.FullName,
                Email = user.Email ?? "",
                MemberSince = user.MemberSince,
                IsActive = user.IsActive
            };
        }
    }
}
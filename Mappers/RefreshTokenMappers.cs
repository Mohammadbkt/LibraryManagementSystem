using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Circulation.User;
using library.Models.Entities;

namespace library.Mappers
{
    public static class RefreshTokenMappers
    {
        public static ActiveSessionDto ToDto(this RefreshToken token)
        {
            return new ActiveSessionDto
            {
                Id = token.Id,
                DeviceName = token.DeviceName ?? "Unknown Device",
                IpAddress = token.IpAddress ?? "Unknown IP",
                ExpiresAt = token.ExpiresAt,
                CreatedAt = token.CreatedAt
            };
        }
    }
}
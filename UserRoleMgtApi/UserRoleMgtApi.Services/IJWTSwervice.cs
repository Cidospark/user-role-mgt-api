using System;
using System.Collections.Generic;
using UserRoleMgtApi.Models;

namespace UserRoleMgtApi.Services
{
    public interface IJWTSwervice
    {
        public string GenerateToken(User user, List<string> userRoles);
    }
}

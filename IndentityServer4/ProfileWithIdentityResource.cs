using IdentityModel;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IndentityServer4
{
    public class ProfileWithIdentityResource : IdentityResources.Profile
    {
        public ProfileWithIdentityResource()
        {
            this.UserClaims.Add(JwtClaimTypes.Role);
        }
    }
}

using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Test;
using IndentityServer4.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IndentityServer4
{
	public class ProfileService : IProfileService
	{
		protected readonly Microsoft.Extensions.Logging.ILogger Logger;

		protected readonly TestUserStore Users;

		public ProfileService(TestUserStore users, ILogger<TestUserProfileService> logger)
		{
			Users = users;
			Logger = logger;
		}

		public virtual Task GetProfileDataAsync(ProfileDataRequestContext context)
		{
			context.LogProfileRequest(Logger);

			if (context.RequestedClaimTypes.Any())
			{
				var user = Users.FindBySubjectId(context.Subject.GetSubjectId());
				if (user != null)
				{

					context.AddRequestedClaims(user.Claims);
				}
			}

			context.LogIssuedClaims(Logger);

			return Task.CompletedTask;
		}


		public virtual Task IsActiveAsync(IsActiveContext context)
		{
			Logger.LogDebug("IsActive called from: {caller}", context.Caller);

			var user = Users.FindBySubjectId(context.Subject.GetSubjectId());
			context.IsActive = user?.IsActive == true;

			return Task.CompletedTask;
		}
	}

}

using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IndentityServer4
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {


        /// <summary>/// Custom resource owner password validator/// </summary>public class CustomResourceOwnerPasswordValidator: IResourceOwnerPasswordValidator{    /// <summary>
        ///  Here to demonstrate that we still use TestUser as the data source,
        ///  For normal use, you should pass in a user repository, etc.
        ///  Database or other media to obtain the object of our user data
        /// </summary>
        private readonly TestUserStore _users; 
        private readonly ISystemClock _clock; 
        public ResourceOwnerPasswordValidator(TestUserStore users, ISystemClock clock)
        {
            _users = users;
            _clock = clock;
        }    /// <summary>
             ///  verification
             /// </summary>
             /// <param name="context"></param>
             /// <returns></returns>
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {        //The user name and password of context.UserName, context.Password are used here to verify with database data
            if (_users.ValidateCredentials(context.UserName, context.Password))
            {
                var user = _users.FindByUsername(context.UserName);            //Return result after verification 
                                                                               //subjectId is the unique identifier of the user, generally the user id
                                                                               //authenticationMethod describes the authentication method of a custom authorization type 
                                                                               //authTime authorization time
                                                                               //claims the user identity information unit that needs to be returned. Here we should add claims based on the user information we read from the database. If the role information is read from the database, then we should add it here. Only the necessary claims should be returned here.
                context.Result = new GrantValidationResult(
                    user.SubjectId ?? throw new ArgumentException("Subject ID not set", nameof(user.SubjectId)),
                    OidcConstants.AuthenticationMethods.Password, _clock.UtcNow.UtcDateTime,
                    user.Claims);
            }
            else
            {            //verification failed
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid custom credential");
            }
            return Task.CompletedTask;
        }

    }
}

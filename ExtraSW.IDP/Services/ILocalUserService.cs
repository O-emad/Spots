using ExtraSW.IDP.Entities;
using Spots.Services.Helpers;
using Spots.Services.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Marvin.IDP.Services
{
    public interface ILocalUserService
    {
        bool UserExists(Guid id);
        Task<User> GetUserByIdAsync(Guid id);
        PagedList<User> GetUsers(IndexResourceParameters userParameters);
        Task<bool> ValidateClearTextCredentialsAsync(
            string userName, 
            string password);
        //Task<bool> ValidateCredentialsAsync(
        //    string userName,
        //    string password);
        Task<bool> ValidateUsernameAndPassword(string userName, string password);
        Task<bool> ValidateUserActive(string userName);
        Task<IEnumerable<UserClaim>> GetUserClaimsBySubjectAsync(
            string subject);         
        Task<User> GetUserByUserNameAsync(
            string userName);
        Task<User> GetUserBySubjectAsync(
            string subject);        
        void AddUser
            (User userToAdd);    
        //void AddUser(
        //    User userToAdd, 
        //    string password); 
        Task<bool> IsUserActive(
            string subject);

        Task<string> LoginExternalUser(User user);
        Task<User> GetOrCreateExternalLoginUser(string provider, string key, string email, string firstName, string lastName);
        //Task<bool> ActivateUser(
        //    string securityCode);
        Task<bool> SaveChangesAsync();
        //Task<string> InitiatePasswordResetRequest(
        //    string email);
        //Task<bool> SetPassword(
        //    string securityCode, 
        //    string password);
        //Task<User> GetUserByExternalProvider(
        //    string provider,
        //    string providerIdentityKey);
        //User ProvisionUserFromExternalIdentity(
        //    string provider,
        //    string providerIdentityKey, 
        //    IEnumerable<Claim> claims);
        //Task AddExternalProviderToUser(
        //    string subject, 
        //    string provider,
        //    string providerIdentityKey);
        //Task<bool> AddUserSecret(
        //    string subject, 
        //    string name, 
        //    string secret);
        //Task<UserSecret> GetUserSecret(
        //    string subject, 
        //    string name);
        //Task<bool> UserHasRegisteredTotpSecret(
        //    string subject);
    }
}

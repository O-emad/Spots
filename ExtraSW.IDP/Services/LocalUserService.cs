using ExtraSW.IDP.DbContexts;
using ExtraSW.IDP.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Spots.Services.Helpers;
using Spots.Services.ResourceParameters;
using System.Text;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Marvin.IDP.Services
{
    public class LocalUserService : ILocalUserService
    {
        private readonly IdentityDbContext _context;
        private readonly IHttpContextAccessor httpContextAccessor;

        public LocalUserService(
            IdentityDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? 
                throw new ArgumentNullException(nameof(context));
            this.httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public PagedList<User> GetUsers(IndexResourceParameters userParameters)
        {
            if (userParameters == null)
            {
                throw new ArgumentNullException(nameof(userParameters));
            }

            var collection = _context.Users as IQueryable<User>;

            #region Filtering
            if (!string.IsNullOrWhiteSpace(userParameters.FilterQuery))
            {
            }
            #endregion

            #region Searching
            if (!string.IsNullOrWhiteSpace(userParameters.SearchQuery))
            {
                var searchQuery = userParameters.SearchQuery.Trim();
                collection = collection.Where(c => c.UserName.Contains(searchQuery));
            }

            #endregion

            collection = (IQueryable<User>)collection.OrderBy(c => c.Active)
                .Include(u => u.Claims);

            return PagedList<User>.Create(collection, userParameters.PageNumber
                , userParameters.PageSize, userParameters.IncludeAll);
        }

        public async Task<bool> IsUserActive(string subject)
        {
            if (string.IsNullOrWhiteSpace(subject))
            {
                return false;
            }

            var user = await GetUserBySubjectAsync(subject);

            if (user == null)
            {
                return false;
            }

            return user.Active;
        }
         
        public async Task<bool> ValidateClearTextCredentialsAsync(string userName,
          string password)
        {
            if (string.IsNullOrWhiteSpace(userName) ||
                string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            var user = await GetUserByUserNameAsync(userName);

            if (user == null)
            {
                return false;
            }

            if (!user.Active)
            {
                return false;
            }

            // Validate credentials
            return (user.Password == password);
        }

        public async Task<bool> ValidateUsernameAndPassword(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName) ||
                string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            var user = await GetUserByUserNameAsync(userName);
            if (user == null)
            {
                return false;
            }
            // Validate credentials
            return (user.Password == password);
        }

        public async Task<bool> ValidateUserActive(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return false;
            }

            var user = await GetUserByUserNameAsync(userName);
            if (user == null)
            {
                return false;
            }
            // Validate credentials
            return (user.Active);
        }

        //public async Task<bool> ValidateCredentialsAsync(string userName, 
        //    string password)
        //{
        //    if (string.IsNullOrWhiteSpace(userName) || 
        //        string.IsNullOrWhiteSpace(password))
        //    {
        //        return false;
        //    }

        //    var user = await GetUserByUserNameAsync(userName);

        //    if (user == null)
        //    {
        //        return false;
        //    }

        //    if (!user.Active)
        //    {
        //        return false;
        //    }

        //    // Validate credentials
        //    var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, password);        
        //    return (verificationResult == PasswordVerificationResult.Success);
        //}

        public async Task<User> GetUserByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }

            return await _context.Users
                 .FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException(nameof(id));
            }

            return await _context.Users
                 .Include(u=>u.Claims)
                 .FirstOrDefaultAsync(u => u.Id == id);
        }

        public bool UserExists(Guid id)
        {
            return _context.Users.Any(u => u.Id == id);
        }

        public async Task<IEnumerable<UserClaim>> GetUserClaimsBySubjectAsync(string subject)
        { 
            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentNullException(nameof(subject));
            }

            return await _context.UserClaims.Where(u => u.User.Subject == subject).ToListAsync(); 
        }

        public IEnumerable<UserClaim> GetUserClaimsByUserId(Guid userId)
        {
            return _context.UserClaims.Where(u => u.UserId == userId).ToList();
        }
         
        public async Task<User> GetUserBySubjectAsync(string subject)
        {
            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentNullException(nameof(subject));
            }

            return await _context.Users.FirstOrDefaultAsync(u => u.Subject == subject);
        }
     
        public void AddUser(User userToAdd)
        {
            if (userToAdd == null)
            { 
                throw new ArgumentNullException(nameof(userToAdd));
            }

            if (_context.Users.Any(u => u.UserName == userToAdd.UserName))
            {
                // in a real-life scenario you'll probably want to 
                // return this as a validation issue
                throw new Exception("Username must be unique");
            }
            
            _context.Users.Add(userToAdd);
        }

        public async Task<User> GetOrCreateExternalLoginUser(string provider, string key,
            string email, string firstName, string lastName)
        {
            //var user = await GetUserBySubjectAsync(key);
            //if (user != null)
            //    return user;
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }
            var user = await GetUserByUserNameAsync(email);

            if (user != null)
            {
                return user;
            }
            else
            {
                user = new User
                {
                    Subject = Guid.NewGuid().ToString(),
                    UserName = email,
                    Password = RandomPassword(),
                    Active = true
                };
                user.Claims.Add(new UserClaim()
                {
                    Type = "role",
                    Value = "User"
                });
                user.Claims.Add(new UserClaim()
                {
                    Type = "given_name",
                    Value = firstName
                });
                user.Claims.Add(new UserClaim()
                {
                    Type = "family_name",
                    Value = lastName
                });

                AddUser(user);
                await SaveChangesAsync();
                //user = await GetUserBySubjectAsync(user.Subject);
                return user;
            }

            

            //var info = new UserLoginInfo(provider, key, provider.ToUpperInvariant());
            //var result = await _userManager.AddLoginAsync(user, info);
            //if (result.Succeeded)
            //    return user;

        }

        public async Task<string> LoginExternalUser(User user)
        {
            var isuser = new IdentityServerUser(user.Subject)
            {
                DisplayName = user.UserName
            };
            var props = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.Add(new TimeSpan(7, 0, 0, 0, 0))
            };

            var httpContext = httpContextAccessor.HttpContext;
            await httpContext.SignInAsync(isuser, props);

            var accessToken = await httpContextAccessor.HttpContext
                .GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            return accessToken;
        }

        public int RandomNumber(int min, int max)
        {
            return new Random().Next(min,max);
        }

        public string RandomString(int size, bool lowerCase = false)
        {
            var builder = new StringBuilder(size);

            // Unicode/ASCII Letters are divided into two blocks
            // (Letters 65–90 / 97–122):
            // The first group containing the uppercase letters and
            // the second group containing the lowercase.  

            // char is a single Unicode character  
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length=26  

            for (var i = 0; i < size; i++)
            {
                var @char = (char) new Random().Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }
        public string RandomPassword()
        {
            var passwordBuilder = new StringBuilder();

            // 4-Letters lower case   
            passwordBuilder.Append(RandomString(4, true));

            // 4-Digits between 1000 and 9999  
            passwordBuilder.Append(RandomNumber(1000, 9999));

            // 2-Letters upper case  
            passwordBuilder.Append(RandomString(2));
            return passwordBuilder.ToString();
        }


        //public void AddUser(User userToAdd, string password)
        //{
        //    if (userToAdd == null)
        //    {
        //        throw new ArgumentNullException(nameof(userToAdd));
        //    }

        //    if (string.IsNullOrWhiteSpace(password))
        //    {
        //        throw new ArgumentNullException(nameof(password));
        //    }

        //    if (_context.Users.Any(u => u.UserName == userToAdd.UserName))
        //    {
        //        // in a real-life scenario you'll probably want to 
        //        // return this a a validation issue
        //        throw new Exception("Username must be unique");
        //    }

        //    if (_context.Users.Any(u => u.Email == userToAdd.Email))
        //    {
        //        // in a real-life scenario you'll probably want to 
        //        // return this a a validation issue
        //        throw new Exception("Email must be unique");
        //    }

        //    // hash & salt the password
        //    userToAdd.Password = _passwordHasher.HashPassword(userToAdd, password);

        //    using (var randomNumberGenerator = new RNGCryptoServiceProvider())
        //    {
        //        var securityCodeData = new byte[128];
        //        randomNumberGenerator.GetBytes(securityCodeData);
        //        userToAdd.SecurityCode = Convert.ToBase64String(securityCodeData);
        //    }
        //    userToAdd.SecurityCodeExpirationDate = DateTime.UtcNow.AddHours(1);
        //    _context.Users.Add(userToAdd);
        //}

        //public async Task<bool> ActivateUser(string securityCode)
        //{
        //    if (string.IsNullOrWhiteSpace(securityCode))
        //    {
        //        throw new ArgumentNullException(nameof(securityCode));
        //    }

        //    // find an user with this security code as an active security code.  
        //    var user = await _context.Users.FirstOrDefaultAsync(u => 
        //        u.SecurityCode == securityCode && 
        //        u.SecurityCodeExpirationDate >= DateTime.UtcNow);

        //    if (user == null)
        //    {
        //        return false;
        //    }

        //    user.Active = true;
        //    user.SecurityCode = null;
        //    return true;
        //}

        //public async Task<bool> AddUserSecret(string subject, string name, string secret)
        //{
        //    if (string.IsNullOrWhiteSpace(subject))
        //    {
        //        throw new ArgumentNullException(nameof(subject));
        //    }

        //    if (string.IsNullOrWhiteSpace(name))
        //    {
        //        throw new ArgumentNullException(nameof(name));
        //    }

        //    if (string.IsNullOrWhiteSpace(secret))
        //    {
        //        throw new ArgumentNullException(nameof(secret));
        //    }

        //    var user = await GetUserBySubjectAsync(subject); 

        //    if (user == null)
        //    {
        //        return false;
        //    }

        //    user.Secrets.Add(new UserSecret() { Name = name, Secret = secret });             
        //    return true;
        //}

        //public async Task<bool> UserHasRegisteredTotpSecret(string subject)
        //{
        //    if (string.IsNullOrWhiteSpace(subject))
        //    {
        //        throw new ArgumentNullException(nameof(subject));
        //    }

        //    return await _context.UserSecrets.AnyAsync(u => u.User.Subject == subject && u.Name == "TOTP");
        //}

        //public async Task<UserSecret> GetUserSecret(string subject, string name)
        //{
        //    if (string.IsNullOrWhiteSpace(subject))
        //    {
        //        throw new ArgumentNullException(nameof(subject));
        //    }

        //    if (string.IsNullOrWhiteSpace(name))
        //    {
        //        throw new ArgumentNullException(nameof(name));
        //    }

        //    return await _context.UserSecrets
        //        .FirstOrDefaultAsync(u => u.User.Subject == subject && u.Name == name);
        //}

        //public async Task<string> InitiatePasswordResetRequest(string email)
        //{
        //    if (string.IsNullOrWhiteSpace(email))
        //    {
        //        throw new ArgumentNullException(nameof(email));
        //    }

        //    var user = await _context.Users.FirstOrDefaultAsync(u =>
        //      u.Email == email);

        //    if (user == null)
        //    {
        //        throw new Exception($"User with email address {email} can't be found.");
        //    }

        //    using (var randomNumberGenerator = new RNGCryptoServiceProvider())
        //    {
        //        var securityCodeData = new byte[128];
        //        randomNumberGenerator.GetBytes(securityCodeData);
        //        user.SecurityCode = Convert.ToBase64String(securityCodeData);
        //    }

        //    user.SecurityCodeExpirationDate = DateTime.UtcNow.AddHours(1);
        //    return user.SecurityCode;
        //}

        //public async Task<bool> SetPassword(string securityCode, string password)
        //{
        //    if (string.IsNullOrWhiteSpace(securityCode))
        //    {
        //        throw new ArgumentNullException(nameof(securityCode));
        //    }

        //    if (string.IsNullOrWhiteSpace(password))
        //    {
        //        throw new ArgumentNullException(nameof(password));
        //    }

        //    var user = await _context.Users.FirstOrDefaultAsync(u =>
        //    u.SecurityCode == securityCode &&
        //    u.SecurityCodeExpirationDate >= DateTime.UtcNow);

        //    if (user == null)
        //    {
        //        return false;
        //    }

        //    user.SecurityCode = null;
        //    // hash & salt the password
        //    user.Password = _passwordHasher.HashPassword(user, password);
        //    return true;        
        //}

        //public async Task<User> GetUserByExternalProvider(
        //    string provider, 
        //    string providerIdentityKey)
        //{
        //    if (string.IsNullOrWhiteSpace(provider))
        //    {
        //        throw new ArgumentNullException(nameof(provider));
        //    }

        //    if (string.IsNullOrWhiteSpace(providerIdentityKey))
        //    {
        //        throw new ArgumentNullException(nameof(providerIdentityKey));
        //    }

        //    var userLogin = await _context.UserLogins.Include(ul => ul.User)
        //        .FirstOrDefaultAsync(ul => ul.Provider == provider && ul.ProviderIdentityKey == providerIdentityKey);

        //    return userLogin?.User;
        //}

        //public async Task AddExternalProviderToUser(
        //    string subject,
        //    string provider,
        //    string providerIdentityKey)
        //{
        //    if (string.IsNullOrWhiteSpace(subject))
        //    {
        //        throw new ArgumentNullException(nameof(subject));
        //    }

        //    if (string.IsNullOrWhiteSpace(provider))
        //    {
        //        throw new ArgumentNullException(nameof(provider));
        //    }

        //    if (string.IsNullOrWhiteSpace(providerIdentityKey))
        //    {
        //        throw new ArgumentNullException(nameof(providerIdentityKey));
        //    }

        //    var user = await GetUserBySubjectAsync(subject);
        //    user.Logins.Add(new UserLogin()
        //    {
        //        Provider = provider,
        //        ProviderIdentityKey = providerIdentityKey
        //    });            
        //}

        //public User ProvisionUserFromExternalIdentity(
        //    string provider, 
        //    string providerIdentityKey,
        //    IEnumerable<Claim> claims)
        //{
        //    if (string.IsNullOrWhiteSpace(provider))
        //    {
        //        throw new ArgumentNullException(nameof(provider));
        //    }

        //    if (string.IsNullOrWhiteSpace(providerIdentityKey))
        //    {
        //        throw new ArgumentNullException(nameof(providerIdentityKey));
        //    }

        //    var user = new User()
        //    {
        //        Active = true,
        //        Subject = Guid.NewGuid().ToString()
        //    };
        //    foreach (var claim in claims)
        //    {
        //        user.Claims.Add(new UserClaim()
        //        {
        //            Type = claim.Type,
        //            Value = claim.Value
        //        });
        //    }
        //    user.Logins.Add(new UserLogin()
        //    {
        //         Provider = provider,
        //         ProviderIdentityKey = providerIdentityKey
        //    });

        //    _context.Users.Add(user);

        //    return user;
        //}



        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() > 0);
        }      
    }
}

using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Bondii.Identity.MySQL
{

    /// <summary>
    /// Class that implements the key ASP.NET Identity user store iterfaces
    /// </summary>
    public class UserStore<TUser> : 
        IUserLoginStore<TUser>,
        IUserClaimStore<TUser>,
        IUserRoleStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IQueryableUserStore<TUser>,
        IUserEmailStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IUserTwoFactorStore<TUser>,
        IUserLockoutStore<TUser>,
        IUserStore<TUser>
        where TUser : IdentityUser
    {
        private UserTable<TUser> userTable;
        private RoleTable roleTable;
        private UserRolesTable userRolesTable;
        private UserClaimsTable<TUser> userClaimsTable;
        private UserLoginsTable userLoginsTable;
        public MySQLDatabase Database { get; private set; }


        /// <summary>
        /// Default constructor that initializes a new MySQLDatabase
        /// instance using the Default Connection string
        /// </summary>
        public UserStore()
        {
            new UserStore<TUser>(new MySQLDatabase());
        }

        /// <summary>
        /// Constructor that takes a MySQLDatabase as argument 
        /// </summary>
        /// <param name="database"></param>
        public UserStore(MySQLDatabase database)
        {
            Database = database;
            userTable = new UserTable<TUser>(database);
            roleTable = new RoleTable(database);
            userRolesTable = new UserRolesTable(database);
            userClaimsTable = new UserClaimsTable<TUser>(database);
            userLoginsTable = new UserLoginsTable(database);
        }


        #region "IUserLoginStore"

        /// <summary>
        /// Inserts a Login in the UserLoginsTable for a given User
        /// </summary>
        /// <param name="user">User to have login added</param>
        /// <param name="login">Login to be added</param>
        /// <returns></returns>
        public Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            userLoginsTable.Insert(user, login);

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Returns an TUser based on the Login info
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (loginProvider == null || providerKey == null)
            {
                throw new ArgumentNullException("login");
            }

            var userId = userLoginsTable.FindUserIdByLogin(loginProvider, providerKey);
            if (userId != null)
            {
                TUser user = userTable.GetUserById(userId) as TUser;
                if (user != null)
                {
                    return Task.FromResult<TUser>(user);
                }
            }

            return Task.FromResult<TUser>(null);
        }

        /// <summary>
        /// Returns list of UserLoginInfo for a given TUser
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            // List<UserLoginInfo> userLogins = new List<UserLoginInfo>();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            List<UserLoginInfo> logins = userLoginsTable.FindByUserId(user.Id);
            if (logins != null)
            {
                return Task.FromResult<IList<UserLoginInfo>>(logins);
            }

            return Task.FromResult<IList<UserLoginInfo>>(null);
        }

        /// <summary>
        /// Deletes a login from UserLoginsTable for a given TUser
        /// </summary>
        /// <param name="user">User to have login removed</param>
        /// <param name="login">Login to be removed</param>
        /// <returns></returns>
        public Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (loginProvider == null || providerKey == null)
            {
                throw new ArgumentNullException("login");
            }

            userLoginsTable.Delete(user, loginProvider, providerKey);

            return Task.FromResult<Object>(null);
        }

        #endregion 

        #region "IUserClaimStore"

        /// <summary>
        /// Inserts a claim to the UserClaimsTable for the given user
        /// </summary>
        /// <param name="user">User to have claim added</param>
        /// <param name="claim">Claim to be added</param>
        /// <returns></returns>
        public Task AddClaimsAsync(TUser user, IEnumerable<Claim> claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            userClaimsTable.Insert(claim, user.Id);

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Returns all claims for a given user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            ClaimsIdentity identity = userClaimsTable.FindByUserId(user.Id);

            return Task.FromResult<IList<Claim>>(identity.Claims.ToList());
        }

        /// <summary>
        /// Get users having a specific claim
        /// </summary>
        /// <param name="claim"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {

            // Not Implemented. Break.

            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            List<TUser> users = userClaimsTable.GetUsersForClaim(claim);
            
            

            if (users != null)
            {
                return Task.FromResult<IList<TUser>>(users);
            }

            return Task.FromResult<IList<TUser>>(null);
        }

        /// <summary>
        /// Removes a claim froma user
        /// </summary>
        /// <param name="user">User to have claim removed</param>
        /// <param name="claim">Claim to be removed</param>
        /// <returns></returns>
        public Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            userClaimsTable.Delete(user, claim);

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Updates the give claim information with the given new claim information
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claim"></param>
        /// <param name="newClaim"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default(CancellationToken))
        {

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            if (newClaim == null)
            {
                throw new ArgumentNullException("claim");
            }

            userClaimsTable.Update(user, claim, newClaim);

            return Task.FromResult<object>(null);
        }

        #endregion

        #region "IUserRoleStore"

        /// <summary>
        /// Inserts a entry in the UserRoles table
        /// </summary>
        /// <param name="user">User to have role added</param>
        /// <param name="roleName">Name of the role to be added to user</param>
        /// <returns></returns>
        public Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("Argument cannot be null or empty: roleName.");
            }

            string roleId = roleTable.GetRoleId(roleName);
            if (!string.IsNullOrEmpty(roleId))
            {
                userRolesTable.Insert(user, roleId);
            }

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Returns the roles for a given TUser
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            List<string> roles = userRolesTable.FindByUserId(user.Id);
            {
                if (roles != null)
                {
                    return Task.FromResult<IList<string>>(roles);
                }
            }

            return Task.FromResult<IList<string>>(null);
        }

        public Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken = default(CancellationToken))
        {

            // TODO: Implement
            return Task.FromResult<IList<TUser>>(null);

        }

        /// <summary>
        /// Verifies if a user is in a role
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public Task<bool> IsInRoleAsync(TUser user, string role, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrEmpty(role))
            {
                throw new ArgumentNullException("role");
            }

            List<string> roles = userRolesTable.FindByUserId(user.Id);
            {
                if (roles != null && roles.Contains(role))
                {
                    return Task.FromResult<bool>(true);
                }
            }

            return Task.FromResult<bool>(false);
        }

        /// <summary>
        /// Removes a user from a role
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public Task RemoveFromRoleAsync(TUser user, string role, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        #endregion

        #region "IUserPasswordStore"

        /// <summary>
        /// Returns the PasswordHash for a given TUser
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            string passwordHash = userTable.GetPasswordHash(user.Id);

            return Task.FromResult<string>(passwordHash);
        }

        /// <summary>
        /// Verifies if user has password
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            var hasPassword = !string.IsNullOrEmpty(userTable.GetPasswordHash(user.Id));

            return Task.FromResult<bool>(Boolean.Parse(hasPassword.ToString()));
        }

        /// <summary>
        /// Sets the password hash for a given TUser
        /// </summary>
        /// <param name="user"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken = default(CancellationToken))
        {
            user.PasswordHash = passwordHash;

            return Task.FromResult<Object>(null);
        }

        #endregion

        #region "IUserSecurityStampStore"

        /// <summary>
        /// Get security stamp
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(user.SecurityStamp);
        }

        /// <summary>
        ///  Set security stamp
        /// </summary>
        /// <param name="user"></param>
        /// <param name="stamp"></param>
        /// <returns></returns>
        public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken = default(CancellationToken))
        {
            user.SecurityStamp = stamp;

            return Task.FromResult(0);

        }

        #endregion

        #region "IQueryableUserStore"

        public IQueryable<TUser> Users
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region "IUserEmailStore"

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<TUser> FindByEmailAsync(string email, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }

            TUser result = userTable.GetUserByEmail(email) as TUser;
            if (result != null)
            {
                return Task.FromResult<TUser>(result);
            }

            return Task.FromResult<TUser>(null);
        }

        /// <summary>
        /// Get email from user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(user.Email);
        }

        /// <summary>
        /// Get if user email is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        /// <summary>
        /// Returns the normalized email.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {

            //TODO: Implement and return correct string.
            return Task.FromResult<string>(null);

        }

        /// <summary>
        /// Set email on user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken = default(CancellationToken))
        {
            user.Email = email;
            userTable.Update(user);

            return Task.FromResult(0);

        }

        /// <summary>
        /// Set when user email is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <param name="confirmed"></param>
        /// <returns></returns>
        public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken = default(CancellationToken))
        {
            user.EmailConfirmed = confirmed;
            userTable.Update(user);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Set the normalized email.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="normalizedEmail"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult<object>(null);
        }


        #endregion

        #region "IUserPhoneNumberStore"

        /// <summary>
        /// Set user phone number
        /// </summary>
        /// <param name="user"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            user.PhoneNumber = phoneNumber;
            userTable.Update(user);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Get user phone number
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(user.PhoneNumber);
        }

        /// <summary>
        /// Get if user phone number is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        /// <summary>
        /// Set phone number if confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <param name="confirmed"></param>
        /// <returns></returns>
        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken = default(CancellationToken))
        {
            user.PhoneNumberConfirmed = confirmed;
            userTable.Update(user);

            return Task.FromResult(0);
        }

        #endregion "IUserPhoneNumberStore"

        #region "IUserTwoFactorStore"

        /// <summary>
        /// Set two factor authentication is enabled on the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken = default(CancellationToken))
        {
            user.TwoFactorEnabled = enabled;
            userTable.Update(user);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Get if two factor authentication is enabled on the user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        #endregion

        #region "IUserLockoutStore"

        /// <summary>
        /// Get failed access count
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary>
        /// Get if lockout is enabled for the user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        /// <summary>
        /// Get user lock out end date
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null) throw new ArgumentNullException("user");
            return Task.FromResult(user.LockoutEndDateUtc);

        }

        /// <summary>
        /// Increment failed access count
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            user.AccessFailedCount++;
            userTable.Update(user);

            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary>
        /// Reset failed access count
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            user.AccessFailedCount = 0;
            userTable.Update(user);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Set lockout enabled for user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken = default(CancellationToken))
        {
            user.LockoutEnabled = enabled;
            userTable.Update(user);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Set user lockout end date
        /// </summary>
        /// <param name="user"></param>
        /// <param name="lockoutEnd"></param>
        /// <returns></returns>
        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken = default(CancellationToken))
        {

            //TODO: Check DateTimeOffset is in UTC
            user.LockoutEndDateUtc = lockoutEnd.Value;
            userTable.Update(user);

            return Task.FromResult(0);
        }

        #endregion

        #region "IUserStore"

        /// <summary>
        /// Insert a new TUser in the UserTable
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            userTable.Insert(user);

            // TODO: Correctly return IdentityResult

            return Task.FromResult<IdentityResult>(null);
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user != null)
            {
                userTable.Delete(user);
            }

            // TODO: Correctly return IdentityResult

            return Task.FromResult<IdentityResult>(null);
        }

        /// <summary>
        /// Returns an TUser instance based on a userId query 
        /// </summary>
        /// <param name="userId">The user's Id</param>
        /// <returns></returns>
        public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("Null or empty argument: userId");
            }

            TUser result = userTable.GetUserById(userId) as TUser;
            if (result != null)
            {
                return Task.FromResult<TUser>(result);
            }

            return Task.FromResult<TUser>(null);
        }

        /// <summary>
        /// Returns an TUser instance based on a userName query 
        /// </summary>
        /// <param name="userName">The user's name</param>
        /// <returns></returns>
        public Task<TUser> FindByNameAsync(string userName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("Null or empty argument: userName");
            }

            List<TUser> result = userTable.GetUserByName(userName) as List<TUser>;

            // Should I throw if > 1 user?
            if (result != null && result.Count == 1)
            {
                return Task.FromResult<TUser>(result[0]);
            }

            return Task.FromResult<TUser>(null);
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult<string>(null);
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult<string>(null);
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult<string>(null);
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult<string>(null);
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult<object>(null);
        }

        public Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult<IdentityResult>(null);
        }

        /// <summary>
        /// Updates the UsersTable with the TUser instance values
        /// </summary>
        /// <param name="user">TUser to be updated</param>
        /// <returns></returns>
        public Task UpdateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            userTable.Update(user);

            return Task.FromResult<object>(null);
        }

        #endregion


        public void Dispose()
        {
            if (Database != null)
            {
                Database.Dispose();
                Database = null;
            }
        }

    }

}

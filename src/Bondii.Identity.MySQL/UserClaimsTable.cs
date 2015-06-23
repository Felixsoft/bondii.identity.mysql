using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Security.Claims;

// TODO: Make any queries that iterate using a foreach loop on IEnumberable to only use one .Execute statement.

namespace Bondii.Identity.MySQL
{
    /// <summary>
    /// Class that represents the UserClaims table in the MySQL Database
    /// </summary>
    public class UserClaimsTable<TUser>
        where TUser : IdentityUser
       
    {
        private MySQLDatabase _database;

        /// <summary>
        /// Constructor that takes a MySQLDatabase instance 
        /// </summary>
        /// <param name="database"></param>
        public UserClaimsTable(MySQLDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Returns a ClaimsIdentity instance given a userId
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public ClaimsIdentity FindByUserId(string userId)
        {
            ClaimsIdentity claims = new ClaimsIdentity();
            string commandText = "Select * from UserClaims where UserId = @userId";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@UserId", userId } };

            var rows = _database.Query(commandText, parameters);
            foreach (var row in rows)
            {
                Claim claim = new Claim(row["ClaimType"], row["ClaimValue"]);
                claims.AddClaim(claim);
            }

            return claims;
        }


        public List<TUser> GetUsersForClaim(Claim claim)
        {

            // TODO: Not implemented. Break.
            // TODO: The calling method requires a TUser, is this the IdentityUser I have specified?
            List<TUser> users = new List<TUser>();

            //foreach (var row in rows)
            //{
            //    var user = new TUser();
            //    users.Add(user);
            //}
            //    return users;

            return users;
        }

        /// <summary>
        /// Deletes all claims from a user given a userId
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public int Delete(string userId)
        {
            string commandText = "Delete from UserClaims where UserId = @userId";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("userId", userId);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Inserts a new claim in UserClaims table
        /// </summary>
        /// <param name="userClaim">User's claim to be added</param>
        /// <param name="userId">User's id</param>
        /// <returns></returns>
        public void Insert(IEnumerable<Claim> userClaim, string userId)
        {
            string commandText = "Insert into UserClaims (ClaimValue, ClaimType, UserId) values (@value, @type, @userId)";
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            foreach (var Claim in userClaim)
            {
                parameters.Clear();
                parameters.Add("value", Claim.Value);
                parameters.Add("type", Claim.Type);
                parameters.Add("userId", userId);
                _database.Execute(commandText, parameters);
            }

            
        }

        public int Update(IdentityUser user, Claim claim, Claim newClaim)
        {
            // TODO: Validate this works.

            string commandText = "Update UserClaims SET UserId = @userId, ClaimValue = @newClaimValue, ClaimType = @newClaimType WHERE UserId = @userId, ClaimValue = @oldClaimValue, ClaimType = @oldClaimType";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("userId", user.Id);
            parameters.Add("newClaimValue", newClaim.Value);
            parameters.Add("newClaimType", newClaim.Type);
            parameters.Add("oldClaimValue", claim.Value);
            parameters.Add("oldClaimType", claim.Type);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Deletes a claim from a user 
        /// </summary>
        /// <param name="user">The user to have a claim deleted</param>
        /// <param name="claim">A claim to be deleted from user</param>
        /// <returns></returns>
        public void Delete(IdentityUser user, IEnumerable<Claim> claim)
        {
            string commandText = "Delete from UserClaims where UserId = @userId and ClaimValue = @value and ClaimType = @type";
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            foreach (var Claim in claim)
            {
                parameters.Clear();
                parameters.Add("userId", user.Id);
                parameters.Add("value", Claim.Value);
                parameters.Add("type", Claim.Type);
                _database.Execute(commandText, parameters);
            }

          
        }
    }
}

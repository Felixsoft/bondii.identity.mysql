using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
//TODO: Correctly return <IdentityResult> where specified - instead of null object?
//TODO: Does this class need to completely re-implement all methods? 
namespace Bondii.Identity.MySQL
{
    /// <summary>
    /// Class that implements the key ASP.NET Identity role store iterfaces
    /// </summary>
    public class RoleStore<TRole> : 
        IQueryableRoleStore<TRole>
        where TRole : IdentityRole
    {
        private RoleTable roleTable;
        public MySQLDatabase Database { get; private set; }

        public IQueryable<TRole> Roles
        {
            get
            {
                throw new NotImplementedException();
            }
        }


        /// <summary>
        /// Default constructor that initializes a new MySQLDatabase
        /// instance using the Default Connection string
        /// </summary>
        public RoleStore()
        {
            new RoleStore<TRole>(new MySQLDatabase());
        }

        /// <summary>
        /// Constructor that takes a MySQLDatabase as argument 
        /// </summary>
        /// <param name="database"></param>
        public RoleStore(MySQLDatabase database)
        {
            Database = database;
            roleTable = new RoleTable(database);
        }


        #region "IQueryableRoleStore"


        /// <summary>
        /// Inserts a new role.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            roleTable.Insert(role);
            //TODO: Return correct IdentityResult
            return Task.FromResult<IdentityResult>(null);
        }

        /// <summary>
        /// DeleteAsync a role.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (role == null)
            {
                throw new ArgumentNullException("user");
            }

            roleTable.Delete(role.Id);
            //TODO: Return correct IdentityResult.
            return Task.FromResult<IdentityResult>(null);
        }

        /// <summary>
        /// Finds a role by id.
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken = default(CancellationToken))
        {
            TRole result = roleTable.GetRoleById(roleId) as TRole;

            return Task.FromResult<TRole>(result);
        }

        /// <summary>
        /// Find a role by normalized name.
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<TRole> FindByNameAsync(string roleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            TRole result = roleTable.GetRoleByName(roleName) as TRole;

            return Task.FromResult<TRole>(result);
        }

        /// <summary>
        /// Get a role's normalized name.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            // TODO: Implement.
            return Task.FromResult<string>(null);
        }

        /// <summary>
        /// Returns a role's id.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            // TODO: Implement.
            return Task.FromResult<string>(null);
        }

        /// <summary>
        /// Returns a role's name.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            // TODO: Implement.
            return Task.FromResult<string>(null);
        }

        /// <summary>
        /// Set a role's normalized name.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="normalizedName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
        {
            // TODO: Implement.
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Set a role's name.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="roleName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            // TODO: Implement.
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Update a role.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (role == null)
            {
                throw new ArgumentNullException("user");
            }

            roleTable.Update(role);
            //TODO: Return correct IdentityResult
            return Task.FromResult<IdentityResult>(null);
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

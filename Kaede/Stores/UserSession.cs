using Kaede.DTOs;
using Kaede.Services.UsersService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.Stores
{
    public class UserSession
    {
        #region Fields
        private readonly ReaderWriterLockSlim _lock = new();
        #endregion

        #region Properties
        private UserDTO? _currentUser;
        public UserDTO? CurrentUser {
            get 
            {
                _lock.EnterReadLock();
                try
                {
                    return _currentUser;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            private set
            {
                _lock.EnterWriteLock();
                try
                {
                    _currentUser = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }
        public bool IsLoggedIn => CurrentUser != null;
        #endregion

        #region Methods
        public void Assign(UserDTO user)
        {
            if (IsLoggedIn)
                throw new InvalidOperationException("A logged in user is already present in the user service.");
            CurrentUser = user;
        }

        public void ResolveChanges()
        {
            if (!IsLoggedIn)
                throw new InvalidOperationException("Can only resolve changes to an already logged in user.");

            // could have injected this but this is cleaner since it is oneshot
            var userService = App.RunningInstance().FetchProviderService<IUserService>();
            var updatedUserDTO = userService?.GetUser(CurrentUser!.Username).GetAwaiter().GetResult();

            if (updatedUserDTO != null)
            {
                CurrentUser = updatedUserDTO;
            }
        }

        public void Remove() => CurrentUser = null;
        #endregion
    }
}

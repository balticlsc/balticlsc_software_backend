using System;
using System.Collections.Generic;
using System.Linq;
using Baltic.Security.Auth;
using Baltic.Security.Entities;
using Baltic.Security.Tables;
using Baltic.Security.Utils;
using Baltic.Web.Interfaces;
using Microsoft.Extensions.Hosting.Internal;
using Serilog;

namespace Baltic.Security
{
    public class UserRegistryRepository : IUserSessionRoutine
    {
        private SessionTable _sessionTable = null;
        private SessionTable SessionTable => _sessionTable ??= new SessionTable();

        private UsersTable _usersTable = null;
        private UsersTable UsersTable => _usersTable ??= new UsersTable();

        private SessionParamsTable _sessionParamsTable = null;
        
        private SessionParamsTable SessionParamsTable => _sessionParamsTable ??= new SessionParamsTable();
        
        private UserPreferencesTable _userPreferencesTable = new UserPreferencesTable();
        private UserPreferencesTable UserPreferencesTable => _userPreferencesTable ??= new UserPreferencesTable();

        public static void InitializeRepository(bool isDevelopment, string userName = "root", string password = "Pa$$w0rd")
        {
            var repo = new UserRegistryRepository();
            var sql = $"TRUNCATE TABLE {repo.SessionTable.TableName} CASCADE";
            
            Log.Information("Running session security initialization");
            repo.SessionTable.ExecuteAsync(sql);

            if (isDevelopment)
            {
                Log.Information("User {userName} was created with id: {id}", userName, repo.CreateUser(userName, password).Id);
                userName = "demo";
                password = "BalticDemo";
                Log.Information("User {userName} was created with id: {id}", userName, repo.CreateUser(userName, password).Id);                
            }
        }        
        
        public bool SaveSession(string userName, string sid, string jti)
        {
            var session = SessionTable.Single(new {Sid = sid});
            var user = UsersTable.Single(new {UserName = userName});

            if (user != null)
            {
                if (session != null)
                {
                    session.jti = jti;
                    session = SessionTable.Update(session);
                }
                else
                {
                    session = SessionTable.Insert(new
                    {
                        Sid = sid,
                        Jti = jti,
                        UserId = user.id
                    });
                }
                return session != null;
            }
            return false;
        }

        public SessionEntity GetSession(string sid)
        {
            var session = SessionTable.Single(new {Sid = sid});

            if (session != null)
            {
                return new SessionEntity()
                {
                    Id = session.id,
                    Jti = session.jti,
                    Sid = session.sid,
                    UserId = session.userid
                };
            }
            return null;
        }

        public bool DeleteSession(string sid)
        {
            var session = GetSession(sid);

            if (session != null)
            {
                return SessionTable.Delete(session) != 0;
            }

            return false;
        }

        public bool SaveSessionParam(string sid, string paramName, string paramValue)
        {
            var session = SessionTable.Single(new {Sid = sid});
            
            if (session != null)
            {
                var param = SessionParamsTable.Single(new {sessionid = session.id, param = paramName});
                
                if (param != null)
                {
                    param.value = paramValue;
                    if (SessionParamsTable.Update(param) != null)
                    {
                        return true;
                    }
                }
                else
                {
                    if (SessionParamsTable.Insert(new {sessionid = session.id, param = paramName, value = paramValue}) != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public string GetSessionParam(string sid, string paramName)
        {
            var session = SessionTable.Single(new {Sid = sid});
            
            if (session != null)
            {
                var param = SessionParamsTable.Single(new {sessionid = session.id, param = paramName});
                return param != null ? (string) param.value : string.Empty;
            }
            return string.Empty;
        }

        public bool SaveUserPref(string userName, string prefName, string prefValue)
        {
            var user = UsersTable.Single(new {username = userName});
            
            if (user != null)
            {
                var param = UserPreferencesTable.Single(new {userid = user.id, param = prefName});
                if (param != null)
                {
                    param.value = prefValue;
                    if (UserPreferencesTable.Update(param) != null)
                    {
                        return true;
                    }
                }
                else
                {
                    if (UserPreferencesTable.Insert(new {userid = user.id, param = prefName, value = prefValue}) != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public string GetUserPref(string userName, string prefName)
        {
            var user = UsersTable.Single(new {username = userName});
            
            if (user != null)
            {
                var param = UserPreferencesTable.Single(new {userid = user.id, param = prefName});
                return param != null ? (string) param.value : string.Empty;
            }
            return string.Empty;
        }

        public SystemUser CreateUser(string name, string password, string emailAddress = "", AccountStatusLookup status = AccountStatusLookup.Created)
        {
            var user = UsersTable.Single(new {Username = name});
            
            if (user == null)
            {
                var pass = Convert.ToBase64String(SecurePasswordHasher.Hash(password));
                user = UsersTable.Insert(new
                {
                    username = name,
                    password = pass,
                    emailaddress = emailAddress,
                    status = (int)status
                });
            }

            return new SystemUser()
            {
                EmailAddress = user.emailaddress,
                Id = user.id,
                Status = (AccountStatusLookup) user.status,
                UserName = user.username
            };
        }

        public SystemUser AuthUser(string name, string password)
        {
            var pass = Convert.ToBase64String(SecurePasswordHasher.Hash(password));
            var user = UsersTable.Single(new {Username = name});

            if (user != null)
            {
                if (SecurePasswordHasher.Verify(password, user.password))
                {
                    return new SystemUser()
                    {
                        Id = user.id,
                        EmailAddress = user.emailaddress,
                        UserName = user.username,
                        Status = (AccountStatusLookup)user.status
                    };
                }
            }

            return null;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            var sysUser = AuthUser(userName, oldPassword);
            if (sysUser != null)
            {
                var user = UsersTable.Single(new {username = userName, id = sysUser.Id});
                if (user != null)
                {
                    user.password = Convert.ToBase64String(SecurePasswordHasher.Hash(newPassword));
                    UsersTable.Update(user);
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<UserEntity> GetUserList(bool activeOrAll)
        {
            if (activeOrAll)
            {
                return UsersTable.All(new
                    {
                        status = true
                    })
                    .Select(user => new UserEntity
                    {
                        Id = user.id,
                        UserName = user.username,
                        Email = user.emailaddress
                    })
                    .ToList();
            }
            else
            {
                return UsersTable.All()
                    .Select(user => new UserEntity
                    {
                        Id = user.id,
                        UserName = user.username,
                        Email = user.emailaddress
                    })
                    .ToList();
            }
        }
    }
}
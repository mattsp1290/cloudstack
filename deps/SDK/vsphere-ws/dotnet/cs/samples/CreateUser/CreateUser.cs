using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web.Services.Protocols;
using VimApi;
using AppUtil;
using VMware.Security.CredentialStore;

namespace CreateUser
{
    public class CreateUser
    {
        private static AppUtil.AppUtil cb = null;
        private void createUser()
        {

            ManagedObjectReference hostLocalAccountManager =
               cb._connection._sic.accountManager;


            ManagedObjectReference hostAuthorizationManager =
               cb._connection._sic.authorizationManager;

            String userName = GenerateUserName();
            String password = GeneratePassword();

            // Create an user
            HostAccountSpec hostAccountSpec = new HostAccountSpec();
            hostAccountSpec.id = userName;
            hostAccountSpec.password = password;
            hostAccountSpec.description = "User Description";
            cb._connection._service.CreateUser(hostLocalAccountManager,
                                                       hostAccountSpec);

            ManagedObjectReference rootFolder =
               cb._connection._sic.rootFolder;

            Permission per = new Permission();
            per.group = false;
            per.principal = userName;
            per.roleId = -1;
            per.propagate = true;
            per.entity = rootFolder;

            cb._connection._service.SetEntityPermissions(hostAuthorizationManager,
                                                                 rootFolder,
                                                                 new Permission[] { per });

            ICredentialStore csObj = CredentialStoreFactory.CreateCredentialStore();
            csObj.AddPassword(GetServerName(), userName, password.ToCharArray());
            Console.WriteLine("Successfully created user and populate the "
                               + "credential store");
        }

        private String GenerateUserName()
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            int rawRandomNumber = random.Next();
            String userName = "user" + rawRandomNumber.ToString();
            return userName;
        }

        private String GeneratePassword()
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            int rawRandomNumber = random.Next();
            String passwd = "passwd" + rawRandomNumber.ToString();
            return passwd;
        }

        private String GetServerName()
        {
            if (cb.get_option("server") != null)
            {
                return cb.get_option("server");
            }
            else
            {
                String urlString = cb.get_option("url");
                if (urlString.IndexOf("https://") != -1)
                {
                    int sind = 8;
                    int lind = urlString.IndexOf("/sdk");
                    return urlString.Substring(sind, lind - 8);
                }
                else if (urlString.IndexOf("http://") != -1)
                {
                    int sind = 7;
                    int lind = urlString.IndexOf("/sdk");
                    return urlString.Substring(sind, lind);
                }
                else
                {
                    return urlString;
                }
            }
        }

        public static void Main(String[] args)
        {
            CreateUser obj = new CreateUser();
            cb = AppUtil.AppUtil.initialize("CreateUser", args);
            cb.connect();
            obj.createUser();
            cb.disConnect();
            Console.WriteLine("Press enter to exit ");
            Console.Read();
        }
    }
}
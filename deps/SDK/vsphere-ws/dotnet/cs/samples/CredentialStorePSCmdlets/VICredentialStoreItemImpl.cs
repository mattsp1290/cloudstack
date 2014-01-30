using System;
using System.Diagnostics;
using VMware.VimAutomation.Types;

namespace VMware.VimAutomation {
   internal class VICredentialStoreItemImpl : VICredentialStoreItem {
      private readonly string _file;
      private readonly string _host;
      private readonly string _password;
      private readonly string _user;

      public VICredentialStoreItemImpl(
         string host, string user, string password, string file) {
         
         if (string.IsNullOrEmpty(host)) {
            throw new ArgumentException("host");
         }
         if (string.IsNullOrEmpty(user)) {
            throw new ArgumentException("user");
         }
         if (password == null) {
            throw new ArgumentNullException("password");
         }

         _host = host;
         _user = user;
         _password = password;
         _file = file;
      }

      #region VICredentialStoreItem Members

      public string Host {
         [DebuggerStepThrough]
         get { return _host; }
      }

      public string User {
         [DebuggerStepThrough]
         get { return _user; }
      }

      public string Password {
         [DebuggerStepThrough]
         get { return _password; }
      }

      public string File {
         [DebuggerStepThrough]
         get { return _file; }
      }

      #endregion
   }
}
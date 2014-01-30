using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using VMware.Security.CredentialStore;

namespace VMware.VimAutomation.Commands {
   [Cmdlet(
      VerbsCommon.New, "VICredentialStoreItem",
      SupportsShouldProcess = false,
      ConfirmImpact = ConfirmImpact.Medium)]
   public class NewVICredentialStoreItem : PSCmdlet {
      private string _file;
      private string _host;
      private string _password;
      private string _user;

      [Parameter(Position=1, Mandatory=true)]
      [ValidateNotNullOrEmpty]
      public new string Host {
         [DebuggerStepThrough]
         get { return _host; }
         set { _host = value; }
      }

      [Parameter(Position=2, Mandatory=true)]
      [ValidateNotNullOrEmpty]
      public string User {
         [DebuggerStepThrough]
         get { return _user; }
         set { _user = value; }
      }

      [Parameter(Position=3, Mandatory=false)]
      public string Password {
         [DebuggerStepThrough]
         get { return _password; }
         set { _password = value; }
      }

      [Parameter(Position=4)]
      [ValidateNotNullOrEmpty]
      public string File {
         [DebuggerStepThrough]
         get { return _file; }
         set { _file = value; }
      }

      protected override void ProcessRecord() {
         FileInfo fileInfo = string.IsNullOrEmpty(File)
                                ? null
                                : new FileInfo(File);

         List<VICredentialStoreItemImpl> result =
            new List<VICredentialStoreItemImpl>();

         ICredentialStore store = null;
         try {
            store = CredentialStoreFactory.CreateCredentialStore(fileInfo);
            if (Password == null) {
               Password = "";
            }

            store.AddPassword(Host, User, Password.ToCharArray());

            result.Add(
               new VICredentialStoreItemImpl(
                  Host,
                  User,
                  new string(store.GetPassword(Host, User)),
                  File));
         } finally {
            if (store != null) {
               store.Close();
            }
         }

         WriteObject(result, true);
      }
   }
}
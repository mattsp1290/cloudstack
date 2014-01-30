using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using VMware.Security.CredentialStore;

namespace VMware.VimAutomation.Commands {
   [Cmdlet(
      VerbsCommon.Get, "VICredentialStoreItem",
      SupportsShouldProcess = false,
      ConfirmImpact = ConfirmImpact.None)]
   public class GetVICredentialStoreItem : PSCmdlet {
      private string _file;
      private string _host;
      private string _user;

      [Parameter(Position=1)]
      [ValidateNotNullOrEmpty]
      public new string Host {
         [DebuggerStepThrough]
         get { return _host; }
         set { _host = value; }
      }

      [Parameter(Position=2)]
      [ValidateNotNullOrEmpty]
      public string User {
         [DebuggerStepThrough]
         get { return _user; }
         set { _user = value; }
      }

      [Parameter(Position=3)]
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

         WildcardPattern hostPattern =
            new WildcardPattern("*", WildcardOptions.Compiled);
         if (!string.IsNullOrEmpty(Host)) {
            hostPattern =
               new WildcardPattern(
                  Host, WildcardOptions.Compiled | WildcardOptions.IgnoreCase);
         }

         WildcardPattern usernamePattern =
            new WildcardPattern("*", WildcardOptions.Compiled);
         if (!string.IsNullOrEmpty(User)) {
            usernamePattern =
               new WildcardPattern(User, WildcardOptions.Compiled);
         }

         List<VICredentialStoreItemImpl> result =
            new List<VICredentialStoreItemImpl>();
         try {
            ICredentialStore store = null;
            try {
               store = CredentialStoreFactory.CreateCredentialStore(fileInfo);
               foreach (string host in store.GetHosts()) {
                  if (hostPattern.IsMatch(host)) {
                     foreach (string username in store.GetUsernames(host)) {
                        if (usernamePattern.IsMatch(username)) {
                           string password =
                              new string(store.GetPassword(host, username));

                           result.Add(
                              new VICredentialStoreItemImpl(
                                 host, username, password, File));
                        }
                     }
                  }
               }
            } finally {
               if (store != null) {
                  store.Close();
               }
            }
         } catch (Exception ex) {
            ThrowTerminatingError(
               new ErrorRecord(
                  ex,
                  "Core_GetVICredentialStoreItem_ProcessRecordget",
                  ErrorCategory.NotSpecified,
                  null));
         }

         WriteObject(result, true);
      }
   }
}
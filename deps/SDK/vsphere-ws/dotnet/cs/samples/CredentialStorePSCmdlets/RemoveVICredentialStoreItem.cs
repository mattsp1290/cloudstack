using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using VMware.Security.CredentialStore;
using VMware.VimAutomation.Types;

namespace VMware.VimAutomation.Commands {
   [Cmdlet(
      VerbsCommon.Remove, "VICredentialStoreItem",
      SupportsShouldProcess = true,
      ConfirmImpact = ConfirmImpact.High)]
   public class RemoveVICredentialStoreItem : PSCmdlet {
      private const string ParameterSetByFilters = "ByFilters";
      private const string ParameterSetByItemObject = "ByCredentialItemObject";

      private string _file;
      private string _host;
      private VICredentialStoreItem[] _items;
      private string _user;

      [Parameter(
         Mandatory=true,
         Position=1,
         ValueFromPipeline=true,
         ParameterSetName = ParameterSetByItemObject)]
      [ValidateNotNull]
      public VICredentialStoreItem[] CredentialStoreItem {
         [DebuggerStepThrough]
         get { return _items; }
         set { _items = value; }
      }

      [Parameter(
         Position=1,
         ParameterSetName = ParameterSetByFilters)]
      [ValidateNotNullOrEmpty]
      public new string Host {
         [DebuggerStepThrough]
         get { return _host; }
         set { _host = value; }
      }

      [Parameter(
         Position=2,
         ParameterSetName = ParameterSetByFilters)]
      [ValidateNotNullOrEmpty]
      public string User {
         [DebuggerStepThrough]
         get { return _user; }
         set { _user = value; }
      }

      [Parameter(
         Position=3,
         ParameterSetName = ParameterSetByFilters)]
      [ValidateNotNullOrEmpty]
      public string File {
         [DebuggerStepThrough]
         get { return _file; }
         set { _file = value; }
      }

      protected override void ProcessRecord() {
         if (ParameterSetName == ParameterSetByFilters &&
             string.IsNullOrEmpty(Host) && string.IsNullOrEmpty(User)) {
            ArgumentException exception =
               new ArgumentException(
                  "At least one of Host or User must be specified.");

            ThrowTerminatingError(
               new ErrorRecord(
                  exception,
                  "Core_RemoveVICredentialStoreItem_ProcessRecord_InvalidArguemnt",
                  ErrorCategory.InvalidArgument,
                  null));
         }

         VICredentialStoreItem[] itemsToRemove;

         if (ParameterSetName == ParameterSetByItemObject) {
            itemsToRemove = _items;
         } else if (ParameterSetName == ParameterSetByFilters) {
            List<VICredentialStoreItemImpl> result =
               new List<VICredentialStoreItemImpl>();

            FileInfo fileInfo = string.IsNullOrEmpty(File)
                                   ? null
                                   : new FileInfo(File);

            ICredentialStore store = null;
            try {
               store = CredentialStoreFactory.CreateCredentialStore(fileInfo);

               WildcardPattern hostPattern =
                  new WildcardPattern(
                     "*", WildcardOptions.Compiled | WildcardOptions.IgnoreCase);
               if (!string.IsNullOrEmpty(Host)) {
                  hostPattern =
                     new WildcardPattern(
                        Host,
                        WildcardOptions.Compiled | WildcardOptions.IgnoreCase);
               }

               WildcardPattern usernamePattern =
                  new WildcardPattern("*", WildcardOptions.Compiled);
               if (!string.IsNullOrEmpty(User)) {
                  usernamePattern =
                     new WildcardPattern(User, WildcardOptions.Compiled);
               }

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
            itemsToRemove = result.ToArray();
         } else {
            Debug.Fail("Unknown parameter set: " + ParameterSetName);
            itemsToRemove = new VICredentialStoreItem[0];
         }

         foreach (VICredentialStoreItem item in itemsToRemove) {
            string message =
               string.Format(
                  "Remove credential store item for host '{0}' and username '{1}'?",
                  item.Host,
                  item.User);

            if (ShouldProcess(message)) {
               FileInfo fileInfo = string.IsNullOrEmpty(item.File)
                                      ? null
                                      : new FileInfo(item.File);

               ICredentialStore store = null;
               try {
                  store =
                     CredentialStoreFactory.CreateCredentialStore(fileInfo);
                  store.RemovePassword(item.Host, item.User);
               } finally {
                  if (store != null) {
                     store.Close();
                  }
               }
            }
         }
      }
   }
}
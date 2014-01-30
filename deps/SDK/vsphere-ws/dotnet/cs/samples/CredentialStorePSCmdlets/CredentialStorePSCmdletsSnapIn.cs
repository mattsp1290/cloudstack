/* *************************************************************************
 * Copyright 2007 VMware, Inc.  All rights reserved. -- VMware Confidential
 * *************************************************************************/

using System.ComponentModel;
using System.Management.Automation;

namespace VMware.VimAutomation
{
   [RunInstaller(true)]
   public class Vim4PSSnapIn : PSSnapIn
   {
      public Vim4PSSnapIn()
         : base()
      {
      }

      public override string Name
      {
         get { return "VICredentialStorePSCmdlets"; }
      }

      public override string Vendor
      {
         get { return "VMware Inc."; }
      }

      public override string Description
      {
         get
         {
            return "This Windows PowerShell snap-in contains Windows PowerShell " +
                   "cmdlets used to manage VMware Credential Store.";
         }
      }

      public override string[] Formats
      {
         get { return new string[] {"CredentialStorePSCmdlets.Format.ps1xml"}; }
      }
   }
}
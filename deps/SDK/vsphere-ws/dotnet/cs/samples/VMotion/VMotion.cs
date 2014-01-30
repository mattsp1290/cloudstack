using System;
using System.Web.Services;
using System.Collections;
using AppUtil;
using VimApi;
using System.Security.Principal;
using System.Net;

namespace VMotion
{
     ///<summary>
     ///This sample is used to validate if VMotion is feasible between two hosts or not,
     ///It is also used to perform migrate/relocate task depending on the data given
     ///</summary>
     ///<param name="vmname">Required: Name of the virtual machine</param>
     ///<param name="targethost">Required: Name of the target host</param>
     ///<param name="sourcehost">Required: Name of the host containg the virtual machine </param>
     ///<param name="targetpool">Required: Name of the target resource pool</param>
     ///<param name="targetdatastore">Required: Name of the target datastore</param>
     ///<param name="priority">Optional: The priority of the migration task: defaultPriority,
     /// highPriority, lowPriority</param>
     ///<param name="state">Optional </param>
     ///<remarks>
     ///Relocate or migrate a VM 
     ///--url [webserviceurl]
     ///--username [username] --password [password]  --targetpool [tpool]
     ///--sourcehost [shost] --targethost [thost] --vmname [myVM] --targetdatastore [tDS]
     ///Validate the vmotion capability
     ///--url [webserviceurl]
     ///--username [username] --password [password]  --targetpool [tpool]
     ///--sourcehost [shost] --targethost [thost] --vmname [myVM] --targetdatastore [tDS]
     ///--validate
     ///</remarks>

    class VMotion {
        private static AppUtil.AppUtil cb = null;
        //private static ManagedObjectReference licMgr = null;
        //private static VMUtils vmUtils = null;

        private void getVersion(String[] args, VMotion vmotionObj) {
           ArrayList apiVersions = VersionUtil.getSupportedVersions(
                                              cb.get_option("url"));
          if(VersionUtil.isApiVersionSupported(apiVersions,"4.0")) {
           Cookie cookie = cb.getConnection()._service.CookieContainer.GetCookies(
                                         new Uri(cb.get_option("url")))[0];

            // go to the v25 sample
           VMotionV25 vmotionV25obj  = new VMotionV25(); 
           vmotionV25obj.useVMotion25(args,cookie,vmotionObj); 
       }
       else {
         if(cb.option_is_set("validate")) {
             Console.WriteLine("Investing the VMotion capability of VM in a Host"+
              " is supported in only 4.0 servers ");
           }
           else {
             migrate_or_relocate_VM();
           }
        }
      }

     public void migrate_or_relocate_VM() {
        // first we need to check if the VM should be migrated of relocated
        // If target host and source host both contains
        //the datastore, virtual machine needs to be migrated
        // If only target host contains the datastore, machine needs to be relocated
        String vmname = cb.get_option("vmname");
        String targetHost = cb.get_option("targethost");
        String targetPool = cb.get_option("targetpool");
        String sourceHost = cb.get_option("sourcehost");
        String targetDS = cb.get_option("targetdatastore");
        String operationName = check_operation_type(targetHost, sourceHost, targetDS );

        if(operationName.Equals("migrate")) {
          migrateVM(vmname,targetPool,targetHost, sourceHost);
        }
        else if (operationName.Equals("relocate"))
        {
          relocateVM(vmname,targetPool,targetHost,targetDS, sourceHost);
        }
     }

   public void migrateVM(String vmname ,String pool, String tHost, String srcHost)  {
         String state;
         //String priority;
         VirtualMachinePowerState st = VirtualMachinePowerState.poweredOff;
         VirtualMachineMovePriority pri =VirtualMachineMovePriority.defaultPriority;
         if(cb.option_is_set("state")) {
          state =cb.get_option("state");
          if (cb.get_option("state").Equals("suspended"))
          {
              st = VirtualMachinePowerState.suspended;
          }
          else if (cb.get_option("state").Equals("poweredOn"))
          {
              st = VirtualMachinePowerState.poweredOn;
          }
          else if (cb.get_option("state").Equals("poweredOff"))
          {
              st = VirtualMachinePowerState.poweredOff;
          }
         }
          pri = getPriority(); 

        try {
          ManagedObjectReference srcMOR = getMOR(srcHost, "HostSystem", null);
          ManagedObjectReference vmMOR = getMOR(vmname, "VirtualMachine", srcMOR);
          ManagedObjectReference poolMOR = getMOR(pool, "ResourcePool", null);
          ManagedObjectReference hMOR = getMOR(tHost, "HostSystem", null);
          if (vmMOR == null || srcMOR == null || poolMOR == null || hMOR == null)
          {
            return;
          }

          Console.WriteLine("Migrating the Virtual Machine " + vmname);
          ManagedObjectReference taskMOR 
             = cb.getConnection().Service.MigrateVM_Task(vmMOR,poolMOR,hMOR,pri,st,true);
          String res = cb.getServiceUtil().WaitForTask(taskMOR); 
          if(res.Equals("sucess")) {
          Console.WriteLine("Migration of Virtual Machine " +vmname + " done successfully to "+tHost);
          }
          else {
            Console.WriteLine("Error::  Migration failed");
          }
        }
        catch(Exception e){
            Console.WriteLine(e.Message);
        }
    }


   public void relocateVM(String vmname,String pool ,String tHost,String tDS, String srcHost) {
    VirtualMachineMovePriority pri = getPriority();
     try {
          ManagedObjectReference srcMOR = getMOR(srcHost, "HostSystem", null);
          ManagedObjectReference vmMOR = getMOR(vmname, "VirtualMachine", srcMOR);
          ManagedObjectReference poolMOR = getMOR(pool, "ResourcePool", null);
          ManagedObjectReference hMOR = getMOR(tHost, "HostSystem", null);
          ManagedObjectReference[] dsTarget
          = (ManagedObjectReference[])cb.getServiceUtil().GetDynamicProperty(hMOR, "datastore");
          ManagedObjectReference dsMOR = browseDSMOR(dsTarget, tDS);
          if(dsMOR == null) {
            Console.WriteLine("Datastore "+tDS+ " not found" );
          }
          if (vmMOR == null || srcMOR == null || poolMOR == null || hMOR == null || dsMOR == null)
          {
            return;
          }
          VirtualMachineRelocateSpec relSpec = new VirtualMachineRelocateSpec();
          relSpec.datastore =(dsMOR);
          relSpec.host = (hMOR);
          relSpec.pool = (poolMOR);
          Console.WriteLine("Relocating the Virtual Machine "+vmname);
          ManagedObjectReference taskMOR =
             cb.getConnection().Service.RelocateVM_Task(vmMOR,relSpec);
          String res = cb.getServiceUtil().WaitForTask(taskMOR); 
          if(res.Equals("sucess")) {
              Console.WriteLine("Relocation done successfully of "+vmname+ " to host "+tHost);
          }
          else {
            Console.WriteLine("Error::  Relocation failed");
          }
          
       }
       catch(Exception e){
           Console.WriteLine(e.Message);
           }
        }

        public VirtualMachineMovePriority getPriority()
        {
            VirtualMachineMovePriority prior = VirtualMachineMovePriority.defaultPriority;
            if (!cb.option_is_set("priority")) {
                prior = VirtualMachineMovePriority.defaultPriority;
            }
            else {
                if (cb.get_option("priority").Equals("lowPriority"))
                {
                    prior = VirtualMachineMovePriority.lowPriority;
                }
                else if (cb.get_option("priority").Equals("highPriority"))
                {
                    prior = VirtualMachineMovePriority.highPriority;
                }
                else if (cb.get_option("priority").Equals("defaultPriority"))
                {
                    prior = VirtualMachineMovePriority.defaultPriority;
                }
            }
            return prior;

        }

        private String check_operation_type(String targetHost, String sourceHost, String targetDS)
        {
            String operation = "";
            try
            {
                ManagedObjectReference targetHostMOR = getMOR(targetHost, "HostSystem", null);
                ManagedObjectReference sourceHostMOR = getMOR(sourceHost, "HostSystem",null);
                if ((targetHostMOR == null) || (sourceHostMOR == null)) {
                    return "";
                }
                ManagedObjectReference[] dsTarget
                  = (ManagedObjectReference[])cb.getServiceUtil().GetDynamicProperty(targetHostMOR, "datastore");
                ManagedObjectReference tarHostDS = browseDSMOR(dsTarget, targetDS);                
                ManagedObjectReference[] dsSource
                  = (ManagedObjectReference[])cb.getServiceUtil().GetDynamicProperty(sourceHostMOR, "datastore");
                ManagedObjectReference srcHostDS = browseDSMOR(dsSource, targetDS);
                if ((tarHostDS != null) && (srcHostDS != null)) {
                    // we have a shared datastore we can do migration 
                    operation = "migrate";
                }
                else {
                    operation = "relocate";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return operation;
        }

    private ManagedObjectReference getMOR(String name, String type, ManagedObjectReference root) {
    
       ManagedObjectReference nameMOR 
       = (ManagedObjectReference)cb.getServiceUtil().GetDecendentMoRef(root,type, name);
       if(nameMOR ==null) {
           Console.WriteLine("Error:: " + name + " not found");
          return null;
       }
       else {
         return nameMOR;
       }
    }


     private ManagedObjectReference browseDSMOR(ManagedObjectReference[] dsMOR, String dsName) {
        ManagedObjectReference dataMOR=null;
      try{
         if(dsMOR != null && dsMOR.Length>0) {
            for (int i = 0; i < dsMOR.Length; i++ ) {
               String dsname  = (String)cb.getServiceUtil().GetDynamicProperty(dsMOR[i], "summary.name");
               if(dsname.Equals(dsName)) {
                  dataMOR = dsMOR[i];
               }
            }
         }
      }
      catch(Exception e) {
          Console.WriteLine(e.Message);
      }
       return dataMOR;
    }


     
   private Boolean customValidation() {
      Boolean flag = true;
      if(cb.option_is_set("state")) {
        String state =cb.get_option("state");
        if(!state.Equals("poweredOn") 
               && !state.Equals("poweredOff")
                    && !state.Equals("suspended")) {
        Console.WriteLine("Must specify 'poweredOn', 'poweredOff' or"+
                     " 'suspended' for 'state' option\n");   	
           flag = false;
           }
       }
       if(cb.option_is_set("priority")) {
        String prior =cb.get_option("priority");
        if(!prior.Equals("defaultPriority") 
               && !prior.Equals("highPriority")
                    && !prior.Equals("lowPriority")) {
         Console.WriteLine("Must specify 'defaultPriority', 'highPriority " +
                      " 'or 'lowPriority' for 'priority' option\n");
           flag = false;
          }
       }
      return flag;
    }


    public static OptionSpec[] constructOptions() {
      OptionSpec [] useroptions = new OptionSpec[8];
      useroptions[0] = new OptionSpec("vmname","String",1,
                                      "Name of the virtual machine"
                                      ,null);
      useroptions[1] = new OptionSpec("targethost","String",1,
                                      "Target host on which VM is to be migrated",
                                      null);
      useroptions[2] = new OptionSpec("targetpool","String",1,
                                      "Name of the target resource pool",
                                      null);
      useroptions[3] = new OptionSpec("priority","String",0,
                                      "The priority of the migration task: defaultPriority, highPriority, lowPriority",
                                      null);
      useroptions[4] = new OptionSpec("validate","String",0,
                                      "Check whether the vmotion feature is legal for 4.0 servers",
                                      null);
      useroptions[5] = new OptionSpec("sourcehost","String",1,
                                      "Name of the host containg the virtual machine.",
                                      null);        
      useroptions[6] = new OptionSpec("targetdatastore","String",1,
                                      "Name of the target datastore",
                                      null);        
      useroptions[7] = new OptionSpec("state","String",0,
                                      "State of the VM poweredOn,poweredOff, suspended",
                                      null);
      return useroptions;
   }

   public static void Main(String[] args)  {
      VMotion app = new VMotion();
      cb = AppUtil.AppUtil.initialize("VMotion",VMotion.constructOptions(),args);
      Boolean valid = app.customValidation();
      if(valid) {
          try
          {
              cb.connect();
              app.getVersion(args, app);
              cb.disConnect();
          }
          catch(Exception e){
          }
         Console.WriteLine("Press any key to exit: ");
         Console.Read();
      }
   }
    }
}
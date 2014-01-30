using System;
using System.Web.Services;
using System.Collections;
using AppUtil;
using Vim25Api;
using System.Security.Principal;
using System.Net;

namespace VMotion
{
    class VMotionV25
    {
        ExtendedAppUtil ecb = null;
        private static Vim25Api.ManagedObjectReference provisionChkr = null;
        public VMotion vmotionObj = null;
        //private static VersionUtil versionUtil = null;

  public void useVMotion25(String [] args, Cookie cookieString, VMotion obj )  {
      ecb = ExtendedAppUtil.initialize("VmotionV25"
                                       ,VMotion.constructOptions()
                                       ,args);
      vmotionObj = obj;
      ecb.connect(cookieString);
      provisionChkr = ecb.getServiceConnectionV25().ServiceContent.vmProvisioningChecker;
      if(ecb.option_is_set("validate")) {
           Console.WriteLine("Investing the VMotion capability of VM in a Host");
           checkVMotionCompatibility();
         }
       else {
        vmotionObj.migrate_or_relocate_VM();
        //do migration
        }
   }
      
 private void checkVMotionCompatibility() {
   
   String vmname = ecb.get_option("vmname");
   String sourcehost = ecb.get_option("sourcehost");
   String targethost = ecb.get_option("targethost");
   String targetpool = ecb.get_option("targetpool");
   String dataname = ecb.get_option("targetdatastore");
   ManagedObjectReference hostMOR = getMOR(sourcehost, "HostSystem", null);
   ManagedObjectReference vmMOR = getMOR(vmname, "VirtualMachine", hostMOR);
    ManagedObjectReference targethostMOR = getMOR(targethost, "HostSystem", null);
   ManagedObjectReference poolMOR = getMOR(targetpool, "ResourcePool", null);
   ManagedObjectReference[] dsTarget
          = (ManagedObjectReference[])ecb.getServiceUtilV25().GetDynamicProperty(targethostMOR, "datastore");
   ManagedObjectReference dsMOR = browseDSMOR(dsTarget, dataname);
   if(dsMOR == null) {
     Console.WriteLine("Datastore "+dataname+ " not found");
   }
     if(vmMOR ==null || hostMOR==null || targethostMOR==null || dsMOR ==null || poolMOR ==null){
        return;
      }
    Boolean query = queryVMotionCompatibility(vmMOR,hostMOR,targethostMOR);
    Boolean migrate = checkMigrate(vmMOR,targethostMOR,poolMOR);
    Boolean relocation = checkRelocation(vmMOR,targethostMOR,poolMOR,dsMOR);
    
    if((query) &&  (migrate) &&(relocation) ) {
      Console.WriteLine("VMotion is feasible on VM "+vmname+" from host "+sourcehost+ " to "+targethost);
    }
    else {
      Console.WriteLine("VMotion is not feasible on VM " + vmname + " from host " + sourcehost + " to " + targethost);
    }
    
   }



 private Boolean checkRelocation(ManagedObjectReference vmMOR,
     ManagedObjectReference hostMOR,ManagedObjectReference poolMOR,
          ManagedObjectReference dsMOR) {
   Boolean relocate=false;
   
   try {
      VirtualMachineRelocateSpec relSpec = new VirtualMachineRelocateSpec();
      relSpec.datastore = (dsMOR);
      relSpec.host = (hostMOR);
      relSpec.pool = (poolMOR);
      ManagedObjectReference taskMOR = 
       ecb.getServiceConnectionV25().Service.CheckRelocate_Task(provisionChkr,vmMOR,relSpec,null);
      String res = monitorTask(taskMOR);
      if(res.Equals("sucess")) {
          relocate= true;
      }
      else {
          relocate=false;
      }
    }
    catch(Exception ){
       relocate = false;
    }
    return relocate;
  }




  private Boolean checkMigrate(ManagedObjectReference vmMOR,
     ManagedObjectReference hostMOR,ManagedObjectReference poolMOR) {
    Boolean migrate = false;

   try {
      ManagedObjectReference taskMOR 
       = ecb.getServiceConnectionV25().Service.CheckMigrate_Task(provisionChkr,vmMOR,hostMOR,poolMOR,VirtualMachinePowerState.poweredOff,false,null);
       String res = monitorTask(taskMOR);
      if(res.Equals("sucess")) {
          migrate= true;
      }
      else {
          migrate=false;
      }
   }
   catch(Exception ){
       migrate = false;
    }
    return migrate;
   }


  private Boolean queryVMotionCompatibility(ManagedObjectReference vmMOR,
       ManagedObjectReference hostMOR, ManagedObjectReference targethostMOR ) {
   Boolean result=false;
   
   try {
    ManagedObjectReference[] vmMORs = new  ManagedObjectReference[] {vmMOR};
    ManagedObjectReference[] hostMORs = new ManagedObjectReference[] {hostMOR,targethostMOR};
    //String[] test = null;
    ManagedObjectReference taskMOR 
       = ecb.getServiceConnectionV25().Service.QueryVMotionCompatibilityEx_Task(provisionChkr,vmMORs,hostMORs);
    String res = monitorTask(taskMOR);
      if(res.Equals("sucess")) {
          result= true;
      }
      else {
          result=false;
      }
    }
    catch(Exception ){
       result=false;
    }
      return result;
  }



   private ManagedObjectReference getMOR(String name, String type, ManagedObjectReference root) {
       ManagedObjectReference nameMOR 
       = (ManagedObjectReference)ecb.getServiceUtilV25().GetDecendentMoRef(root, type, name);
       if(nameMOR ==null) {   
          Console.WriteLine("Error:: "+name+ " not found");
          return null;
       }
       else {
         return nameMOR;
       }
    }

        private ManagedObjectReference browseDSMOR(ManagedObjectReference[] dsMOR, String dsName) {
            ManagedObjectReference dataMOR = null;
            try {
                if (dsMOR != null && dsMOR.Length > 0){
                    for (int i = 0; i < dsMOR.Length; i++) {
                        String dsname = (String)ecb.getServiceUtilV25().GetDynamicProperty(dsMOR[i], "summary.name");
                        if (dsname.Equals(dsName)) {
                            dataMOR = dsMOR[i];
                        }
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            return dataMOR;
        }


    public String monitorTask(ManagedObjectReference taskmor) {
      Object[] result = ecb.getServiceUtilV25().WaitForValues(
                        taskmor, new String[] { "info.state", "info.error" }, 
                        new String[] { "state" },
                        new Object[][] { new Object[] { TaskInfoState.success, TaskInfoState.error } });      
      if (result[0].Equals(TaskInfoState.success)) {
         return "sucess";
      } 
      else {
         TaskInfo tinfo = (TaskInfo)ecb.getServiceUtilV25().GetDynamicProperty(taskmor,"info");
         LocalizedMethodFault fault = tinfo.error;
         String error = "Error Occured";
         if(fault!=null) {
             error = fault.localizedMessage;
         }
         return error;
      }
   }

    }
}

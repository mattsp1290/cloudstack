using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Services.Protocols;
using AppUtil;
using VimApi;

namespace Delete
{
  public class Delete
    {
      private static AppUtil.AppUtil cb = null;    

   Log log = new Log();
   private String getMeName()  {
      return cb.get_option("meName");
   }

   private Boolean doDelete()  {
      try {
         String errmsg = "";
         
         ManagedObjectReference memor 
            = cb.getServiceUtil().GetDecendentMoRef(null, "ManagedEntity", getMeName());
         if (memor == null) {
            errmsg = "Unable to find a Managed Entity named : " + getMeName() 
                   + " in Inventory";
            Console.WriteLine(errmsg);
            return false;
         }
         
         if (memor == null) {
            log.LogLine(errmsg);
            //throw new Exception(errmsg);
         }

         ManagedObjectReference taskmor 
            =  cb.getConnection()._service.Destroy_Task(memor);
         
         // If we get a valid task reference, monitor the task for success or failure
         // and report task completion or failure.
         if (taskmor != null) {
               Object[] result = 
               cb.getServiceUtil().WaitForValues(
                  taskmor, new String[] { "info.state", "info.error" }, 
                  new String[] { "state" }, // info has a property - 
                                           //state for state of the task
                  new Object[][] { new Object[] { 
                     TaskInfoState.success, TaskInfoState.error } 
                  }
               );

            // Wait till the task completes.
            if (result[0].Equals(TaskInfoState.success)) {
               log.LogLine(cb.getAppName() + " : Successful delete of Managed Entity : " 
                         + getMeName());
            } else {
               log.LogLine(cb.getAppName() + " : Failed delete of Managed Entity : " 
                         + getMeName());
               if (result.Length == 2 && result[1] != null) {
                  if (result[1].GetType().Equals("MethodFault")) {
                     cb.getUtil().LogException((Exception)result[1]);
                  }
               }
            }
         }
      } catch (Exception e) {
         cb.getUtil().LogException(e);
         log.LogLine(cb.getAppName() + " : Failed delete of Managed Entity : " 
                   + getMeName());
         throw e;
      }
      return true;
   }
   private static OptionSpec[] constructOptions() {
      OptionSpec [] useroptions = new OptionSpec[1];
      useroptions[0] = new OptionSpec("meName","String",1
                                     ,"Virtual Machine|ClusterComputeResource|folder"
                                     ,null);      
      return useroptions;
   }
   public static void Main(String[] args)  {
      Delete app = new Delete();
      cb = AppUtil.AppUtil.initialize("Delete", Delete.constructOptions(), args);
      cb.connect();
      Boolean status = app.doDelete();
      cb.disConnect();
      Console.WriteLine("Please enter any key to exit: ");
      Console.Read();      
   }
    }
}

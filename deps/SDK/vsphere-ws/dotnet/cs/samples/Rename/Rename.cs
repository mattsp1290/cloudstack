using System;
using System.Collections.Generic;
using System.Text;
using AppUtil;
using VimApi;

namespace Rename
{
    public class Rename
    {
        private static AppUtil.AppUtil cb = null;
   
   private void runOperation() {
      doRename();
   }
   
   private void doRename()  {
      String entityname = cb.get_option("entityname");
      String newname = cb.get_option("newname");
      ManagedObjectReference memor 
         = cb.getServiceUtil().GetDecendentMoRef(null, "ManagedEntity", entityname);
      if (memor == null) {
         Console.WriteLine("Unable to find a Managed Entity '" 
                           + entityname + "' in the Inventory");
         return;
      }
      else {
         try {
            ManagedObjectReference taskmor 
               =  cb.getConnection()._service.Rename_Task(memor, newname);
            String status = cb.getServiceUtil().WaitForTask(taskmor);
            if(status.Equals("failure")) {
               Console.WriteLine("Failure -: Managed Entity Cannot Be Renamed");
            }
            
            if(status.Equals("sucess")) {
               Console.WriteLine("ManagedEntity '" + entityname 
                                + "' renamed successfully.");          
            }
           
         }
         catch(Exception e) {
            Console.WriteLine("Error: " + e.ToString());
            e.StackTrace.ToString();
         }
      }
   }   
   private static OptionSpec[] constructOptions() {
      OptionSpec [] useroptions = new OptionSpec[2];
      useroptions[0] = new OptionSpec("entityname","String",1
                                     ,"Name of the virtual entity"
                                     ,null);
      useroptions[1] = new OptionSpec("newname","String",1,
                                      "New name of the virtual entity",
                                      null);
      return useroptions;
   }   
   public static void Main(String[] args)  {        
      Rename obj = new Rename();
      cb = AppUtil.AppUtil.initialize("Rename"
                              ,Rename.constructOptions()
                              ,args);
      cb.validate();
      cb.connect();
      obj.runOperation();
      cb.disConnect();
      Console.WriteLine("Press any key to exit: ");
      Console.Read();
      Environment.Exit(1);
   }
    }
}

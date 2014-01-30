using System;
using System.Web.Services;
using System.Collections;
using AppUtil;
using VimApi;
using System.Security.Principal;
using System.Net;

namespace HostPowerOps
{
    public class HostPowerOps
    {
        private static AppUtil.AppUtil cb = null;   
   
   private void PowerDownHost(String [] args) {
      String hostname = cb.get_option("hostname");
      ManagedObjectReference hmor  = 
            cb.getServiceUtil().GetDecendentMoRef(null,"HostSystem",hostname);
      ArrayList supportedVersions = VersionUtil.getSupportedVersions(cb.get_option("url"));    
          
      if(hmor != null) {
         if(cb.get_option("operation").Equals("reboot")) {
            ManagedObjectReference taskmor 
               = cb.getConnection()._service.RebootHost_Task(hmor,true);
            String result = cb.getServiceUtil().WaitForTask(taskmor);
            if(result.Equals("sucess")) {
               Console.WriteLine("Operation reboot host"
                                  +" completed sucessfully");
            }
         }
         else if(cb.get_option("operation").Equals("shutdown")) {
            ManagedObjectReference taskmor 
               = cb.getConnection()._service.ShutdownHost_Task(hmor,true);
            String result = cb.getServiceUtil().WaitForTask(taskmor);
            if(result.Equals("sucess")) {
               Console.WriteLine("Operation shutdown host"
                                  +" completed sucessfully");
            } 
         }
         else if(cb.get_option("operation").Equals("powerdowntostandby")) {
             if (VersionUtil.isApiVersionSupported(supportedVersions, "2.5"))
             {
                 Cookie cookie = cb.getConnection()._service.CookieContainer.GetCookies(
                          new Uri(cb.get_option("url")))[0];
                 HostPowerOpsV25.powerDownHost(hmor, args, supportedVersions, cookie);
             }
             else
             {
               Console.WriteLine("Operation Not Supported On ESX 3.0.1 and VC 2.0.1");               
            }
            
         }
      }
      else {
         Console.WriteLine("Host "+ cb.get_option("hostname")+" not found");
      }
   }
   
   private Boolean customValidation() {
      Boolean flag = true;
      String operation = cb.get_option("operation");
      if((!operation.Equals("reboot")) && (!operation.Equals("shutdown"))
         && (!operation.Equals("powerdowntostandby"))) {
         Console.WriteLine("Invalid operations ; [reboot | shutdown | powerdowntostandby]");
         flag = false;
      }
      return flag;
   }
   
   public static OptionSpec[] constructOptions() {
      OptionSpec [] useroptions = new OptionSpec[2];
      useroptions[0] = new OptionSpec("hostname","String",1
                                      ,"Name of the host"
                                      ,null);
      useroptions[1] = new OptionSpec("operation","String",1
                                      ,"Name of the operation"
                                      ,null);
      return useroptions;
   }

        public static void Main(String[] args)
        {
      HostPowerOps obj = new HostPowerOps();      
      cb = AppUtil.AppUtil.initialize("PowerDownHostToStandBy"
                                 ,HostPowerOps.constructOptions()
                                 ,args);
      Boolean valid = obj.customValidation();
      if(valid) {
         cb.connect();
         obj.PowerDownHost(args);
         cb.disConnect();  
       Console.WriteLine("Press any key to exit: ");
       Console.Read();
       Environment.Exit(1);
      }
   }
    }
}

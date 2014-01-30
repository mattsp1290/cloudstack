using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Services.Protocols;
using AppUtil;
using VimApi;

namespace Create
{
   public class Create
    {
       private static AppUtil.AppUtil cb = null;
   Log log = new Log();

   private String getParentName()  {
      return cb.get_option("parentName");
   }

   private String getItemType()  {
      return cb.get_option("itemType");
   }

   private String getItemName()  {
      return cb.get_option("itemName");
   }

   private String getparentType()  {
      return cb.get_option("parentType");
   }

   private static String getUserName()  {
      Console.WriteLine("Enter the userName for the host: ");      
      return (Console.ReadLine());
   }

   private static String getPassword()  {
      Console.WriteLine("Enter the password for the host: ");
      return (Console.ReadLine());
   }

   private static int getPort()  {
      Console.WriteLine("Enter the port for the host : "
            + "[Hit enter for default:] ");

      String portStr = Console.ReadLine();
      if ((portStr == null) || portStr.Length == 0)
         return 902;
      else
         return int.Parse(portStr);
   }

   private void doCreate()  {
      try {
         String type = getItemType();
         String name = getItemName();

         ManagedObjectReference taskMoRef = null;

         ManagedObjectReference folderMoRef = cb.getServiceUtil()
               .GetDecendentMoRef(null, "Folder", getParentName());

         if (folderMoRef == null) {
            Console.WriteLine("Parent folder '" + getParentName()
                  + "' not found");
         } else {
            if (type.Equals("Folder")) {
               cb.getConnection()._service.CreateFolder(folderMoRef,
                     name);
               Console.WriteLine("Sucessfully created::" + name);
            } else if (type.Equals("Datacenter")) {
               cb.getConnection()._service.CreateDatacenter(
                  folderMoRef, name);
            Console.WriteLine("Sucessfully created::" + name);
            } else if (type.Equals("Cluster")) {
               ClusterConfigSpec clusterSpec = new ClusterConfigSpec();
               cb.getConnection()._service.CreateCluster(folderMoRef,
                    name, clusterSpec);
               Console.WriteLine("Sucessfully created::" + name);
            } else if (type.Equals("Host-Standalone")) {
               HostConnectSpec hostSpec = new HostConnectSpec();
               hostSpec.hostName=name;
               hostSpec.userName=getUserName();
               hostSpec.password=getPassword();
               hostSpec.port=getPort();
               if (type.Equals("Host-Standalone")) {
                  taskMoRef = cb.getConnection()._service
                        .AddStandaloneHost_Task(folderMoRef, hostSpec,
                              true);

                  if (taskMoRef != null) {
                     String status = cb.getServiceUtil().WaitForTask(
                           taskMoRef);
                     if (status.Equals("sucess")) {
                        Console.WriteLine("Sucessfully created::"
                              + name);
                     } else {
                        Console.WriteLine("Host'" + name
                           + " not created::");
                     }
                  }
               }
            } else {
               Console.WriteLine("Unknown Type. Allowed types are:");
               Console.WriteLine(" Host-Standalone");
               Console.WriteLine(" Cluster");
               Console.WriteLine(" Datacenter");
               Console.WriteLine(" Folder");
            }
         }
      } catch (SoapException e) {
           if (e.Detail.FirstChild.LocalName.Equals("DuplicateNameFault"))
               {
                     Console.WriteLine("Managed Entity with the name already exists");
               }
               else if (e.Detail.FirstChild.LocalName.Equals("InvalidArgumentFault"))
               {
                     Console.WriteLine("Specification is invalid");
               }
               else if (e.Detail.FirstChild.LocalName.Equals("InvalidNameFault"))
               {
                   Console.WriteLine("Managed Entity Name is empty or too long");
               }
               else if (e.Detail.FirstChild.LocalName.Equals("RuntimeFault"))
               {
                     Console.WriteLine(e.Message.ToString() + " "
                     + "Either parent name or item name is invalid");
               }
                else if (e.Detail.FirstChild.LocalName.Equals("NotSupportedFault"))
               {
                     Console.WriteLine(e.Message.ToString());
               }
               else {
                   throw e;
               }
         
      }
   }
   private static OptionSpec[] constructOptions() {
      OptionSpec [] useroptions = new OptionSpec[3];
      useroptions[0] = new OptionSpec("parentName","String",1
                                     ,"Specifies the name of the parent folder"
                                     ,null);
      useroptions[1] = new OptionSpec("itemType","String",1,
                                      "Host-Standalone|Cluster| |Folder",
                                      null);
      useroptions[2] = new OptionSpec("itemName","String",1,
                                      "Name of the item being added: For Host " 
                                      + "please specify the name of the host machine.",
                                      null);
      return useroptions;
   }
   public static void Main (String[] args)  {
      Create app = new Create();
      cb = AppUtil.AppUtil.initialize("Create", Create.constructOptions(), args);
      cb.connect();
      app.doCreate();
      cb.disConnect();
      Console.WriteLine("Please enter to exit: ");
      Console.Read();
      
   }
    }
}

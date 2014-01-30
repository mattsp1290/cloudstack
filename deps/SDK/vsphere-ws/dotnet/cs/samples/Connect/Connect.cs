using System;
using System.Collections;
using AppUtil;
using VimApi;

namespace Connect {
    /// <summary>
    /// Simple client that only exercises the login/logout
    /// </summary>
    public class Connect{
      private static AppUtil.AppUtil cb = null;
      static VimService _service;
      static ServiceContent _sic;
      public Connect(string name, string[] args) { }     

      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      public static void Main(string[] args) {
         Connect cc = new Connect("Connect", args);
         cb = AppUtil.AppUtil.initialize("Connect"
                             , args);
         try
         {
             cb.connect();
             Console.WriteLine("Connected Successfully");
             Console.WriteLine("Server Time -: " +
                               cb.getConnection().Service.CurrentTime(cb.getConnection().ServiceRef));
             cb.disConnect();
         }catch(Exception e)
         {
             Console.WriteLine("Connection Unsuccessfull");
         }
         Console.WriteLine("Press enter to exit.");
         Console.Read();
      }
   }
}

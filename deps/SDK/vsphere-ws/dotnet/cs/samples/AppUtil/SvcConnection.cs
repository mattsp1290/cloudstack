using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml;
using System.Net;
using VimApi;

namespace AppUtil
{
   /// <summary>
   /// Connection Handler for WebService
   /// </summary>
   public class SvcConnection {

      public enum ConnectionState {
         Connected,
         Disconnected,
      }

      public VimService _service;
      protected ConnectionState _state;
      public ServiceContent _sic;
      protected ManagedObjectReference _svcRef;

      public event ConnectionEventHandler AfterConnect;
      public event ConnectionEventHandler AfterDisconnect;
      public event ConnectionEventHandler BeforeDisconnect;

      public SvcConnection(string svcRefVal) {
         _state = ConnectionState.Disconnected;
         String strComputerName;
         strComputerName = Environment.GetEnvironmentVariable("VI_IGNORECERT");

         if (strComputerName == "true" || strComputerName == "TRUE")
         {
             System.Net.ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();
         }
         else
         {
             System.Net.ServicePointManager.CertificatePolicy = new CertPolicy();
         }
         _svcRef = new ManagedObjectReference();
         _svcRef.type = "ServiceInstance";
         _svcRef.Value = svcRefVal;
      }

      /// <summary>
      /// Creates an instance of the VMA proxy and establishes a connection
      /// </summary>
      /// <param name="url"></param>
      /// <param name="username"></param>
      /// <param name="password"></param>
      public void Connect(string url, string username, string password) {
         if (_service != null) {
            Disconnect();
         }

         _service = new VimService();
         _service.Url = url;
         _service.Timeout = 600000; //The value can be set to some higher value also.
         _service.CookieContainer = new System.Net.CookieContainer();
      
        _sic = _service.RetrieveServiceContent(_svcRef);

         if (_sic.sessionManager != null) {
            _service.Login(_sic.sessionManager, username, password, null);
         }

         _state = ConnectionState.Connected;
         if (AfterConnect != null) {
            AfterConnect(this, new ConnectionEventArgs());
         }
      }

      public void SaveSession(String fileName, String urlString) {         
         Cookie cookie = _service.CookieContainer.GetCookies(
                         new Uri(urlString))[0];
         BinaryFormatter bf = new BinaryFormatter();
         Stream s = File.Open(fileName, FileMode.Create);
         bf.Serialize(s, cookie);
         s.Close();
      }

      public void LoadSession(String fileName, String urlString) {
          if (_service != null) {
              Disconnect();
          }
          _service = new VimService();
          _service.Url = urlString;
          _service.Timeout = 600000;
          _service.CookieContainer = new System.Net.CookieContainer();

          BinaryFormatter bf = new BinaryFormatter();
          Stream s = File.Open(fileName, FileMode.Open);
          Cookie c = bf.Deserialize(s) as Cookie;
          s.Close();
          _service.CookieContainer.Add(c);
          _sic = _service.RetrieveServiceContent(_svcRef);         
          _state = ConnectionState.Connected;
          if (AfterConnect != null) {
              AfterConnect(this, new ConnectionEventArgs());
          }
      }

      public VimService Service {
         get {
            return _service;
         }
      }

      public ManagedObjectReference ServiceRef {
         get {
            return _svcRef;
         }
      }

      public ServiceContent ServiceContent {
         get {
            return _sic;
         }
      }

      public ManagedObjectReference PropCol {
         get {
            return _sic.propertyCollector;
         }
      }

      public ManagedObjectReference Root {
         get {
            return _sic.rootFolder;
         }
      }

      public ConnectionState State {
         get {
            return _state;
         }
      }

      /// <summary>
      /// Disconnects the Connection
      /// </summary>
      public void Disconnect() {
         if (_service != null) {
            if (BeforeDisconnect != null) {
               BeforeDisconnect(this, new ConnectionEventArgs());
            }

            _service.Logout(_sic.sessionManager);

            _service.Dispose();
            _service = null;
            _sic = null;

            _state = ConnectionState.Disconnected;
            if (AfterDisconnect != null) {
               AfterDisconnect(this, new ConnectionEventArgs());
            }
         }
      }
   }

   public class ConnectionEventArgs : System.EventArgs {
   }

   public delegate void ConnectionEventHandler(object sender, ConnectionEventArgs e);

}

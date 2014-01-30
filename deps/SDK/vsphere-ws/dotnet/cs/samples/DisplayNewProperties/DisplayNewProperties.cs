using System;
using System.Web.Services;
using System.Collections;
using AppUtil;
using VimApi;
using System.Security.Principal;
using System.Net;

namespace DisplayNewProperties
{
    class DisplayNewProperties
    {
        private static AppUtil.AppUtil cb = null;
        static VimService _service;
        static ServiceContent _sic;
        private void displayProperties(String[] args)
        {
            _service = cb.getConnection()._service;
            _sic = cb.getConnection()._sic;
            String hostName = cb.get_option("hostname");
            ManagedObjectReference hmor = _service.FindByIp(_sic.searchIndex, null, hostName, false);
            if (hmor != null)
            {
                ArrayList supportedVersions = VersionUtil.getSupportedVersions(cb.get_option("url"));                
                if(VersionUtil.isApiVersionSupported(supportedVersions,"2.5"))
                {                    
                    Cookie cookie = cb._connection._service.CookieContainer.GetCookies(
                                    new Uri(cb.get_option("url")))[0];
                    
                    DisplayNewPropertiesV25.displayNewProperties(hmor, args, supportedVersions, cookie);
                }
                else
                {
                    Object[] vmProps = getProperties(hmor, new String[] { "name" });
                    String serverName = (String)vmProps[0];
                    Console.WriteLine("Server Name " + serverName);
                }
            }
            else
            {
                Console.WriteLine("Host Not Found");
            }
        }
        public static Object getObjectProperty(ManagedObjectReference moRef, String propertyName)
        {
            return getProperties(moRef, new String[] { propertyName })[0];
        }

        /*
         * getProperties --
         * 
         * Retrieves the specified set of properties for the given managed object
         * reference into an array of result objects (returned in the same oder
         * as the property list).
         */
        public static Object[] getProperties(ManagedObjectReference moRef, String[] properties)
        {
            // PropertySpec specifies what properties to
            // retrieve and from type of Managed Object
            PropertySpec pSpec = new PropertySpec();
            pSpec.type = moRef.type;
            pSpec.pathSet = properties;

            // ObjectSpec specifies the starting object and
            // any TraversalSpecs used to specify other objects 
            // for consideration
            ObjectSpec oSpec = new ObjectSpec();
            oSpec.obj = moRef;

            // PropertyFilterSpec is used to hold the ObjectSpec and 
            // PropertySpec for the call
            PropertyFilterSpec pfSpec = new PropertyFilterSpec();
            pfSpec.propSet = new PropertySpec[] { pSpec };
            pfSpec.objectSet = new ObjectSpec[] { oSpec };

            // retrieveProperties() returns the properties
            // selected from the PropertyFilterSpec


            ObjectContent[] ocs = new ObjectContent[20];
            ocs = _service.RetrieveProperties(_sic.propertyCollector, new PropertyFilterSpec[] { pfSpec });

            // Return value, one object for each property specified
            Object[] ret = new Object[properties.Length];

            if (ocs != null)
            {
                for (int i = 0; i < ocs.Length; ++i)
                {
                    ObjectContent oc = ocs[i];
                    DynamicProperty[] dps = oc.propSet;
                    if (dps != null)
                    {
                        for (int j = 0; j < dps.Length; ++j)
                        {
                            DynamicProperty dp = dps[j];
                            // find property path index
                            for (int p = 0; p < ret.Length; ++p)
                            {
                                if (properties[p].Equals(dp.name))
                                {
                                    ret[p] = dp.val;
                                }
                            }
                        }
                    }
                }
            }
            return ret;
        }
        public static OptionSpec[] constructOptions()
        {
            OptionSpec[] useroptions = new OptionSpec[1];
            useroptions[0] = new OptionSpec("hostname", "String", 1
                                            , "Name of the host"
                                            , null);
            return useroptions;
        }
        public static void Main(String[] args)
        {
            DisplayNewProperties obj = new DisplayNewProperties();
            cb = AppUtil.AppUtil.initialize("DisplayNewProperties"
                                    , DisplayNewProperties.constructOptions()
                                   , args);
            //cb.loadSession();
            cb.connect();
            //cb.saveSession("C:\\1.txt");
            obj.displayProperties(args);
            cb.disConnect();
           
        }
    }
}

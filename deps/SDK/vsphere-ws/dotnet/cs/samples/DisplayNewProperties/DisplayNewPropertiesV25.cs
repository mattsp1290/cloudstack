using System;
using System.Collections;
using AppUtil;
using Vim25Api;
using System.Net;

namespace DisplayNewProperties
{
    class DisplayNewPropertiesV25
    {
        static Vim25Api.VimService _service;
        static ServiceContent _sic;
        public static void displayNewProperties(VimApi.ManagedObjectReference hmor1, String[] args, ArrayList apiVersions, Cookie cookie)
        {
            ManagedObjectReference hmor = VersionUtil.convertManagedObjectReference(hmor1);
            ExtendedAppUtil ecb = null;
            ecb = ExtendedAppUtil.initialize("DisplayNewProperties"
                                             , DisplayNewProperties.constructOptions()
                                             , args);
            ecb.connect(cookie);
            _service = ecb.getServiceConnectionV25().Service;
            _sic = ecb.getServiceConnectionV25().ServiceContent;
            String hostName = ecb.get_option("hostname");
            Object[] vmProps = getProperties(hmor, new String[] { "name" });
            String serverName = (String)vmProps[0];
            Console.WriteLine("Server Name " + serverName);           
            
            HostRuntimeInfo runtimeInfo = (HostRuntimeInfo)getObjectProperty(hmor, "runtime");
            DateTime dt = runtimeInfo.bootTime;
            Console.WriteLine("Boot Time " + dt.ToString());

            String powerState = runtimeInfo.powerState.ToString();
            Console.WriteLine("Power State " + powerState);

            HostConfigInfo configInfo
               = (HostConfigInfo)getObjectProperty(hmor, "config");
            String timeZone = configInfo.dateTimeInfo.timeZone.key;
            Console.WriteLine("Time Zone " + timeZone);

            Boolean flag = VersionUtil.isApiVersionSupported(apiVersions, "4.0");
            if (flag)
            {
                Console.WriteLine("\nProperties added in 4.0 VI API\n");
                Object objFlag = getObjectProperty(hmor, "capability.ipmiSupported");
                if (objFlag != null)
                {
                    Boolean infoFlag = (Boolean)objFlag;
                    Console.WriteLine("IPMI Supported  " + infoFlag);
                }
                else
                {
                    Console.WriteLine("\nIPMI Flag not set");
                }
                objFlag = getObjectProperty(hmor, "capability.tpmSupported");
                if (objFlag != null)
                {
                    Boolean infoFlag = (Boolean)objFlag;
                    Console.WriteLine("TPM Supported  " + infoFlag);
                }
                else
                {
                    Console.WriteLine("\nTPM Flag not set");
                }
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
    }
}

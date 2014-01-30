using System;
using System.Collections;
using AppUtil;
using Vim25Api;
using System.Net;

namespace CIMInfo
{
    class CIMUtil
    {
        static VimService _service;
        static ServiceContent _sic; 
        public static string getCIMSessionId (VimApi.ManagedObjectReference hmor1, String[] args, Cookie cookie)
        {
            ExtendedAppUtil ecb = null;
            try
            {
                ecb = ExtendedAppUtil.initialize("GetCIMSessioId"
                                                 , args);
                ecb.connect(cookie);
                _service = ecb.getServiceConnectionV25().Service;
                _sic = ecb.getServiceConnectionV25().ServiceContent;
                ManagedObjectReference hmor = VersionUtil.convertManagedObjectReference(hmor1);
                string sessionId = _service.AcquireCimServicesTicket(hmor).sessionId;
                return sessionId;
            }
            catch (Exception e)
            {
                ecb.log.LogLine("Get GetSessionID : Failed Connect");
                throw e;
            }
            finally
            {
                ecb.log.LogLine("Ended GetSessionID");
                ecb.log.Close();
            }
        }
    }
}

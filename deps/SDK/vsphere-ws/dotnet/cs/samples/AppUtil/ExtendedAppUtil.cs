using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace AppUtil
{
    public class ExtendedAppUtil: AppUtil
    {
        public SvcConnectionV25 _connection;
        private ServiceUtilV25 _svcUtil;

        public static ExtendedAppUtil initialize(String name, OptionSpec[] userOptions,
                                                 String[] args)
        {
            ExtendedAppUtil cb = new ExtendedAppUtil(name);
            if (userOptions != null)
            {
                cb.addOptions(userOptions);
                cb.parseInput(args);
                cb.validate();
            }
            else
            {
                cb.parseInput(args);
                cb.validate();
            }
            return cb;
        }

        public static ExtendedAppUtil initialize(String name, String[] args)
        {
            ExtendedAppUtil cb = initialize(name, null, args);
            return cb;
        }

        public ExtendedAppUtil(String name):base(name)
        {
            setup(name);
        }

        public void setup(String name)
        {
            _svcUtil = new ServiceUtilV25();
            _connection = new SvcConnectionV25("ServiceInstance");
        }
        public void connect()
        {
            log.LogLine("Started ");
            try
            {
                initConnection();
                getServiceUtilV25().ClientConnect();
            }
            catch (Exception e)
            {
                log.LogLine("Exception running : " + getAppName());
                getUtil().LogException(e);
                log.Close();
                throw new ArgumentHandlingException("Exception running : "
                                                                       + getAppName());
            }
        }
        public void connect(Cookie cookie)
        {
            log.LogLine("Started ");
            try
            {
                initConnection();
                getServiceUtilV25().ClientConnect(cookie);
            }
            catch (Exception e)
            {
                log.LogLine("Exception running : " + getAppName());
                getUtil().LogException(e);
                log.Close();
                throw new ArgumentHandlingException("Exception running : "
                                                                       + getAppName());
            }
        }
        public void initConnection()
        {
            getServiceUtilV25().Init(this);
        }

        public ServiceUtilV25 getServiceUtilV25()
        {
            return _svcUtil;
        }
        public SvcConnectionV25 getServiceConnectionV25()
        {
            return _connection;
        }
        public void disConnect()
        {
            log.LogLine("Ended " + getAppName());
            try
            {
                getServiceUtilV25().ClientDisconnect();
            }
            catch (Exception e)
            {
                log.LogLine("Exception running : " + getAppName());
                getUtil().LogException(e);
                log.Close();
                throw new ArgumentHandlingException("Exception running : "
                                                                       + getAppName());
            }
        }
        public void validate()
        {
            // DO NOTHING
        }
    }
}

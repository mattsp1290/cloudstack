using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Web.Services.Protocols;
using VimApi;


namespace AppUtil
{
    /// <summary>
    /// useful Client utility functions.
    /// </summary>
    public class ClientUtil
    {

        private AppUtil _ci;

        public ClientUtil(AppUtil c)
        {
            _ci = c;
        }
       

       /*
    * Prompt user for an integer value
    */
   public int getIntInput(String prompt, int defaultVal)
    {
       string input = getStrInput(prompt);
       if (input == null || input.Length == 0)
           return defaultVal;
       else
           return Int32.Parse(input);
   }

   /*
    * Prompt user for an integer value
    */
   public long getLongInput(String prompt, long defaultVal)
   {
       string input = getStrInput(prompt);
       if (input == null || input.Length == 0)
           return defaultVal;
       else
           return long.Parse(input);
   }

   public String getStrInput(String prompt) {
       Console.Write(prompt);
      TextReader reader = Console.In;
       return reader.ReadLine();
   }
   
   ///**
   // * Log the Exception - fault or otherwise
   // * TODO: Enhance to handle different detail contents.
   // */
   //public void logException(Exception e) {
   //    if (e.GetType() == System.Type.GetType("SoapException"))
   //    {
   //        SoapException mf = (SoapException)e;
   //        logFault(mf);
   //    }
   //    else
   //    {
   //        _cb.getLog().logLine(
   //           "Caught Exception : " +
   //           " Exception : " + e.GetType() +
   //           " Message : " + e.Message() +
   //           " StackTrace : ");
   //        e.StackTrace();
   //    }
   //}
   ///**
   //    * Log a fault.
   //    */
   //public void logFault(MethodFault mf)
   //{
   //    _ci.log.LogLine("Caught Fault - " +
   //          "\n Type : " + mf.dynamicType.ToString() +
   //          "\n Actor : " + mf.dynamicProperty.ToString() +
   //          "\n Code : " + mf.getFaultNode() +
   //          "\n Reason : " + mf.getFaultReason() +
   //          "\n Fault String : " + mf.getFaultString());
   //}
        //TODO: Enhance to handle different detail contents.
        public void LogException(Exception e)
        {
            if (e.GetType() == System.Type.GetType("SoapException"))
            {
                SoapException se = (SoapException)e;
                _ci.log.LogLine("Caught SoapException - " +
                   " Actor : " + se.Actor +
                   " Code : " + se.Code +
                   " Detail XML : " + se.Detail.OuterXml);
            }
            else
            {
                _ci.log.LogLine("Caught Exception : " +
                   " Name : " + e.GetType().Name +
                   " Message : " + e.Message +
                   " Trace : " + e.StackTrace);
            }
        }

    }
}

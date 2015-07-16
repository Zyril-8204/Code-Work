//------------------------------------------------------------------------------
// MSMQListenerService.cs
//------------------------------------------------------------------------------
// Windows service to receive messages from EndStations and populate the database
//
// Message queues are currently discovered using msmqConnectionString found in App.config
//  (example:  FormatName:Direct=TCP:10.20.200.24\private$\SITA_IMMS_GMSqueue)
//
// The following configuration details can be found in SITA.IMMS.MSMQListenerService/App.Config 
//      msmqConnectionString      - FormatName location of MSMQ resource
//      msmqNumListeners          - Number of listeners on the source queue
//------------------------------------------------------------------------------
// last modified: 10/16/2014
//------------------------------------------------------------------------------
using System;
using System.IO;
using System.Configuration;
using System.ServiceProcess;
using SITA.IMMS.Framework;

namespace SITA.IMMS.MSMQListenerService
{
    public partial class MSMQListenerService : ServiceBase
    {
        int msmqNumListeners;
        string msmqConnectionString;
        MSMQListener[] dataListeners;

        /// <summary>
        /// Get AppSettings from the app.config file
        /// Set up MSMQ listeners to receive
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListenerService.  Method OnStart.");
            base.OnStart(args);

            msmqConnectionString = ConfigurationManager.AppSettings["msmqConnectionString"];            
            msmqNumListeners = Convert.ToInt32(ConfigurationManager.AppSettings["msmqNumListeners"]);                        
            IMMSLogger.LogInfo("Beginning MSMQ service with source queue: "+msmqConnectionString+", with " + msmqNumListeners + " listeners");

            dataListeners = new MSMQListener[msmqNumListeners];

            for (int i = 0; i < msmqNumListeners; i++)
            {
                dataListeners[i] = new MSMQListener(msmqConnectionString);            
            }
            IMMSLogger.LogDebug("Method End. Class MSMQListenerService.  Method OnStart.");
        }

        /// <summary>
        /// Dispose of all MSMQ listeners
        /// </summary>
        protected override void OnStop()
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListenerService.  Method OnStop.");
            base.OnStop();
            for (int i = 0; i < msmqNumListeners; i++)
            {
                dataListeners[i].Dispose(true);
            }
            IMMSLogger.LogDebug("Method End. Class MSMQListenerService.  Method OnStop.");
        }
    }
}

//------------------------------------------------------------------------------
// MSMQListener.cs
//------------------------------------------------------------------------------
// Provides functionality for message queue interaction
// Sets up an event handler to receive, validate, and serialize xml packets 
//     as they become available on the queue
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
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Messaging;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using SITA.IMMS.Data.DataModel;
using SITA.IMMS.Entities;
using SITA.IMMS.Framework;

namespace SITA.IMMS.MSMQListenerService
{
    public class MSMQListener : IDisposable
    {
        [Serializable]
        public enum Destination { Status, Usage }
        
        #region Private Vars
        private MessageQueue queue;        
        private Destination dataDestination;

        //Message Queue metrics
        private int numPacketsReceived;
        private int numPacketsProcessed;
        private int numInvalidPackets;

        private XmlSerializer outerSerializer;
        private XmlSerializer innerSerializer;
        private XmlReaderSettings innerReadSettings;        
               
        private bool outerValidationError;
        private bool innerValidationError;
        private string outerValidationMessage = string.Empty;
        private string innerValidationMessage = string.Empty;

        private const string loginMessage = "1";
        private const string logoutMessage = "2";
        private const string deviceStatusMessage = "16";
        private const string turnstileStatusMessage = "15";
        private const string validationStatusMessage = "14";
        private const string boardingPassMessage = "11";

        private string [] turnstileStatuses;    

        private static readonly List<string> statusMessageTypes = new List<string>()
        { deviceStatusMessage, turnstileStatusMessage, loginMessage, logoutMessage };
        
        private static readonly List<string> usageMessageTypes = new List<string>()
        { validationStatusMessage, boardingPassMessage };
        #endregion

        /// <summary>
        /// Exposes functionality to Create/Connect to queue, set up asynchronous recv and xml validation
        /// </summary>
        /// <param name="msmqConnectionString"> name of machine where MSMQ is hosted </param>
        public MSMQListener(string msmqConnectionString)
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListener.  Method MSMQListener.");
            try
            {                                
                ConnectToMSMQ(msmqConnectionString);
                turnstileStatuses = GetPossibleTurnstileStatuses();
                SetupXmlValidation();                

                queue.ReceiveCompleted += new ReceiveCompletedEventHandler(HandleMSMQRecv);
                queue.BeginReceive();
                IMMSLogger.LogInfo("Listening to message queue...");
            }
            catch (Exception ex)
            {                
                IMMSLogger.LogError("Exception occurred when setting up MSMQ listener");
                IMMSLogger.LogError(ex.Message);
                throw;
            }
            IMMSLogger.LogDebug("Method End. Class MSMQListener.  Method MSMQListener.");
        }

        /// <summary>
        /// Make a database query to retrieve all of the possible turnstile statuses
        /// </summary>
        /// <returns></returns>
        private string[] GetPossibleTurnstileStatuses()
        {
            IMMSLogger.LogDebug("Method Beginning.  Class MSMQListener.  Method GetPossibleTurnstileStatuses");
            List<string> statuses = new List<string>();
            using (var context = new IMMSEntities())
            {
                try
                {
                    var result = context.Database.SqlQuery<string>("usp_GetPossibleTurnstileStatuses");
                    foreach ( string status in result )
                    {
                        statuses.Add(status);
                    }
                }
                catch (Exception ex)
                {
                    IMMSLogger.LogError(ex.Message);
                    throw;
                }                
            }
            IMMSLogger.LogDebug("Method End.  Class MSMQListener.  Method GetPossibleTurnstileStatuses");
            return statuses.ToArray();
        }        

        /// <summary>
        /// Connect to the message described by msmqConnectionString
        /// </summary>
        /// <param name="msmqConnectionString"></param>
        private void ConnectToMSMQ(string msmqConnectionString)
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListener.  Method ConnectToMSMQ.");          
            IMMSLogger.LogInfo("Attempting to connect to source queue: " + msmqConnectionString);

            try
            {
                queue = new MessageQueue(msmqConnectionString);                
            }
            catch (Exception ex)
            {
                IMMSLogger.LogError("queue was not found: "+ex.Message);
                throw; 
            }
            IMMSLogger.LogDebug("Method End. Class MSMQListener.  Method ConnectToMSMQ.");          
        }

        #region Message Handling Methods
        /// <summary>
        /// Take message off queue and process it if it's valid
        ///     -send raw xml to database
        ///     -derive status/usage data and add to database
        ///     -derive audit details and add to database
        ///     -generate alerts data based off message contents
        /// </summary>
        /// <param name="source"></param>
        /// <param name="asyncResult"></param>
        private void HandleMSMQRecv(object source, ReceiveCompletedEventArgs asyncResult)
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListener.  Method HandleMSMQRecv.");          
            IMMSLogger.LogInfo("Receiving packet from queue: " + queue.Path);

            Message msg = new Message();
            try
            {                
                msg.Formatter = new XmlMessageFormatter(new Type[] { typeof(EventNotification), typeof(string) });
                msg = queue.EndReceive(asyncResult.AsyncResult);

                numPacketsReceived++;

                ProcessMessage(msg);                
            }
            catch (Exception ex)
            {
                IMMSLogger.LogError("Dropping message... Exception occurred when processing message: "+ex.Message);                
                PrepareRecv();
            }
            finally
            {
                msg.Dispose();
            }
            IMMSLogger.LogDebug("Method End. Class MSMQListener.  Method HandleMSMQRecv.");          
        }

        /// <summary>
        /// Validate contents of message against schema
        /// Determine type of message and process accordingly
        /// </summary>
        /// <param name="msg"></param>
        private void ProcessMessage(Message msg)
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListener.  Method ProcessMessage.");          
            EventNotification outerData = null;
            DeviceData innerData = null;
            
            string msgContent;
            try
            {
                outerData = msg.Body as EventNotification;
                msgContent = outerData.ToString();
            }
            catch(Exception ex)
            {
                IMMSLogger.LogError("Unable to deserialize message body: "+ex.Message);                
                throw;
            }
                                 
            if (statusMessageTypes.Contains(outerData.MessageType))
            {
                dataDestination = Destination.Status;
            }
            else if (usageMessageTypes.Contains(outerData.MessageType))
            {
                dataDestination = Destination.Usage;
                innerData = DeserializeInnerMessage(outerData.StatsData.EncryptedInfoData);
            }

            if (!statusMessageTypes.Contains(outerData.MessageType) &&
                 !usageMessageTypes.Contains(outerData.MessageType))
            {
                IMMSLogger.LogInfo("Dropping message because is not an applicable type (type "+outerData.MessageType+")");
                outerValidationError = true;
            }

            int endStationId = GetEndStationId(outerData.WorkstationName);

            if (outerValidationError || innerValidationError)
            {
                numInvalidPackets++;
                IMMSLogger.LogInfo("Dropping invalid message");
            }
            else
            {
                IMMSLogger.LogInfo("Message is valid, processing incoming data");
                                
                DumpRawXmlToDatabase(msg);                

                HandleMessage(endStationId, innerData, outerData);

                numPacketsProcessed++;
            }            
            PrepareRecv();
            IMMSLogger.LogDebug("Method End. Class MSMQListener.  Method ProcessMessage.");          
        }

        /// <summary>
        /// Reset validation variables, prepare to receive again from the queue
        /// </summary>
        private void PrepareRecv()
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListener.  Method PrepareRecv.");          
            outerValidationError = false;
            outerValidationMessage = string.Empty;

            innerValidationError = false;
            innerValidationMessage = string.Empty;

            IMMSLogger.LogInfo("Preparing to receive again");
            queue.BeginReceive();
            IMMSLogger.LogDebug("Method End. Class MSMQListener.  Method PrepareRecv.");          
        }
        
        /// <summary>
        /// Call the appropriate message handler function based off the outerData.MessageType
        /// </summary>
        /// <param name="endStationId"></param>
        /// <param name="innerData"></param>
        /// <param name="outerData"></param>
        private void HandleMessage(int endStationId, DeviceData innerData, EventNotification outerData)
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListener.  Method HandleMessage.");          
            if (outerData.MessageType.Equals(loginMessage))
            {
                HandleLoginMessage(endStationId, outerData);
            }
            else if (outerData.MessageType.Equals(logoutMessage))
            {
                HandleLogoutMessage(endStationId, outerData);
            }
            else if (outerData.MessageType.Equals(deviceStatusMessage))
            {
                HandleDeviceStatusMessage(endStationId, outerData);
            }
            else if (outerData.MessageType.Equals(turnstileStatusMessage))
            {
                HandleTurnstileStatusMessage(endStationId, outerData);
            }
            else if (outerData.MessageType.Equals(validationStatusMessage))
            {
                HandleValidationStatusMessage(endStationId, innerData);
            }
            else if (outerData.MessageType.Equals(boardingPassMessage))
            {
                HandleBoardingPassMessage(endStationId, innerData);
            }
            IMMSLogger.LogDebug("Method End. Class MSMQListener.  Method HandleMessage.");          
        }

        /// <summary>
        /// Handle login message (type 1)
        /// Update EndStationTransaction table with the endstation's logged in group
        /// Log change into audit table
        /// </summary>
        /// <param name="endStationId"></param>
        /// <param name="outerData"></param>
        private void HandleLoginMessage(int endStationId, EventNotification outerData)
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListener.  Method HandleLoginMessage.");
            IMMSLogger.LogInfo("Processing login message (type 1)");

            string id = PopulateStatusData(endStationId, outerData.WorkstationName, "LoggedIn", outerData.AirlineGroupName);
            LogAuditData("EndStationTransaction", "LoggedIn", outerData.AirlineGroupName, id);
            IMMSLogger.LogDebug("Method End. Class MSMQListener.  Method HandleLoginMessage.");
        }

        /// <summary>
        /// Handle login message (type 2)
        /// Update EndStationTransaction table with the endstation's logged in group
        /// Log change into audit table
        /// </summary>
        /// <param name="endStationId"></param>
        /// <param name="outerData"></param>
        private void HandleLogoutMessage(int endStationId, EventNotification outerData)
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListener.  Method HandleLogoutMessage.");
            IMMSLogger.LogInfo("Processing logout message (type 2)");

            string id = PopulateStatusData(endStationId, outerData.WorkstationName, "LoggedIn", null);
            LogAuditData("EndStationTransaction", "LoggedIn", null, id);
            IMMSLogger.LogDebug("Method End. Class MSMQListener.  Method HandleLogoutMessage.");
        }

        /// <summary>
        /// Handle turnstile status message (type 15)
        /// Update EndStationTransaction table with the endstation's new turnstile status
        /// Log change into audit table
        /// </summary>
        /// <param name="endStationId"></param>
        /// <param name="outerData"></param>
        private void HandleTurnstileStatusMessage(int endStationId, EventNotification outerData)
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListener.  Method HandleTurnstileStatusMessage.");
            IMMSLogger.LogInfo("Processing turnstile status message (type 15)");

            foreach (string statusVal in turnstileStatuses)
            {
                if (outerData.DeviceResponses.Message.ToUpper().Contains(statusVal))
                {
                    string id = PopulateStatusData(endStationId, outerData.WorkstationName, "Turnstile", statusVal);
                    LogAuditData("EndStationTransaction", "Turnstile", statusVal, id);
                    GenerateStatusAlertsData(outerData.WorkstationName, "Turnstile", statusVal);
                    break;
                }
            }
            
            IMMSLogger.LogDebug("Method End. Class MSMQListener.  Method HandleTurnstileStatusMessage.");
        }

        /// <summary>
        /// Handle device status message (type 16)
        /// Update EndStationTransaction table with the endstation's new device status
        /// Log change into audit table
        /// </summary>
        /// <param name="endStationId"></param>
        /// <param name="outerData"></param>
        private void HandleDeviceStatusMessage(int endStationId, EventNotification outerData)
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListener.  Method HandleDeviceStatusMessage.");
            IMMSLogger.LogInfo("Processing device status message (type 16)");

            string id = PopulateStatusData(endStationId, outerData.WorkstationName, "Device", outerData.DeviceStatus.ConnectionStatus);
            LogAuditData("EndStationTransaction", "Device", outerData.DeviceStatus.ConnectionStatus, id);
            GenerateStatusAlertsData(outerData.WorkstationName, "Device", outerData.DeviceStatus.ConnectionStatus);
            IMMSLogger.LogDebug("Method End. Class MSMQListener.  Method HandleDeviceStatusMessage.");
        }

        /// <summary>
        /// If the corresponding Validation message has already been processed,  
        ///     get that message's data from the TempUsageData table
        ///     (based on DeviceData.ATBData.HashData) and insert usage information into UsageTransaction table
        /// Otherwise, insert boarding pass message data into the TempUsageData table
        /// </summary>
        /// <param name="endStationId"></param>
        /// <param name="innerData"></param>
        private void HandleBoardingPassMessage(int endStationId, DeviceData innerData)
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListener.  Method HandleBoardingPassMessage.");
            IMMSLogger.LogInfo("Processing boarding pass message (type 11)");

            TempUsageData messageData = new TempUsageData();
            messageData.HashData = innerData.ATBData.HashData;
            messageData.BarcodeId = innerData.ATBData.BarcodeID;
            messageData.AirlineId = innerData.ATBData.AirlineId;
            messageData.FlightDateAsJulianDate = innerData.ATBData.FlightDateAsJulianDate;
            messageData.FlightNumber = innerData.ATBData.FlightNumber;

            TempUsageData existingData = GetExistingUsageData(messageData.HashData);

            if (existingData.HashData != null)
            {
                messageData.IsSuccess = existingData.IsSuccess;
                messageData.Reason = existingData.Reason;

                InsertIntoUsageTransaction(endStationId, messageData);
                DeleteFromTempUsage(messageData);
            }
            else
            {
                InsertIntoTempUsage(messageData);
            }
            IMMSLogger.LogDebug("Method End. Class MSMQListener.  Method HandleBoardingPassMessage.");
        }

        /// <summary>
        /// If the corresponding boarding pass message has already been processed,  
        ///     get that message's data from the TempUsageData table
        ///     (based on DeviceData.ATBData.HashData) and insert usage information into UsageTransaction table
        /// Otherwise, insert validation message data into the TempUsageData table
        /// </summary>
        /// <param name="endStationId"></param>
        /// <param name="innerData"></param>
        private void HandleValidationStatusMessage(int endStationId, DeviceData innerData)
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListener.  Method HandleValidationStatusMessage.");
            IMMSLogger.LogInfo("Processing validation message (type 14)");

            TempUsageData messageData = new TempUsageData();
            messageData.HashData = innerData.ValidationResult.HashData;
            messageData.IsSuccess = innerData.ValidationResult.IsSuccess ? 1 : 0;
            messageData.Reason = null;
            if (messageData.IsSuccess == 0)
            {
                //Find the element which caused the failure
                foreach (DeviceDataValidationResultPFMValidatorResult validationItem in innerData.ValidationResult.ValidationsPerformed)
                {
                    if (validationItem.IsSuccess == false && validationItem.ResultText != null)
                    {
                        messageData.Reason = validationItem.ResultText;
                    }
                }
            }

            TempUsageData existingData = GetExistingUsageData(messageData.HashData);

            if (existingData.HashData != null)
            {
                messageData.AirlineId = existingData.AirlineId;
                messageData.BarcodeId = existingData.BarcodeId;
                messageData.FlightDateAsJulianDate = existingData.FlightDateAsJulianDate;
                messageData.FlightNumber = existingData.FlightNumber;

                InsertIntoUsageTransaction(endStationId, messageData);
                DeleteFromTempUsage(messageData);
            }
            else
            {
                InsertIntoTempUsage(messageData);
            }
            IMMSLogger.LogDebug("Method End. Class MSMQListener.  Method HandleValidationStatusMessage.");
        }
        #endregion

        #region Database Operations
        /// <summary>
        /// Call a stored procedure to retreive the EndStationKey associated with an EndStationName
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private int GetEndStationId(string name)
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListener.  Method GetEndStationId.");
            int endStationId = -1;
            using (var context = new IMMSEntities())
            {
                try
                {
                    IEnumerable<EndStation> result = new List<EndStation>();

                    SqlParameter [] sqlParams = {new SqlParameter("@EndStationName", name)};
                    result = context.Database.SqlQuery<EndStation>("usp_SearchEndStationKey @EndStationName", sqlParams);
                    foreach (EndStation es in result)
                    {
                        endStationId = es.EndStationKey;
                    }
                }
                catch (Exception ex)
                {
                    IMMSLogger.LogError("Exception thrown when attempting to get EndStationID");
                    IMMSLogger.LogError(ex.Message);
                    throw;
                }
            }
            if (endStationId == -1)
            {
                outerValidationError = true;
                IMMSLogger.LogInfo("Message references nonexistent EndStation: "+name);
            }
            IMMSLogger.LogDebug("Method End. Class MSMQListener.  Method GetEndStationId.");
            return endStationId;
        }

        /// <summary>
        /// Get the rowGUID in EndStationTransaction corresponding to (endStation,statusCode,statusValue)
        /// </summary>
        /// <param name="endStationName"></param>
        /// <param name="statusCode"></param>
        /// <param name="statusValue"></param>
        /// <returns></returns>
        private static string GetStatusGUID(string endStationName, string statusCode, string statusValue)
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListener.  Method GetStatusGUID.");
            Guid result = Guid.NewGuid();
            using (var context = new IMMSEntities())
            {                             
                SqlParameter statusValueSqlParam = statusValue == null ?
                    new SqlParameter("@StatusValue", DBNull.Value) : new SqlParameter("@StatusValue", statusValue);
                
                SqlParameter[] sqlParams = new SqlParameter[]
                {
                    new SqlParameter("@EndStationName", endStationName),
                    new SqlParameter("@StatusCode", statusCode),                    
                    statusValueSqlParam
                };

                try
                {
                    var query_result = context.Database.SqlQuery<Guid>("usp_GetStatusTransactionGUID @EndStationName, @StatusCode, @StatusValue", sqlParams);
                    foreach (var item in query_result)
                    {
                        result = (Guid)item;
                    }
                    IMMSLogger.LogInfo("Status GUID for EndStation: " + endStationName + " and status: " + statusCode + " is: " + result.ToString());
                }
                catch (Exception ex)
                {
                    IMMSLogger.LogError(ex.Message);
                    throw;
                }                
            }
            IMMSLogger.LogDebug("Method End. Class MSMQListener.  Method GetStatusGUID.");
            return result.ToString();
        }        

        /// <summary>
        /// Inserts (or updates) derived data into EndStationTransaction table
        /// Returns the rowGUID (in string format) of the inserted (or updated) row.
        /// </summary>
        /// <param name="endStationId"></param>
        /// <param name="statusCode"></param>
        /// <param name="statusValue"></param>
        private string PopulateStatusData(int endStationId, string endStationName, string statusCode, string statusValue)
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListener.  Method PopulateStatusData.");
            string id;
            using (var context = new IMMSEntities())
            {
                SqlParameter statusValueSqlParam = statusValue == null ?
                    new SqlParameter("@StatusValue", DBNull.Value) : new SqlParameter("@StatusValue", statusValue);

                SqlParameter [] sqlParams = 
                {
                    new SqlParameter("@EndStationKey", endStationId),
                    new SqlParameter("@StatusCode", statusCode),
                    statusValueSqlParam
                };

                try
                {
                    context.Database.ExecuteSqlCommand("usp_UpdateStatusData @EndStationKey, @StatusCode, @StatusValue",sqlParams);
                    id = GetStatusGUID(endStationName, statusCode, statusValue);
                }
                catch (Exception ex)
                {         
                    IMMSLogger.LogError("Exception encounterd when attempting to populate derived login data");
                    IMMSLogger.LogError(ex.Message);
                    throw;
                }
            }
            IMMSLogger.LogDebug("Method End. Class MSMQListener.  Method PopulateStatusData.");
            return id;
        }

        /// <summary>
        /// Make call to database stored procedure to generate alert data based off status data
        /// </summary>
        /// <param name="data"></param>
        private static void GenerateStatusAlertsData(string endstationName, string statusCode, string statusValue)
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListener.  Method GenerateStatusAlertsData.");

            //Generate Xml to send to stored procedure
            StringBuilder builder = new StringBuilder();
            builder.Append("<EndStations>");
            builder.Append("<EndStation>");
            builder.Append("<EndStationCode>" + endstationName + "</EndStationCode>");
            builder.Append("<StatusIndicator>" + statusCode + "</StatusIndicator>");
            builder.Append("<StatusIndicatorValue>" + statusValue + "</StatusIndicatorValue>");
            builder.Append("</EndStation>");
            builder.Append("</EndStations>");

            using (var context = new IMMSEntities())
            {
                try
                {
                    SqlParameter [] xmlSqlParam = {new SqlParameter("@XmlStatus", builder.ToString())};
                    context.Database.ExecuteSqlCommand("usp_InsertStatusAlerts @XmlStatus", xmlSqlParam);
                }
                catch (Exception ex)
                {                 
                    IMMSLogger.LogError("Exception encountered when attempting to generate status alerts");
                    IMMSLogger.LogError(ex.Message);
                    throw;
                }
            }
            IMMSLogger.LogDebug("Method End. Class MSMQListener.  Method GenerateStatusAlertsData.");
        }

        /// <summary>
        /// Temporarily store the messageData's usage data in the TempUsageData table until it's hashdata counterpart arrives
        /// </summary>
        /// <param name="messageData"></param>
        private static void InsertIntoTempUsage(TempUsageData messageData)
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListener.  Method InsertIntoTempUsage.");

            SqlParameter airlineIdSqlParam = messageData.AirlineId == null ?
                new SqlParameter("@AirlineId", DBNull.Value) : new SqlParameter("@AirlineId", messageData.AirlineId);

            SqlParameter flightDateSqlParam = messageData.FlightDateAsJulianDate == null ?
                new SqlParameter("@FlightDateAsJulianDate", DBNull.Value) : new SqlParameter("@FlightDateAsJulianDate", messageData.FlightDateAsJulianDate);

            SqlParameter barcodeIdSqlParam = messageData.BarcodeId == null ?
                new SqlParameter("@BarcodeId", DBNull.Value) : new SqlParameter("@BarcodeId", messageData.BarcodeId);

            SqlParameter isSuccessSqlParam = new SqlParameter("@IsSuccess", messageData.IsSuccess);

            SqlParameter reasonSqlParam = messageData.Reason == null ?
                new SqlParameter("@Reason", DBNull.Value) : new SqlParameter("@Reason", messageData.Reason);

            SqlParameter flightNumberSqlParam = messageData.FlightNumber == null ?
                new SqlParameter("@FlightNumber", DBNull.Value) : new SqlParameter("@FlightNumber", messageData.FlightNumber);

            SqlParameter[] sqlParams = 
            {
                new SqlParameter("@HashData", messageData.HashData),
                airlineIdSqlParam,
                flightDateSqlParam,
                barcodeIdSqlParam,
                isSuccessSqlParam,
                reasonSqlParam,
                flightNumberSqlParam
            };

            try
            {
                using (var context = new IMMSEntities())
                {
                    context.Database.ExecuteSqlCommand("usp_InsertIntoTempUsage @HashData, @AirlineId,  @FlightDateAsJulianDate, @BarcodeId, @IsSuccess, @Reason, @FlightNumber", sqlParams);
                }
            }
            catch (Exception ex)
            {
                IMMSLogger.LogError("Exception occurred in InsertIntoTempUsage");
                IMMSLogger.LogError(ex.Message);
                throw;
            }
            IMMSLogger.LogDebug("Method End. Class MSMQListener.  Method InsertIntoTempUsage.");
        }

        /// <summary>
        /// Remove entry in TempUsageData table after its hashdata counterpart arrives
        /// </summary>
        /// <param name="messageData"></param>
        private static void DeleteFromTempUsage(TempUsageData messageData)
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListener.  Method DeleteFromTempUsage.");
            using (var context = new IMMSEntities())
            {
                SqlParameter [] hashDataSqlParam = {new SqlParameter("@HashData", messageData.HashData)};
                try
                {
                    context.Database.ExecuteSqlCommand("usp_DeleteFromTempUsage @HashData", hashDataSqlParam);
                }
                catch (Exception ex)
                {
                    IMMSLogger.LogError("Exception occurred in DeleteFromTempUsage");
                    IMMSLogger.LogError(ex.Message);
                    throw;
                }
            }            
            IMMSLogger.LogDebug("Method End. Class MSMQListener.  Method DeleteFromTempUsage.");
        }

        /// <summary>
        /// Retrieve the entry in TempUsageData associated with hashData (if it exists)
        /// </summary>
        /// <param name="hashData"></param>
        /// <returns></returns>
        private static TempUsageData GetExistingUsageData(string hashData)
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListener.  Method GetExistingUsageData.");
            TempUsageData existingData = new TempUsageData();
            using (var context = new IMMSEntities())
            {
                SqlParameter [] hashDataSqlParam = {new SqlParameter("@HashData", hashData)};
                try
                {
                    var result = context.Database.SqlQuery<TempUsageData>("usp_GetHashDataFromTempUsage @HashData", hashDataSqlParam);
                    foreach (var item in result)
                    {
                        existingData = item;
                    }
                }
                catch (Exception ex)
                {             
                    IMMSLogger.LogError("Exception occurred in GetExistingUsageData");
                    IMMSLogger.LogError(ex.Message);
                    throw;
                }
            }
            IMMSLogger.LogDebug("Method End. Class MSMQListener.  Method GetExistingUsageData.");
            return existingData;
        }        

        /// <summary>
        /// Insert complete messageData (boarding pass scan & validation info) into UsageTransaction table
        /// </summary>
        /// <param name="endStationId"></param>
        /// <param name="messageData"></param>
        private void InsertIntoUsageTransaction(int endStationId, TempUsageData messageData)
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListener.  Method InsertIntoUsageTransaction.");
            string usageXml = BuildUsageXml(messageData);

            SqlParameter[] sqlParams = 
            {
                new SqlParameter("@EndStationKey", endStationId),
                new SqlParameter("@UsageCode", "BarcodeReader"),
                new SqlParameter("@UsageXml", usageXml),
                new SqlParameter("@AirlineCode", messageData.AirlineId),
                new SqlParameter("@Flight", messageData.FlightNumber)                       
            };

            using (var context = new IMMSEntities())
            {
                try
                {            
                    context.Database.ExecuteSqlCommand("usp_InsertIntoUsageTransaction @EndStationKey, @UsageCode, @UsageXml, @AirlineCode, @Flight", sqlParams);
                }
                catch (Exception ex)
                {
                    IMMSLogger.LogError("Exception occurred in InsertIntoUsageTransaction");
                    IMMSLogger.LogError(ex.Message);
                    throw;
                }
            }            
            LogAuditData("UsageTransaction", "BoardingPassData", usageXml,messageData.HashData);
            IMMSLogger.LogDebug("Method End. Class MSMQListener.  Method InsertIntoUsageTransaction.");
        }

        /// <summary>
        /// Create the XML which will be inserted into UsageTransaction table for the summary table on the front end
        /// </summary>
        /// <param name="messageData"></param>
        /// <returns></returns>
        private string BuildUsageXml(TempUsageData messageData)
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListener.  Method BuildUsageXml.");
            StringBuilder usageXmlBuilder = new StringBuilder();
            usageXmlBuilder.Append("<BoardingScanData>");

            usageXmlBuilder.Append("<Airline>");
            usageXmlBuilder.Append(messageData.AirlineId);
            usageXmlBuilder.Append("</Airline>");

            usageXmlBuilder.Append("<Flight>");
            usageXmlBuilder.Append(messageData.FlightNumber);
            usageXmlBuilder.Append("</Flight>");

            usageXmlBuilder.Append("<DateTime>");
            usageXmlBuilder.Append(ConvertJulianDateToDateTime(messageData.FlightDateAsJulianDate));
            usageXmlBuilder.Append("</DateTime>");

            usageXmlBuilder.Append("<BarcodeId>");
            usageXmlBuilder.Append(messageData.BarcodeId);
            usageXmlBuilder.Append("</BarcodeId>");

            usageXmlBuilder.Append("<ValidationStatus>");
            usageXmlBuilder.Append(messageData.IsSuccess);
            usageXmlBuilder.Append("</ValidationStatus>");

            usageXmlBuilder.Append("<Reason>");
            usageXmlBuilder.Append(messageData.Reason);
            usageXmlBuilder.Append("</Reason>");

            usageXmlBuilder.Append("</BoardingScanData>");

            IMMSLogger.LogDebug("Method End. Class MSMQListener.  Method BuildUsageXml.");
            return usageXmlBuilder.ToString();
        }

        /// <summary>
        /// Convert julianDate to dateTime in preparation for the UsageTable
        /// </summary>
        /// <param name="julianDate"></param>
        /// <returns></returns>
        private string ConvertJulianDateToDateTime(string julianDate)
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListener.  Method ConvertJulianDateToDateTime.");
            string result;
            double doubleJulianDate;
            if (!double.TryParse(julianDate, out doubleJulianDate))
            {
                result = new DateTime().ToString();
            }

            DateTime convertedDate = new DateTime(DateTime.Now.Year, 1, 1).AddDays(Convert.ToInt64(julianDate) - 1);

            //Determine if julianDate describes this year or next year
            DateTime now = DateTime.Now;
            DateTime nowCopy = now;
            int todaysJulianDate = 1;
            while (nowCopy.Month != 1 || nowCopy.Day != 1)
            {
                todaysJulianDate++;
                nowCopy = nowCopy.AddDays(-1);
            }                        
            if (todaysJulianDate > doubleJulianDate)
            {
                convertedDate = convertedDate.AddYears(1);
            }

            result = convertedDate.Year + "-" + convertedDate.Month.ToString("D2") + "-" + convertedDate.Day.ToString("D2") + " " + convertedDate.TimeOfDay;
            IMMSLogger.LogDebug("Method End. Class MSMQListener.  Method ConvertJulianDateToDateTime.");
            return result;
        }                

        /// <summary>
        /// Logs database changes in the AuditDetails table
        /// </summary>
        /// <param name="table"></param>
        /// <param name="fieldModified"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="rowGuid"></param>
        private void LogAuditData(string table, string fieldModified, string newValue, string rowGuid)
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListener.  Method LogAuditData.");

            SqlParameter newValueSqlParam = newValue == null ?
                new SqlParameter("@NewValue", DBNull.Value) : new SqlParameter("@NewValue", newValue);

            SqlParameter[] sqlParams = 
            {
                new SqlParameter("@TableName", table),
                new SqlParameter("@FieldModified", fieldModified),
                new SqlParameter("@OldValue", string.Empty),
                newValueSqlParam,
                new SqlParameter("@RowGUID", rowGuid)
            };

            using (var context = new IMMSEntities())
            {
                try
                {
                    context.Database.ExecuteSqlCommand("usp_InsertAuditData @TableName, @FieldModified, @OldValue, @NewValue, @RowGUID", sqlParams);            
                }
                catch (Exception ex)
                {
                    IMMSLogger.LogError("Exception encountered when attempting to populate database with audit data");
                    IMMSLogger.LogError(ex.Message);
                    throw;
                }
            }
            IMMSLogger.LogDebug("Method End. Class MSMQListener.  Method LogAuditData.");
        }        

        /// <summary>
        /// Insert a copy of the message's raw xml to the database
        /// Status messages go to EndStationStatusRaw
        /// Usage messages go to EndStationUsageRaw
        /// </summary>
        /// <param name="msg"></param>
        private void DumpRawXmlToDatabase(Message msg)
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListener.  Method DumpRawXmlToDatabase.");

            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                outerSerializer.Serialize(sw, msg.Body as EventNotification);
            }

            using (var context = new IMMSEntities())
            {
                SqlParameter Message = new SqlParameter("@Message", sb.ToString());
                try
                {
                    if (this.dataDestination == Destination.Status)
                    {
                        context.Database.ExecuteSqlCommand("usp_DumpStatusRawXML @Message", Message);
                    }
                    if (this.dataDestination == Destination.Usage)
                    {
                        context.Database.ExecuteSqlCommand("usp_DumpUsageRawXML @Message", Message);

                    }                    
                    IMMSLogger.LogInfo("Dumped xml to database: " + sb.ToString());
                }
                catch (Exception ex)
                {
                    IMMSLogger.LogError("Exception thrown when attempting to dump raw xml to database");
                    IMMSLogger.LogError(ex.Message);
                    throw;
                }
            }
            IMMSLogger.LogDebug("Method End. Class MSMQListener.  Method DumpRawXmlToDatabase.");
        }
        #endregion

        #region XML Validation/Serialization
        /// <summary>
        /// Logic for handling invalid messages (set validationError flag)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="vArgs"></param>
        private void IMMS_InnerXMLValidationEventHandler(object sender, ValidationEventArgs vArgs)
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListener.  Method IMMS_InnerXMLValidationEventHandler.");
            innerValidationMessage = vArgs.Message;
            innerValidationError = vArgs.Severity == XmlSeverityType.Error;
            IMMSLogger.LogError("Xml was found to be invalid: " + innerValidationMessage);
            IMMSLogger.LogDebug("Method End. Class MSMQListener.  Method IMMS_InnerXMLValidationEventHandler.");
        }

        /// <summary>
        /// Prepare for xml validation using xsd found at bottom of file
        /// </summary>
        private void SetupXmlValidation()
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListener.  Method SetupXmlValidation.");
            queue.Formatter = new XmlMessageFormatter(new Type[] { typeof(EventNotification), typeof(string), typeof(object) });
            outerSerializer = new XmlSerializer(typeof(EventNotification));
            innerSerializer = new XmlSerializer(typeof(DeviceData));
            XmlSchemaSet innerSchemaSet = new XmlSchemaSet();
            XmlSchema innerSchema = XmlSchema.Read(new StringReader(InnerXmlSchema), IMMS_InnerXMLValidationEventHandler);
            innerSchemaSet.Add(innerSchema);
            innerReadSettings = new XmlReaderSettings
            {
                Schemas = innerSchemaSet,
                ValidationType = ValidationType.Schema,
                ValidationFlags =
                    XmlSchemaValidationFlags.ProcessIdentityConstraints |
                    XmlSchemaValidationFlags.ReportValidationWarnings
            };
            innerReadSettings.ValidationEventHandler += new ValidationEventHandler(IMMS_InnerXMLValidationEventHandler);
            IMMSLogger.LogDebug("Method End. Class MSMQListener.  Method SetupXmlValidation.");
        }

        /// <summary>
        /// Decrypt and deserialize XML in original message's EncryptedInfoData field
        /// </summary>
        /// <param name="encryptedData"></param>
        /// <returns></returns>
        private DeviceData DeserializeInnerMessage(string encryptedData)
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListener.  Method DeserializeInnerMessage.");
            DeviceData result = null;
            XmlReader xmlReader = null;
            StringReader strReader = null;
            string data = IMMSEncryption.DecryptMessage(encryptedData);

            IMMSLogger.LogDebug("Attempting to deserialize inner data: " + data);
            try
            {
                strReader = new StringReader(data);
                xmlReader = XmlReader.Create(strReader, innerReadSettings);

                result = innerSerializer.Deserialize(xmlReader) as DeviceData;

                IMMSLogger.LogDebug("Deserialize success");
            }
            catch (Exception ex)
            {
                IMMSLogger.LogError("Error deserializing XML packet: " + data);
                IMMSLogger.LogError(ex.Message);
                innerValidationError = true;
                innerValidationMessage = "Deserialization exception occurred";
            }
            finally
            {
                xmlReader.Dispose();
                strReader.Dispose();
            }
            IMMSLogger.LogDebug("Method End. Class MSMQListener.  Method DeserializeInnerMessage.");
            return result;
        }

        #region XML Schema
        private const string InnerXmlSchema = @"<?xml version='1.0' encoding='utf-8'?>
<xs:schema id='dataschema' elementFormDefault='qualified' xmlns:xs='http://www.w3.org/2001/XMLSchema'>
	<xs:simpleType name='TypeOfBarcodes'>
		<xs:annotation>
			<xs:documentation>
				Describes different types of barcode

				4 = Data Matrix
				5 = Quick Response-S
				7 = Quick Response-C
				6 = Horizontal PDF417
				R = Vertical PDF417
				V = Aztec-S
				8 = Aztec-C
			</xs:documentation>
		</xs:annotation>
		<xs:restriction base='xs:string'>
			<xs:enumeration value='Data Matrix'/>
			<xs:enumeration value='Quick Response-S'/>
			<xs:enumeration value='Quick Response-C'/>
			<xs:enumeration value='Horizontal PDF417'/>
			<xs:enumeration value='Vertical PDF417'/>
			<xs:enumeration value='Aztec-S'/>
			<xs:enumeration value='Aztec-C'/>
		</xs:restriction>
	</xs:simpleType>


	<xs:complexType name='BarcodeFormats'>
		<xs:annotation >
			<xs:documentation >
				Wrapper for a device.  Includes information
				on its supported interface modes and
				other parameters.
			</xs:documentation>
		</xs:annotation>
		<xs:choice minOccurs='1' maxOccurs='1'>
			<xs:element name='mFormat'
						type='mFormatT'/>
			<xs:element name='sFormat'
						type='sFormatT'/>
		</xs:choice>
	</xs:complexType>

	<xs:complexType name ='errorSectionT'>
		<xs:sequence>
			<xs:element name='Error' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='ErrorMessage' type='xs:string' minOccurs='0' maxOccurs='1' />
		</xs:sequence>
	</xs:complexType>

	<xs:complexType name='mFormatT'>
		<xs:annotation>
			<xs:documentation>
				Standard wrapper for mFormatT barcode data.
			</xs:documentation>
		</xs:annotation>
		<xs:sequence>
			<xs:element name='BaggageTagLicensePlateNumber' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='BaggageTagLicensePlateNumber1' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='BaggageTagLicensePlateNumber2' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='BeginningOfSecurityData' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='BeginningOfVersionNumber' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='DocumentType' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='FieldSizeOfSecurityData' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='FieldSizeOfStructuredMessageUnique' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='SecurityData' type='xs:string' minOccurs='0' maxOccurs='unbounded' />
			<xs:element name='SourceOfBoardingPassIssuance' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='SourceOfCheckIn' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='TypeOfSecurityData' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='VersionNumber' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='FastTrack' type='xs:string' minOccurs='0' maxOccurs='1' />
		</xs:sequence>
	</xs:complexType>

	<xs:complexType name='sFormatT'>
		<xs:annotation>
			<xs:documentation>
				Standard wrapper for sFormatT barcode data.
			</xs:documentation>
		</xs:annotation>
		<xs:sequence>
			<xs:element name='AirlineCode' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='AirlineNumericCode' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='AirportProcessingIndicator' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='BarcodeMessage' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='AdditionalSeatInformation' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='CouponNumber' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='DateOfIssueOfDocument' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='DocumentCheckDigit' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='DocumentFormOrSerialNumber' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='ElectronicTicketIndicator' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='ForIndividualAirlineUseData'  type='xs:unsignedByte' minOccurs='0' maxOccurs='unbounded' />
			<xs:element name='FreeBaggageAllowance' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='FrequentFlyerNumber' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='IDADIndicator' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='InternationalDocVerification' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='OperatingCarrierPNRcode' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='PassengerStatus' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='XOIndicator' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='ServicingCarrierPNR' type='xs:string' minOccurs='0' maxOccurs='1' />
			<xs:element name='ToCityAirportCode' type='xs:string' minOccurs='0' maxOccurs='1' />
		</xs:sequence>
	</xs:complexType>

	<xs:element name='DeviceData'>
		<xs:complexType>
			<xs:sequence>
				<xs:element name='ATBData' minOccurs='0' maxOccurs='1'>
					<xs:complexType>
						<xs:sequence>
							<xs:element name='HashData' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='TypeOfBarcode' type='TypeOfBarcodes' minOccurs='0' maxOccurs='1' />
							<xs:element name='WorkstationName' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='ScanTime' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='PrintTime' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='AirlineId' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='FlightNumber' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='PNRCode' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='Origin' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='Destination' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='FlightDateAsJulianDate' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='Compartment' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='NoOfSegments' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='CheckInNumber' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='SeatNumber' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='ETktIndicator' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='PaxStatus' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='PassengerName' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='BarcodeFormatCode' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='BarcodeID' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='DateOfIssue' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='PassengerDescription' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='BarcodeFormat' type='BarcodeFormats'/>
							<xs:element name='ErrorSection' type='errorSectionT' minOccurs='0' maxOccurs='1' />
						</xs:sequence>
					</xs:complexType>
				</xs:element>
				<xs:element name='BTPData' minOccurs='0' maxOccurs='1'>
					<xs:complexType>
						<xs:sequence>
							<xs:element name='WorkstationName' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='PrintTime' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='AirlineId' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='FlightNumber' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='BaggageId' type='xs:string' minOccurs='0' maxOccurs='1'/>
							<xs:element name='FlightDate' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='BagCount' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='WeightAllowance' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='PaxName' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='Origin' type='xs:string' minOccurs='0' maxOccurs='1' />
							<xs:element name='Destination' type='xs:string' minOccurs='0' maxOccurs='1' />
						</xs:sequence>
					</xs:complexType>
				</xs:element>
				<xs:element name='ValidationResult' minOccurs='0' maxOccurs='1'>
					<xs:complexType>
						<xs:sequence>
						<xs:element name = 'ValidationsPerformed'>
							<xs:complexType>
								<xs:sequence>
									<xs:element name='PFMValidatorResult' maxOccurs='unbounded'>
										<xs:complexType>
											<xs:attribute name='Validator' type='xs:string' />
											<xs:attribute name='IsSuccess' type='xs:boolean' use='required' />
											<xs:attribute name='ResultText' type='xs:string' />
											<xs:attribute name='MessagePayload' type='xs:string' />
										</xs:complexType>
									</xs:element>
								</xs:sequence>
							</xs:complexType>
						</xs:element>
						</xs:sequence>
						<xs:attribute name='Id' type='xs:string' />
						<xs:attribute name='HashData' type='xs:string' />
						<xs:attribute name='IsSuccess' type='xs:boolean' use='required' />
						<xs:attribute name='ResponseTime' type='xs:string' />
						<xs:attribute name='EventLogTime' type='xs:string' />
						<xs:attribute name='WorkstationName' type='xs:string' />
					</xs:complexType>
				</xs:element>
			</xs:sequence>
		</xs:complexType>
	</xs:element>
</xs:schema>";
        #endregion

        #endregion

        #region IDisposable Implementation
        ~MSMQListener()
        {
            Dispose(false);
        }

        public void Cleanup()
        {
            this.Dispose(true);
        }

        void IDisposable.Dispose()
        {
            Dispose(true);            
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Remove all disposable resources held by the class
        /// This includes MessageQueue handles and receive event handler delegates
        /// </summary>
        /// <param name="disposing"></param>
        public virtual void Dispose(bool disposing)
        {
            IMMSLogger.LogDebug("Method Beginning. Class MSMQListener.  Method Dispose.");
            if (disposing)
            {
                queue.ReceiveCompleted -= new ReceiveCompletedEventHandler(HandleMSMQRecv);
                queue.Dispose();
                queue = null;
            }
            IMMSLogger.LogDebug("Method End. Class MSMQListener.  Method Dispose.");
        }
        #endregion
    }
}

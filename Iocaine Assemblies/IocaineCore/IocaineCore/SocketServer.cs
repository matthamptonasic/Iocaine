﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Iocaine2
{
    public static class SocketServer
    {
        #region Enums
        public enum STATE
        {
            STOPPED,
            RUNNING
        }
        #endregion Enums
        #region Private Members
        private const int c_buffer_sz = 256;
        private static TcpListener m_server = null;
        private static ProtocolType m_protType = ProtocolType.IPv4;
        private static IPAddress m_addr = null;
        private static int m_port = 0;
        private static byte[] m_buffer = null;
        private static string m_bufferStr = "";
        private static Thread m_bgThread = null;
        private static STATE m_state = STATE.STOPPED;
        private static volatile bool m_stop = false;
        #endregion Private Members
        #region Public Properties
        public static ProtocolType ProtType
        {
            get
            {
                return m_protType;
            }
            set
            {
                m_protType = value;
            }
        }
        public static int Port
        {
            get
            {
                return m_port;
            }
        }
        public static STATE State
        {
            get
            {
                return m_state;
            }
        }
        #endregion Public Properties
        #region Events & Delegates
        public delegate void DataRecieved_Delegate(string oString);
        private static DataRecieved_Delegate _dataRecieved;
        public static DataRecieved_Delegate _DataRecieved
        {
            get
            {
                return _dataRecieved;
            }
            set
            {
                _dataRecieved = value;
            }
        }
        #endregion Events & Delegates
        #region Public Methods
        public static void Start()
        {
            while (m_stop == true)
            {
                Thread.Sleep(10);
            }
            if (m_state == STATE.RUNNING)
            {
                return;
            }
            m_server = null;
            try
            {
                m_server = new TcpListener(IPAddress.Loopback, 0);
                m_server.Start(10);
                m_addr = ((IPEndPoint)m_server.LocalEndpoint).Address;
                m_port = ((IPEndPoint)m_server.LocalEndpoint).Port;
                m_buffer = new byte[c_buffer_sz];
                m_bufferStr = "";
                m_bgThread = new Thread(new ThreadStart(runThreadFunction));
                m_bgThread.IsBackground = true;
                m_bgThread.Name = "SocketServer_runThreadFunction";
                m_bgThread.Start();
            }
            catch (Exception e)
            {
                Logging.LoggingFunctions.Error(e.ToString());
            }
        }
        public static void Stop(bool iBlock = true)
        {
            m_stop = true;
            m_server.Stop();
            if (iBlock == true)
            {
                while (m_state == STATE.RUNNING)
                {
                    Thread.Sleep(10);
                }
            }
            m_stop = false;
        }
        #endregion Public Methods
        #region Private Methods
        private static void runThreadFunction()
        {
            try
            {
                Logging.LoggingFunctions.AddDebugScope(Logging.LoggingFunctions.DBG_SCOPE.BACKGROUND);
                m_state = STATE.RUNNING;
                NetworkStream l_stream = null;
                TcpClient l_client = null;
                Thread l_thread = null;
                int l_bytesRead = 0;
                while (m_stop == false)
                {
                    Logging.LoggingFunctions.Debug("SocketServer: Waiting for a connection at " + m_addr.ToString() + ", port " + m_port + "...", Logging.LoggingFunctions.DBG_SCOPE.BACKGROUND);

                    // Perform a blocking call to accept requests.
                    l_client = m_server.AcceptTcpClient();
                    Logging.LoggingFunctions.Debug("SocketServer: Connected!", Logging.LoggingFunctions.DBG_SCOPE.BACKGROUND);

                    m_bufferStr = "";

                    // Get a stream object for reading and writing
                    l_stream = l_client.GetStream();

                    // Loop to receive all the data sent by the client.
                    while ((l_bytesRead = l_stream.Read(m_buffer, 0, m_buffer.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        m_bufferStr = System.Text.Encoding.ASCII.GetString(m_buffer, 0, l_bytesRead);
                        Logging.LoggingFunctions.Debug("Recieved: '" + m_bufferStr + "'", Logging.LoggingFunctions.DBG_SCOPE.BACKGROUND);
                        eventFwdParser.Parse(m_bufferStr);
                        if (_dataRecieved != null)
                        {
                            l_thread = new Thread(new ParameterizedThreadStart(fireDataRecieved));
                            l_thread.IsBackground = true;
                            l_thread.Name = "SocketServer_fireDataRecieved";
                            l_thread.Start(m_bufferStr);
                        }
                    }
                    
                    // Shutdown and end connection
                    l_client.Close();
                }
            }
            catch (SocketException e)
            {
                Logging.LoggingFunctions.Error(e.ToString());
            }
            finally
            {
                // Stop listening for new clients.
                m_server.Stop();
                m_state = STATE.STOPPED;
            }
        }
        private static void fireDataRecieved(object iValue)
        {
            _dataRecieved((string)iValue);
        }
        #endregion Private Methods
    }
}

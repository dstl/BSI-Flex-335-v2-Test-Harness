// File:              $Workfile: SocketCommsCommon.cs$
// <copyright file="SocketCommsCommon.cs" >
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// </copyright>

namespace SapientServices.Communication
{
    using Google.Protobuf;
    using log4net;
    using Sapient.Data;
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading;

    /// <summary>
    /// class to provide TCP common services
    /// </summary>
    public class SocketCommsCommon
    {
        #region Constant Definitions

        /// <summary>
        /// default maximum packet size in bytes
        /// </summary>
        public const uint MaximumPacketSize = 2048;

        #endregion

        /// <summary>
        /// Logger for log4net logging
        /// </summary>
        protected static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Public Static Methods

        /// <summary>
        /// Whether connection is OK
        /// </summary>
        /// <param name="client">connection object</param>
        /// <param name="stream">stream object</param>
        /// <returns>true if connected</returns>
        public static bool Connected(TcpClient client, NetworkStream stream)
        {
            bool retval = false;
            if (client != null)
            {
                if ((stream != null) && client.Connected)
                {
                    retval = true;
                }
            }

            return retval;
        }

        /// <summary>
        /// send a data packet to TCP client
        /// </summary>
        /// <param name="msg">message data</param>
        /// <param name="packet_size">size in bytes</param>
        /// <param name="client">connection object</param>
        /// <param name="stream">stream object</param>
        /// <param name="nullTermination">whether to add null on end of message</param>
        /// <returns>true if connection valid</returns>
        public static bool SendPacket(SapientMessage msg, TcpClient client, NetworkStream stream)
        {
            var retval = false;
            //if (packet_size > msg.Length)
            //{
            //    Log.Info("Send Failed - Invalid packet size|");
            //    return true;
            //}

            try
            {
                if (Connected(client, stream))
                {
                    byte[] msgBytes = msg.ToByteArray();
                    var len = msgBytes.Length;

                    byte[] lenBytes = BitConverter.GetBytes(len);

                    if (!BitConverter.IsLittleEndian)
                        Array.Reverse(lenBytes);

                    byte[] packet = lenBytes.Concat(msgBytes).ToArray();

                    stream.Write(packet, 0, packet.Length);

                    retval = true;
                }
                else
                {
                    Log.Info("Send Failed: No Connection");
                }
            }
            catch (IOException ex)
            {
                Log.Info("Send Timeout - connection closed");
            }
            catch (SocketException ex)
            {
                Log.Info("Send Failed - connection closed");
            }
            catch (Exception ex)
            {
                Log.Error("SendPacket error", ex);
            }

            return retval;
        }

        /// <summary>
        /// receive from TCP stream, recoverable in the case of a problem
        /// </summary>
        /// <param name="stream">stream object</param>
        /// <param name="buffer">receive buffer</param>
        /// <param name="offset">offset in to buffer in bytes</param>
        /// <param name="size">size of buffer in bytes</param>
        /// <param name="client">connection object</param>
        /// <returns>true if successful</returns>
        public static bool SingleSafeReceive(NetworkStream stream, byte[] buffer, int offset, ref int size, TcpClient client)
        {
            bool status = Connected(client, stream);

            if (status && (offset + size <= buffer.Length))
            {
                int data_available = DataAvailable(stream);

                if (data_available > 0)
                {
                    int recv_return = stream.Read(buffer, offset, size);

                    switch (recv_return)
                    {
                        case -1:
                        case 0:
                            status = false;
                            size = 0;
                            break;
                        default:
                            size = recv_return;
                            break;
                    }
                }
                else if (data_available < 0)
                {
                    size = 0;
                    status = false;
                }
                else if (!Connected(client, stream))
                {
                    size = 0;
                    status = false;
                }
                else
                {
                    size = 0;
                }
            }

            return status;
        }

        /// <summary>
        /// receive from TCP stream, recoverable in the case of a problem
        /// </summary>
        /// <param name="stream">stream object</param>
        /// <param name="buffer">receive buffer</param>
        /// <param name="offset">offset in to buffer in bytes</param>
        /// <param name="size">size of buffer in bytes</param>
        /// <param name="client">connection object</param>
        /// <returns>true if successful</returns>
        public static bool SafeReceive(NetworkStream stream, byte[] buffer, int offset, int size, TcpClient client)
        {
            int received_bytes = 0;
            bool status = Connected(client, stream);

            while ((received_bytes != size) && status)
            {
                status = Connected(client, stream);
                if (status)
                {
                    int data_available = DataAvailable(stream);

                    if (data_available > 0)
                    {
                        int recv_return = stream.Read(buffer, offset + received_bytes, size - received_bytes);

                        switch (recv_return)
                        {
                            case -1:
                            case 0:
                                status = false;
                                break;
                            default:
                                received_bytes += recv_return;
                                break;
                        }
                    }
                    else if (data_available < 0)
                    {
                        status = false;
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                }
            }

            return status;
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// whether data available to read
        /// </summary>
        /// <param name="stream">stream object</param>
        /// <returns>1 if there is, 0 if there isn't but connection is ok, -1 if error or connection closed</returns>
        private static int DataAvailable(NetworkStream stream)
        {
            var retval = -1;
            try
            {
                retval = stream.DataAvailable ? 1 : 0;
            }
            catch (Exception ex)
            {
                Log.Error("Data Available Error:" + ex.ToString());
            }

            return retval;
        }

        #endregion
    }
}

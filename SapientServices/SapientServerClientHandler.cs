// Crown-owned copyright, 2021-2024
namespace SapientServices.Communication
{
    using log4net;
    using Sapient.Data;
    using SAPIENT.MessageProcessor.TCP;
    using SAPIENT.MessageProcessor;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    public class SapientServerClientHandler : ClientBase, IConnection
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SapientServerClientHandler));

        private static readonly ILog SendLog = LogManager.GetLogger("SendLogger");

        private readonly TcpClient client;

        private readonly NetworkStream stream;

        private bool thread_running;

        private readonly Thread client_receive_thread;

        private SapientCommsCommon.DataReceivedCallback data_receivedcallback;

        private SapientCommsCommon.StatusCallback connect_callback;

        private bool connected;

        private IByteDataMessageBuilder byteDataMessageBuilder = new ByteDataMessageBuilder();

        public SapientServerClientHandler(TcpClient client_socket, uint maximum_packet_size, bool send_only, uint connection_id)
        {
            client = client_socket;
            stream = client.GetStream();

            if (!send_only)
            {
                client_receive_thread = new Thread(ClientReceiveThread) { Name = "Server Receive Thread" };
                client_receive_thread.Start();
            }

            ConnectionID = connection_id;

            if (client != null)
            {
                RemoteEndPoint = client.Client.RemoteEndPoint;
            }

            byteDataMessageBuilder.DataReceived += ByteDataMessageBuilderDataReceived;
        }

        public uint ConnectionID { get; private set; }

        public EndPoint RemoteEndPoint { get; private set; }

        public void SetDataReceivedCallback(SapientCommsCommon.DataReceivedCallback callback)
        {
            data_receivedcallback += callback;
        }

        public void SetConnectedCallback(SapientCommsCommon.StatusCallback statuscallback)
        {
            connect_callback += statuscallback;
        }

        public void SetNoDelay(bool no_delay)
        {
            if (client != null)
            {
                client.NoDelay = no_delay;
            }
        }

        public bool IsConnected()
        {
            if (connected && (client != null))
            {
                connected = SocketCommsCommon.Connected(client, stream);

                if (!connected)
                {
                    logger.Error("Client Connection " + ConnectionID + " lost");
                }
            }

            return connected;
        }

        public void Shutdown()
        {
            connected = false;
            thread_running = false;
            if (client_receive_thread != null)
            {
                if (client_receive_thread.IsAlive)
                {
                    client_receive_thread.Join(1000);
                }
            }

            if (connect_callback != null)
            {
                connect_callback("Socket Disconnected", this);
            }

            data_receivedcallback = null;
            connect_callback = null;
            client.Close();
        }

        public bool SendMessage(SapientMessage msg)
        {
            bool retval = false;
            if (this.Validate(msg, true, logger))
            {
                logger.DebugFormat("SendMessage: {0}", msg.ContentCase);
                this.OnDataSend(new SapientMessageEventArgs() { Message = msg });
                SendLog.InfoFormat("Send to {0}: {1}", client?.Client?.RemoteEndPoint, msg?.ToString());
                retval = SocketCommsCommon.SendPacket(msg, client, stream);
                if (!retval)
                {
                    this.OnDataError(new SapientMessageEventArgs() { Error = $"Failed to send {msg?.ContentCase.ToString()}" });
                    SendLog.InfoFormat("Send failed to {0}: {1}", client?.Client?.RemoteEndPoint, msg?.ToString());
                }
            }
            else
            {
                SendLog.InfoFormat("Invalid message to {0}: {1}", client?.Client?.RemoteEndPoint, msg?.ToString());
            }
            return retval;
        }

        private void ClientReceiveThread()
        {
            try
            {
                thread_running = true;
                connected = true;
                do
                {
                    if (thread_running && stream != null && stream.DataAvailable)
                    {
                        byte[] buffer = new byte[9000];
                        int bytesRead = 0;

                        if (stream.CanRead)
                        {
                            using (BinaryReader binaryReader = new BinaryReader(stream, Encoding.UTF8, true))
                            {
                                for (int i = 0; i < buffer.Length; i++)
                                {
                                    buffer[i] = binaryReader.ReadByte();

                                    if (!stream.DataAvailable)
                                    {
                                        bytesRead = i + 1;
                                        break;
                                    }
                                }
                            }
                        }

                        byteDataMessageBuilder.AddData(buffer.Take(bytesRead).ToArray());
                        logger.Info($"Bytes received: {bytesRead}");

                    }

                    Thread.Sleep(1);

                } while (thread_running);

                connected = false;

                if (stream != null)
                {
                    stream.Close();
                }

                client.Close();
            }
            catch (SocketException ex)
            {
                logger.Error(ex.ToString());
                logger.Error("Socket Disconnected");
                if (connect_callback != null)
                {
                    connect_callback("Socket Disconnected", this);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
        }

        private void ByteDataMessageBuilderDataReceived(object sender, ByteDataMessageEventArgs e)
        {
            try
            {
                SapientMessage msg = SapientMessage.Parser.ParseFrom(e.MessageData);
                SendLog.InfoFormat("Received on {0}: {1}", client?.Client?.RemoteEndPoint, msg?.ToString());
                if (this.Validate(msg, false, logger))
                {
                    this.OnDataReceived(new SapientMessageEventArgs() { Message = msg });
                    if (data_receivedcallback != null)
                    {
                        data_receivedcallback(msg, this);
                    }
                }
                else
                {
                    SendLog.InfoFormat("Invalid message from {0}: {1}", client?.Client?.RemoteEndPoint, msg?.ToString());
                }
            }
            catch (Exception ex)
            {
                SendLog.InfoFormat("Received failed on {0}: {1}", client?.Client?.RemoteEndPoint, ex?.ToString());
                logger.Error(ex.ToString());
                this.OnDataError(new SapientMessageEventArgs() { Error = ex.Message });
            }
        }
    }
}

// Crown-owned copyright, 2021-2024
namespace SapientServices
{
    using log4net;
    using Sapient.Data;
    using SAPIENT.MessageProcessor;
    using SAPIENT.MessageProcessor.TCP;
    using SapientServices.Communication;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    public class SapientClient : ClientBase, ICommsConnection, IConnection
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SapientClient));

        private static readonly ILog SendLog = LogManager.GetLogger("SendLogger");

        private readonly string server_name = "server";

        private readonly int server_port = 12000;

        private TcpClient client;

        private NetworkStream stream;

        private object thisLock = new object();

        private bool thread_running;

        private bool send_only;

        private bool no_delay;

        private bool connected;

        private Thread client_receive_thread;

        private SapientCommsCommon.DataReceivedCallback data_receivedcallback;

        private SapientCommsCommon.StatusCallback connect_callback;

        private uint max_packet_size = SocketCommsCommon.MaximumPacketSize;

        private IByteDataMessageBuilder byteDataMessageBuilder = new ByteDataMessageBuilder();

        public SapientClient(string server, int port)
        {
            server_name = server;
            server_port = port;
            ConnectionID = 1;
            ConnectionName = "Server";
            byteDataMessageBuilder.DataReceived += ByteDataMessageBuilderDataReceived;
        }

        public uint ConnectionID { get; private set; }

        public EndPoint RemoteEndPoint { get; private set; }

        public string ConnectionName { get; set; }

        public void SetDataReceivedCallback(SapientCommsCommon.DataReceivedCallback callback)
        {
            data_receivedcallback += callback;
        }

        public void SetConnectedCallback(SapientCommsCommon.StatusCallback statuscallback)
        {
            connect_callback += statuscallback;
        }

        public void SetNoDelay(bool _no_delay)
        {
            no_delay = _no_delay;
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
                    logger.ErrorFormat("Connection to {0} lost", ConnectionName);
                }
            }

            return connected;
        }

        public void Start(uint maximum_packet_size, bool _send_only)
        {
            send_only = _send_only;
            max_packet_size = maximum_packet_size;
            Start();
        }

        public void Start()
        {
            if (!send_only)
            {
                client_receive_thread = new Thread(ClientReceiveThread)
                {
                    Name = server_name + ":" + server_port + " Client Receive Thread",
                };
                client_receive_thread.Start();
            }
            else
            {
                ConnectToServer();
            }
        }

        public void Shutdown()
        {
            thread_running = false;
            Close(); // close the socket
            if (client_receive_thread != null)
            {
                if (client_receive_thread.IsAlive)
                {
                    client_receive_thread.Join(1000);
                }
            }
        }

        public bool SendMessage(SapientMessage msg)
        {
            bool retval = false;
            if (this.Validate(msg, true, logger))
            {
                logger.DebugFormat("SendMessage: {0}", msg?.ContentCase);
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
            thread_running = true;

            while (thread_running)
            {
                try
                {
                    ConnectToServer();

                    while (thread_running && stream != null)
                    {
                        if (stream.DataAvailable)
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
                    }

                    Close();
                }
                catch (SocketException ex)
                {
                    logger.Error(ex.ToString());
                    logger.ErrorFormat("Socket Disconnected to {0}", ConnectionName);
                    if (connect_callback != null)
                    {
                        connect_callback("Socket Disconnected", this);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }

                if (thread_running)
                {
                    Thread.Sleep(1000); 
                }
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

        private void Close()
        {
            connected = false;
            if (stream != null)
            {
                stream.Close();
            }

            if (client != null)
            {
                if (client.Client != null && client.Client.Connected)
                {
                    logger.InfoFormat("Socket Connection to {0} Closed", ConnectionName);
                }

                client.Close();
            }
        }

        private void ConnectToServer()
        {
            try
            {
                client = new TcpClient(server_name, server_port)
                {
                    NoDelay = no_delay,
                    ReceiveTimeout = 10000,
                    ReceiveBufferSize = (int)max_packet_size,
                    SendBufferSize = (int)max_packet_size,
                };
                stream = client.GetStream();
                if (stream != null)
                {
                    RemoteEndPoint = client.Client.RemoteEndPoint;

                    logger.InfoFormat("Connected To {0}", ConnectionName);
                    if (connect_callback != null)
                    {
                        connect_callback("Connected To Server", this);
                    }

                    connected = true;
                }
            }
            catch (SocketException ex)
            {
                logger.Error("Unable to connect to " + ConnectionName + " on host:" + server_name + " Port:" + server_port);

                if (ex.ErrorCode == (int)SocketError.ConnectionRefused)
                {
                    logger.ErrorFormat("Cannot connect to {2} on Host:{0} Port:{1}. Check these settings and then check that the {2} is installed and running.", server_name, server_port, ConnectionName);
                }
                else
                {
                    logger.Error(ex);
                }

                if (connect_callback != null)
                {
                    connect_callback("Unable to connect to server", this);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
        }
    }
}

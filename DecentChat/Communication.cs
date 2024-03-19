using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using Microsoft.Extensions.Logging;

namespace DecentChat
{
    public class Communication
    {
        private string ip_address;
        private int port;
        private bool status;
        private ILogger sender_logger;
        private ILogger receiver_logger;
        private void print_dict(Dictionary<string, Object> payload, ILogger logger)
        {
            logger.LogInformation("Printing dictionary---------------------------");
            foreach (var item in payload)
            {
                if (item.Value != null)
                    logger.LogInformation(item.Key + " " + item.Value.ToString());
                else logger.LogInformation(item.Key + " null");
            }
            logger.LogInformation("End of dictionary----------------------------");
        }
        public Communication(string ip_address, int port, ILogger sender_logger, ILogger receiver_logger)
        {
            this.ip_address = ip_address;
            this.port = port;
            this.status = false;
            this.sender_logger = sender_logger;
            this.receiver_logger = receiver_logger;
            // Console.WriteLine("Communication object created");
            Console.WriteLine(this.ip_address);
            Console.WriteLine(this.port);
        }

        public void Off()
        {
            this.status = false;
        }

        public void On()
        {
            this.status = true;
        }

        public void reciver_func(object arg)
        {
            Func<Dictionary<string, object>, Dictionary<string, object>> ProcessPayload = (Func<Dictionary<string, object>, Dictionary<string, object>>)arg;
            try
            {
                Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(new IPEndPoint(IPAddress.Parse(this.ip_address), this.port));
                var knownTypes = new List<Type> { typeof(Node), typeof(Dictionary<string, object>[]), typeof(Dictionary<string, object>), typeof(Message), typeof(Message[]) };
                var serializer = new DataContractJsonSerializer(typeof(Dictionary<string, object>), knownTypes);
                listener.Listen(1);
                while (this.status)
                {

                    receiver_logger.LogInformation("Waiting for a connection...");
                    Socket handler = listener.Accept();
                    receiver_logger.LogInformation("Connected to " + handler.RemoteEndPoint.ToString());
                    byte[] bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);
                    // receiver_logger.LogInformation("Received " + bytesRec + " bytes");
                    byte[] actualBytes = new byte[bytesRec];
                    Array.Copy(bytes, actualBytes, bytesRec);
                    string data = Encoding.ASCII.GetString(actualBytes);
                    receiver_logger.LogInformation("Received " + data);
                    var ms = new MemoryStream(actualBytes);
                    receiver_logger.LogInformation("Received and deserializing");
                    Dictionary<string, object> incomingData = (Dictionary<string, object>)serializer.ReadObject(ms);
                    receiver_logger.LogInformation("Received and processing");
                    Dictionary<string, object> outgoingData = ProcessPayload(incomingData);

                    receiver_logger.LogInformation("Processed and Sending");
                    ms = new MemoryStream();
                    outgoingData["date_time"] = DateTime.UtcNow;
                    serializer.WriteObject(ms, outgoingData);
                    byte[] msg = ms.ToArray();
                    data = Encoding.ASCII.GetString(msg);
                    receiver_logger.LogInformation("Sending " + data);
                    handler.Send(msg);
                    receiver_logger.LogInformation("Sent");

                    if ((outgoingData["data"] != null) && (outgoingData["data"].ToString() == "close_node"))
                    {
                        this.Off();
                    }
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception e)
            {
                receiver_logger?.LogError(e.Message ?? "An error occurred");
                // Console.WriteLine(e.ToString());
            }
        }

        public Dictionary<string, object> sender_func(string ip_address, int port, Dictionary<string, object> payload)
        {
            if (this.status)
            {
                try
                {
                    Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    sender.Connect(new IPEndPoint(IPAddress.Parse(ip_address), port));
                    var knownTypes = new List<Type> { typeof(Node), typeof(Dictionary<string, object>[]), typeof(Dictionary<string, object>), typeof(Message), typeof(Message[]) };
                    var serializer = new DataContractJsonSerializer(typeof(Dictionary<string, object>), knownTypes);
                    var ms = new MemoryStream();
                    // sender_logger.LogInformation("Sending");
                    // print_dict(payload, sender_logger);
                    payload["date_time"] = DateTime.UtcNow;
                    serializer.WriteObject(ms, payload);
                    byte[] serializedData = ms.ToArray();
                    // sender_logger.LogInformation("Sending " + data);
                    sender.Send(serializedData);
                    // Console.WriteLine("Sent");

                    byte[] bytes = new byte[1024];
                    int bytesRec = sender.Receive(bytes);
                    byte[] actualBytes = new byte[bytesRec];
                    Array.Copy(bytes, actualBytes, bytesRec);
                    string data = Encoding.ASCII.GetString(actualBytes);
                    sender_logger.LogInformation("Received " + data);
                    ms = new MemoryStream(actualBytes);
                    var deserializedData = (Dictionary<string, object>)serializer.ReadObject(ms);
                    // Console.WriteLine("Received");
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                    sender_logger.LogInformation("Received");
                    print_dict(deserializedData, sender_logger);
                    return deserializedData;
                }
                catch (Exception e)
                {
                    sender_logger?.LogError(e.Message ?? "An error occurred");
                    // Console.WriteLine(e.ToString());
                    return new Dictionary<string, object>();
                }
            }
            else
            {
                sender_logger?.LogError("Communication is off");
                // Console.WriteLine("Communication is off");
                Dictionary<string, object> response = new Dictionary<string, object>();
                response["query"] = null;
                response["data"] = null;
                return response;
            }
        }
    }
}

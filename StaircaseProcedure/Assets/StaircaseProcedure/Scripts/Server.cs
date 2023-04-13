using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using UnityEngine;

namespace Staircase
{

    public class ServerSocket
    {

        private Socket handler;
        private Socket listener;

        public void ExecuteServer(string ipAddress, int port)
        {
            IPAddress ipAddr = IPAddress.Parse(ipAddress);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port);

            try
            {
                Debug.Log("Starting server @ " + ipAddress + ":" + port + " ...");
                listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(localEndPoint);
                listener.Listen(10);
                listener.Blocking = false;
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        public void AcceptClient()
        {
            handler = listener.Accept();
            Debug.Log("Client connected.");
        }

        public void SendInitToClient(params object[] list)
        {
            string init = string.Join(";", list);
            Send("init::" + init);
        }

        public void SendTrialDataToClient(TrialData data)
        {
            string msg_data = "" + data.indexTrial + ";" + data.sequence + ";" + data.stimulus.ToString(StaircaseProcedure.nfi) + ";" + data.stimulusNoticed + ";" + data.indexSequence + ";" + data.reversal;
            Send("trial::" + msg_data);
        }

        public void SendThresholdToClient(float threshold)
        {
            Send("threshold::" + threshold.ToString(StaircaseProcedure.nfi));
            CloseConnection();
        }

        public void Send(string msg)
        {
            var message = Encoding.UTF8.GetBytes(msg);
            var message_length = message.Length;
            var send_l = Encoding.UTF8.GetBytes(message_length.ToString());
            var duplicate_num = 64 - send_l.Length;
            string result = new String(' ', duplicate_num);
            string r = message_length.ToString() + result;
            var send_length = Encoding.UTF8.GetBytes(r);

            if (handler != null)
            {
                handler.Send(send_length);
                handler.Send(message);
            }
            else
            {
                Debug.Log("No client connected.");
            }
        }

        public void CloseConnection()
        {
            if (handler != null)
            {
                Send("!DISCONNECT::d");
                Debug.Log("Client disconnected.");
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                handler = null;
            }
        }
    }
}

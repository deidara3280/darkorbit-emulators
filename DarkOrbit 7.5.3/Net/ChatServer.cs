﻿using Ow.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ow.Net
{
    class ChatServer
    {
        private static ManualResetEvent AllDone = new ManualResetEvent(false);
        private static int Port = 9338;

        public static void StartListening()
        {
            var localEndPoint = new IPEndPoint(IPAddress.Any, Port);
            var listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                Out.WriteLine("Listening on port " + Port + ".", "ChatSocket");

                while (true)
                {
                    AllDone.Reset();
                    listener.BeginAccept(AcceptCallback,
                        listener);
                    AllDone.WaitOne();
                }

            }
            catch (Exception)
            {
                Out.WriteLine("An application is already listening the port " + Port + ".", "ERROR", ConsoleColor.Red);
            }
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                AllDone.Set();
                var listener = (Socket)ar.AsyncState;
                var handler = listener.EndAccept(ar);
                var chatClient = new Chat.ChatClient(handler);
            }
            catch (Exception e)
            {
                Out.WriteLine(e.Message, "ChatServer ERROR");
            }
        }

    }
}

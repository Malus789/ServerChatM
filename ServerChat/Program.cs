using System;


namespace ServerChat
{
    using System;
    using System.Net.Sockets;
    using System.Net;
    using System.Collections.Generic;
    using System.IO; //con esto puedo hacer estructuras
    using System.Threading; // con esto puedo crear hilos o Threads

    class ChatServer
    {
        private TcpListener server;
        private TcpClient client = new TcpClient();
        private IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 8000);
        private List<Connection> list = new List<Connection>();

        Connection con;

        private struct Connection
        {
            public NetworkStream stream;
            public StreamReader streamR;
            public StreamWriter streamW;
            public String nick;
        }

        public void Chat_Server()
        {
            StartServer();
        }

        public void StartServer()
        {
            Console.WriteLine("Server activated!");
            server = new TcpListener(ipEndPoint);
            server.Start();

            while (true)
            {
                client = server.AcceptTcpClient();

                con = new Connection();

                con.stream = client.GetStream();
                con.streamR = new StreamReader(con.stream);
                con.streamW = new StreamWriter(con.stream);

                con.nick = con.streamR.ReadLine();

                list.Add(con);
                Console.WriteLine(con.nick + " is connected.");

                Thread t = new Thread(ListenConnection);
                t.Start();
            }
        }

        void ListenConnection()
        {
            Connection hcon = con;
            do
            {
                try
                {
                    string tmp = hcon.streamR.ReadLine();

                    int tmpPosition = tmp.IndexOf("=");

                    if (tmpPosition != -1) { 
                        string findW = tmp.Substring(0, tmpPosition);
                        if (findW == "ChangeNickConfig")
                        {                           
                            string findWP = tmp.Substring(tmpPosition+1);            
                            Console.WriteLine(hcon.nick+ " changed the nickname to " + findWP);            
                            hcon.nick = findWP;               
                        } 
                    }

                    Console.WriteLine(hcon.nick + ":" + tmp);
                    foreach (Connection C in list)
                    {
                        try
                        {
                            C.streamW.WriteLine(hcon.nick + ": " + tmp);
                            C.streamW.Flush();
                        }
                        catch
                        {
                        }
                    }
                }

                catch
                {
                    list.Remove(hcon);
                    Console.WriteLine("The user " + hcon.nick + " is left");
                    break;
                }
            } while (true);          
        }

    }


class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("MChatServer 0.1B");
            ChatServer Server = new ChatServer();
            Server.Chat_Server();
        }
    }
}

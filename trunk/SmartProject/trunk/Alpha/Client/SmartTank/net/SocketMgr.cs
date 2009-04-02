using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TankEngine2D.Helpers;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace SmartTank.net
{
    struct SocketConfig
    {
        public string ServerIP;
        public int ServerPort;
    };

    static public class SocketMgr
    {
        static SocketConfig config;
        static Socket sckt;
        static Thread thread;
        /// <summary>
        /// 初始化，读配置文件
        /// </summary>
        static public void Initial()
        {
            ReadConfig();
        }

        static void ReadConfig()
        {
            try
            {
                FileStream stream = new FileStream("SocketConfig.txt", FileMode.Open);
                StreamReader reader = new StreamReader(stream);
                string line = reader.ReadLine();
                while (line != null)
                {
                    if (line.Contains("ServerIP=")) config.ServerIP = line.Remove(0, 9);
                    else if (line.Contains("ServerPort=")) config.ServerPort = int.Parse(line.Remove(0, 11));
                    line = reader.ReadLine();
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Log.Write("net.SocketMgr ReadConfig error!" + ex.ToString());
            }
        }

        static public bool ConnectToServer()
        {
            try
            {

                IPAddress IPAdd = IPAddress.Parse(config.ServerIP);
                IPEndPoint endPoint = new IPEndPoint(IPAdd, config.ServerPort);
                sckt = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                sckt.Connect(endPoint);
                if (sckt.Connected)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        static public void SendPackge(SyncCashe cash)
        {
            try
            {
                if (sckt.Connected)
                {
                    Stream netStream = new NetworkStream(sckt);
                    Stream bufStream = new BufferedStream(netStream);
                    StreamWriter SW = new StreamWriter(bufStream);
                    byte[] buf = new byte[4];
                    buf[0] = 1;

                    //SW.Write(buf);
                    bufStream.Write(buf, 0, 4);
                    SW.WriteLine("Fuck server!");

                    SW.WriteLine(cash.ObjStaInfoList.Count);
                    foreach (ObjStatusSyncInfo info in cash.ObjStaInfoList)
                    {
                        SW.WriteLine(info.objName);
                        SW.WriteLine(info.statusName);
                        SW.WriteLine(info.values.Length);
                        foreach (object obj in info.values)
                        {
                            SW.WriteLine(obj.GetType().ToString());
                            SW.WriteLine(obj);
                        }
                    }
                    SW.Flush();
                    bufStream.Flush();

                    Console.WriteLine("Send Package To Server");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }
        }

        static public void StartReceiveThread(SyncCashe cash)
        {
            //thread = new Thread(ReceivePackge);
            //thread.Start(cash);
        }

        static public void CloseThread()
        {
            thread.Abort();
        }

        static public void ReceivePackge(object obj)
        {
            SyncCashe cashe = (SyncCashe)obj;
            try
            {
                while (true)
                {
                    if (sckt.Connected)
                    {
                        Stream netStream = new NetworkStream(sckt);
                        BufferedStream bufStream = new BufferedStream(netStream);
                        if (bufStream.CanRead)
                        {
                            StreamReader SR = new StreamReader(netStream);

                            byte[] head = new byte[4];
                            netStream.Read(head, 0, 4);
                            Console.WriteLine(SR.ReadLine());

                            int staCount = int.Parse(SR.ReadLine());
                            for (int i = 0; i < staCount; i++)
                            {
                                ObjStatusSyncInfo info = new ObjStatusSyncInfo();
                                info.objName = SR.ReadLine();
                                info.statusName = SR.ReadLine();
                                int valuesLength = int.Parse(SR.ReadLine());
                                info.values = new object[valuesLength];
                                for (int j = 0; j < valuesLength; j++)
                                {
                                    string type = SR.ReadLine();
                                    switch (type)
                                    {
                                        case "float":
                                            info.values[j] = float.Parse(SR.ReadLine());
                                            break;
                                        case "string":
                                            info.values[j] = SR.ReadLine();
                                            break;
                                        default:
                                            break;
                                    }
                                }

                                cashe.ObjStaInfoList.Add(info);

                                Console.WriteLine(info.objName);
                                Console.WriteLine(info.statusName);
                                foreach (Object value in info.values)
                                {
                                    Console.WriteLine(value);
                                }
                            }



                            SR.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }
        }

        static public bool CloseConnect()
        {
            sckt.Disconnect(false);
            return true;
        }

    }
}

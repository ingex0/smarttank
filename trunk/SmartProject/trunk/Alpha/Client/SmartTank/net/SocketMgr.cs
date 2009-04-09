using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TankEngine2D.Helpers;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;

namespace SmartTank.net
{
    struct SocketConfig
    {
        public string ServerIP;
        public int ServerPort;
    };


    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct stPkgHead
    {
        static public int Size = 8;
        //static public int MaxDataSize = 1028;
        //static public int TotolSize
        //{
        //    get { return Size + MaxDataSize; }
        //}


        public int iSytle;
        public int dataSize;

        //[MarshalAs(UnmanagedType.ByValStr, SizeConst = 1024)]
        //public string data;
        //int���飬SizeConst��ʾ����ĸ�������ת����
        //byte����ǰ�����ȳ�ʼ�����飬��ʹ�ã���ʼ��
        //�����鳤�ȱ����SizeConstһ�£���test = new int[6];
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        //public int[] test;

    }



    static public class SocketMgr
    {
        static SocketConfig config;
        static TcpClient client;
        //static Socket sckt;
        static Thread thread;

        static bool ReadOver = false;

        /// <summary>
        /// ��ʼ�����������ļ�
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
                //sckt = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                //sckt.Connect(endPoint);
                //if (sckt.Connected)
                //    return true;
                //else
                //    return false;
                client = new TcpClient();
                client.Connect(endPoint);
                if (client.Connected)
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

        static public void SendGameLogicPackge(SyncCashe cash)
        {
            try
            {
                if (client.Connected)
                {
                    Stream netStream = client.GetStream();

                    if (netStream.CanWrite)
                    {

                        // Head
                        stPkgHead head = new stPkgHead();
                        head.iSytle = (int)PACKAGE_SYTLE.DATA;

                        MemoryStream MStream = new MemoryStream();
                        StreamWriter SW = new StreamWriter(MStream);

                        // Data
                        SW.WriteLine("Fuck server!");

                        // ״̬ͬ������
                        SW.WriteLine(cash.ObjStaInfoList.Count);
                        foreach (ObjStatusSyncInfo info in cash.ObjStaInfoList)
                        {
                            SW.WriteLine(info.objMgPath);
                            SW.WriteLine(info.statusName);
                            SW.WriteLine(info.values.Length);
                            foreach (object obj in info.values)
                            {
                                WriteObjToStream(SW, obj);
                            }
                        }

                        // �¼�ͬ������
                        SW.WriteLine(cash.ObjEventInfoList.Count);
                        foreach (ObjEventSyncInfo info in cash.ObjEventInfoList)
                        {
                            SW.WriteLine(info.objMgPath);
                            SW.WriteLine(info.EventName);
                            SW.WriteLine(info.values.Length);
                            foreach (object obj in info.values)
                            {
                                WriteObjToStream(SW, obj);
                            }
                        }

                        // �������ͬ������
                        SW.WriteLine(cash.ObjMgInfoList.Count);
                        foreach (ObjMgSyncInfo info in cash.ObjMgInfoList)
                        {
                            SW.WriteLine(info.objPath);
                            SW.WriteLine(info.objMgKind);
                            SW.WriteLine(info.objType);
                            SW.WriteLine(info.args.Length);
                            foreach (object obj in info.args)
                            {
                                WriteObjToStream(SW, obj);
                            }
                        }

                        SW.Flush();
                        int dataLength = (int)MStream.Length;
                        //int dataLength = (int)SW.BaseStream.Length;
                        head.dataSize = dataLength;
                        netStream.Write(StructToBytes(head), 0, stPkgHead.Size);
                        MStream.WriteTo(netStream);

                        // �ͷ���Դ
                        SW.Close();
                        MStream.Close();

                        Console.WriteLine("Send " + head.iSytle + " " + head.dataSize);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }

        }

        static public void SendCommonPackge(stPkgHead head, MemoryStream data)
        {
            if (head.dataSize != 0 && (data == null || head.dataSize != data.Length))
                throw new Exception("���͵İ�ͷ����");

            try
            {
                if (client.Connected)
                {
                    Stream netStream = client.GetStream();

                    if (netStream.CanWrite)
                    {
                        netStream.Write(StructToBytes(head), 0, stPkgHead.Size);
                        if (data != null)
                            data.WriteTo(netStream);

                        Console.WriteLine("Send " + head.iSytle + " " + head.dataSize);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }
        }


        static public void ReceivePackge(object obj)
        {
            SyncCashe cashe = (SyncCashe)obj;
            try
            {
                while (!ReadOver)
                {
                    if (client.Connected)
                    {
                        byte[] buffer = new byte[stPkgHead.Size];
                        Stream netStream = client.GetStream();

                        // ����Ҫȷ��ÿ�ζ�һ������
                        netStream.Read(buffer, 0, stPkgHead.Size);
                        stPkgHead pkg = (stPkgHead)BytesToStuct(buffer, typeof(stPkgHead));

                        Console.WriteLine("ReceiveDataPkg:");
                        Console.WriteLine("Sytle : " + pkg.iSytle);
                        Console.WriteLine("Size : " + pkg.dataSize);

                        // �˳��ź�
                        if (pkg.iSytle == (int)PACKAGE_SYTLE.EXIT)
                        {
                            Console.WriteLine("read close info");
                            break;
                        }

                        // ����
                        else if (pkg.iSytle == (int)PACKAGE_SYTLE.DATA)
                        {

                            byte[] readData = new byte[pkg.dataSize];
                            netStream.Read(readData, 0, pkg.dataSize);

                            MemoryStream memStream = new MemoryStream(readData);

                            StreamReader SR = new StreamReader(memStream);
                            try
                            {
                                string temp = SR.ReadLine();
                                //Console.WriteLine("Begin: " + temp);


                                // ����״̬ͬ������
                                temp = SR.ReadLine();
                                //Console.WriteLine("StaCount: " + temp);
                                int StaCount = int.Parse(temp);


                                for (int i = 0; i < StaCount; i++)
                                {
                                    ObjStatusSyncInfo info = new ObjStatusSyncInfo();

                                    temp = SR.ReadLine();
                                    //Console.WriteLine("ObjName:" + temp);
                                    info.objMgPath = temp;


                                    temp = SR.ReadLine();
                                    //Console.WriteLine("StatusName: " + temp);
                                    info.statusName = temp;


                                    temp = SR.ReadLine();
                                    //Console.WriteLine("ValueCount: " + temp);
                                    int valueCount = int.Parse(temp);


                                    info.values = new object[valueCount];
                                    for (int j = 0; j < valueCount; j++)
                                    {
                                        info.values[j] = ReadObjFromStream(SR);
                                    }

                                    // ���Info��������
                                    Monitor.Enter(cashe);
                                    cashe.ObjStaInfoList.Add(info);
                                    Monitor.Exit(cashe);

                                    //Console.WriteLine("Read Pkg Success!");
                                }

                                // �¼�ͬ������
                                temp = SR.ReadLine();
                                int EventCount = int.Parse(temp);
                                for (int i = 0; i < EventCount; i++)
                                {
                                    ObjEventSyncInfo info = new ObjEventSyncInfo();

                                    temp = SR.ReadLine();
                                    info.objMgPath = temp;


                                    temp = SR.ReadLine();
                                    info.EventName = temp;


                                    temp = SR.ReadLine();
                                    int valueCount = int.Parse(temp);

                                    info.values = new object[valueCount];
                                    for (int j = 0; j < valueCount; j++)
                                    {
                                        info.values[j] = ReadObjFromStream(SR);
                                    }

                                    // ���Info��������
                                    Monitor.Enter(cashe);
                                    cashe.ObjEventInfoList.Add(info);
                                    Monitor.Exit(cashe);

                                }

                                // �������ͬ������

                                temp = SR.ReadLine();
                                int ObjMgCount = int.Parse(temp);
                                for (int i = 0; i < ObjMgCount; i++)
                                {
                                    ObjMgSyncInfo info = new ObjMgSyncInfo();

                                    temp = SR.ReadLine();
                                    info.objPath = temp;


                                    temp = SR.ReadLine();
                                    info.objMgKind = int.Parse(temp);


                                    temp = SR.ReadLine();
                                    info.objType = temp;

                                    temp = SR.ReadLine();
                                    int valueCount = int.Parse(temp);

                                    info.args = new object[valueCount];
                                    for (int j = 0; j < valueCount; j++)
                                    {
                                        info.args[j] = ReadObjFromStream(SR);
                                    }

                                    // ���Info��������
                                    Monitor.Enter(cashe);
                                    cashe.ObjMgInfoList.Add(info);
                                    Monitor.Exit(cashe);

                                }

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                            finally
                            {
                                memStream.Close();
                                ////��Ҫ��յ�ǰ����ʣ������
                                //int left = stPakage.TotolSize / sizeof(char) - (int)SR.BaseStream.Position;
                                //char[] temp = new char[left];
                                //SR.Read(temp, 0, left);
                            }
                        }
                        else
                        {
                            if (pkg.dataSize < 1024 * 10 && pkg.dataSize >= 0)
                            {
                                byte[] temp = new byte[pkg.dataSize];
                                netStream.Read(temp, 0, pkg.dataSize);

                            }
                            else
                            {
                                Log.Write("��ͷ������: type: " + pkg.iSytle + " , size: " + pkg.dataSize);
                                Console.WriteLine("��ͷ������: type: " + pkg.iSytle + " , size: " + pkg.dataSize);
                                //throw new Exception("��ͷ�����Ⱑ������");
                                // ��ͷ�ܿ���������
                            }
                        }

                    }
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                client.Close();
            }
        }



        static public void StartReceiveThread(SyncCashe cash)
        {
            if (client.Connected)
            {
                thread = new Thread(ReceivePackge);
                thread.Start(cash);
            }
        }

        static public bool Close()
        {
            if (client.Connected)
            {
                stPkgHead pkg = new stPkgHead();
                pkg.iSytle = (int)PACKAGE_SYTLE.EXIT;
                client.GetStream().Write(StructToBytes(pkg), 0, stPkgHead.Size);
            }
            return true;
        }

        static public void CloseThread()
        {
            ReadOver = true;
            //thread.Abort();

            //if (sckt.Connected)
            //{
            //    sckt.Shutdown(SocketShutdown.Both);
            //    sckt.Disconnect(false);
            //    sckt.Close();
            //}
        }

        private static void WriteObjToStream(StreamWriter SW, object obj)
        {
            if (obj == null)
            {
                SW.WriteLine("null");
                return;
            }

            SW.WriteLine(obj.GetType().ToString());
            switch (obj.GetType().ToString())
            {
                case "Microsoft.Xna.Framework.Vector2":
                    SW.WriteLine(((Microsoft.Xna.Framework.Vector2)obj).X);
                    SW.WriteLine(((Microsoft.Xna.Framework.Vector2)obj).Y);
                    break;
                case "SmartTank.net.GameObjSyncInfo":
                    SW.WriteLine(((GameObjSyncInfo)obj).MgPath);
                    break;
                case "TankEngine2D.Graphics.CollisionResult":
                    TankEngine2D.Graphics.CollisionResult result = obj as TankEngine2D.Graphics.CollisionResult;
                    WriteObjToStream(SW, result.InterPos);
                    WriteObjToStream(SW, result.NormalVector);
                    WriteObjToStream(SW, result.IsCollided);
                    break;
                case "SmartTank.GameObjs.GameObjInfo":
                    SmartTank.GameObjs.GameObjInfo objInfo = obj as SmartTank.GameObjs.GameObjInfo;
                    SW.WriteLine(objInfo.ObjClass);
                    SW.WriteLine(objInfo.Script);
                    break;
                default:
                    SW.WriteLine(obj);
                    break;
            }
        }

        private static object ReadObjFromStream(StreamReader SR)
        {
            string temp;

            temp = SR.ReadLine();
            //Console.WriteLine("TypeName: " + temp);

            if (temp == "null")
                return null;

            switch (temp)
            {
                case "System.String":
                    temp = SR.ReadLine();
                    //Console.WriteLine("StringValue: " + temp);
                    return temp;
                case "System.Single":
                    temp = SR.ReadLine();
                    //Console.WriteLine("FloatValue: " + temp);
                    return float.Parse(temp);
                case "Microsoft.Xna.Framework.Vector2":
                    temp = SR.ReadLine();
                    float x = float.Parse(temp);
                    temp = SR.ReadLine();
                    float y = float.Parse(temp);
                    return new Microsoft.Xna.Framework.Vector2(x, y);
                case "SmartTank.net.GameObjSyncInfo":
                    temp = SR.ReadLine();
                    return new GameObjSyncInfo(temp);
                case "TankEngine2D.Graphics.CollisionResult":
                    Vector2 InterPos = (Vector2)ReadObjFromStream(SR);
                    Vector2 NormalVector = (Vector2)ReadObjFromStream(SR);
                    bool IsCollided = (bool)ReadObjFromStream(SR);
                    return new TankEngine2D.Graphics.CollisionResult(IsCollided, InterPos, NormalVector);
                case "System.Boolean":
                    temp = SR.ReadLine();
                    return bool.Parse(temp);
                case "SmartTank.GameObjs.GameObjInfo":
                    string objClass = SR.ReadLine();
                    string script = SR.ReadLine();
                    return new SmartTank.GameObjs.GameObjInfo(objClass, script);
                default:
                    Console.WriteLine(temp);
                    throw new Exception("ĳ����ʽ������û�б�����");
            }
        }


        /// <summary>
        /// �ṹ��תbyte����
        /// </summary>
        /// <param name="structObj">Ҫת���Ľṹ��</param>
        /// <returns>ת�����byte����</returns>
        static public byte[] StructToBytes(object structObj)
        {
            //�õ��ṹ��Ĵ�С
            int size = Marshal.SizeOf(structObj);
            //����byte����
            byte[] bytes = new byte[size];
            //����ṹ���С���ڴ�ռ�
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //���ṹ�忽������õ��ڴ�ռ�
            Marshal.StructureToPtr(structObj, structPtr, false);
            //���ڴ�ռ俽��byte����
            Marshal.Copy(structPtr, bytes, 0, size);
            //�ͷ��ڴ�ռ�
            Marshal.FreeHGlobal(structPtr);
            //����byte����
            return bytes;
        }

        /// <summary>
        /// byte����ת�ṹ��
        /// </summary>
        /// <param name="bytes">byte����</param>
        /// <param name="type">�ṹ������</param>
        /// <returns>ת����Ľṹ��</returns>
        public static object BytesToStuct(byte[] bytes, Type type)
        {
            //�õ��ṹ��Ĵ�С
            int size = Marshal.SizeOf(type);
            //byte���鳤��С�ڽṹ��Ĵ�С
            if (size > bytes.Length)
            {
                //���ؿ�
                return null;
            }
            //����ṹ���С���ڴ�ռ�
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //��byte���鿽������õ��ڴ�ռ�
            Marshal.Copy(bytes, 0, structPtr, size);
            //���ڴ�ռ�ת��ΪĿ��ṹ��
            object obj = Marshal.PtrToStructure(structPtr, type);
            //�ͷ��ڴ�ռ�
            Marshal.FreeHGlobal(structPtr);
            //���ؽṹ��
            return obj;
        }

    }
}

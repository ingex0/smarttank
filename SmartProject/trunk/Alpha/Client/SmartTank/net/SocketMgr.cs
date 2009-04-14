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
        //int数组，SizeConst表示数组的个数，在转换成
        //byte数组前必须先初始化数组，再使用，初始化
        //的数组长度必须和SizeConst一致，例test = new int[6];
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        //public int[] test;

    }



    static public class SocketMgr
    {
        public delegate void ReceivePkgEventHandler(stPkgHead head, byte[] readData);
        public static event ReceivePkgEventHandler OnReceivePkg;

        static SocketConfig config;
        static TcpClient client;
        //static Socket sckt;
        static Thread thread;
        static SyncCashe sInputCashe;


        /// <summary>
        /// 初始化，读配置文件
        /// </summary>
        public static void Initial()
        {
            ReadConfig();
            InitialTypeTable();
        }

        static public void SetInputCahes(SyncCashe inputCashe)
        {
            sInputCashe = inputCashe;
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
                stream.Close();
            }
            catch (Exception ex)
            {
                Log.Write("net.SocketMgr ReadConfig error!" + ex.ToString());
            }
        }

        static public bool ConnectToServer()
        {
            if (client != null && client.Connected == true)
                return true;

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
                        //StreamWriter SW = new StreamWriter(MStream);
                        BinaryWriter BW = new BinaryWriter(MStream);


                        // 状态同步数据
                        BW.Write(cash.ObjStaInfoList.Count);

                        
                        //SW.WriteLine(cash.ObjStaInfoList.Count);
                        foreach (ObjStatusSyncInfo info in cash.ObjStaInfoList)
                        {
                            //SW.WriteLine(info.objMgPath);
                            BW.Write(info.objMgPath);
                            BW.Write(info.statusName);
                            BW.Write(info.values.Length);
                            foreach (object obj in info.values)
                            {
                                WriteObjToStream(BW, obj);
                            }
                        }

                        // 事件同步数据
                        BW.Write(cash.ObjEventInfoList.Count);
                        foreach (ObjEventSyncInfo info in cash.ObjEventInfoList)
                        {
                            BW.Write(info.objMgPath);
                            BW.Write(info.EventName);
                            BW.Write(info.values.Length);
                            foreach (object obj in info.values)
                            {
                                WriteObjToStream(BW, obj);
                            }
                        }

                        // 物体管理同步数据
                        BW.Write(cash.ObjMgInfoList.Count);
                        foreach (ObjMgSyncInfo info in cash.ObjMgInfoList)
                        {
                            BW.Write(info.objPath);
                            BW.Write(info.objMgKind);
                            BW.Write(info.objType);
                            BW.Write(info.args.Length);
                            foreach (object obj in info.args)
                            {
                                WriteObjToStream(BW, obj);
                            }
                        }

                        // 用户自定义数据
                        BW.Write(cash.UserDefineInfoList.Count);
                        foreach (UserDefineInfo info in cash.UserDefineInfoList)
                        {
                            BW.Write(info.infoName);
                            BW.Write(info.infoID);
                            BW.Write(info.args.Length);
                            foreach (object obj in info.args)
                            {
                                WriteObjToStream(BW, obj);
                            }
                        }

                        BW.Flush();
                        int dataLength = (int)MStream.Length;
                        //int dataLength = (int)SW.BaseStream.Length;

                        byte[] temp = new byte[dataLength];
                        MStream.Position = 0;
                        MStream.Read(temp, 0, dataLength);

                        head.dataSize = temp.Length;


                        netStream.Write(StructToBytes(head), 0, stPkgHead.Size);
                        netStream.Write(temp, 0, head.dataSize);

                        // 释放资源
                        BW.Close();
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
                throw new Exception("发送的包头有误");

            try
            {
                if (client != null && client.Connected)
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

        static public void ReceivePackge()
        {
            
            try
            {
                while (true)
                {
                    if (client.Connected)
                    {
                        byte[] buffer = new byte[stPkgHead.Size];
                        Stream netStream = client.GetStream();

                        if (netStream.CanRead)
                        {
                            // 这里要确保每次读一个整包
                            netStream.Read(buffer, 0, stPkgHead.Size);
                            stPkgHead pkg = (stPkgHead)BytesToStuct(buffer, typeof(stPkgHead));

                            Console.WriteLine("ReceiveDataPkg:");
                            Console.WriteLine("Sytle : " + pkg.iSytle);
                            Console.WriteLine("Size : " + pkg.dataSize);

                            int recvLength = 0;
                            byte[] readData = new byte[pkg.dataSize];
                            while (recvLength < pkg.dataSize)
                            {
                                recvLength += netStream.Read(readData, recvLength, pkg.dataSize - recvLength);
                            }
                            if (recvLength != pkg.dataSize)
                            {
                                throw new Exception("危险：收到包的长度不等于实际包长");
                            }

                            // 退出信号
                            if (pkg.iSytle == (int)PACKAGE_SYTLE.EXIT)
                            {
                                Console.WriteLine("read close info");
                                break;
                            }

                            // 数据
                            else if (pkg.iSytle == (int)PACKAGE_SYTLE.DATA)
                            {

                                //byte[] readData = new byte[pkg.dataSize];
                                //netStream.Read(readData, 0, pkg.dataSize);
                                SyncCashe cashe = sInputCashe;

                                MemoryStream memStream = new MemoryStream(readData);

                                //StreamReader SR = new StreamReader(memStream);
                                BinaryReader BR = new BinaryReader(memStream);
                                try
                                {
                                    //string temp = ""; 

                                    // 物体状态同步数据
                                    //temp = SR.ReadLine();
                                    int StaCount = BR.ReadInt32();//int.Parse(temp);


                                    for (int i = 0; i < StaCount; i++)
                                    {
                                        ObjStatusSyncInfo info = new ObjStatusSyncInfo();

                                        //temp = SR.ReadLine();
                                        //Console.WriteLine("ObjName:" + temp);
                                        info.objMgPath = BR.ReadString();//temp;


                                        //temp = SR.ReadLine();
                                        //Console.WriteLine("StatusName: " + temp);
                                        info.statusName = BR.ReadString();//temp;


                                        //temp = SR.ReadLine();
                                        //Console.WriteLine("ValueCount: " + temp);
                                        int valueCount = BR.ReadInt32(); //int.Parse(temp);


                                        info.values = new object[valueCount];
                                        for (int j = 0; j < valueCount; j++)
                                        {
                                            info.values[j] = ReadObjFromStream(BR);
                                        }

                                        // 添加Info到缓冲区
                                        Monitor.Enter(cashe);
                                        cashe.ObjStaInfoList.Add(info);
                                        Monitor.Exit(cashe);

                                        //Console.WriteLine("Read Pkg Success!");
                                    }

                                    // 事件同步数据
                                    //temp = SR.ReadLine();
                                    int EventCount = BR.ReadInt32(); //int.Parse(temp);
                                    for (int i = 0; i < EventCount; i++)
                                    {
                                        ObjEventSyncInfo info = new ObjEventSyncInfo();

                                        //temp = SR.ReadLine();
                                        info.objMgPath = BR.ReadString();//temp;


                                        //temp = SR.ReadLine();
                                        info.EventName = BR.ReadString();// temp;


                                        //temp = SR.ReadLine();
                                        int valueCount = BR.ReadInt32();// int.Parse(temp);

                                        info.values = new object[valueCount];
                                        for (int j = 0; j < valueCount; j++)
                                        {
                                            info.values[j] = ReadObjFromStream(BR);
                                        }

                                        // 添加Info到缓冲区
                                        Monitor.Enter(cashe);
                                        cashe.ObjEventInfoList.Add(info);
                                        Monitor.Exit(cashe);

                                    }

                                    // 物体管理同步数据

                                    //temp = SR.ReadLine();
                                    int ObjMgCount = BR.ReadInt32(); //int.Parse(temp);
                                    for (int i = 0; i < ObjMgCount; i++)
                                    {
                                        ObjMgSyncInfo info = new ObjMgSyncInfo();

                                        //temp = SR.ReadLine();
                                        info.objPath = BR.ReadString();//temp;


                                        //temp = SR.ReadLine();
                                        info.objMgKind = BR.ReadInt32(); //int.Parse(temp);


                                        //temp = SR.ReadLine();
                                        info.objType = BR.ReadString();//temp;

                                        //temp = SR.ReadLine();
                                        int valueCount = BR.ReadInt32();// int.Parse(temp);

                                        info.args = new object[valueCount];
                                        for (int j = 0; j < valueCount; j++)
                                        {
                                            info.args[j] = ReadObjFromStream(BR);
                                        }

                                        // 添加Info到缓冲区
                                        Monitor.Enter(cashe);
                                        cashe.ObjMgInfoList.Add(info);
                                        Monitor.Exit(cashe);

                                    }

                                    // 用户自定义数据
                                    //temp = BR.ReadLine();
                                    int UserDefinCount = BR.ReadInt32();// int.Parse(temp);
                                    for (int i = 0; i < UserDefinCount; i++)
                                    {
                                        UserDefineInfo info = new UserDefineInfo();

                                        //temp = SR.ReadLine();
                                        info.infoName = BR.ReadString();// temp;
                                        //temp = SR.ReadLine();
                                        info.infoID = BR.ReadString();//temp;

                                        //temp = SR.ReadLine();
                                        int valueCount = BR.ReadInt32();//int.Parse(temp);

                                        info.args = new object[valueCount];
                                        for (int j = 0; j < valueCount; j++)
                                        {
                                            info.args[j] = ReadObjFromStream(BR);
                                        }
                                        Monitor.Enter(cashe);
                                        cashe.UserDefineInfoList.Add(info);
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
                                    ////需要清空当前包中剩余内容
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
                                    readData.CopyTo(temp, 0);
                                    if (OnReceivePkg != null)
                                        OnReceivePkg(pkg, temp);

                                }
                                else
                                {
                                    Log.Write("包头有问题: type: " + pkg.iSytle + " , size: " + pkg.dataSize);
                                    Console.WriteLine("包头有问题: type: " + pkg.iSytle + " , size: " + pkg.dataSize);
                                    //throw new Exception("包头有问题啊，我日");
                                    // 包头很可能有问题
                                }
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
                client = null;
            }
        }



        static public void StartReceiveThread()
        {
            if (client.Connected && thread == null)
            {
                thread = new Thread(ReceivePackge);
                thread.Start();
            }
        }

        static public bool Close()
        {
            if (client.Connected)
            {
                stPkgHead pkg = new stPkgHead();
                pkg.iSytle = (int)PACKAGE_SYTLE.EXIT;
                client.GetStream().Write(StructToBytes(pkg), 0, stPkgHead.Size);
                thread = null;
            }
            return true;
        }

        static Dictionary<string,UInt16> TypeIndexTable = new Dictionary<string,ushort>();

        static void InitialTypeTable()
        {
            TypeIndexTable.Clear();
            TypeIndexTable.Add("null", 0);            
            TypeIndexTable.Add("Microsoft.Xna.Framework.Vector2", 1);
            TypeIndexTable.Add("SmartTank.net.GameObjSyncInfo", 2);
            TypeIndexTable.Add("TankEngine2D.Graphics.CollisionResult", 3);
            TypeIndexTable.Add("SmartTank.GameObjs.GameObjInfo", 4);
            TypeIndexTable.Add("System.Boolean", 5);
            TypeIndexTable.Add("System.Int32", 6);
            TypeIndexTable.Add("System.String", 7);
            TypeIndexTable.Add("System.Single", 8);
            
        }

        private static void WriteObjToStream(BinaryWriter BW, object obj)
        {
            if (obj == null)
            {
                BW.Write(TypeIndexTable["null"]);
                return;
            }

            //BW.Write(obj.GetType().ToString());
            BW.Write(TypeIndexTable[obj.GetType().ToString()]);
            switch (obj.GetType().ToString())
            {
                case "Microsoft.Xna.Framework.Vector2":
                    BW.Write(((Microsoft.Xna.Framework.Vector2)obj).X);
                    BW.Write(((Microsoft.Xna.Framework.Vector2)obj).Y);
                    break;
                case "SmartTank.net.GameObjSyncInfo":
                    BW.Write(((GameObjSyncInfo)obj).MgPath);
                    break;
                case "TankEngine2D.Graphics.CollisionResult":
                    TankEngine2D.Graphics.CollisionResult result = obj as TankEngine2D.Graphics.CollisionResult;
                    WriteObjToStream(BW, result.InterPos);
                    WriteObjToStream(BW, result.NormalVector);
                    WriteObjToStream(BW, result.IsCollided);
                    break;
                case "SmartTank.GameObjs.GameObjInfo":
                    SmartTank.GameObjs.GameObjInfo objInfo = obj as SmartTank.GameObjs.GameObjInfo;
                    BW.Write(objInfo.ObjClass);
                    BW.Write(objInfo.Script);
                    break;
                case "System.Boolean":
                    BW.Write((bool)obj);
                    break;
                case "System.Int32":
                    BW.Write((Int32)obj);
                    break;
                case "System.String":
                    BW.Write((String)obj);
                    break;
                case "System.Single":
                    BW.Write((Single)obj);
                    break;

                default:
                    throw new Exception("某种类型的数据没有发送");
                    break;
            }
        }

        private static object ReadObjFromStream(BinaryReader BR)
        {
            //string temp;

            //temp = BR.ReadString();
            //Console.WriteLine("TypeName: " + temp);
            UInt16 typeIndex = BR.ReadUInt16();

            if (typeIndex == TypeIndexTable["null"])
                return null;

            switch (typeIndex)
            {
                case 7://TypeIndexTable["System.String"]:
                    return BR.ReadString();
                case 8://TypeIndexTable["System.Single"]:
                    return BR.ReadSingle();//float.Parse(temp);
                case 1://TypeIndexTable["Microsoft.Xna.Framework.Vector2"]:
                    //temp = SR.ReadLine();
                    float x = BR.ReadSingle();// float.Parse(temp);
                    //temp = SR.ReadLine();
                    float y = BR.ReadSingle();// float.Parse(temp);
                    return new Microsoft.Xna.Framework.Vector2(x, y);
                case 2://TypeIndexTable["SmartTank.net.GameObjSyncInfo"]:
                    //temp = ;//SR.ReadLine();
                    return new GameObjSyncInfo(BR.ReadString());
                case 3://TypeIndexTable["TankEngine2D.Graphics.CollisionResult"]:
                    Vector2 InterPos = (Vector2)ReadObjFromStream(BR);
                    Vector2 NormalVector = (Vector2)ReadObjFromStream(BR);
                    bool IsCollided = (bool)ReadObjFromStream(BR);
                    return new TankEngine2D.Graphics.CollisionResult(IsCollided, InterPos, NormalVector);
                case 5://TypeIndexTable["System.Boolean"]:
                    //temp = SR.ReadLine();
                    return BR.ReadBoolean();//bool.Parse(temp);
                case 6://TypeIndexTable["System.Int32"]:
                    //temp = SR.ReadLine();
                    return BR.ReadInt32();// int.Parse(temp);
                case 4://TypeIndexTable["SmartTank.GameObjs.GameObjInfo"]:
                    string objClass = BR.ReadString();
                    string script = BR.ReadString();
                    return new SmartTank.GameObjs.GameObjInfo(objClass, script);
                default:
                    Console.WriteLine(typeIndex);
                    throw new Exception("某个格式的数据没有被解析");
            }
        }


        /// <summary>
        /// 结构体转byte数组
        /// </summaryss
        /// <param name="structObj">要转换的结构体</param>
        /// <returns>转换后的byte数组</returns>
        static public byte[] StructToBytes(object structObj)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(structObj);
            //创建byte数组
            byte[] bytes = new byte[size];
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(structObj, structPtr, false);
            //从内存空间拷到byte数组
            Marshal.Copy(structPtr, bytes, 0, size);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回byte数组
            return bytes;
        }

        /// <summary>
        /// byte数组转结构体
        /// </summary>
        /// <param name="bytes">byte数组</param>
        /// <param name="type">结构体类型</param>
        /// <returns>转换后的结构体</returns>
        public static object BytesToStuct(byte[] bytes, Type type)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(type);
            //byte数组长度小于结构体的大小
            if (size > bytes.Length)
            {
                //返回空
                return null;
            }
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将byte数组拷到分配好的内存空间
            Marshal.Copy(bytes, 0, structPtr, size);
            //将内存空间转换为目标结构体
            object obj = Marshal.PtrToStructure(structPtr, type);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回结构体
            return obj;
        }


    }
}

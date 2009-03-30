using System;
using System.Collections.Generic;
using System.Text;
using TankEngine2D.Helpers;
using SmartTank.AI;
using System.IO;
using System.Reflection;

namespace SmartTank.Helpers.DependInject
{
    public class AILoader
    {
        readonly static string AIDirectory = "AIs";
        readonly static string AIListPath = "AIs\\List.xml";

        AssetList list;

        List<Type> interList;

        List<Type> compatibleAIs;
        List<string> compatibleAINames;

        public AILoader ()
        {
            Directory.SetCurrentDirectory( System.Environment.CurrentDirectory );
            if (!Directory.Exists( AIDirectory ))
            {
                Directory.CreateDirectory( AIDirectory );
            }
            if (File.Exists( AIListPath ))
            {
                list = AssetList.Load( File.Open( AIListPath, FileMode.OpenOrCreate ) );
            }
            interList = new List<Type>();
            compatibleAIs = new List<Type>();
            compatibleAINames = new List<string>();

        }

        public void InitialCompatibleAIs ( Type OrderServerType, Type CommonServerType )
        {
            foreach (Type type in interList)
            {
                if (!IsTankAI( type ))
                    continue;

                if (IsCompatibleAI( type, OrderServerType, CommonServerType ))
                {
                    object[] attributes = type.GetCustomAttributes( false );
                    foreach (object attri in attributes)
                    {
                        if (attri is AIAttribute)
                        {
                            string name = ((AIAttribute)attri).Name;
                            compatibleAINames.Add( name );
                            compatibleAIs.Add( type );
                            break;
                        }
                    }
                }
            }

            if (list != null)
            {
                foreach (AssetItem item in list.list)
                {
                    Directory.SetCurrentDirectory( System.Environment.CurrentDirectory );
                    //Assembly assembly = DIHelper.GetAssembly( Path.Combine( AIDirectory, item.DLLName ) );
                    Assembly assembly = GetAssembly( Path.Combine( AIDirectory, item.DLLName ) );
                    foreach (Type type in assembly.GetTypes())
                    {

                        if (!IsTankAI( type ))
                            continue;

                        if (IsCompatibleAI( type, OrderServerType, CommonServerType ))
                        {
                            object[] attributes = type.GetCustomAttributes( false );
                            foreach (object attri in attributes)
                            {
                                if (attri is AIAttribute)
                                {
                                    string name = ((AIAttribute)attri).Name;
                                    compatibleAINames.Add( name );
                                    compatibleAIs.Add( type );
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void AddInterAI ( Type AIType )
        {
            interList.Add( AIType );
        }

        public string[] GetAIList ()
        {
            return compatibleAINames.ToArray();
        }

        public IAI GetAIInstance ( int index )
        {
            if (index < 0 || index > compatibleAIs.Count)
                return null;
            return (IAI)DIHelper.GetInstance( compatibleAIs[index] );
        }

        public bool IsTankAI ( Type type )
        {
            foreach (Type interfa in type.GetInterfaces())
            {
                if (interfa.Equals( typeof( IAI ) ))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsCompatibleAI ( Type type, Type OrderServerType, Type CommonServerType )
        {
            Type maxIAI = MaxIAI( type );

            object[] attributes = maxIAI.GetCustomAttributes( false );
            foreach (object attri in attributes)
            {
                if (attri is IAIAttribute)
                {
                    bool orderServerCompatible = IsChildType( OrderServerType, ((IAIAttribute)attri).OrderServerType ) ||
                        OrderServerType == ((IAIAttribute)attri).OrderServerType;
                    bool commonServerCompatible = IsChildType( CommonServerType, ((IAIAttribute)attri).CommonServerType ) ||
                        CommonServerType == ((IAIAttribute)attri).CommonServerType;
                    if (orderServerCompatible && commonServerCompatible)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private Type MaxIAI ( Type AIClasstype )
        {
            List<Type> IAIs = new List<Type>();
            foreach (Type interfa in AIClasstype.GetInterfaces())
            {
                if (IsChildType( interfa, typeof( IAI ) ))
                {
                    IAIs.Add( interfa );
                }
            }

            Type maxIAI = typeof( IAI );
            foreach (Type type in IAIs)
            {
                if (IsChildType( type, maxIAI ))
                    maxIAI = type;
            }

            return maxIAI;
        }

        private bool IsChildType ( Type type, Type childType )
        {
            foreach (Type interfa in type.GetInterfaces())
            {
                if (interfa.Equals( childType ))
                {
                    return true;
                }
            }
            return false;
        }

        static public Assembly GetAssembly ( string assetFullName )
        {
            Assembly assembly = null;
            try
            {
                assembly = Assembly.LoadFile( Path.Combine( System.Environment.CurrentDirectory, assetFullName ) );
            }
            catch (Exception)
            {
                Log.Write( "Load Assembly error : " + assetFullName + " load unsucceed!" );
                return null;
            }

            return assembly;
        }
    }
}

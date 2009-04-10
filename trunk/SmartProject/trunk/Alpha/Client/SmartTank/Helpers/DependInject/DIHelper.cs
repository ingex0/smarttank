using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using TankEngine2D.Helpers;
using System.IO;

namespace SmartTank.Helpers.DependInject
{

    public static class DIHelper
    {

        /// <summary>
        /// 使用Assembly>.LoadFile函数获取程序集。
        /// </summary>
        /// <param name="assetFullName"></param>
        /// <returns></returns>
        static public Assembly GetAssembly ( string assetPath )
        {
            Assembly assembly = null;

            try
            {
                assembly = Assembly.LoadFile( Path.Combine( System.Environment.CurrentDirectory, assetPath ) );
            }
            catch (Exception)
            {
                Log.Write( "Load Assembly error : " + assetPath + " load unsucceed!" );
                return null;
            }

            return assembly;
        }

        static public object GetInstance ( Type type )
        {
            return Activator.CreateInstance( type );
        }

        internal static Type GetType(string typepath)
        {
            string[] path = typepath.Split('.');
            if (path[0] == "SmartTank")
            {
                Assembly assembly = Assembly.Load("SmartTank, Version=1.0.0.0, Culture=neutral, PublicKeyToToken=null");
                return assembly.GetType(typepath);
            }
            if (path[0] == "InterRules")
            {
                Assembly assembly = Assembly.Load("InterRules, Version=1.0.0.0, Culture=neutral, PublicKeyToToken=null");
                return assembly.GetType(typepath);
            }
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using GameBase.Helpers;
using System.IO;

namespace Platform.DependInject
{

    public static class DIHelper
    {

        /// <summary>
        /// ʹ��Assembly>.LoadFile������ȡ���򼯡�
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
    }
}

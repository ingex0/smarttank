using System;
using System.Collections.Generic;
using System.Text;
using TankEngine2D.Helpers;
using SmartTank.Rule;
using System.IO;
using System.Reflection;

namespace SmartTank.Helpers.DependInject
{
    static class RuleLoader
    {
        readonly static string ruleDirectory = "Rule";
        readonly static string ruleListPath = "Rule\\List.xml";

        static AssetList list;

        static public void Initial()
        {
            list = AssetList.Load( File.OpenRead( Path.Combine( Directories.GameBaseDirectory, ruleListPath ) ) );
        }

        static public string[] GetRulesList()
        {
            return list.GetTypeList();
        }

        static public IGameRule CreateRuleInstance( int index )
        {
            TypeAssetPath assetPath = list.GetTypeAssetPath( index );
            if (assetPath.typeName.Length > 0)
            {
                IGameRule gameRule = (IGameRule)GetInstance( assetPath.DLLName, assetPath.typeName );
                if (gameRule != null)
                    return gameRule;
                else
                    Log.Write( "Load GameRule error!" );
            }
            return null;
        }

        static public object GetInstance( string assetFullName, string typeName )
        {
            Assembly assembly = null;
            try
            {
                AppDomain.CurrentDomain.AppendPrivatePath( ruleDirectory );
                assembly = Assembly.Load( assetFullName );
                AppDomain.CurrentDomain.ClearPrivatePath();

            }
            catch (Exception)
            {
                Log.Write( "Load Assembly error : " + assetFullName + " load unsucceed!" );
                return null;
            }

            try
            {
                object obj = assembly.CreateInstance( typeName );
                if (obj != null)
                    return obj;
                else
                    Log.Write( "assembly.CreateInstance error!" );
            }
            catch (Exception ex)
            {
                Log.Write( "Load Type error : " + typeName + " create unsucceed!" + ex.Message );
                return null;
            }
            return null;
        }
    }
}

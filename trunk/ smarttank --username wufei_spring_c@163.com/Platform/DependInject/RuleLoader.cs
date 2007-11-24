using System;
using System.Collections.Generic;
using System.Text;
using GameBase.Helpers;
using Platform.GameRules;
using System.IO;
using System.Reflection;

namespace Platform.DependInject
{
    static class RuleLoader
    {
        readonly static string ruleDirectory = "Rules";
        readonly static string ruleListPath = "Rules\\List.xml";

        static AssetList list;

        static public void Initial ()
        {
            list = AssetList.Load( FileHelper.LoadGameContentFile( ruleListPath ) );
        }

        static public string[] GetRulesList ()
        {
            return list.GetTypeList();
        }

        static public IGameRule CreateRuleInstance ( int index )
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

        static public object GetInstance ( string assetFullName, string typeName )
        {
            Assembly assembly = null;
            try
            {
                AppDomain.CurrentDomain.AppendPrivatePath( "Rules" );
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
            catch (Exception)
            {
                Log.Write( "Load Type error : " + typeName + " create unsucceed!" );
                return null;
            }
            return null;
        }
    }
}

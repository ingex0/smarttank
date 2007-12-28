using System;
using System.Collections.Generic;
using System.Text;

namespace MapEditer
{
    class DisplayMgr
    {
        Dictionary<string, ObjDisplay> displays;
        List<ObjClassInfo> objClasses;

        public DisplayMgr ()
        {
        }

        public void Initialize ()
        {

        }

        public void SaveHistoryData ()
        {

        }

        public bool AddDisplay ( Type objClassType, string objDataPath )
        {
            // ��Ҫ���Զ�ָ�����ơ���objData�����Ƽ����ֱ�š�

            return true;
        }

        public bool DelDisplay ( string displayName )
        {
            return true;
        }
        public bool DelDisplay ( int index )
        {
            return true;
        }

        public ObjClassInfo[] GetTypeList ()
        {
            return objClasses.ToArray();
        }

        public int SearchObjClasses ( string DLLPath )
        {
            List<ObjClassInfo> result;
            result = ObjClassSearcher.Search( DLLPath );
            if (result.Count > 0)
                objClasses.AddRange( result );
            return result.Count;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.Shelter;
using SmartTank.Update;
using SmartTank.PhiCol;
using Microsoft.Xna.Framework;

namespace SmartTank.Senses.Memory
{
    public class ObjMemoryManager
    {
        #region Type Define
        struct Group
        {
            public IRaderOwner[] raderOwners;
            public ObjMemoryKeeper memory;

            public Group ( IRaderOwner[] raderOwners, ObjMemoryKeeper memory )
            {
                this.raderOwners = raderOwners;
                this.memory = memory;
            }
        }
        #endregion


        List<Group> groups = new List<Group>();

        public void AddGroup ( IRaderOwner[] raderOwners )
        {
            ObjMemoryKeeper memory = new ObjMemoryKeeper();
            groups.Add( new Group( raderOwners, memory ) );
            foreach (IRaderOwner raderOwner in raderOwners)
            {
                raderOwner.Rader.ObjMemoryKeeper = memory;
            }
        }

        public void AddSingle ( IRaderOwner raderOwner )
        {
            ObjMemoryKeeper memory = new ObjMemoryKeeper();
            groups.Add( new Group( new IRaderOwner[] { raderOwner }, memory ) );
            raderOwner.Rader.ObjMemoryKeeper = memory;
        }

        public void ClearGroups ()
        {
            groups.Clear();
        }

        public void Update ()
        {
            foreach (Group group in groups)
            {
                foreach (IRaderOwner raderOwner in group.raderOwners)
                {
                    EyeableBorderObjInfo[] curObjInfo = raderOwner.Rader.EyeableBorderObjInfos;

                    // �����Ƿ�����ʧ������
                    List<IHasBorderObj> disappearedObjs = new List<IHasBorderObj>();
                    List<EyeableBorderObjInfo> disappearedObjInfos = new List<EyeableBorderObjInfo>();


                    foreach (KeyValuePair<IHasBorderObj, EyeableBorderObjInfo> pair in group.memory.MemoryObjs)
                    {
                        if (pair.Value.IsDisappeared)
                            continue;

                        // �Ƿ�ص���ԭ�������������λ��
                        bool back = false;
                        foreach (Vector2 keyPoint in pair.Value.EyeableInfo.CurKeyPoints)
                        {
                            if (raderOwner.Rader.PointInRader( keyPoint ))
                            {
                                back = true;
                                break;
                            }
                        }

                        if (back)
                        {
                            // �����Ƿ񿴵��˸�����
                            bool find = false;
                            foreach (EyeableBorderObjInfo objInfo in curObjInfo)
                            {
                                if (objInfo.Obj == pair.Key)
                                {
                                    find = true;
                                    break;
                                }
                            }
                            if (!find)
                            {
                                disappearedObjs.Add( pair.Key );
                                disappearedObjInfos.Add( pair.Value );
                            }
                        }
                    }

                    group.memory.HandlerObjDisappear( disappearedObjs );

                    // �õ��߽��и��µ�����
                    List<EyeableBorderObjInfo> updatedObjInfo = new List<EyeableBorderObjInfo>();

                    // ����������Ϣ
                    foreach (EyeableBorderObjInfo info in curObjInfo)
                    {
                        if (group.memory.ApplyEyeableBorderObjInfo( info ))
                        {
                            // ������߽��и���ʱ��ӵ�����������
                            updatedObjInfo.Add( info );
                        }
                    }

                    // ����������ʧʱҲ�ж������и���
                    foreach (EyeableBorderObjInfo info in disappearedObjInfos)
                    {
                        updatedObjInfo.Add( info );
                    }

                    // ֪ͨ�������˸��µ���Ϣ
                    if (updatedObjInfo.Count != 0)
                    {
                        raderOwner.BorderObjUpdated( updatedObjInfo.ToArray() );
                    }
                }
            }
        }

    }
}

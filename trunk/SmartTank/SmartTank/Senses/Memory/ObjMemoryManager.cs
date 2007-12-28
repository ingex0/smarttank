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

                    // 查找是否有消失的物体
                    List<IHasBorderObj> disappearedObjs = new List<IHasBorderObj>();
                    List<EyeableBorderObjInfo> disappearedObjInfos = new List<EyeableBorderObjInfo>();


                    foreach (KeyValuePair<IHasBorderObj, EyeableBorderObjInfo> pair in group.memory.MemoryObjs)
                    {
                        if (pair.Value.IsDisappeared)
                            continue;

                        // 是否回到了原来看到该物体的位置
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
                            // 现在是否看到了该物体
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

                    // 得到边界有更新的物体
                    List<EyeableBorderObjInfo> updatedObjInfo = new List<EyeableBorderObjInfo>();

                    // 更新物体信息
                    foreach (EyeableBorderObjInfo info in curObjInfo)
                    {
                        if (group.memory.ApplyEyeableBorderObjInfo( info ))
                        {
                            // 当物体边界有更新时添加到更新物体中
                            updatedObjInfo.Add( info );
                        }
                    }

                    // 发现物体消失时也判定物体有更新
                    foreach (EyeableBorderObjInfo info in disappearedObjInfos)
                    {
                        updatedObjInfo.Add( info );
                    }

                    // 通知物体获得了更新的消息
                    if (updatedObjInfo.Count != 0)
                    {
                        raderOwner.BorderObjUpdated( updatedObjInfo.ToArray() );
                    }
                }
            }
        }

    }
}

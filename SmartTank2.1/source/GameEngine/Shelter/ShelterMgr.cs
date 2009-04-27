using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using Common.Helpers;
using GameEngine.Graphics;
using Common.DataStructure;

namespace GameEngine.Shelter
{
    /*
     * 遮挡关系管理类。
     * 
     * */
    public partial class ShelterMgr
    {
        #region Private Type Def
        struct RaderShelterGroup
        {
            public IEnumerable<IRaderOwner> raderOwners;
            public IEnumerable<IShelterObj>[] shelterGroups;

            public RaderShelterGroup( IEnumerable<IRaderOwner> raderOwners, IEnumerable<IShelterObj>[] shelterGroups )
            {
                this.shelterGroups = shelterGroups;
                this.raderOwners = raderOwners;
            }
        }
        #endregion

        #region Constants

        public const int gridSum = 150;

        public const int texSize = 300;
        public const int texSizeSmall = 40;

        #endregion

        #region Variables

        List<RaderShelterGroup> raderShelterGroups = new List<RaderShelterGroup>();

        RaderDrawer raderDrawer;

        #endregion

        #region Construction
        public ShelterMgr()
        {
            raderDrawer = new RaderDrawer();
        }
        #endregion

        #region Group Methods

        public void AddRaderShelterGroup( IEnumerable<IRaderOwner> raderOwners, IEnumerable<IShelterObj>[] shelterGroups )
        {
            raderShelterGroups.Add( new RaderShelterGroup( raderOwners, shelterGroups ) );
        }

        public void ClearGroups()
        {
            raderShelterGroups.Clear();
        }

        #endregion

        #region Update

        public void Update()
        {
            foreach (RaderShelterGroup group in raderShelterGroups)
            {
                foreach (IRaderOwner rader in group.raderOwners)
                {
                    rader.Rader.Update();
                    CalRaderMap( rader.Rader, group.shelterGroups );
                }
            }
        }

        /* 
         * 更新雷达中的可见区域贴图
         * 
         * 
         * 分为以下几个步骤：
         * 
         * <1>获得可能在雷达范围中的所有遮挡物
         * 
         * <2>将遮挡物的边界点坐标转换到雷达空间中，并在已经栅格化的雷达空间中填充边界点的深度信息，获得深度图。
         * 
         * <3>用显卡计算出可见区域贴图
         * 
         * */
        private void CalRaderMap( Rader rader, IEnumerable<IShelterObj>[] shelterObjGroup )
        {
            // 获得可能在雷达范围中的所有遮挡物。
            List<IShelterObj> sheltersInRader = new List<IShelterObj>( 16 );

            foreach (IEnumerable<IShelterObj> group in shelterObjGroup)
            {
                foreach (IShelterObj shelter in group)
                {
                    if (shelter.BoundingBox.Intersects( rader.BoundBox ))
                        sheltersInRader.Add( shelter );
                }
            }

            // 将遮挡物的边界点坐标转换到雷达空间中，并在已经栅格化的雷达空间中填充边界点的深度信息，获得深度图。
            rader.depthMap.ClearMap( 1 );

            foreach (IShelterObj shelter in sheltersInRader)
            {
                rader.depthMap.ApplyShelterObj( rader, shelter );
            }

            // 用显卡计算出可见区域贴图
            raderDrawer.DrawRader( rader );

            // 更新各项数据
            rader.UpdateTexture();
        }



        #endregion


    }




}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using TankEngine2D.Helpers;
using TankEngine2D.Graphics;
using TankEngine2D.DataStructure;

namespace SmartTank.Shelter
{
    /*
     * �ڵ���ϵ�����ࡣ
     * 
     * */
    public partial class ShelterManager
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

        //readonly float gridLength = Coordin.TexelSize.X;

        #endregion

        #region Variables

        List<RaderShelterGroup> raderShelterGroups = new List<RaderShelterGroup>();

        RaderDrawer raderDrawer;

        #endregion

        #region Construction
        public ShelterManager()
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
         * �����״��еĿɼ�������ͼ
         * 
         * 
         * ��Ϊ���¼������裺
         * 
         * <1>��ÿ������״ﷶΧ�е������ڵ���
         * 
         * <2>���ڵ���ı߽������ת�����״�ռ��У������Ѿ�դ�񻯵��״�ռ������߽��������Ϣ��������ͼ��
         * 
         * <3>���Կ�������ɼ�������ͼ
         * 
         * */
        private void CalRaderMap( Rader rader, IEnumerable<IShelterObj>[] shelterObjGroup )
        {
            // ��ÿ������״ﷶΧ�е������ڵ��
            List<IShelterObj> sheltersInRader = new List<IShelterObj>( 16 );

            foreach (IEnumerable<IShelterObj> group in shelterObjGroup)
            {
                foreach (IShelterObj shelter in group)
                {
                    if (shelter.BoundingBox.Intersects( rader.BoundBox ))
                        sheltersInRader.Add( shelter );
                }
            }

            // ���ڵ���ı߽������ת�����״�ռ��У������Ѿ�դ�񻯵��״�ռ������߽��������Ϣ��������ͼ��
            RaderDepthMap depthMap = new RaderDepthMap( BaseGame.CoordinMgr.TexelSize.X, rader.Ang, rader.R );

            foreach (IShelterObj shelter in sheltersInRader)
            {
                depthMap.ApplyShelterObj( rader, shelter );
            }

            // ���Կ�������ɼ�������ͼ
            rader.SetTexture( raderDrawer.DrawRader( rader, depthMap ) );

            // ���ڵ������Ϣ���õ�Rader��     
            depthMap.CalSheltersVisiBorder();

            rader.ShelterObjs = depthMap.GetShelters();
            rader.ShelterVisiBorders = depthMap.GetSheltersVisiBorder();
            rader.DepthMap = depthMap.DepthMap;
        }



        #endregion


    }




}

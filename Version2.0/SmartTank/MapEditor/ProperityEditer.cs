using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SmartTank.GameObjs;

namespace MapEditor
{
    public partial class ProperityEditer : DataGridView
    {
        #region TypeDefine

        class Element
        {
            public string name;
            public object value;
            public Type valueType;

            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            public object Value
            {
                get { return value; }
                set { this.value = value; }
            }

            public Element ( string name, object value )
            {
                this.name = name;
                this.value = value;
                this.valueType = value.GetType();
            }
        }

        class ProperityList
        {
            IGameObj obj;
            List<Element> list;

            public List<Element> List
            {
                get { return list; }
            }

            public ProperityList ( IGameObj obj )
            {
                this.obj = obj;
                list = new List<Element>();

                UpdateList();
            }

            public void UpdateList ()
            {
                Type type = obj.GetType();

                list = new List<Element>();
                foreach (PropertyInfo info in type.GetProperties())
                {
                    if (info.GetCustomAttributes( typeof( EditableProperty ), false ).Length != 0)
                    {
                        list.Add( new Element( info.Name, info.GetValue( obj, null ) ) );
                    }
                }
            }

            public void UpdateData ()
            {
                foreach (Element ele in list)
                {
                    PropertyInfo info = obj.GetType().GetProperty( ele.name );
                    info.SetValue( obj, ConvertData( ele.valueType, ele.value ), null );
                }
            }

            private object ConvertData ( Type type, object value )
            {
                try
                {
                    if (value is string)
                    {
                        string sValue = (string)value;
                        if (type.Name == "Vector2")
                        {
                            string[] values;
                            values = sValue.Split( ':', 'Y', ':', '}' );
                            Vector2 result = new Vector2( float.Parse( values[1] ), float.Parse( values[3] ) );
                            return result;
                        }
                        else if (type.Name == "Color")
                        {
                            string[] values;
                            values = sValue.Split( ':', 'G', ':', 'B', ':', 'A', ':', '}' );
                            Color result = new Color( byte.Parse( values[1] ), byte.Parse( values[3] ), byte.Parse( values[5] ), byte.Parse( values[7] ) );
                            return result;
                        }
                        else if (type.Name == "Single")
                        {
                            return float.Parse( sValue );
                        }
                    }
                    else
                        return Convert.ChangeType( value, type );

                    throw new Exception();
                }
                catch (Exception)
                {
                    // 在此添加异常处理
                    throw;
                }
            }

            public Type GetTypeAt ( int index )
            {
                return list[index].valueType;
            }
        }

        #endregion

        IGameObj obj;
        ProperityList properityList;

        public ProperityEditer ()
        {
            InitializeComponent();
            InitializeDataGridView();
        }

        private void InitializeDataGridView ()
        {
            // 在此设定外观
        }

        public void SetObj ( IGameObj obj )
        {
            this.obj = obj;
            this.properityList = new ProperityList( obj );

            this.DataSource = properityList.List;
        }

        public void ClearObj ()
        {
            this.obj = null;
            this.properityList = null;
        }

        protected override void OnCellBeginEdit ( DataGridViewCellCancelEventArgs e )
        {
            base.OnCellBeginEdit( e );

            if (properityList != null)
            {
                Type type = properityList.GetTypeAt( e.RowIndex );

                // 在此添加对不同类型变量的格外处理过程
            }
        }

        protected override void OnCellValueChanged ( DataGridViewCellEventArgs e )
        {
            base.OnCellValueChanged( e );

            if (properityList != null)
            {
                properityList.UpdateData();
                properityList.UpdateList();
            }
        }
    }
}

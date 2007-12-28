using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTank.GameObjects
{
    [AttributeUsage( AttributeTargets.Property, Inherited = false )]
    public class EditableProperty : Attribute
    {
        public EditableProperty ()
        {

        }
    }
}

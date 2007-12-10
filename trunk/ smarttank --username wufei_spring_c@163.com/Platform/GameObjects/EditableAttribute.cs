using System;
using System.Collections.Generic;
using System.Text;

namespace Platform.GameObjects
{
    [AttributeUsage( AttributeTargets.Property, Inherited = false )]
    public class EditableProperty : Attribute
    {
        public EditableProperty ()
        {

        }
    }
}

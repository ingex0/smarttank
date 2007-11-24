using System;
using System.Collections.Generic;
using System.Text;

namespace Platform.GameObjects.Tank.TankAIs
{

    [AttributeUsage( AttributeTargets.Class, Inherited = false )]
    public class AIAttribute : Attribute
    {
        public string Name;
        public string Programer;
        public string Script;
        public DateTime Time;

        public AIAttribute (string name, string programer, string script, int year, int mouth, int day )
        {
            this.Name = name;
            this.Programer = programer;
            this.Script = script;
            this.Time = new DateTime( year, mouth, day );
        }

    }
}

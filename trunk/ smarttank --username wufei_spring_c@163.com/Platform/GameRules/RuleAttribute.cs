using System;
using System.Collections.Generic;
using System.Text;

namespace Platform.GameRules
{
    [AttributeUsage( AttributeTargets.Class, Inherited = false )]
    public class RuleAttribute : Attribute
    {
        public string Name;
        public string RuleInfo;
        public string Programer;
        public DateTime time;


        public RuleAttribute ( string name, string ruleInfo, string programer, int year, int mouth, int day )
        {
            this.Name = name;
            this.RuleInfo = ruleInfo;
            this.Programer = programer;
            time = new DateTime( year, mouth, day );
        }


    }
}

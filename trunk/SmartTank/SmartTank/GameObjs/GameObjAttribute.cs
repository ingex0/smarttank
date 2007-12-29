using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTank.GameObjs
{
    [AttributeUsage( AttributeTargets.Class, Inherited = false )]
    class GameObjAttribute : Attribute
    {
        string className;
        string classRemark;
        string programer;
        int year;
        int month;
        int day;

        public string ClassName
        {
            get { return className; }
        }
        public string ClassRemark
        {
            get { return classRemark; }
        }
        public string Programer
        {
            get { return programer; }
        }
        public int Year
        {
            get { return year; }
        }
        public int Month
        {
            get { return month; }
        }
        public int Day
        {
            get { return day; }
        }

        public GameObjAttribute ( string className, string classRemark, string programer, int year, int month, int day )
        {
            this.className = className;
            this.classRemark = classRemark;
            this.programer = programer;
            this.year = year;
            this.month = month;
            this.day = day;
        }
    }
}

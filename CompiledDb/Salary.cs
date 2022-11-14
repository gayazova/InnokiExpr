using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledDb
{
    public class Salary
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public Month Month { get; set; }

        public int Year { get; set; }

        public Department Department { get; set; }

        public long Amount { get; set; }

        public SkillLevel SkillLevel { get; set; }

        public DateTime? ActualFrom { get; set; }

        public DateTime? ActualTo { get; set; }
    }

    public enum SkillLevel
    {
        Junior,
        Middle,
        Senior,
        Architect
    }

    public enum Department
    {
        NET,
        Java,
        Vue,
        QA,
        DevOps
    }

    public enum Month
    {
        January = 1,

        February = 2,

        March = 3,

        April = 4,

        May = 5,

        June = 6,

        July = 7,


        August = 8,

        September = 9,

        October = 10,

        November = 11,

        December = 12
    }
}

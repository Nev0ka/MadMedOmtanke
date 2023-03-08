using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Models
{
    public class Employee
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string TlfNr { get; set; }
        public string FirmEmail { get; set; }
        public string Position { get; set; }
        public int DepartmentID { get; set; }
        public string ClosetManager { get; set; }
        public string Skills { get; set; }
    }
}

namespace Employee.Models
{
    public class Employee
    {
        public int ID { get; set; }
        public string Initials { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string TlfNr { get; set; }
        public string FirmEmail { get; set; }
        public int PositionID { get; set; }
        public int DepartmentID { get; set; }
        public string ClosetManager { get; set; }
        public string Skills { get; set; }
    }
}

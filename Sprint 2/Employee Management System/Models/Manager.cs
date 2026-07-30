using System.Collections.Generic;

namespace Employee_Management_System.Models
{
    public class Manager : Employee
    {
        public List<Employee> TeamMembers { get; set; } = new List<Employee>();
    }
}

using System;
using System.Collections.Generic;

namespace Employee_Management_System.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public int DepartmentId { get; set; }
        public decimal Salary { get; set; }
        public List<string> Skills { get; set; } = new List<string>();
    }
}

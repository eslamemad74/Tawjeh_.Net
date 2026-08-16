using System;
using Employee_Management_System.Models;

namespace Employee_Management_System.Events
{
    public class EmployeeEventArgs : EventArgs
    {
        public Employee Employee { get; }

        public EmployeeEventArgs(Employee employee)
        {
            Employee = employee ?? throw new ArgumentNullException(nameof(employee));
        }
    }
}

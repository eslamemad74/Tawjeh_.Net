using System;
using System.Collections.Generic;

namespace Employee_Management_System.Models
{
    public class Manager : Employee
    {
        private readonly List<Employee> _teamMembers = new List<Employee>();

        public IReadOnlyList<Employee> TeamMembers => _teamMembers;

        public Manager(int id, string name, DateTime hireDate, int departmentId, decimal salary)
            : base(id, name, hireDate, departmentId, salary)
        {
        }

        public void AddTeamMember(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));
            if (!_teamMembers.Contains(employee))
            {
                _teamMembers.Add(employee);
            }
        }
    }
}

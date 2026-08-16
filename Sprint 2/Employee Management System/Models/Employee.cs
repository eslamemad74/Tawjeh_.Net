using System;
using System.Collections.Generic;

namespace Employee_Management_System.Models
{
    public class Employee
    {
        private readonly List<string> _skills = new List<string>();

        public int Id { get; private set; }
        public string Name { get; private set; }
        public DateTime HireDate { get; private set; }
        public int DepartmentId { get; private set; }
        public decimal Salary { get; private set; }
        public IReadOnlyList<string> Skills => _skills;

        public Employee(int id, string name, DateTime hireDate, int departmentId, decimal salary)
        {
            if (id <= 0)
                throw new ArgumentException("Employee ID must be a positive integer.", nameof(id));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Employee name cannot be empty or whitespace.", nameof(name));
            if (salary < 0)
                throw new ArgumentException("Salary cannot be negative.", nameof(salary));

            Id = id;
            Name = name;
            HireDate = hireDate;
            DepartmentId = departmentId;
            Salary = salary;
        }

        public void AddSkill(string skill)
        {
            if (string.IsNullOrWhiteSpace(skill))
                throw new ArgumentException("Skill cannot be empty or whitespace.", nameof(skill));
            if (!_skills.Contains(skill))
            {
                _skills.Add(skill);
            }
        }
    }
}

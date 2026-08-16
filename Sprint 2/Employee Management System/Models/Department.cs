using System;

namespace Employee_Management_System.Models
{
    public class Department
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;

        public Department(int id, string name)
        {
            if (id <= 0)
                throw new ArgumentException("Department ID must be a positive integer.", nameof(id));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Department name cannot be empty or whitespace.", nameof(name));

            Id = id;
            Name = name;
        }
    }
}

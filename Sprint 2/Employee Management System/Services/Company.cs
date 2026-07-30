using System;
using System.Collections.Generic;
using Employee_Management_System.Models;

namespace Employee_Management_System.Services
{
    public class Company
    {
        public List<Employee> Employees { get; set; } = new List<Employee>();
        public Dictionary<int, Department> Departments { get; set; } = new Dictionary<int, Department>();
        public Queue<Employee> OnboardingQueue { get; set; } = new Queue<Employee>();
        public Stack<string> ActionHistory { get; set; } = new Stack<string>();
        public HashSet<string> UniqueSkills { get; set; } = new HashSet<string>();

        public void AddToOnboarding(Employee employee)
        {
            OnboardingQueue.Enqueue(employee);
            ActionHistory.Push($"Added employee {employee.Name} (ID: {employee.Id}) to onboarding queue.");
        }

        public void ProcessOnboarding()
        {
            if (OnboardingQueue.Count == 0)
            {
                Console.WriteLine("No employees in onboarding queue.");
                return;
            }
            Employee emp = OnboardingQueue.Dequeue();
            Employees.Add(emp);
            ActionHistory.Push($"Processed onboarding for {emp.Name} (ID: {emp.Id}) and added to active employees.");
        }

        public void AddDepartment(Department department)
        {
            if (Departments.ContainsKey(department.Id))
            {
                Console.WriteLine("Department ID already exists.");
                return;
            }
            Departments.Add(department.Id, department);
            ActionHistory.Push($"Added department {department.Name} (ID: {department.Id}).");
        }

        public void RegisterSkill(int employeeId, string skill)
        {
            Employee? target = null;
            foreach (Employee emp in Employees)
            {
                if (emp.Id == employeeId)
                {
                    target = emp;
                    break;
                }
            }

            if (target != null)
            {
                if (!target.Skills.Contains(skill))
                {
                    target.Skills.Add(skill);
                }
                UniqueSkills.Add(skill);
                ActionHistory.Push($"Registered skill '{skill}' for employee {target.Name} (ID: {target.Id}).");
            }
            else
            {
                Console.WriteLine("Employee not found in active employees.");
            }
        }

        public Employee? FindEmployeeById(int id)
        {
            foreach (Employee emp in Employees)
            {
                if (emp.Id == id)
                {
                    return emp;
                }
            }
            return null;
        }

        public List<Employee> FindEmployeesByName(string name)
        {
            List<Employee> matches = new List<Employee>();
            foreach (Employee emp in Employees)
            {
                if (emp.Name != null && emp.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                {
                    matches.Add(emp);
                }
            }
            return matches;
        }

        public void DisplayEmployeesByDepartment(int departmentId)
        {
            if (Departments.TryGetValue(departmentId, out Department? dept))
            {
                // In C# net8.0, TryGetValue guarantees dept is not null when returning true.
                Console.WriteLine($"Employees in Department: {dept!.Name} (ID: {dept.Id})");
                bool found = false;
                foreach (Employee emp in Employees)
                {
                    if (emp.DepartmentId == departmentId)
                    {
                        Console.WriteLine($"- {emp.Name} (ID: {emp.Id}, Salary: {emp.Salary:C})");
                        found = true;
                    }
                }
                if (!found)
                {
                    Console.WriteLine("No employees found in this department.");
                }
            }
            else
            {
                Console.WriteLine("Department not found.");
            }
        }

        public decimal CalculateAverageSalary()
        {
            if (Employees.Count == 0)
            {
                return 0;
            }
            decimal sum = 0;
            foreach (Employee emp in Employees)
            {
                sum += emp.Salary;
            }
            return sum / Employees.Count;
        }

        public void PrintDepartmentReport()
        {
            foreach (var kvp in Departments)
            {
                Department dept = kvp.Value;
                int count = 0;
                foreach (Employee emp in Employees)
                {
                    if (emp.DepartmentId == dept.Id)
                    {
                        count++;
                    }
                }
                Console.WriteLine($"Department: {dept.Name} (ID: {dept.Id}) - Active Employees Count: {count}");
            }
        }

        public void PrintActionHistory()
        {
            if (ActionHistory.Count == 0)
            {
                Console.WriteLine("No action history available.");
                return;
            }
            foreach (string action in ActionHistory)
            {
                Console.WriteLine($"- {action}");
            }
        }

        public void PrintAllUniqueSkills()
        {
            if (UniqueSkills.Count == 0)
            {
                Console.WriteLine("No skills registered yet.");
                return;
            }
            foreach (string skill in UniqueSkills)
            {
                Console.WriteLine($"- {skill}");
            }
        }

        public void SeedData()
        {
            Department dept1 = new Department { Id = 1, Name = "IT" };
            Department dept2 = new Department { Id = 2, Name = "HR" };
            Department dept3 = new Department { Id = 3, Name = "Finance" };

            Departments.Add(dept1.Id, dept1);
            Departments.Add(dept2.Id, dept2);
            Departments.Add(dept3.Id, dept3);

            Employee emp1 = new Employee { Id = 101, Name = "Alice Johnson", HireDate = new DateTime(2024, 1, 15), DepartmentId = 1, Salary = 6000m };
            Employee emp2 = new Employee { Id = 102, Name = "Bob Smith", HireDate = new DateTime(2023, 6, 20), DepartmentId = 1, Salary = 8000m };
            Employee emp3 = new Employee { Id = 103, Name = "Charlie Brown", HireDate = new DateTime(2025, 2, 10), DepartmentId = 2, Salary = 5000m };

            Employees.Add(emp1);
            Employees.Add(emp2);
            Employees.Add(emp3);

            Employee onboarding1 = new Employee { Id = 201, Name = "David Miller", HireDate = new DateTime(2026, 8, 1), DepartmentId = 1, Salary = 5500m };
            Employee onboarding2 = new Employee { Id = 202, Name = "Eva Green", HireDate = new DateTime(2026, 8, 15), DepartmentId = 3, Salary = 6200m };

            OnboardingQueue.Enqueue(onboarding1);
            OnboardingQueue.Enqueue(onboarding2);

            UniqueSkills.Add("C#");
            UniqueSkills.Add("SQL");
            UniqueSkills.Add("Recruiting");

            emp1.Skills.Add("C#");
            emp1.Skills.Add("SQL");
            emp2.Skills.Add("C#");
            emp3.Skills.Add("Recruiting");

            ActionHistory.Push("System Initialized");
            ActionHistory.Push("Seeded initial departments: IT, HR, Finance.");
            ActionHistory.Push("Seeded initial active employees: Alice, Bob, Charlie.");
            ActionHistory.Push("Seeded onboarding queue: David, Eva.");
            ActionHistory.Push("Seeded skills: C#, SQL, Recruiting.");
        }
    }
}

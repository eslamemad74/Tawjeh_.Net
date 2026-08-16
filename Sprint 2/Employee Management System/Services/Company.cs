using System;
using System.Collections.Generic;
using Employee_Management_System.Models;
using Employee_Management_System.Common;
using Employee_Management_System.Delegates;
using Employee_Management_System.Events;

namespace Employee_Management_System.Services
{
    public class Company
    {
        private readonly List<Employee> _employees = new List<Employee>();
        private readonly Dictionary<int, Department> _departments = new Dictionary<int, Department>();
        private readonly Queue<Employee> _onboardingQueue = new Queue<Employee>();
        private readonly Stack<string> _actionHistory = new Stack<string>();
        private readonly HashSet<string> _uniqueSkills = new HashSet<string>();

        // Lifecycle Events
        public event EventHandler<EmployeeEventArgs>? EmployeeOnboarded;
        public event EventHandler<EmployeeEventArgs>? EmployeePromoted;

        public Result<Employee> AddToOnboarding(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            // Validate duplicate employee ID in active list
            foreach (var emp in _employees)
            {
                if (emp.Id == employee.Id)
                {
                    return Result<Employee>.Failure($"Employee ID {employee.Id} is already in use by an active employee.");
                }
            }

            // Validate duplicate employee ID in onboarding queue
            foreach (var emp in _onboardingQueue)
            {
                if (emp.Id == employee.Id)
                {
                    return Result<Employee>.Failure($"Employee ID {employee.Id} is already in the onboarding queue.");
                }
            }

            // Validate department existence
            if (!_departments.ContainsKey(employee.DepartmentId))
            {
                return Result<Employee>.Failure($"Department ID {employee.DepartmentId} does not exist. Cannot onboard employee.");
            }

            _onboardingQueue.Enqueue(employee);
            _actionHistory.Push($"Added employee {employee.Name} (ID: {employee.Id}) to onboarding queue.");
            return Result<Employee>.Success(employee, $"Successfully added {employee.Name} to onboarding queue.");
        }

        public Result<Employee> ProcessOnboarding()
        {
            if (_onboardingQueue.Count == 0)
            {
                return Result<Employee>.Failure("No employees in onboarding queue.");
            }
            Employee emp = _onboardingQueue.Dequeue();
            _employees.Add(emp);
            _actionHistory.Push($"Processed onboarding for {emp.Name} (ID: {emp.Id}) and added to active employees.");
            
            // Raise Event
            EmployeeOnboarded?.Invoke(this, new EmployeeEventArgs(emp));
            
            return Result<Employee>.Success(emp, $"Successfully processed onboarding for {emp.Name} (ID: {emp.Id}).");
        }

        public Result<Department> AddDepartment(Department department)
        {
            if (department == null)
                throw new ArgumentNullException(nameof(department));

            if (_departments.ContainsKey(department.Id))
            {
                return Result<Department>.Failure($"Department ID {department.Id} already exists.");
            }

            // Validate duplicate department names (case-insensitive)
            foreach (var existingDept in _departments.Values)
            {
                if (existingDept.Name.Equals(department.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return Result<Department>.Failure($"Department name '{department.Name}' already exists.");
                }
            }

            _departments.Add(department.Id, department);
            _actionHistory.Push($"Added department {department.Name} (ID: {department.Id}).");
            return Result<Department>.Success(department, $"Successfully added department {department.Name}.");
        }

        public Result<Manager> PromoteToManager(int employeeId)
        {
            Employee? target = FindEmployeeById(employeeId);
            if (target == null)
            {
                return Result<Manager>.Failure($"Employee with ID {employeeId} not found in active employees.");
            }

            if (target is Manager)
            {
                return Result<Manager>.Failure($"Employee {target.Name} (ID: {employeeId}) is already a Manager.");
            }

            // Create Manager instance
            Manager manager = new Manager(target.Id, target.Name, target.HireDate, target.DepartmentId, target.Salary);
            
            // Copy skills
            foreach (string skill in target.Skills)
            {
                manager.AddSkill(skill);
            }

            // Replace in _employees
            int index = _employees.IndexOf(target);
            _employees[index] = manager;

            _actionHistory.Push($"Promoted {target.Name} (ID: {target.Id}) to Manager.");

            // Raise Event
            EmployeePromoted?.Invoke(this, new EmployeeEventArgs(manager));

            return Result<Manager>.Success(manager, $"Successfully promoted {target.Name} (ID: {target.Id}) to Manager.");
        }

        public List<Employee> FilterEmployees(EmployeeFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            List<Employee> filteredList = new List<Employee>();
            foreach (Employee emp in _employees)
            {
                if (filter(emp))
                {
                    filteredList.Add(emp);
                }
            }
            return filteredList;
        }

        public void RegisterSkill(int employeeId, string skill)
        {
            if (string.IsNullOrWhiteSpace(skill))
                throw new ArgumentException("Skill cannot be empty or whitespace.", nameof(skill));

            Employee? target = null;
            foreach (Employee emp in _employees)
            {
                if (emp.Id == employeeId)
                {
                    target = emp;
                    break;
                }
            }

            if (target != null)
            {
                target.AddSkill(skill);
                _uniqueSkills.Add(skill);
                _actionHistory.Push($"Registered skill '{skill}' for employee {target.Name} (ID: {target.Id}).");
            }
            else
            {
                throw new KeyNotFoundException($"Employee with ID {employeeId} not found in active employees.");
            }
        }

        public Employee? FindEmployeeById(int id)
        {
            foreach (Employee emp in _employees)
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
            foreach (Employee emp in _employees)
            {
                if (emp.Name != null && emp.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                {
                    matches.Add(emp);
                }
            }
            return matches;
        }

        public Department GetDepartmentById(int departmentId)
        {
            if (_departments.TryGetValue(departmentId, out Department? dept))
            {
                return dept;
            }
            throw new KeyNotFoundException($"Department ID {departmentId} not found.");
        }

        public List<Employee> GetEmployeesByDepartment(int departmentId)
        {
            if (!_departments.ContainsKey(departmentId))
            {
                throw new KeyNotFoundException($"Department ID {departmentId} not found.");
            }

            List<Employee> list = new List<Employee>();
            foreach (Employee emp in _employees)
            {
                if (emp.DepartmentId == departmentId)
                {
                    list.Add(emp);
                }
            }
            return list;
        }

        public decimal CalculateAverageSalary()
        {
            if (_employees.Count == 0)
            {
                return 0;
            }
            decimal sum = 0;
            foreach (Employee emp in _employees)
            {
                sum += emp.Salary;
            }
            return sum / _employees.Count;
        }

        public List<(Department Department, int EmployeeCount)> GetDepartmentReport()
        {
            var report = new List<(Department Department, int EmployeeCount)>();
            foreach (var kvp in _departments)
            {
                Department dept = kvp.Value;
                int count = 0;
                foreach (Employee emp in _employees)
                {
                    if (emp.DepartmentId == dept.Id)
                    {
                        count++;
                    }
                }
                report.Add((dept, count));
            }
            return report;
        }

        public IReadOnlyCollection<string> GetActionHistory()
        {
            return _actionHistory;
        }

        public IReadOnlyCollection<string> GetUniqueSkills()
        {
            return _uniqueSkills;
        }

        public void SeedData()
        {
            Department dept1 = new Department(1, "IT");
            Department dept2 = new Department(2, "HR");
            Department dept3 = new Department(3, "Finance");

            _departments.Add(dept1.Id, dept1);
            _departments.Add(dept2.Id, dept2);
            _departments.Add(dept3.Id, dept3);

            Employee emp1 = new Employee(101, "Alice Johnson", new DateTime(2024, 1, 15), 1, 6000m);
            Employee emp2 = new Employee(102, "Bob Smith", new DateTime(2023, 6, 20), 1, 8000m);
            Employee emp3 = new Employee(103, "Charlie Brown", new DateTime(2025, 2, 10), 2, 5000m);

            _employees.Add(emp1);
            _employees.Add(emp2);
            _employees.Add(emp3);

            Employee onboarding1 = new Employee(201, "David Miller", new DateTime(2026, 8, 1), 1, 5500m);
            Employee onboarding2 = new Employee(202, "Eva Green", new DateTime(2026, 8, 15), 3, 6200m);

            _onboardingQueue.Enqueue(onboarding1);
            _onboardingQueue.Enqueue(onboarding2);

            _uniqueSkills.Add("C#");
            _uniqueSkills.Add("SQL");
            _uniqueSkills.Add("Recruiting");

            emp1.AddSkill("C#");
            emp1.AddSkill("SQL");
            emp2.AddSkill("C#");
            emp3.AddSkill("Recruiting");

            _actionHistory.Push("System Initialized");
            _actionHistory.Push("Seeded initial departments: IT, HR, Finance.");
            _actionHistory.Push("Seeded initial active employees: Alice, Bob, Charlie.");
            _actionHistory.Push("Seeded onboarding queue: David, Eva.");
            _actionHistory.Push("Seeded skills: C#, SQL, Recruiting.");
        }
    }
}

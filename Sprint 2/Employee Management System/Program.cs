using System;
using System.Collections.Generic;
using Employee_Management_System.Models;
using Employee_Management_System.Services;
using Employee_Management_System.Common;
using Employee_Management_System.Delegates;
using Employee_Management_System.Events;

namespace Employee_Management_System
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Company company = new Company();

            // Subscribe to Lifecycle Events
            company.EmployeeOnboarded += (sender, e) =>
            {
                Console.WriteLine($"\n[EVENT NOTIFICATION] Employee Onboarded successfully!");
                Console.WriteLine($"Name: {e.Employee.Name}, ID: {e.Employee.Id}, Department ID: {e.Employee.DepartmentId}");
            };

            company.EmployeePromoted += (sender, e) =>
            {
                Console.WriteLine($"\n[EVENT NOTIFICATION] Employee Promoted to Manager successfully!");
                Console.WriteLine($"Name: {e.Employee.Name}, ID: {e.Employee.Id}, New Salary: {e.Employee.Salary:C}");
            };

            company.SeedData();

            bool exit = false;
            do
            {
                Console.WriteLine("\n=== Employee Management System ===");
                Console.WriteLine("1. Add Employee to Onboarding Queue");
                Console.WriteLine("2. Process Onboarding (FIFO)");
                Console.WriteLine("3. Add Department");
                Console.WriteLine("4. Promote Employee to Manager");
                Console.WriteLine("5. Register Skill for Employee");
                Console.WriteLine("6. Search Employee by ID or Name");
                Console.WriteLine("7. Filter Employees (using Delegates & Lambdas)");
                Console.WriteLine("8. Display Employees of a Specific Department");
                Console.WriteLine("9. Calculate Average Salary");
                Console.WriteLine("10. Display Department Employee Counts");
                Console.WriteLine("11. Display Action History");
                Console.WriteLine("12. Display All Unique Skills");
                Console.WriteLine("13. Exit");
                Console.Write("Enter your choice (1-13): ");

                string choice = Console.ReadLine() ?? "";
                switch (choice)
                {
                    case "1":
                        AddEmployeeToOnboarding(company);
                        break;
                    case "2":
                        var processResult = company.ProcessOnboarding();
                        if (processResult.IsSuccess)
                        {
                            Console.WriteLine(processResult.Message);
                        }
                        else
                        {
                            Console.WriteLine($"Error processing onboarding: {processResult.Message}");
                        }
                        break;
                    case "3":
                        AddDepartment(company);
                        break;
                    case "4":
                        PromoteEmployeeToManager(company);
                        break;
                    case "5":
                        RegisterSkillForEmployee(company);
                        break;
                    case "6":
                        SearchEmployee(company);
                        break;
                    case "7":
                        FilterEmployees(company);
                        break;
                    case "8":
                        DisplayEmployeesByDepartment(company);
                        break;
                    case "9":
                        decimal avgSalary = company.CalculateAverageSalary();
                        Console.WriteLine($"Average Salary: {avgSalary:C}");
                        break;
                    case "10":
                        var report = company.GetDepartmentReport();
                        if (report.Count == 0)
                        {
                            Console.WriteLine("No departments found.");
                        }
                        else
                        {
                            foreach (var item in report)
                            {
                                Console.WriteLine($"Department: {item.Department.Name} (ID: {item.Department.Id}) - Active Employees Count: {item.EmployeeCount}");
                            }
                        }
                        break;
                    case "11":
                        var history = company.GetActionHistory();
                        if (history.Count == 0)
                        {
                            Console.WriteLine("No action history available.");
                        }
                        else
                        {
                            foreach (string action in history)
                            {
                                Console.WriteLine($"- {action}");
                            }
                        }
                        break;
                    case "12":
                        var skills = company.GetUniqueSkills();
                        if (skills.Count == 0)
                        {
                            Console.WriteLine("No skills registered yet.");
                        }
                        else
                        {
                            foreach (string skill in skills)
                            {
                                Console.WriteLine($"- {skill}");
                            }
                        }
                        break;
                    case "13":
                        exit = true;
                        Console.WriteLine("Exiting program. Goodbye!");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please enter a number between 1 and 13.");
                        break;
                }
            } while (!exit);
        }

        static void AddEmployeeToOnboarding(Company company)
        {
            Console.Write("Enter Employee ID (integer): ");
            if (!int.TryParse(Console.ReadLine() ?? "", out int id))
            {
                Console.WriteLine("Invalid input. ID must be an integer.");
                return;
            }

            Console.Write("Enter Employee Name: ");
            string name = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Invalid input. Name cannot be empty.");
                return;
            }

            Console.Write("Enter Hire Date (yyyy-MM-dd): ");
            if (!DateTime.TryParse(Console.ReadLine() ?? "", out DateTime hireDate))
            {
                Console.WriteLine("Invalid input. Hire Date is not a valid date.");
                return;
            }

            Console.Write("Enter Department ID (integer): ");
            if (!int.TryParse(Console.ReadLine() ?? "", out int departmentId))
            {
                Console.WriteLine("Invalid input. Department ID must be an integer.");
                return;
            }

            Console.Write("Enter Salary: ");
            if (!decimal.TryParse(Console.ReadLine() ?? "", out decimal salary))
            {
                Console.WriteLine("Invalid input. Salary must be a decimal number.");
                return;
            }

            try
            {
                Employee employee = new Employee(id, name, hireDate, departmentId, salary);
                var result = company.AddToOnboarding(employee);
                if (result.IsSuccess)
                {
                    Console.WriteLine(result.Message);
                }
                else
                {
                    Console.WriteLine($"Failed to add employee: {result.Message}");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Validation Error: {ex.Message}");
            }
        }

        static void AddDepartment(Company company)
        {
            Console.Write("Enter Department ID (integer): ");
            if (!int.TryParse(Console.ReadLine() ?? "", out int id))
            {
                Console.WriteLine("Invalid input. Department ID must be an integer.");
                return;
            }

            Console.Write("Enter Department Name: ");
            string name = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Invalid input. Name cannot be empty.");
                return;
            }

            try
            {
                Department department = new Department(id, name);
                var result = company.AddDepartment(department);
                if (result.IsSuccess)
                {
                    Console.WriteLine(result.Message);
                }
                else
                {
                    Console.WriteLine($"Failed to add department: {result.Message}");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Validation Error: {ex.Message}");
            }
        }

        static void PromoteEmployeeToManager(Company company)
        {
            Console.Write("Enter Employee ID to promote (integer): ");
            if (!int.TryParse(Console.ReadLine() ?? "", out int id))
            {
                Console.WriteLine("Invalid input. ID must be an integer.");
                return;
            }

            var result = company.PromoteToManager(id);
            if (result.IsSuccess)
            {
                Console.WriteLine(result.Message);
            }
            else
            {
                Console.WriteLine($"Promotion failed: {result.Message}");
            }
        }

        static void RegisterSkillForEmployee(Company company)
        {
            Console.Write("Enter Employee ID (integer): ");
            if (!int.TryParse(Console.ReadLine() ?? "", out int id))
            {
                Console.WriteLine("Invalid input. ID must be an integer.");
                return;
            }

            Console.Write("Enter Skill: ");
            string skill = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(skill))
            {
                Console.WriteLine("Invalid input. Skill cannot be empty.");
                return;
            }

            try
            {
                company.RegisterSkill(id, skill);
                Console.WriteLine("Skill registered successfully.");
            }
            catch (Exception ex) when (ex is ArgumentException || ex is KeyNotFoundException)
            {
                Console.WriteLine($"Error registering skill: {ex.Message}");
            }
        }

        static void SearchEmployee(Company company)
        {
            Console.WriteLine("Search by:");
            Console.WriteLine("1. ID");
            Console.WriteLine("2. Name");
            Console.Write("Enter choice (1-2): ");
            string searchChoice = Console.ReadLine() ?? "";

            if (searchChoice == "1")
            {
                Console.Write("Enter Employee ID: ");
                if (!int.TryParse(Console.ReadLine() ?? "", out int id))
                {
                    Console.WriteLine("Invalid input. ID must be an integer.");
                    return;
                }
                Employee? emp = company.FindEmployeeById(id);
                if (emp != null)
                {
                    DisplayEmployeeDetails(emp);
                }
                else
                {
                    Console.WriteLine("Employee not found.");
                }
            }
            else if (searchChoice == "2")
            {
                Console.Write("Enter Employee Name: ");
                string name = Console.ReadLine() ?? "";
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Invalid input. Name cannot be empty.");
                    return;
                }
                List<Employee> results = company.FindEmployeesByName(name);
                if (results.Count > 0)
                {
                    Console.WriteLine("Search Results:");
                    foreach (Employee emp in results)
                    {
                        DisplayEmployeeDetails(emp);
                    }
                }
                else
                {
                    Console.WriteLine("No employees found with that name.");
                }
            }
            else
            {
                Console.WriteLine("Invalid choice.");
            }
        }

        static void FilterEmployees(Company company)
        {
            Console.WriteLine("Choose filter condition:");
            Console.WriteLine("1. Managers Only");
            Console.WriteLine("2. High Salary Employees (Salary > 7,000)");
            Console.WriteLine("3. Hired in 2024 or later");
            Console.Write("Enter choice (1-3): ");
            string filterChoice = Console.ReadLine() ?? "";

            EmployeeFilter? filter = null;
            switch (filterChoice)
            {
                case "1":
                    filter = emp => emp is Manager;
                    break;
                case "2":
                    filter = emp => emp.Salary > 7000m;
                    break;
                case "3":
                    filter = emp => emp.HireDate.Year >= 2024;
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    return;
            }

            List<Employee> results = company.FilterEmployees(filter);
            if (results.Count > 0)
            {
                Console.WriteLine("\nFiltered Employees:");
                foreach (Employee emp in results)
                {
                    DisplayEmployeeDetails(emp);
                }
            }
            else
            {
                Console.WriteLine("No employees matched the filter condition.");
            }
        }

        static void DisplayEmployeesByDepartment(Company company)
        {
            Console.Write("Enter Department ID (integer): ");
            if (!int.TryParse(Console.ReadLine() ?? "", out int id))
            {
                Console.WriteLine("Invalid input. Department ID must be an integer.");
                return;
            }

            try
            {
                Department dept = company.GetDepartmentById(id);
                List<Employee> list = company.GetEmployeesByDepartment(id);
                Console.WriteLine($"Employees in Department: {dept.Name} (ID: {dept.Id})");
                if (list.Count == 0)
                {
                    Console.WriteLine("No employees found in this department.");
                }
                else
                {
                    foreach (Employee emp in list)
                    {
                        Console.WriteLine($"- {emp.Name} (ID: {emp.Id}, Salary: {emp.Salary:C})");
                    }
                }
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void DisplayEmployeeDetails(Employee emp)
        {
            string typeStr = emp is Manager ? "Manager" : "Employee";
            Console.WriteLine($"-----------------------------------");
            Console.WriteLine($"Role: {typeStr}");
            Console.WriteLine($"ID: {emp.Id}");
            Console.WriteLine($"Name: {emp.Name}");
            Console.WriteLine($"Hire Date: {emp.HireDate:yyyy-MM-dd}");
            Console.WriteLine($"Department ID: {emp.DepartmentId}");
            Console.WriteLine($"Salary: {emp.Salary:C}");
            Console.Write("Skills: ");
            if (emp.Skills.Count > 0)
            {
                Console.WriteLine(string.Join(", ", emp.Skills));
            }
            else
            {
                Console.WriteLine("None");
            }
            if (emp is Manager mgr && mgr.TeamMembers.Count > 0)
            {
                Console.WriteLine("Team Members:");
                foreach (Employee teamMember in mgr.TeamMembers)
                {
                    Console.WriteLine($"  - {teamMember.Name} (ID: {teamMember.Id})");
                }
            }
            Console.WriteLine($"-----------------------------------");
        }
    }
}

using System;
using System.Collections.Generic;
using Employee_Management_System.Models;
using Employee_Management_System.Services;

namespace Employee_Management_System
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Company company = new Company();
            company.SeedData();

            bool exit = false;
            do
            {
                Console.WriteLine("\n=== Employee Management System ===");
                Console.WriteLine("1. Add Employee to Onboarding Queue");
                Console.WriteLine("2. Process Onboarding (FIFO)");
                Console.WriteLine("3. Add Department");
                Console.WriteLine("4. Register Skill for Employee");
                Console.WriteLine("5. Search Employee by ID or Name");
                Console.WriteLine("6. Display Employees of a Specific Department");
                Console.WriteLine("7. Calculate Average Salary");
                Console.WriteLine("8. Display Department Employee Counts");
                Console.WriteLine("9. Display Action History");
                Console.WriteLine("10. Display All Unique Skills");
                Console.WriteLine("11. Exit");
                Console.Write("Enter your choice (1-11): ");

                string choice = Console.ReadLine() ?? "";
                switch (choice)
                {
                    case "1":
                        AddEmployeeToOnboarding(company);
                        break;
                    case "2":
                        company.ProcessOnboarding();
                        break;
                    case "3":
                        AddDepartment(company);
                        break;
                    case "4":
                        RegisterSkillForEmployee(company);
                        break;
                    case "5":
                        SearchEmployee(company);
                        break;
                    case "6":
                        DisplayEmployeesByDepartment(company);
                        break;
                    case "7":
                        decimal avgSalary = company.CalculateAverageSalary();
                        Console.WriteLine($"Average Salary: {avgSalary:C}");
                        break;
                    case "8":
                        company.PrintDepartmentReport();
                        break;
                    case "9":
                        company.PrintActionHistory();
                        break;
                    case "10":
                        company.PrintAllUniqueSkills();
                        break;
                    case "11":
                        exit = true;
                        Console.WriteLine("Exiting program. Goodbye!");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please enter a number between 1 and 11.");
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

            Console.Write("Enter Hire Date: ");
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

            Employee employee = new Employee
            {
                Id = id,
                Name = name,
                HireDate = hireDate,
                DepartmentId = departmentId,
                Salary = salary
            };

            company.AddToOnboarding(employee);
            Console.WriteLine("Employee successfully added to onboarding queue.");
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

            Department department = new Department
            {
                Id = id,
                Name = name
            };

            company.AddDepartment(department);
            Console.WriteLine("Department added successfully.");
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

            company.RegisterSkill(id, skill);
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

        static void DisplayEmployeesByDepartment(Company company)
        {
            Console.Write("Enter Department ID (integer): ");
            if (!int.TryParse(Console.ReadLine() ?? "", out int id))
            {
                Console.WriteLine("Invalid input. Department ID must be an integer.");
                return;
            }
            company.DisplayEmployeesByDepartment(id);
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

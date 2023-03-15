using EmployeeLibary.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeOverview.Classes
{
    internal class DatabaseHandler
    {
        private static string connectionString = "Data Source=10.130.54.82;Initial Catalog=MadMedOmtanke;User ID=ManagerLogin;Password=mmo";

        /// <summary>
        /// Reads the employees to find the closet leader/manager.
        /// </summary>
        internal void ReadFromDataBaseEmpolyeeFromStartup(ref List<Employee> EmployeeListForStartup)
        {
            EmployeeListForStartup = new();
            using (SqlConnection connection = new(connectionString))
            {
                string Query = "SELECT [DepartmentID],[PositionID],[ClosetManager],[Name],[ID] FROM [dbo].[EmployeeViewToCreate]";


                SqlCommand command = new(Query, connection);
                connection.Open();

                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    EmployeeLibary.Models.Employee employee = new();
                    employee.DepartmentID = dataReader.GetInt32(0);
                    employee.PositionID = dataReader.GetInt32(1);
                    employee.ClosetManager = dataReader.GetString(2);
                    employee.Name = dataReader.GetString(3);
                    employee.ID = dataReader.GetInt32(4);
                    EmployeeListForStartup.Add(employee);
                }
            }
            return;
        }

        /// <summary>
        /// Reads the departments for the department dictionary.
        /// </summary>
        internal void ReadDepartmentFromDatabase(ref Dictionary<int, string> Department)
        {
            using (SqlConnection connection = new(connectionString))
            {
                string Query = "SELECT [ID],[Location] FROM [dbo].[DepartmentView]";

                SqlCommand command = new(Query, connection);
                connection.Open();

                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    Department department = new();
                    department.ID = dataReader.GetInt32(0);
                    department.Location = dataReader.GetString(1);
                    Department.Add(department.ID, department.Location);

                }
            }
            return;
        }

        /// <summary>
        /// Reads the positions for the position dictionary.
        /// </summary>
        internal void ReadPositionFromDatabase(ref Dictionary<int,string> Position)
        {
            using (SqlConnection connection = new(connectionString))
            {
                string Query = "SELECT [ID],[PositionName] FROM [dbo].[PositionView]";

                SqlCommand command = new(Query, connection);
                connection.Open();

                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    Position position = new();
                    position.ID = dataReader.GetInt32(0);
                    position.PositionName = dataReader.GetString(1);
                    Position.Add(position.ID, position.PositionName);
                }
            }
            return;
        }

        /// <summary>
        /// Write the employee to the database.
        /// </summary>
        /// <param name="employee">The employee that need to be added.</param>
        internal int WriteEmployeeToDataBase(Employee employee)
        {
            int isAdded = 0;
            using (SqlConnection conn = new(connectionString))
            {
                string Query = $"EXEC @return_value =  [dbo].[CreateEmployee] @Initials = '{employee.Initials}',@Name = '{employee.Name}',@Address = '{employee.Address}',@TlfNr = '{employee.TlfNr}',@FirmEmail = '{employee.FirmEmail}',@PositionID = '{employee.PositionID}',@DepartmentID = '{employee.DepartmentID}',@ClosetManager = '{employee.ClosetManager}',@Skills = '{employee.Skills}'";
                SqlParameter returnValue = new SqlParameter("return_value", SqlDbType.Int);
                returnValue.Direction = ParameterDirection.Output;
                SqlCommand command = new(Query, conn);
                command.Parameters.Add(returnValue);
                conn.Open();
                command.ExecuteNonQuery();
                isAdded = (int)returnValue.Value;
                conn.Close();
            }
            return isAdded;
        }
    }
}

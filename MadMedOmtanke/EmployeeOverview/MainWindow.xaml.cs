
using EmployeeLibary.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EmployeeOverview
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FillDropdowns();
        }

        List<Employee> EmployeeListForStartup = new();
        List<Employee> EmployeeList;
        Employee Employee = new();
        Dictionary<int, string> Department = new();
        Dictionary<int, string> Position = new();
        Dictionary<int, string> closestLeader = new();
        private static string _filename = string.Empty;

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.FileName = "Document";
            dialog.DefaultExt = ".csv";
            dialog.Filter = "CSV (.csv)|*.csv";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                _filename = dialog.FileName;
                FileNameLabel.Content = Path.GetFileName(_filename);
            }
        }

        private void AddEmployeesFromFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (_filename == string.Empty)
            {
                return;
            }
            //string[] FileContent = File.ReadAllLines(_filename, Encoding.GetEncoding("iso-8859-1"));
            string[] FileContent = File.ReadAllLines(_filename, Encoding.Latin1);
            bool FirstLine = true;
            foreach (string line in FileContent)
            {
                if (FirstLine)
                {
                    FirstLine = false;
                    continue;
                }
                var spiltLine = line.Split(';');
                Employee employee = new();
                employee.Name = spiltLine[0].Trim();
                employee.TlfNr = spiltLine[1].Trim();
                employee.Address = spiltLine[2].Trim();
                employee.PositionName = spiltLine[3].Trim();
                employee.PositionID = Position.First(x => x.Value == employee.PositionName).Key;
                employee.DepartmentName = spiltLine[4].Trim();
                employee.DepartmentID = Department.First(x => x.Value == employee.DepartmentName).Key;
                employee.ClosetManager = spiltLine[5].Trim();
                employee.FirmEmail = MakeFirmMail(employee.Name);
                employee.Initials = MakeInitials(employee.Name);
                WriteEmployeeToDataBase(employee);
            }
        }

        private bool GetDepartments()
        {
            ReadDepartmentFromDatabase();
            if (Department.Count == 0)
            {
                return false;
            }
            DepartmentComboBox.Items.Clear();
            foreach (var Department in Department.Values)
            {
                DepartmentComboBox.Items.Add(Department);
            }
            return true;
        }

        private bool GetPositions()
        {
            ReadPositionFromDatabase();
            if (Position.Count == 0)
            {
                return false;
            }
            PositionComboBox.Items.Clear();
            foreach (var Position in Position.Values)
            {
                PositionComboBox.Items.Add(Position);
            }
            return true;
        }

        private bool GetClosestLeader()
        {
            if (EmployeeListForStartup.Count == 0)
            {
                ReadFromDataBaseEmpolyeeFromStartup();
                if (EmployeeListForStartup.Count == 0)
                {
                    return false;
                }
            }

            //if (DepartmentComboBox.SelectedItem != null && PositionComboBox.SelectedItem != null)
            //{
            //    closestLeader.Clear();
            //    var listOfManagers = EmployeeListForStartup.Where(x => x.PositionName == PositionComboBox.SelectedItem.ToString() && x.DepartmentName == DepartmentComboBox.SelectedItem.ToString());
            //    foreach (var employee in listOfManagers)
            //    {
            //        string nameAndPosition = $"{employee.Name} - {employee.PositionName} - {employee.DepartmentName}";
            //        closestLeader.Add(employee.ID, nameAndPosition);
            //    }
            //}

            //else if (PositionComboBox.SelectedItem != null)
            //{
            //    closestLeader.Clear();
            //    var listOfManagers = EmployeeListForStartup.Where(x => x.PositionName == PositionComboBox.SelectedItem.ToString());
            //    foreach (var employee in listOfManagers)
            //    {
            //        string nameAndPosition = $"{employee.Name} - {employee.PositionName} - {employee.DepartmentName}";
            //        closestLeader.Add(employee.ID, nameAndPosition);
            //    }
            //}

            if (DepartmentComboBox.SelectedItem != null)
            {
                closestLeader.Clear();
                var listOfManagers = EmployeeListForStartup.Where(x => x.DepartmentName == DepartmentComboBox.SelectedItem.ToString());
                listOfManagers = listOfManagers.Where(x => x.PositionName.Equals("Manager", StringComparison.OrdinalIgnoreCase));
                foreach (var employee in listOfManagers)
                {
                    string nameAndPosition = $"{employee.Name} - {employee.PositionName} - {employee.DepartmentName}";
                    closestLeader.Add(employee.ID, nameAndPosition);
                }
            }

            else if (DepartmentComboBox.SelectedItem == null && closestLeader.Count == 0)
            {
                closestLeader.Clear();
                var listOfManagers = EmployeeListForStartup;
                foreach (var employee in listOfManagers)
                {
                    string nameAndPosition = $"{employee.Name} - {employee.PositionName} - {employee.DepartmentName}";
                    closestLeader.Add(employee.ID, nameAndPosition);
                }
            }
            //else if (DepartmentComboBox.SelectedItem == null && PositionComboBox.SelectedItem == null && closestLeader.Count == 0)
            //{
            //    closestLeader.Clear();
            //    var listOfManagers = EmployeeListForStartup;
            //    foreach (var employee in listOfManagers)
            //    {
            //        string nameAndPosition = $"{employee.Name} - {employee.PositionName} - {employee.DepartmentName}";
            //        closestLeader.Add(employee.ID, nameAndPosition);
            //    }
            //}


            if (closestLeader.Count == 0)
            {
                return false;
            }

            ClosestLeaderComboBox.Items.Clear();
            ClosestLeaderComboBox.Items.Add("None");
            if (PositionComboBox.SelectedItem != null)
            {
                if (PositionComboBox.SelectedItem.ToString().Equals("Manager", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            foreach (var ClosestLeader in closestLeader.Values)
            {
                ClosestLeaderComboBox.Items.Add(ClosestLeader);
            }
            return true;
        }

        private void FillDropdowns()
        {
            ErrorLabel.Foreground = Brushes.Red;
            ErrorLabel.Content = string.Empty;
            if (!GetPositions())
            {
                ErrorLabel.Content = "Failed to get Positions from database!\n";
            }
            if (!GetDepartments())
            {
                ErrorLabel.Content += "Failed to get departments from database!\n";
            }
            if (!GetClosestLeader())
            {
                ErrorLabel.Content += "Failed to get closest leader from database!\n";
            }
        }

        private void CreateEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Foreground = Brushes.Red;
            string name = NameTextBox.Text.Trim();
            string tlfNr = TelefonNumberTextBox.Text.Trim();
            string address = AddressTextBox.Text.Trim();
            string skills = SkillsTextBox.Text.Trim();
            string postion = PositionComboBox.SelectedItem.ToString().Trim();
            string department = DepartmentComboBox.SelectedItem.ToString().Trim();
            string closestLeader = ClosestLeaderComboBox.SelectedItem.ToString().Trim();
            if (name.Length == 0 || name == string.Empty || name.ToCharArray().Any(x => char.IsDigit(x)) || name == " ")
            {
                ErrorLabel.Content = "Please enter a Fullname";
                return;
            }

            if (tlfNr.Length == 0 || tlfNr == string.Empty || tlfNr.ToCharArray().Any(x => char.IsLetter(x)) || tlfNr == " ")
            {
                ErrorLabel.Content = "Please enter a Telefon Number";
                return;
            }

            if (address.Length == 0 || address == string.Empty || address == " ")
            {
                ErrorLabel.Content = "Please enter a Address";
                return;
            }

            if (skills.Length == 0 || skills == string.Empty || skills.ToCharArray().Any(x => char.IsDigit(x)) || skills == " ")
            {
                ErrorLabel.Content = "Please enter some Skills";
                return;
            }

            if (postion.Length == 0 || postion == string.Empty || postion == " ")
            {
                ErrorLabel.Content = "Please select a Postion";
                return;
            }

            if (department.Length == 0 || department == string.Empty || department == " ")
            {
                ErrorLabel.Content = "Please select a Department";
                return;
            }

            if (postion.Equals("Manager", StringComparison.OrdinalIgnoreCase) && closestLeader.Equals("None", StringComparison.OrdinalIgnoreCase))
            {
                closestLeader = "God";    //Need to find out what to put here.
            }

            if (closestLeader.Length == 0 || closestLeader == string.Empty || closestLeader == " " || closestLeader.Equals("None", StringComparison.OrdinalIgnoreCase))
            {
                ErrorLabel.Content = "Please select a Closest leader";
                return;
            }

            Employee.Name = name;
            Employee.Address = address;
            Employee.TlfNr = tlfNr;
            Employee.Skills = skills;
            Employee.PositionID = Position.First(x => x.Value == postion).Key;
            Employee.PositionName = postion;
            Employee.DepartmentID = Department.First(x => x.Value == department).Key;
            Employee.DepartmentName = department;
            Employee.ClosetManager = closestLeader.Split("-")[0].Trim();
            Employee.Initials = MakeInitials(Employee.Name);
            Employee.FirmEmail = MakeFirmMail(Employee.Name);
            WriteEmployeeToDataBase(Employee);
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NameTextBox.Text != string.Empty || NameTextBox.Text != " " || NameTextBox.Text != null)
            {
                NameLabel.Content = string.Empty;
            }

            if (NameTextBox.Text == string.Empty || NameTextBox.Text == " " || NameTextBox.Text == null)
            {
                NameLabel.Content = "Name";
            }
        }

        private void TelefonNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TelefonNumberTextBox.Text != string.Empty || TelefonNumberTextBox.Text != " " || TelefonNumberTextBox.Text != null)
            {
                TelefonLabel.Content = string.Empty;
            }

            if (TelefonNumberTextBox.Text == string.Empty || TelefonNumberTextBox.Text == " " || TelefonNumberTextBox.Text == null)
            {
                TelefonLabel.Content = "TlfNr";
            }
        }

        private void AddressTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AddressTextBox.Text != string.Empty || AddressTextBox.Text != " " || AddressTextBox.Text != null)
            {
                AddressLabel.Content = string.Empty;
            }

            if (AddressTextBox.Text == string.Empty || AddressTextBox.Text == " " || AddressTextBox.Text == null)
            {
                AddressLabel.Content = "Address";
            }
        }

        private void SkillsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SkillsTextBox.Text != string.Empty || SkillsTextBox.Text != " " || SkillsTextBox.Text != null)
            {
                SkillsLabel.Content = string.Empty;
            }

            if (SkillsTextBox.Text == string.Empty || SkillsTextBox.Text == " " || SkillsTextBox.Text == null)
            {
                SkillsLabel.Content = "Skills";
            }
        }
        private static string connectionString = "Data Source=10.130.54.82;Initial Catalog=MadMedOmtanke;User ID=casp103a;Password=1234";

        private void ReadFromDataBaseEmpolyeeFromStartup()
        {
            EmployeeListForStartup = new();
            using (SqlConnection connection = new(connectionString))
            {
                string Query = "SELECT [DepartmentID],[PositionID],[ClosetManager],[Name],[Location],[PositionName],[ID] FROM [dbo].[EmployeeViewToCreate]";

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
                    employee.DepartmentName = dataReader.GetString(4);
                    employee.PositionName = dataReader.GetString(5);
                    employee.ID = dataReader.GetInt32(6);
                    EmployeeListForStartup.Add(employee);
                }
            }
            return;
        }

        private void ReadDepartmentFromDatabase()
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

        private void ReadPositionFromDatabase()
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

        private void WriteEmployeeToDataBase(Employee employee)
        {
            using (SqlConnection conn = new(connectionString))
            {
                string Query = $"EXEC [dbo].[CreateEmployee] @Initials = '{employee.Initials}',@Name = '{employee.Name}',@Address = '{employee.Address}',@TlfNr = '{employee.TlfNr}',@FirmEmail = '{employee.FirmEmail}',@PositionID = '{employee.PositionID}',@DepartmentID = '{employee.DepartmentID}',@ClosetManager = '{employee.ClosetManager}',@Skills = '{employee.Skills}', @PositionName = '{employee.PositionName}', @DepartmentName = '{employee.DepartmentName}'";
                SqlCommand command = new(Query, conn);
                conn.Open();
                command.ExecuteNonQuery();
                conn.Close();
            }

            ErrorLabel.Foreground = Brushes.Green;
            ErrorLabel.Content = "Employee was added to database";
        }

        private void PositionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PositionLabel.Content = string.Empty;
        }

        private void DepartmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DepartmentLabel.Content = string.Empty;
            GetClosestLeader();
        }

        private string GetTwoFirstLetters(string name)
        {
            string Result = string.Empty;
            foreach (char Letter in name)
            {
                Result += Letter.ToString();
                if (Result.Length == 2)
                {
                    break;
                }
            }
            return Result;
        }

        private string MakeInitials(string name)
        {
            if (name == null || name == string.Empty)
            {
                return string.Empty;
            }

            string initials = string.Empty;

            if (name.Split(" ").Length == 1)
            {
                if (name.Length >= 4)
                {
                    int length = name.Length;
                    string test = name[length - 2].ToString() + name[length - 1];
                    initials = $"{GetTwoFirstLetters(name)}{GetTwoFirstLetters(test)}";
                    return initials;
                }
                else
                {
                    return name;
                }
            }

            string[] names = name.Split(" ");
            initials += $"{GetTwoFirstLetters(names[0])}{GetTwoFirstLetters(names[names.Length - 1])}";

            return initials.ToUpper();
        }

        private string MakeFirmMail(string name)
        {
            var nameSpilt = name.Split(" ");
            return $"{name.ToCharArray()[0]}{nameSpilt[nameSpilt.Length - 1]}@MadMedOmtanke.dk";
        }

        private void ClosestLeaderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClosestLeaderLabel.Content = string.Empty;
        }
    }
}

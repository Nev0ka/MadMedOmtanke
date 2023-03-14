using EmployeeLibary.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

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
        //List<Employee> EmployeeList;
        Employee Employee = new();
        Dictionary<int, string> Department = new();
        Dictionary<int, string> Position = new();
        Dictionary<int, string> closestLeader = new();
        private static string _filename = string.Empty;

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
            string Error = string.Empty;
            if (!GetPositions())
            {
                Error = "Failed to get Positions from database!\n";
            }
            if (!GetDepartments())
            {
                Error = "Failed to get departments from database!\n";
            }
            if (!GetClosestLeader())
            {
                Error += "Failed to get closest leader from database!\n";
            }
            if (Error != string.Empty)
            {
                MessageBox.Show(Error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim();
            string tlfNr = TelefonNumberTextBox.Text.Trim();
            string address = AddressTextBox.Text.Trim();
            string skills = SkillsTextBox.Text.Trim();
            string postion = PositionComboBox.SelectedItem.ToString().Trim();
            string department = DepartmentComboBox.SelectedItem.ToString().Trim();
            string closestLeader = ClosestLeaderComboBox.SelectedItem.ToString().Trim();
            if (name.Length == 0 || name == string.Empty || name.ToCharArray().Any(x => char.IsDigit(x)) || name == " ")
            {
                MessageBox.Show("Please enter a Fullname", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (tlfNr.Length == 0 || tlfNr == string.Empty || tlfNr.ToCharArray().Any(x => char.IsLetter(x)) || tlfNr == " ")
            {
                MessageBox.Show("Please enter a Telefon Number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (address.Length == 0 || address == string.Empty || address == " ")
            {
                MessageBox.Show("Please enter a Address", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (skills.Length == 0 || skills == string.Empty || skills.ToCharArray().Any(x => char.IsDigit(x)) || skills == " ")
            {
                MessageBox.Show("Please enter some Skills", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (postion.Length == 0 || postion == string.Empty || postion == " ")
            {
                MessageBox.Show("Please select a Postion", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (department.Length == 0 || department == string.Empty || department == " ")
            {
                MessageBox.Show("Please select a Department", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (postion.Equals("Manager", StringComparison.OrdinalIgnoreCase) && closestLeader.Equals("None", StringComparison.OrdinalIgnoreCase))
            {
                closestLeader = "God";    //Need to find out what to put here.
            }

            if (closestLeader.Length == 0 || closestLeader == string.Empty || closestLeader == " " || closestLeader.Equals("None", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Please select a Closest leader", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Employee.Name = name;
            Employee.Address = address;
            Employee.TlfNr = tlfNr;
            Employee.Skills = skills;
            Employee.PositionID = Position.First(x => x.Value == postion).Key;
            Employee.DepartmentID = Department.First(x => x.Value == department).Key;
            Employee.ClosetManager = closestLeader.Split("-")[0].Trim();
            Employee.Initials = MakeInitials(Employee.Name);
            Employee.FirmEmail = MakeFirmMail(Employee.Name);
            WriteEmployeeToDataBase(Employee);
            RestartPage();
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

            if (isAdded == 1)
            {
                Log($"Employee {employee.Name} was added to database", "Green");
            }
            else if (isAdded <= 0)
            {
                Log($"Employee {employee.Name} wasn't added to database", "Red");
            }
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

        private void RestartPage()
        {
            PositionComboBox.SelectedIndex = -1;
            PositionLabel.Content = "Position";
            DepartmentComboBox.SelectedIndex = -1;
            DepartmentLabel.Content = "Department";
            ClosestLeaderComboBox.SelectedIndex = -1;
            ClosestLeaderLabel.Content = "Closet manager";
            NameTextBox.Text = string.Empty;
            TelefonNumberTextBox.Text = string.Empty;
            AddressTextBox.Text = string.Empty;
            SkillsTextBox.Text = string.Empty;
        }
        private void Log(string LogMessage, string color)
        {
            string filecontent = string.Empty;
            string filepath = $"{Environment.CurrentDirectory}\\log\\EmployeeLog.txt";
            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filepath));
            }
            
            DateTime dateTime = DateTime.Now;
            LogMessage += $"  ;{color};  {dateTime.ToString("dd/MMM/yyyy  HH:mm")} \n";
            if (File.Exists(filepath))
            {
                filecontent = File.ReadAllText(filepath);
            }
            filecontent += LogMessage;
            File.WriteAllText(filepath, filecontent);
        }

        private void BrowseFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.FileName = "Document";
            dialog.DefaultExt = ".csv";
            dialog.Filter = "CSV (.csv)|*.csv";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                _filename = dialog.FileName;
                //FileNameLabel.Content = Path.GetFileName(_filename);
            }
        }

        private void AddEmployeeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_filename == string.Empty)
            {
                MessageBox.Show("Please select a file","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }
            if (!File.Exists(_filename))
            {
                MessageBox.Show("Please select a file that exists", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
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
                employee.Skills = spiltLine[6].Trim();
                employee.FirmEmail = MakeFirmMail(employee.Name);
                employee.Initials = MakeInitials(employee.Name);
                WriteEmployeeToDataBase(employee);
            }
            MessageBox.Show("Button clicked","Succes",MessageBoxButton.OK,MessageBoxImage.Information);
        }
    }
}

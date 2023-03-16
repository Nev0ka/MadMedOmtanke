using EmployeeLibary.Models;
using EmployeeOverview.Classes;
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
using System.Windows.Media;

namespace EmployeeOverview
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FillDropdowns();
            if (File.Exists(logging.logFilePath))
            {
                UpdateLogList(ReadFile());
            }
        }

        List<Employee> EmployeeListForStartup = new();
        Employee Employee = new();
        Dictionary<int, string> Department = new();
        Dictionary<int, string> Position = new();
        Dictionary<int, string> closestLeader = new();
        private static string _filename = string.Empty;

        /// <summary>
        /// Get Departments from database.
        /// </summary>
        /// <returns>Returns true if it can get the departments.</returns>
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

        /// <summary>
        /// Get positions from database.
        /// </summary>
        /// <returns>Returns true if it can get the positions.</returns>
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

        /// <summary>
        /// Gets closest leader/manager and sort so you only can see those who are in the same department if there is a department selected.
        /// </summary>
        /// <returns>Returns true if it can get closest leader/manager.</returns>
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

            if (DepartmentComboBox.SelectedItem != null)
            {
                closestLeader.Clear();
                var listOfManagers = EmployeeListForStartup.Where(x => x.DepartmentID == Department.First(x => x.Value == DepartmentComboBox.SelectedItem.ToString()).Key);
                listOfManagers = listOfManagers.Where(x => Position.First(y => y.Key == x.PositionID).Value.Equals("Manager", StringComparison.OrdinalIgnoreCase));
                foreach (var employee in listOfManagers)
                {
                    string nameAndPosition = $"{employee.Name} - {Position.First(x => x.Key == employee.PositionID).Value} - {Department.First(x => x.Key == employee.DepartmentID).Value}";
                    closestLeader.Add(employee.ID, nameAndPosition);
                }
            }

            else if (DepartmentComboBox.SelectedItem == null && closestLeader.Count == 0)
            {
                closestLeader.Clear();
                var listOfManagers = EmployeeListForStartup;
                foreach (var employee in listOfManagers)
                {
                    string nameAndPosition = $"{employee.Name} - {Position.First(x => x.Key == employee.PositionID).Value} - {Department.First(x => x.Key == employee.DepartmentID).Value}";

                    closestLeader.Add(employee.ID, nameAndPosition);
                }
            }

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
                closestLeader = "God";
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
            UpdateLogList(ReadFile());
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

        DatabaseHandler database = new();

        /// <summary>
        /// Reads the employees to find the closet leader/manager.
        /// </summary>
        private void ReadFromDataBaseEmpolyeeFromStartup()
        {
            EmployeeListForStartup = new();
            database.ReadFromDataBaseEmpolyeeFromStartup(ref EmployeeListForStartup);
            return;
        }

        /// <summary>
        /// Reads the departments for the department dictionary.
        /// </summary>
        private void ReadDepartmentFromDatabase()
        {
            database.ReadDepartmentFromDatabase(ref Department);
            return;
        }

        /// <summary>
        /// Reads the positions for the position dictionary.
        /// </summary>
        private void ReadPositionFromDatabase()
        {
            database.ReadPositionFromDatabase(ref Position);
            return;
        }

        /// <summary>
        /// Write the employee to the database.
        /// </summary>
        /// <param name="employee">The employee that need to be added.</param>
        private void WriteEmployeeToDataBase(Employee employee)
        {
            int isAdded = database.WriteEmployeeToDataBase(employee);

            if (isAdded == 1)
            {
                Log($"Employee {employee.Name} was added to database");
            }
            else if (isAdded <= 0)
            {
                Log($"Employee {employee.Name} wasn't added to database");
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

        /// <summary>
        /// Make the initials from the name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>returns initials in uppercase.</returns>
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

        /// <summary>
        /// Make the frim mail
        /// </summary>
        /// <param name="name"></param>
        /// <returns>returns the mail.</returns>
        private string MakeFirmMail(string name)
        {
            var nameSpilt = name.Split(" ");
            return $"{name.ToCharArray()[0]}{nameSpilt[nameSpilt.Length - 1]}@MadMedOmtanke.dk";
        }

        /// <summary>
        /// Emptys the you select the closet manager/leader.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClosestLeaderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClosestLeaderLabel.Content = string.Empty;
        }

        /// <summary>
        /// Reset the everthing to do with creating a employee.
        /// </summary>
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

        Logging logging = new();
        /// <summary>
        /// Used to write a log in the log file.
        /// </summary>
        /// <param name="LogMessage">The thing you what to write down.</param>
        /// <param name="color">Is color that the log needs to be.</param>
        private void Log(string LogMessage)
        {
            logging.Log(LogMessage);
        }

        /// <summary>
        /// Reads the file at converts it into a list
        /// </summary>
        /// <returns>Returns a list of string containing all the lines from the file.</returns>
        private List<string> ReadFile()
        {
            return logging.ReadFile();
        }

        /// <summary>
        /// Clears the log view and make a marking in the file that is have been cleared.
        /// </summary>
        private void ClearLog()
        {
            logging.ClearLog();
            LogMenuListView.Items.Clear();
        }

        /// <summary>
        /// Updates the log view in the menu with latest logings by looking at the markings in the file.
        /// </summary>
        /// <param name="filecontent"></param>
        private void UpdateLogList(List<string> filecontent)
        {
            filecontent = logging.UpdateLogList(filecontent);
            foreach (var line in filecontent)
            {
                LogMenuListView.Items.Add(line.Split(";")[0].Trim());
            }
        }

        /// <summary>
        /// Browse for the csv file that needs to be converted into employees.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            }
        }

        /// <summary>
        /// Adds employees to the database with the file that have been selected from the file menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddEmployeeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_filename == string.Empty)
            {
                MessageBox.Show("Please select a file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                employee.PositionID = Position.First(x => x.Value == spiltLine[3].Trim()).Key;
                employee.DepartmentID = Department.First(x => x.Value == spiltLine[4].Trim()).Key;
                employee.ClosetManager = spiltLine[5].Trim();
                employee.Skills = spiltLine[6].Trim();
                employee.FirmEmail = MakeFirmMail(employee.Name);
                employee.Initials = MakeInitials(employee.Name);
                WriteEmployeeToDataBase(employee);
            }
            UpdateLogList(ReadFile());
            MessageBox.Show("Button clicked", "Infomation", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LogClearMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ClearLog();
            MessageBox.Show("Log is cleared", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

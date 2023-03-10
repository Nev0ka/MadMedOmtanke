
using EmployeeLibary.Models;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

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
            dialog.FileName = "Document"; // Default file name
            dialog.DefaultExt = ".csv"; // Default file extension
            dialog.Filter = "CSV (.csv)|*.csv"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
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
            string[] FileContent = File.ReadAllLines(_filename);
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

            if (DepartmentComboBox.SelectedItem != null && PositionComboBox.SelectedItem != null)
            {
                closestLeader.Clear();
                var listOfManagers = EmployeeListForStartup.Where(x => x.PositionName == PositionComboBox.SelectedItem.ToString() && x.DepartmentName == DepartmentComboBox.SelectedItem.ToString());
                foreach (var employee in listOfManagers)
                {
                    string nameAndPosition = $"{employee.Name} - {employee.PositionName} - {employee.DepartmentName}";
                    closestLeader.Add(employee.ID, nameAndPosition);
                }
            }

            else if (PositionComboBox.SelectedItem != null)
            {
                closestLeader.Clear();
                var listOfManagers = EmployeeListForStartup.Where(x => x.PositionName == PositionComboBox.SelectedItem.ToString());
                foreach (var employee in listOfManagers)
                {
                    string nameAndPosition = $"{employee.Name} - {employee.PositionName} - {employee.DepartmentName}";
                    closestLeader.Add(employee.ID, nameAndPosition);
                }
            }

            else if (DepartmentComboBox.SelectedItem != null)
            {
                closestLeader.Clear();
                var listOfManagers = EmployeeListForStartup.Where(x => x.DepartmentName == DepartmentComboBox.SelectedItem.ToString());
                foreach (var employee in listOfManagers)
                {
                    string nameAndPosition = $"{employee.Name} - {employee.PositionName} - {employee.DepartmentName}";
                    closestLeader.Add(employee.ID, nameAndPosition);
                }
            }

            else if (DepartmentComboBox.SelectedItem == null && PositionComboBox.SelectedItem == null && closestLeader.Count == 0)
            {
                closestLeader.Clear();
                var listOfManagers = EmployeeListForStartup;
                foreach (var employee in listOfManagers)
                {
                    string nameAndPosition = $"{employee.Name} - {employee.PositionName} - {employee.DepartmentName}";
                    closestLeader.Add(employee.ID, nameAndPosition);
                }
            }


            if (closestLeader.Count == 0)
            {
                return false;
            }

            ClosestLeaderComboBox.Items.Clear();
            foreach (var ClosestLeader in closestLeader.Values)
            {
                ClosestLeaderComboBox.Items.Add(ClosestLeader);
            }
            return true;
        }

        private void FillDropdowns()
        {
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

            if (closestLeader.Length == 0 || closestLeader == string.Empty || closestLeader == " ")
            {
                ErrorLabel.Content = "Please select a Closest leader";
                return;
            }
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

        private void PositionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetClosestLeader();
        }

        private void DepartmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetClosestLeader();
        }

        //public void WriteProductToDatabase()
        //{
        //    using (SqlConnection conn = new(connectionString))
        //    {
        //        string Query = $"INSERT INTO [dbo].[Product]([Title],[Type],[Stock],[ProductID]) VALUES ('{Product.Title}','{Product.Type}','{Product.Stock}','{Product.ProductID}')";
        //        SqlCommand command = new(Query, conn);
        //        conn.Open();
        //        command.ExecuteNonQuery();
        //        conn.Close();
        //    }
        //}
    }
}

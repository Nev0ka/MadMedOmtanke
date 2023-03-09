
using Microsoft.Win32;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Windows;

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
            GetDepartments();
            GetPositions();
            GetClosestLeader();
            FillDropdowns();
        }

        Dictionary<int,string> Department = new();
        Dictionary<int,string> Position = new();
        Dictionary<int,string> closestLeader = new();
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

        }

        private bool GetDepartments()
        {
            return false;
        }

        private bool GetPositions()
        {
            return false;

        }

        private bool GetClosestLeader()
        {
            return false;
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

            foreach (var Position in Position.Values)
            {
                PositionComboBox.Items.Clear();
                PositionComboBox.Items.Add(Position);
            }

            foreach (var Department in Department.Values)
            {
                DepartmentComboBox.Items.Clear();
                DepartmentComboBox.Items.Add(Department);
            }

            foreach (var ClosestLeader in closestLeader.Values)
            {
                ClosestLeaderComboBox.Items.Clear();
                ClosestLeaderComboBox.Items.Add(ClosestLeader);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autodesk.Revit.UI;
using Microsoft.Win32;


namespace RAA_Level2
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class MyForm : Window
    {
        
        public MyForm()
        {
            InitializeComponent();
        }
        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.RestoreDirectory = true;
            openFile.Filter = "csv files (*.csv)|*.csv";

            if(openFile.ShowDialog() == true)
            {
                tbxSelect.Text = openFile.FileName;
            }
            else
            {
                tbxSelect.Text = "";
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        { 
            //TaskDialog.Show("Test", "Operation is Cancelled");
            this.DialogResult = false;
            this.Close();
        }

        internal string GetFilePath()
        {
            if (tbxSelect.Text != "")
                return tbxSelect.Text;
            else
                TaskDialog.Show("Error", "Please Select File");
            return null;
        }

        internal string GetUnits()
        {
            if (rb1.IsChecked == true)
                return "imperial";
            return "metric";
        }

        internal bool CreateFloorPlan()
        {
            if (cb1.IsChecked == true)
                return true;
            return false;
        }

        internal bool CreateCeilingPlan()
        {
            if (cb2.IsChecked == true)
                return true;
            return false;
        }
    }

}

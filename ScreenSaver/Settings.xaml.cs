using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ScreenSaver
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private ScreenSaverSettings screenSettings { get; set; }

        public Settings()
        {
            InitializeComponent();
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter(ddColor.Color);
            var dueDateColorData = converter.ConvertToString(ddColor.Color);

            System.ComponentModel.TypeConverter converter2 = System.ComponentModel.TypeDescriptor.GetConverter(ccMessage.Color);
            var messageTextColor = converter2.ConvertToString(ccMessage.Color);

            System.ComponentModel.TypeConverter converter3 = System.ComponentModel.TypeDescriptor.GetConverter(backgroundDueDate.Color);
            var BackgroundDueDateColor = converter3.ConvertToString(backgroundDueDate.Color);

            System.ComponentModel.TypeConverter converter4 = System.ComponentModel.TypeDescriptor.GetConverter(backgroundMessageColor.Color);
            var BackgroundMessageColor = converter4.ConvertToString(backgroundMessageColor.Color);

            string dueDate = "";
            string companyLogo = "";

            if (dpDueDate.SelectedDate != null)
            {
                dueDate = dpDueDate.SelectedDate.Value.ToString();
            }

            if (!String.IsNullOrWhiteSpace(screenSettings.CompanyLogo))
            {
                companyLogo = screenSettings.CompanyLogo;
            }

            screenSettings = new ScreenSaverSettings()
            {
                Message = txtMessage.Text,
                DueDateMessage = txtDueDateMessage.Text,
                DueDate = dueDate,
                CompanyLogo = companyLogo,
                DueDateTextColor = dueDateColorData,
                MessageTextColor = messageTextColor,
                BackgroundDueDateColor = BackgroundDueDateColor,
                BackgroundMessageColor = BackgroundMessageColor
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(screenSettings);

            try
            {
                var sw = new StreamWriter(@"c:\screen\data.enigma");
                sw.Write(json);
                sw.Close();
            }
            catch(Exception exp)
            {
                MessageBox.Show(exp.Message, "Unable to write to directory");
            }

            this.Close();
        }

        private void btnBrowseImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".png";
            dlg.Filter = "png (.png)|*.png";

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                screenSettings.CompanyLogo = dlg.FileName;
                imgCompanyLogo.Source = new BitmapImage(new Uri(screenSettings.CompanyLogo));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists(@"c:\screen\data.enigma"))
            {
                string jsonData = "";

                try
                {
                    var reader = new StreamReader(@"c:\screen\data.enigma");
                    jsonData = reader.ReadToEnd();
                    reader.Close();

                } catch(Exception) { } // file may not exist

                if (!String.IsNullOrWhiteSpace(jsonData))
                {
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ScreenSaverSettings>(jsonData);
                    screenSettings = data;

                    if (!String.IsNullOrWhiteSpace(data.Message))
                    {
                        txtMessage.Text = data.Message;
                    }

                    if (!String.IsNullOrWhiteSpace(data.DueDateMessage))
                    {
                        txtDueDateMessage.Text = data.DueDateMessage;
                    }

                    if (!String.IsNullOrWhiteSpace(data.DueDate))
                    {
                        dpDueDate.SelectedDate = Convert.ToDateTime(data.DueDate);
                    }

                    if (!String.IsNullOrWhiteSpace(data.MessageTextColor))
                    {
                        ccMessage.Color = ConvertStringToColor(data.MessageTextColor);
                    }

                    if (!String.IsNullOrWhiteSpace(data.DueDateTextColor))
                    {
                        ddColor.Color = ConvertStringToColor(data.DueDateTextColor);
                    }

                    if (!String.IsNullOrWhiteSpace(data.BackgroundDueDateColor))
                    {
                        backgroundDueDate.Color = ConvertStringToColor(data.BackgroundDueDateColor);
                    }

                    if (!String.IsNullOrWhiteSpace(data.BackgroundMessageColor))
                    {
                        backgroundMessageColor.Color = ConvertStringToColor(data.BackgroundMessageColor);
                    }

                    if (!String.IsNullOrWhiteSpace(data.CompanyLogo))
                    {
                        imgCompanyLogo.Source = new BitmapImage(new Uri(data.CompanyLogo));
                    }
                }
            }
            else
            {
                screenSettings = new ScreenSaverSettings();
            }
        }

        private System.Windows.Media.Color ConvertStringToColor(String hex)
        {
            //remove the # at the front
            hex = hex.Replace("#", "");

            byte a = 255;
            byte r = 255;
            byte g = 255;
            byte b = 255;

            int start = 0;

            //handle ARGB strings (8 characters long)
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                start = 2;
            }

            //convert RGB characters to bytes
            r = byte.Parse(hex.Substring(start, 2), System.Globalization.NumberStyles.HexNumber);
            g = byte.Parse(hex.Substring(start + 2, 2), System.Globalization.NumberStyles.HexNumber);
            b = byte.Parse(hex.Substring(start + 4, 2), System.Globalization.NumberStyles.HexNumber);

            return System.Windows.Media.Color.FromArgb(a, r, g, b);
        }
    }

    public class ScreenSaverSettings
    {
        public string Message { get; set; }
        public string MessageTextColor { get; set; }
        public string DueDate { get; set; }
        public string DueDateMessage { get; set; }
        public string DueDateTextColor { get; set; }
        public string CompanyLogo { get; set; }
        public string BackgroundDueDateColor { get; set; }
        public string BackgroundMessageColor { get; set; }
    }
}

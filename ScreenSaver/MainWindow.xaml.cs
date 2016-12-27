using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ScreenSaver
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }

        private string Message { get; set; }
        private DateTime releaseDate { get; set; }
        private string Ready { get; set; }
        private Color DueDateTextColor { get; set; }
        private Color MessageTextColor { get; set; }

        private Color DueDateBackgroundColor { get; set; }
        private Color MessageBackgroundColor { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(35);
                timer.Tick += timer_Tick;
                timer.IsEnabled = true;
                timer.Start();

                if (File.Exists(@"c:\screen\data.enigma"))
                {
                    string jsonData = "";
                    try
                    {
                        var reader = new StreamReader(@"c:\screen\data.enigma");
                        jsonData = reader.ReadToEnd();
                        reader.Close();
                    }
                    catch(Exception) { }

                    if (!String.IsNullOrWhiteSpace(jsonData))
                    {
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ScreenSaverSettings>(jsonData);
                        if (data != null)
                        {
                            if (!String.IsNullOrWhiteSpace(data.Message))
                            {
                                Message = data.Message;
                            }

                            if (!String.IsNullOrWhiteSpace(data.DueDateMessage))
                            {
                                Ready = data.DueDateMessage;
                            }

                            if (!String.IsNullOrWhiteSpace(data.DueDate))
                            {
                                releaseDate = DateTime.Parse(data.DueDate);
                            }

                            if (!String.IsNullOrWhiteSpace(data.DueDateTextColor))
                            {
                                DueDateTextColor = ConvertStringToColor(data.DueDateTextColor);
                            }

                            if (!String.IsNullOrWhiteSpace(data.MessageTextColor))
                            {
                                MessageTextColor = ConvertStringToColor(data.MessageTextColor);
                            }

                            if (!String.IsNullOrWhiteSpace(data.BackgroundDueDateColor))
                            {
                                DueDateBackgroundColor = ConvertStringToColor(data.BackgroundDueDateColor);
                            }

                            if (!String.IsNullOrWhiteSpace(data.BackgroundMessageColor))
                            {
                                MessageBackgroundColor = ConvertStringToColor(data.BackgroundMessageColor);
                            }

                            if (!String.IsNullOrWhiteSpace(data.CompanyLogo))
                            {
                                imgCompanyLogo.Source = new BitmapImage(new Uri(data.CompanyLogo));
                            }
                        }
                    }
                }

                timer_Tick(null, null);
            }
            catch(Exception exp)
            {
                MessageBox.Show("error", exp.Message);
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(Message))
            {
                lblTitle.Text = Message;
            }
            else
            {
                lblTitle.Text = "";
            }

            var start = DateTime.Now;
            var endTime = releaseDate;

            TimeSpan remainingTime = endTime - start;

            if (remainingTime < TimeSpan.Zero)
            {
                SolidColorBrush brush = new SolidColorBrush(DueDateBackgroundColor);
                MasterWindow.Background = brush;

                SolidColorBrush titleBrush = new SolidColorBrush(DueDateTextColor);

                lblTitle.Foreground = titleBrush;
                lblDay.Foreground = titleBrush;
                lblTitle.Text = "";
                lblDay.Text = Ready;
                lblDay.FontSize = 100;
                lblDay.Margin = new Thickness(0, -480, 0, 0);
            }
            else
            {
                var days = "Days";
                if (remainingTime.Days == 1)
                {
                    days = "Day";
                }

                var Hours = "Hours";
                if (remainingTime.Hours == 1)
                {
                    Hours = "Hour";
                }

                SolidColorBrush titleBrush = new SolidColorBrush(MessageTextColor);

                SolidColorBrush brush = new SolidColorBrush(MessageBackgroundColor);
                MasterWindow.Background = brush;

                lblDay.Foreground = titleBrush;
                lblTitle.Foreground = titleBrush;
                lblDay.Margin = new Thickness(0, 0, 0, 0);
                lblDay.Text = (remainingTime.Days + " " + days + " " + remainingTime.Hours + " " + Hours);
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            //this.Close();
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
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
}

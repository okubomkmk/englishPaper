using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Threading;
namespace Microsoft.Samples.Kinect.InfraredBasics
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string makeTimestampFilename(DateTime printTime)
        {
            string mikachan = printTime.ToString().Replace(@"/", "");
            mikachan = mikachan.Replace(@":", "");
            mikachan = mikachan.Replace(" ", "");
            return mikachan;
        }

        private void ChangeData_Click(object sender, RoutedEventArgs e)
        {
            if (mapIsIR)
            {
                this.ChangeData.Content = "ToColor";
                this.Picture.Source = depthBitmap;
            }
            else
            {
                this.ChangeData.Content = "ToDepth";
                this.Picture.Source = colorBitmap;
            }
            mapIsIR = !mapIsIR;
        }

        private void CheckFileNameStable_Checked(object sender, RoutedEventArgs e)
        {
            FileNameStableFlag = true;
            IsTimestampNeeded = false;
            this.CheckNonTimeStamp.IsEnabled = false;
        }

        private void CheckFileNameStable_Unchecked(object sender, RoutedEventArgs e)
        {
            FileNameStableFlag = false;
            IsTimestampNeeded = true;
            this.CheckNonTimeStamp.IsEnabled = true;
        }

        private void Picture_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!WritingFlag)
            {
                this.textXlock.Text = e.GetPosition(Picture).X.ToString();
                this.textYlock.Text = e.GetPosition(Picture).Y.ToString();
            }


        }

        private void ButtonXup_Click(object sender, RoutedEventArgs e)
        {
            Point pointNow = getLockPosition();
            pointNow.X++;
            this.textXlock.Text = pointNow.X.ToString();

        }

        private void ButtonXdown_Click(object sender, RoutedEventArgs e)
        {
            Point pointNow = getLockPosition();
            pointNow.X--;
            this.textXlock.Text = pointNow.X.ToString();
        }

        private void ButtonYup_Click(object sender, RoutedEventArgs e)
        {
            Point pointNow = getLockPosition();
            pointNow.Y++;
            this.textYlock.Text = pointNow.Y.ToString();
        }

        private void ButtonYdown_Click(object sender, RoutedEventArgs e)
        {
            Point pointNow = getLockPosition();
            pointNow.Y--;
            this.textYlock.Text = pointNow.Y.ToString();
        }
        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            // set the status text
            //this.StatusText = this.kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
            //: Properties.Resources.SensorNotAvailableStatusText;
        }


        private void CheckNonTimeStamp_Checked(object sender, RoutedEventArgs e)
        {
            IsTimestampNeeded = false;
        }

        private void CheckNonTimeStamp_Unchecked(object sender, RoutedEventArgs e)
        {
            IsTimestampNeeded = true;
        }

        private void ButtonWriteDown_Click(object sender, RoutedEventArgs e)
        {

            DispatcherTimer ButtonEditorTimer = new DispatcherTimer(DispatcherPriority.Normal);
            ButtonEditorTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            ButtonEditorTimer.Tick += new EventHandler(ButtonEdit);
            ButtonEditorTimer.Start();
            writeDownedCounter = 0;
            ButtonWriteDown.IsEnabled = false;
            textXlock.IsEnabled = false;
            textYlock.IsEnabled = false;
            timestamp = DateTime.Now;  //timestamp is the time when the record is started


        }

        private void ButtonEdit(object sender, EventArgs e)
        {

            this.ButtonWriteDown.Content = (WaitForStartingRecord).ToString();
            WaitForStartingRecord--;
            if (WaitForStartingRecord == -1)
            {
                WritingFlag = true;
            }
        }

        private void CheckLockCenter_Checked(object sender, RoutedEventArgs e)
        {
            cursol_locked = true;
            this.ButtonWriteDown.IsEnabled = true;
        }

        private void CheckLockCenter_Unchecked(object sender, RoutedEventArgs e)
        {
            cursol_locked = false;
            this.ButtonWriteDown.IsEnabled = false;
        }
    }
}

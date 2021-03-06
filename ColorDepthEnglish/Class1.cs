﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
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
       

        private unsafe ushort shiburinkawaiiyoo(ushort* ProcessData, double X, double Y)
        {
            return ProcessData[(int)(Y * this.depthFrameDescription.Width + X)];
        }
        private unsafe ushort shiburinkawaiiyoo(ushort* ProcessData, Point location)
        {
            return ProcessData[(int)(location.Y * this.depthFrameDescription.Width + location.X)];
        }

        private unsafe ushort shiburinkawaiiyoo(ushort[] ProcessData, double X, double Y)
        {
            return ProcessData[(int)(Y * this.depthFrameDescription.Width + X)];

        }

        private unsafe void writeToText(ushort[] measureArray, ushort[] centerArray, string type)
        {

            string StartedTime = makeTimestampFilename(timestamp);
            string filenamePartialIR = FileNameStableFlag ? System.IO.Path.Combine(@"V:\EnglishPaperPresentation\", type + "Measure" + this.FileNameTextbox.GetLineText(0) + ".dat") : System.IO.Path.Combine(@"V:\KinectIR\capturedData\", StartedTime + type + ".dat");
            this.filenameLabel.Content = filenamePartialIR;
            string filenameCenterIR = FileNameStableFlag ? System.IO.Path.Combine(@"V:\EnglishPaperPresentation\", type + "Center" + this.FileNameTextbox.GetLineText(0) + ".dat") : System.IO.Path.Combine(@"V:\KinectIR\capturedData\", StartedTime + "IRcenter.dat");
            string framesizedataFile = System.IO.Path.Combine(@"V:\EnglishPaperPresentation\", "sizeofframe" + this.FileNameTextbox.GetLineText(0) + ".dat");

            System.IO.StreamWriter writingSwIR = new System.IO.StreamWriter(filenamePartialIR, false, System.Text.Encoding.GetEncoding("shift_jis"));
            System.IO.StreamWriter writingCenterIR = new System.IO.StreamWriter(filenameCenterIR, false, System.Text.Encoding.GetEncoding("shift_jis"));

            System.IO.StreamWriter FramesizeData = new System.IO.StreamWriter(framesizedataFile, false, System.Text.Encoding.GetEncoding("shift_jis"));


            if (!TimeStampFrag && IsTimestampNeeded)
            {
                writingSwIR.Write("\nwriting start\n" + timestamp.ToString() + "\r\n"); //time stamp writelinedeyokune?
                writingCenterIR.Write("\nwriting start\n" + timestamp.ToString() + "\r\n"); //time stamp
                TimeStampFrag = true;
            }
            for (int i = 0; i < measureArray.Length; i++)
            {
                writingSwIR.Write(measureArray[i] + "\r\n");
            }
            for (int j = 0; j < centerArray.Length; j++)
            {
                writingCenterIR.Write(centerArray[j].ToString() + "\r\n");
            }

            if (IsTimestampNeeded)
            {
                DateTime dtnow = DateTime.Now;
                writingSwIR.Write(dtnow.ToString() + "redord ended");
                writingCenterIR.Write(dtnow.ToString() + "redord ended");
            }
            timestamp = DateTime.Now;
            writingSwIR.Close();
            writingCenterIR.Close();

            FramesizeData.Write(FrameSizePoint.X + "\r\n" + FrameSizePoint.Y + "\r\n" + RECORD_SIZE + "\r\n");
            FramesizeData.Close();


        }

        private unsafe void writeToTextColor(byte[] buffer, int length)
        {
            for (int i = 0; i < length; i++)
            {
                writingColor.Write(buffer[i].ToString() + "\r\n");
            }

        }
        private Point getLockPosition()
        {
            double temp;
            Point LockPosition = new Point();
            Point InvalidNum = new Point();
            InvalidNum.X = 256;
            InvalidNum.Y = 212;

            if (double.TryParse(this.textXlock.Text, out temp))
            {
                LockPosition.X = temp;
            }
            else
            {
                return InvalidNum;
            }

            if (double.TryParse(this.textYlock.Text, out temp))
            {
                LockPosition.Y = temp;
            }
            else
            {
                return InvalidNum;
            }
            if ((0 <= LockPosition.X && LockPosition.X < this.depthFrameDescription.Width) && (0 <= LockPosition.Y && LockPosition.Y < this.depthFrameDescription.Height))
            {
                return LockPosition;
            }
            else
            {
                return InvalidNum;
            }

        }
        private unsafe void writeToArrayRectangle(ushort* ProcessData, Point location)
        {
            int recordPixelX = 201; //水平方向の記録ピクセル数 odd
            int recordPixelY = 201; //垂直方向の記録ピクセル数 odd
            int marginX = 1; // 記録するピクセルの間隔　1=連続
            int marginY = 1; // 記録するピクセルの間隔　1=連続

            FrameSizePoint.X = recordPixelX;
            FrameSizePoint.Y = recordPixelY;

            int x = (int)(recordPixelX / 2);
            int y = (int)(recordPixelY / 2);
            if (!ArrayResized)
            {
                Array.Resize(ref measureDepthArray, RECORD_SIZE * (recordPixelX * recordPixelY));
                Array.Resize(ref centerDepthArray, RECORD_SIZE);
                Array.Resize(ref measureIrArray, RECORD_SIZE * (recordPixelX * recordPixelY));
                Array.Resize(ref centerIrArray, RECORD_SIZE);
            }
            ArrayResized = true;
            int index_value = 0;
            for (int j = -y; j <= y; j++)
            {
                for (int i = -x; i <= x; i++)
                {
                    index_value = (j + y) * recordPixelY + (i + x);
                    measureDepthArray[index_value + writeDownedCounter * recordPixelX * recordPixelY] = shiburinkawaiiyoo(DepthGlobalArray, location.X + i * marginX, location.Y + j * marginY);
                    measureIrArray[index_value + writeDownedCounter * recordPixelX * recordPixelY] = shiburinkawaiiyoo(IrGlobalArray, location.X + i * marginX, location.Y + j * marginY);

                }
            }
            centerDepthArray[writeDownedCounter] = shiburinkawaiiyoo(DepthGlobalArray, location.X, location.Y);
            centerIrArray[writeDownedCounter] = shiburinkawaiiyoo(IrGlobalArray, location.X, location.Y);

            writeDownedCounter++;
            if (writeDownedCounter == centerDepthArray.Length)
            {
                WritingFlag = false;
                writeToText(measureDepthArray, centerDepthArray, "Depth");
                writeToText(measureIrArray, centerIrArray, "Infrared");
                ButtonWriteDown.IsEnabled = true;
            }
        }

        private unsafe void writeToArrayPoint(ushort* ProcessData, Point location)
        {
            int leftX = 0, leftY = 0, rightX = depthFrameDescription.Width - 1, rightY = depthFrameDescription.Height - 1;

            Point R1 = new Point(leftX, leftY);
            Point R2 = new Point(rightX, rightY);

            FrameSizePoint.X = R2.X - R1.X + 1;
            FrameSizePoint.Y = R2.Y - R1.Y + 1;
            int Framesize = (int)(FrameSizePoint.X * FrameSizePoint.Y);
            if (!ArrayResized)
            {
                Array.Resize(ref measureDepthArray, RECORD_SIZE * (int)(FrameSizePoint.X * FrameSizePoint.Y));
                Array.Resize(ref centerDepthArray, RECORD_SIZE);
                Array.Resize(ref measureIrArray, RECORD_SIZE * (int)(FrameSizePoint.X * FrameSizePoint.Y));
                Array.Resize(ref centerIrArray, RECORD_SIZE);
            }

            ArrayResized = true;
            int index_value = 0;
            for (int j = (int)R1.Y; j <= R2.Y; j++)
            {
                for (int i = (int)R1.X; i <= R2.X; i++)
                {
                    measureDepthArray[writeDownedCounter * Framesize + index_value] = shiburinkawaiiyoo(DepthGlobalArray, i, j);
                    index_value++;
                }
            }
            centerDepthArray[writeDownedCounter] = shiburinkawaiiyoo(DepthGlobalArray, location.X, location.Y);

            writeDownedCounter++;
            if (writeDownedCounter == centerDepthArray.Length)
            {
                WritingFlag = false;
                writeToText(measureDepthArray, centerDepthArray, "Depth");
                ButtonWriteDown.IsEnabled = true;
            }
        }

        private unsafe void writeFullFrameToArray(ushort* ProcessData)
        {
            FrameSizePoint.X = depthFrameDescription.Width;
            FrameSizePoint.Y = depthFrameDescription.Height;
            if (!ArrayResized)
            {
                Array.Resize(ref measureDepthArray, RECORD_SIZE * (int)(FrameSizePoint.X * FrameSizePoint.Y));
                Array.Resize(ref centerDepthArray, RECORD_SIZE);

            }

            ArrayResized = true;
            int Framesize = (int)(FrameSizePoint.X * FrameSizePoint.Y);
            
            int index_value = 0;
            for (int j = 0; j < depthFrameDescription.Height ; j++)
            {
                for (int i = 0;i < depthFrameDescription.Width ; i++)
                {
                    measureDepthArray[writeDownedCounter * Framesize + index_value] = shiburinkawaiiyoo(ProcessData, i, j);
                    index_value++;
                }
            }

            

        }
        private void writeColorFrameToFile(byte[] buffer)
        {
            int Height = 1920;
            int Width = 1080;


            for (int i = 0; i < buffer.Length; i++)
            {
                writingColor.Write(buffer[i].ToString() + "\r\n");
            }

            
        }

    }
}

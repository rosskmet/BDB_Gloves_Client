using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Devices.Enumeration;
using TobiasErichsen.teVirtualMIDI;
using NAudio.Midi;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.Util;
using DirectShowLib;

namespace BDBGlovesClient
{
    public partial class BDBGCForm : Form, IDisposable
    {
        #region Objects and Global Variables
        //Objects and Global Variables for Camera System
        private VideoCapture _captureOne;
        private MotionHistory _motionHistoryOne;
        private IBackgroundSubtractor _forgroundDetectorOne = null;
        private VideoCapture _captureTwo;
        private MotionHistory _motionHistoryTwo;
        private IBackgroundSubtractor _forgroundDetectorTwo = null;

        private Mat _segMask = new Mat();
        private Mat _segMaskTwo = new Mat();
        private Mat _forgroundMask = new Mat();
        private Mat _forgroundMaskTwo = new Mat();
        private Mat MotionImageOne;
        private Mat MotionImageTwo;
        Mat imageOne = new Mat();
        Mat imageTwo = new Mat();

        public double MotionCountOne;
        public double MotionCountTwo;

        int camIndexOne;
        int camIndexTwo;

        //Objects and Parallel task creation for BLE Devices
        public static BLEHandlingDiscovery bleDevice;
        public static BLEHandlingDiscovery bleDevice_2;
        public BLEHandlingDiscovery UpdateAllData;
        Task[] devicesTask = new Task[2]
        {
            Task.Factory.StartNew(() => { bleDevice = new BLEHandlingDiscovery("System.ItemNameDisplay:~~\"Papageorgio\""); }),
            Task.Factory.StartNew(() => { bleDevice_2 = new BLEHandlingDiscovery("System.ItemNameDisplay:~~\"BDB\""); })
        };
        #endregion

        #region Form Initialization and Disposal
        //++++++++++++++ Program / Form Initialization and Disposal ++++++++++++++//

        public BDBGCForm()
        {
            InitializeComponent();
        }


        public event EventHandler LoadCompleted;
        protected override void OnLoad(EventArgs eload)
        {
            base.OnLoad(eload);
            this.OnLoadCompleted(EventArgs.Empty);
        }


        protected virtual void OnLoadCompleted(EventArgs eload)
        {   
            //Run After Form has Loaded
            var handler = LoadCompleted;
            if (handler != null)
                handler(this, eload);
        }


        private void BDBGCForm_Load(object sender, EventArgs eload)
        {
            BDBGlovesClient.CreateMidiPort(); //Open "BDB_MIDI" port

            //If BLEHandlingDiscovery Class has been Instantiated, Look for Devices
            if(bleDevice != null)
            {
                try { bleDevice.StartBLEDeviceWatcher(); }
                catch { MessageBox.Show("Glove One is Disconnected!"); }
            }

            if(bleDevice_2 != null)
            {
                try { bleDevice_2.StartBLEDeviceWatcher(); }
                catch { MessageBox.Show("Glove Two Is Disconnected!"); }
            }

            Debug.WriteLine("Form Load");

            //Fill Camera Combo Boxes with Camera Index, Disable Start Buttons
            FillCaptureComboBoxes();

            captureOneStartButton.Enabled = false;
            captureTwoStartButton.Enabled = false;
        }


        private void BDBGCForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }


        private void BDBGCForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Dispose of Camera Feeds When Main Form Closes
            if (_captureOne != null) { _captureOne.Dispose(); }
            if (_captureTwo != null) { _captureTwo.Dispose(); }
        }
        #endregion

        #region Function For Printing IMU Data in Real Time
        //++++++++++++++ IMU Data Display Handling ++++++++++++++//

        //Background Worker to allow data to be printed while the stop button is enabled
        private BackgroundWorker _StopButtonBackgroundWorker = null;
        private void UpdateIMUData()
        {
              _StopButtonBackgroundWorker = new BackgroundWorker();
              _StopButtonBackgroundWorker.WorkerSupportsCancellation = true;

              _StopButtonBackgroundWorker.DoWork += new DoWorkEventHandler(async (state, args) =>
              {
                  do
                  {   //Stop Printing When Stop Button is Pressed
                      if (_StopButtonBackgroundWorker.CancellationPending)
                        break;

                      bleDevice.UpdateAllData(); //Grab IMU Data
                      bleDevice_2.UpdateAllData(); //Grab IMU Data

                      Debug.WriteLine("DEVICE 1:");
                      Debug.WriteLine($"Acc X: {bleDevice.accAngleX}");
                      Debug.WriteLine($"Gyro Y: {bleDevice.gyroAngleY}");

                      Debug.WriteLine("DEVICE 2:");
                      Debug.WriteLine($"Acc X_2: {bleDevice_2.accAngleX}");
                      Debug.WriteLine($"Gyro Y_2: {bleDevice_2.gyroAngleY}");

                    /* await UseDelay(1);*/ //Delay to improve redability of output

                } while (true);
            });

            _StopButtonBackgroundWorker.RunWorkerAsync(); //print data in parallel to other tasks
            printDataButton.Enabled = false; //Disable print button while data is printing
            StopPrintingButton.Enabled = true; //Enable stop button while data is printing
        }


        async Task UseDelay(int delay)
        {
            await Task.Delay(delay); // In milliseconds
        }
        #endregion

        #region Function for MIDI Handling and Event Triggering
        //++++++++++++++ MIDI Handling / Event Triggering ++++++++++++++//

        //Background worker which allows triggering logic to send MIDI data while box is checked
        private BackgroundWorker _MidiTriggeringBackgroundWorker = null;
        public void MIDINoteTriggering() 
        {
            int note = 60; //MIDI Note C5
            int velocity_1 = 127; //Max Velocity
            int velocity_2 = 127;
            int command = 0x90; //Send Note over BDB_MIDI Channel 1
            int command_2 = 0x91; //Send Note over BDB_MIDI Channel 2
            int curstate = 0; //Marks when hand is rising
            int curstate_2 = 0;

            float accX;
            float gyroY;
            float accX_2;
            float gyroY_2;

            _MidiTriggeringBackgroundWorker = new BackgroundWorker();
            _MidiTriggeringBackgroundWorker.WorkerSupportsCancellation = true;
            _MidiTriggeringBackgroundWorker.RunWorkerAsync();

            _MidiTriggeringBackgroundWorker.DoWork += new DoWorkEventHandler(async (state, args) =>
            {
                do
                {
                    if (_MidiTriggeringBackgroundWorker.CancellationPending)
                        break;

                    while (StartIMUCheckBox.Checked)
                    {
                        bleDevice.UpdateAllData(); //Grab IMU Data
                        bleDevice_2.UpdateAllData(); //Grab IMU Data

                        accX = bleDevice.accAngleX;
                        gyroY = bleDevice.gyroAngleY;

                        //Use Angular Velocity to determine dynamic quality (scaled between 0 and 127)
                        velocity_1 = (int)((Math.Abs(gyroY) / 2.0) * 127); 
                        Debug.WriteLine($"Velocity 1: {velocity_1}");

                        //Note Triggered if enough motion is detected and the hand is accelerating downard
                        if (MotionCountOne > 3) //Camera one represents sampler channel one
                        {
                            if ((accX < 0.1) && (curstate == 0))
                            {
                                if (curstate == 0)
                                {   //Send MIDI Note
                                    BDBGlovesClient.SendNextMidiOutMessage(command, note, 127);
                                    Debug.WriteLine($"Acc X: {bleDevice.accAngleX}");
                                    curstate = 1;
                                }
                            }
                        }
                        if ((bleDevice.accAngleX > 0.1) && (curstate == 1))
                        {
                            curstate = 0; //reset when hand is rising
                        }

                        accX_2 = bleDevice_2.accAngleX;
                        gyroY_2 = bleDevice_2.gyroAngleY;

                        velocity_2 = (int)((Math.Abs(gyroY_2) / 2.0) * 127);
                        Debug.WriteLine($"Velocity 2: {velocity_2}");
                        if (MotionCountTwo > 3) 
                        {
                            if ((accX_2 < 0.1) && (curstate_2 == 0)) 
                            {
                                if (curstate_2 == 0)
                                {
                                    BDBGlovesClient.SendNextMidiOutMessage(command_2, note, 127);
                                    Debug.WriteLine($"Acc X_2: {bleDevice_2.accAngleX}");
                                    curstate_2 = 1;
                                }
                            }
                         
                        }
                        if ((bleDevice_2.accAngleX > 0.1) && (curstate_2 == 1))
                        {
                            curstate_2 = 0;
                        }
                    }
                } while (true);
            });
        }
        #endregion

        #region Camera Feed Processing and Motion Detection for Two Cameras
        //++++++++++++++ Camera Feed Processing / Motion Detection ++++++++++++++//

        public void ProcessFrame(object sender, EventArgs e)
        {
            //Separate Background and Foreground
            if (_forgroundDetectorOne == null)
            {
                _forgroundDetectorOne = new BackgroundSubtractorMOG2();
            }

            _captureOne.Retrieve(imageOne); //Apply camera one feed to object

            _forgroundDetectorOne.Apply(imageOne, _forgroundMask); //detect foreground (object in motion)
            _motionHistoryOne.Update(_forgroundMask, DateTime.Now); //update motion history

            #region get a copy of the motion mask and enhance its color
            double[] minValues, maxValues; //determines minimum rectangle size for motion sectors
            Point[] minLoc, maxLoc;
            _motionHistoryOne.Mask.MinMax(out minValues, out maxValues, out minLoc, out maxLoc);
            Mat motionMask = new Mat();
            using (ScalarArray sa = new ScalarArray(255.0 / maxValues[0]))
                CvInvoke.Multiply(_motionHistoryOne.Mask, sa, motionMask, 1, DepthType.Cv8U);
            //Image<Gray, Byte> motionMask = _motionHistory.Mask.Mul(255.0 / maxValues[0]);
            #endregion

            //create the motion image 
            Mat motionImageOne = new Mat(motionMask.Size.Height, motionMask.Size.Width, DepthType.Cv8U, 3);
            MotionImageOne = motionImageOne;
            motionImageOne.SetTo(new MCvScalar(0));
            //display the motion pixels in blue (first channel)
            //motionImage[0] = motionMask;
            CvInvoke.InsertChannel(motionMask, motionImageOne, 0);

            //Threshold to define a motion area, reduce the value to detect smaller motion
            double minArea = 5000; //Lower = More Sensitive Motion Detection

            Rectangle[] rects;
            using (VectorOfRect boundingRect = new VectorOfRect())
            {
                _motionHistoryOne.GetMotionComponents(_segMask, boundingRect);
                rects = boundingRect.ToArray();
            }

            //iterate through each of the motion component
            foreach (Rectangle comp in rects)
            {
                int area = comp.Width * comp.Height;
                //reject the components that have small area;
                if (area < minArea) continue;

                // find the angle and motion pixel count of the specific area
                double angle, motionPixelCount;
                _motionHistoryOne.MotionInfo(_forgroundMask, comp, out angle, out motionPixelCount);

                //reject the area that contains too few motion
                if (motionPixelCount < area * 0.0005) continue; //originally (motionPixelCount < area * 0.05)

                //Draw each individual motion in red
                DrawMotion(motionImageOne, comp, angle, new Bgr(Color.Red));
            }

            // find and draw the overall motion angle
            double overallAngle, overallMotionPixelCount;

            _motionHistoryOne.MotionInfo(_forgroundMask, new Rectangle(Point.Empty, motionMask.Size), out overallAngle, out overallMotionPixelCount);
            DrawMotion(motionImageOne, new Rectangle(Point.Empty, motionMask.Size), overallAngle, new Bgr(Color.Green));

            if (cameraOneCheckBox.Checked == true)
            {
                cameraOneImageBox.Image = MotionImageOne;
            }
            else
            {
                cameraOneImageBox.Image = imageOne;
            }

            //Display the amount of motions found on the current image
            //UpdateText(String.Format("Total Motions found: {0}; Motion Pixel count: {1}", rects.Length, overallMotionPixelCount));
            UpdateText(String.Format("Total Motions found: {0}; Motion Pixel count: {1}", ((rects.Length * 0.021) * 1.5), overallMotionPixelCount));

            MotionCountOne = (rects.Length * 0.021) * 1.5;
        }

        public void ProcessFrameTwo(object sender, EventArgs e)
        {
            if (_forgroundDetectorTwo == null)
            {
                _forgroundDetectorTwo = new BackgroundSubtractorMOG2();
            }

            _captureTwo.Retrieve(imageTwo);

            _forgroundDetectorTwo.Apply(imageTwo, _forgroundMaskTwo);
            _motionHistoryTwo.Update(_forgroundMaskTwo, DateTime.Now);
            //if (dims <= 2 && step[0] > 0) { _motionHistoryTwo.Update(_forgroundMaskTwo) };


            #region get a copy of the motion mask and enhance its color
            double[] minValues_2, maxValues_2;
            Point[] minLoc_2, maxLoc_2;
            _motionHistoryTwo.Mask.MinMax(out minValues_2, out maxValues_2, out minLoc_2, out maxLoc_2); ;
            Mat motionMask_2 = new Mat();
            using (ScalarArray sa2 = new ScalarArray(255.0 / maxValues_2[0]))
                CvInvoke.Multiply(_motionHistoryTwo.Mask, sa2, motionMask_2, 1, DepthType.Cv8U);
            //Image<Gray, Byte> motionMask = _motionHistory.Mask.Mul(255.0 / maxValues[0]);
            #endregion

            Mat motionImageTwo = new Mat(motionMask_2.Size.Height, motionMask_2.Size.Width, DepthType.Cv8U, 3);
            MotionImageTwo = motionImageTwo;
            motionImageTwo.SetTo(new MCvScalar(0));
            CvInvoke.InsertChannel(motionMask_2, motionImageTwo, 0);

            //Threshold to define a motion area, reduce the value to detect smaller motion
            double minArea_2 = 5000;

            Rectangle[] rects_2;
            using (VectorOfRect boundingRect_2 = new VectorOfRect())
            {
                _motionHistoryTwo.GetMotionComponents(_segMaskTwo, boundingRect_2);
                rects_2 = boundingRect_2.ToArray();
            }

            foreach (Rectangle comp_2 in rects_2)
            {
                int area_2 = comp_2.Width * comp_2.Height;
                //reject the components that have small area;
                if (area_2 < minArea_2) continue;

                // find the angle and motion pixel count of the specific area
                double angle_2, motionPixelCount_2;
                _motionHistoryTwo.MotionInfo(_forgroundMaskTwo, comp_2, out angle_2, out motionPixelCount_2);

                //reject the area that contains too few motion
                if (motionPixelCount_2 < area_2 * 0.0005) continue;

                //Draw each individual motion in red
                DrawMotion(motionImageTwo, comp_2, angle_2, new Bgr(Color.Red));
            }

            double overallAngle_2, overallMotionPixelCount_2;

            _motionHistoryTwo.MotionInfo(_forgroundMaskTwo, new Rectangle(Point.Empty, motionMask_2.Size), out overallAngle_2, out overallMotionPixelCount_2);
            DrawMotion(motionImageTwo, new Rectangle(Point.Empty, motionMask_2.Size), overallAngle_2, new Bgr(Color.Green));

            if (this.Disposing || this.IsDisposed)
                return;

            if (cameraTwoCheckBox.Checked == true)
            {
                cameraTwoImageBox.Image = MotionImageTwo;
            }
            else
            {
                cameraTwoImageBox.Image = imageTwo;
            }

            //Display the amount of motions found on the current image
            UpdateTextTwo(String.Format("Total Motions found: {0}; Motion Pixel count: {1}", ((rects_2.Length * 0.021) * 1.5), overallMotionPixelCount_2));

            MotionCountTwo = (rects_2.Length * 0.021) * 1.5;
        }

        private void UpdateText(String text)
        {
            if (!IsDisposed && !Disposing && InvokeRequired)
            {
                Invoke((Action<String>)UpdateText, text);
            }
            else
            {
                label3.Text = text;
            }
        }

        private void UpdateTextTwo(String text)
        {
            if (!IsDisposed && !Disposing && InvokeRequired)
            {
                Invoke((Action<String>)UpdateTextTwo, text);
            }
            else
            {
                captureTwoMotionLabel.Text = text;
            }
        }

        private static void DrawMotion(IInputOutputArray image, Rectangle motionRegion, double angle, Bgr color)
        {
            CvInvoke.Rectangle(image, motionRegion, new MCvScalar(255, 255, 0)); //Draw rectangle around motion sectors
            float circleRadius = (motionRegion.Width + motionRegion.Height) >> 2;
            Point center = new Point(motionRegion.X + (motionRegion.Width >> 1), motionRegion.Y + (motionRegion.Height >> 1));

            CircleF circle = new CircleF(
               center,
               circleRadius);

            int xDirection = (int)(Math.Cos(angle * (Math.PI / 180.0)) * circleRadius);
            int yDirection = (int)(Math.Sin(angle * (Math.PI / 180.0)) * circleRadius);
            Point pointOnCircle = new Point(
                center.X + xDirection,
                center.Y - yDirection);
            LineSegment2D line = new LineSegment2D(center, pointOnCircle);
            CvInvoke.Circle(image, Point.Round(circle.Center), (int)circle.Radius, color.MCvScalar);
            CvInvoke.Line(image, line.P1, line.P2, color.MCvScalar);

        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }
        #endregion

        #region Event Handlers for GUI Compoents
        //++++++++++++++ GUI Component Event Handlers ++++++++++++++//

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void cameraOneCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cameraTwoCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void printDataButton_Click(object sender, EventArgs e)
        {
            bleDevice.UpdateAllData();
            bleDevice_2.UpdateAllData();

            UpdateIMUData();
        }

        private void StopPrintingButton_Click(object sender, EventArgs e)
        {
            StopPrintingButton.Enabled = false;
            printDataButton.Enabled = true;
            _StopButtonBackgroundWorker.CancelAsync();
        }

        private void sendMidiNoteButton_Click(object sender, EventArgs e)
        {
            BDBGlovesClient.SendNextMidiOutMessage(0x91, 60, 127);
        }

        private void MidiNoteValueBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void MIDIVelocityValueBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void StartIMUCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (StartIMUCheckBox.Checked)
            {
                MIDINoteTriggering();
            }
        }

        
        private void FillCaptureComboBoxes()
        {
            int count = 0;

            while (count < 3)
            {
                captureOneComboBox.Items.Add(count);
                captureTwoComboBox.Items.Add(count);
                count++;
            }
        }

        private void captureOneComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            camIndexOne = captureOneComboBox.SelectedIndex;

            //try to create the capture
            if (_captureOne == null)
            {
                try
                {
                    _captureOne = new VideoCapture(camIndexOne); //Create Instance of First Camera
                }
                catch (NullReferenceException excpt)
                {   //show errors if there is any
                    MessageBox.Show(excpt.Message);
                }
            }

            if (_captureOne != null) //if camera capture has been successfully created
            {
                _motionHistoryOne = new MotionHistory(
                    1.0, //in second, the duration of motion history you wants to keep
                    0.05, //in second, maxDelta for cvCalcMotionGradient
                    0.5); //in second, minDelta for cvCalcMotionGradient

                captureOneStartButton.Enabled = true;
            }

            captureOneComboBox.Enabled = false;
        }

        private void captureTwoComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            camIndexTwo = captureTwoComboBox.SelectedIndex;

            //try to create the capture
            if (_captureTwo == null)
            {
                try
                {
                    _captureTwo = new VideoCapture(camIndexTwo); //Create Instance of First Camera
                }
                catch (NullReferenceException excpt)
                {   //show errors if there is any
                    MessageBox.Show(excpt.Message);
                }
            }

            if (_captureTwo != null) //if camera capture has been successfully created
            {
                _motionHistoryTwo = new MotionHistory(
                    1.0, //in second, the duration of motion history you wants to keep
                    0.05, //in second, maxDelta for cvCalcMotionGradient
                    0.5); //in second, minDelta for cvCalcMotionGradient

                captureTwoStartButton.Enabled = true;
            }

            captureTwoComboBox.Enabled = false;
        }

        private void captureOneStartButton_Click(object sender, EventArgs e)
        {
            if (_captureOne != null)
            {
                if (captureOneStartButton.Text == "Pause")
                {
                    _captureOne.ImageGrabbed -= ProcessFrame;
                    _captureOne.Stop();
                    //if camera is getting frames then pause the capture and set button Text to
                    // "Resume" for resuming capture
                    captureOneStartButton.Text = "Resume"; 
                }
                else
                {
                    _captureOne.ImageGrabbed += ProcessFrame;
                    _captureOne.Start(); 
                    //if camera is NOT getting frames then start the capture and set button
                    // Text to "Pause" for pausing capture
                    captureOneStartButton.Text = "Pause";
                }

            }
        }

        private void captureTwoStartButton_Click(object sender, EventArgs e)
        {
            if (_captureTwo != null)
            {
                if (captureTwoStartButton.Text == "Pause")
                {
                    _captureTwo.ImageGrabbed -= ProcessFrame;
                    _captureTwo.Stop();
                    //if camera is getting frames then pause the capture and set button Text to
                    // "Resume" for resuming capture
                    captureTwoStartButton.Text = "Resume"; 
                }
                else
                {
                    _captureTwo.ImageGrabbed += ProcessFrameTwo;
                    _captureTwo.Start();
                    //if camera is NOT getting frames then start the capture and set button
                    // Text to "Pause" for pausing capture
                    captureTwoStartButton.Text = "Pause";
                }

            }
        }
        #endregion
    }
}
       


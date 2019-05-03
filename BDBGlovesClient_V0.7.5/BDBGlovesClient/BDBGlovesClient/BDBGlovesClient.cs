using System;
using System.Threading;
using System.Windows.Forms;
using TobiasErichsen.teVirtualMIDI;
using NAudio.Midi;

namespace BDBGlovesClient
{
    public class BDBGlovesClient 
    {
        public static TeVirtualMIDI port;
        static BDBGCForm BDBGCForm;
        static SplashForm SplashForm;

        //Main Entry Point for the Application
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new BDBGCForm());

            //SlashForm created for loading screen
            SplashForm = new SplashForm();
            if (SplashForm != null)
            {
                //Thread to run loading screen
                Thread splashThread = new Thread(new ThreadStart(
                    () => { Application.Run(SplashForm); }));
                splashThread.SetApartmentState(ApartmentState.STA);
                splashThread.Start();
            }
            //Create and Show Main Form
            BDBGCForm = new BDBGCForm();
            BDBGCForm.LoadCompleted += BDBGCForm_LoadCompleted;
            Application.Run(BDBGCForm);
            if (!(SplashForm == null || SplashForm.Disposing || SplashForm.IsDisposed))
                SplashForm.Invoke(new Action(() => {
                    SplashForm.TopMost = true;
                    SplashForm.Activate();
                    SplashForm.TopMost = false;
                }));

            

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
        }

        private static void BDBGCForm_LoadCompleted(object sender, EventArgs e)
        {
            //Dispose of loading screen form when main form is finished loading
            if (SplashForm == null || SplashForm.Disposing || SplashForm.IsDisposed)
                return;
            SplashForm.Invoke(new Action(() => { SplashForm.Close(); }));
            SplashForm.Dispose();
            SplashForm = null;
            BDBGCForm.TopMost = true;
            BDBGCForm.Activate();
            BDBGCForm.TopMost = false;
        }

        #region Create and Open MIDI Port, Enumerate Devices, Destroy MIDI Port
        public static void CreateMidiPort()
        {
            //Writing to Log File
            TeVirtualMIDI.logging(TeVirtualMIDI.TE_VM_LOGGING_MISC | 
                TeVirtualMIDI.TE_VM_LOGGING_RX | TeVirtualMIDI.TE_VM_LOGGING_TX);

            Guid manufacturer = new Guid("aa4e075f-3504-4aab-9b06-9a4104a91cf0");
            Guid product = new Guid("bb4e075f-3504-4aab-9b06-9a4104a91cf0");

            //Create MIDI Port
            string portName = "BDB_MIDI";
            port = new TeVirtualMIDI(portName);

            //Create Thread for MIDI Events
            Thread thread = new Thread(new ThreadStart(SendNextMidiOutMessage));
            thread.Start();
        }


        public static void DestroyMidiPort()
        {
            port.shutdown();
        }


        static void OnProcessExit(object sender, EventArgs e)
        {
            DestroyMidiPort();
        }


        public static string[] EnumerateMidiOutDevices()
        {
            string[] outDevices = new string[MidiOut.NumberOfDevices];

            for (int device = 0; device < MidiOut.NumberOfDevices; device++)
            {
                outDevices[device] = MidiOut.DeviceInfo(device).ProductName;
            }

            int noOutDevices = MidiOut.NumberOfDevices;

            string deviceOutOne = MidiOut.DeviceInfo(1).ProductName;

            return outDevices;
        }


        public static void SendNextMidiOutMessage()
        {
            //Overloaded Method Used to Start Thread
        }
        #endregion 

        #region Start and Stop Midi Note messages
        public static void SendNextMidiOutMessage(int message, int note, int velocity)
        {
            //byte[] onCommand = { 0x90, 0x3C, 0x7F }; //Standard Values for Midi On, C5, and 127 Velocity
            byte[] onCommand = { (byte)message, (byte)note, (byte)velocity };

            port.sendCommand(onCommand);
        }


        public static void StopNextMidiOutMessage(int message, int note, int velocity)
        {
            //byte[] offCommand = { 0x80, 0x3C, 0x00 }; //Standard Values for Midi On, C5, and 0 Velocity
            byte[] offCommand = { (byte)message, (byte)note, (byte)velocity };

            port.sendCommand(offCommand);
        }
        #endregion
    }
}

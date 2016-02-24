using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skene;
using PhysicalSpaceManager;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Windows.Forms;
using EmoteCommonMessages;
using System.Diagnostics;

namespace Skene
{
    class MainWindowController
    {
        public event EventHandler SetupListChanged;
        public event EventHandler ActiveSetupChanged;

        static private string SETUPS_PATH = "./setups/";
        static private string SETUP_EXTENSION = ".spacesetup";
        
        MainWindow _window;
        SkeneClient _client;
        public SkeneClient Client
        {
            get { return _client; }
        }

        List<PhysicalSpace> _setups = new List<PhysicalSpace>();
        public List<PhysicalSpace> Setups { 
            get {
                return _setups;
            }
        }
        Stopwatch refreshStopwatch;

        public MainWindowController(MainWindow window, string characterName = "")
        {
            _window = window;
            refreshStopwatch = new Stopwatch();
            refreshStopwatch.Start();
            //if (_client != null && _client.IsConnected) _client.Dispose();
            _client = SkeneClient.GetInstance(characterName);
            _client.ClientConnected += delegate() { _window.StatusMessage("connected", System.Drawing.Color.Green); };
            _client.ClientDisconnected += delegate { _window.StatusMessage("NOT connected", System.Drawing.Color.Red); };
            _window.FormClosed += delegate (object sender, System.Windows.Forms.FormClosedEventArgs e){
                _client.Dispose();
            };
            _client.ClickPointChanged += (SkeneClient.ClickPointChangedHandler)((point) => _window.Invoke((MethodInvoker)(()=>_window.SetTrackingClickPoint(point))));
            _client.PersonAngleChanged += (SkeneClient.PersonAngleChangedHandler)((userId, angle) =>
            {
                if (refreshStopwatch.ElapsedMilliseconds > 1000)
                {
                    _window.Invoke((MethodInvoker)(() => _window.SetTrackingPersonAngle(userId, angle)));
                    refreshStopwatch.Restart();
                }
            });
            _client.ScreenPointChanged += (SkeneClient.ScreenPointChangedHandler)((point) => _window.Invoke((MethodInvoker)(() => _window.SetTrackingScreenAngle(point))));
        }

        public void Save(PhysicalSpace spaceSetup){
            CheckSetupsDirectoryExists();
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(SETUPS_PATH+spaceSetup._name + SETUP_EXTENSION, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, spaceSetup);
            stream.Close();
            LoadSetups();
        }

        public void Delete(PhysicalSpace spaceSetup)
        {
            CheckSetupsDirectoryExists();
            if (spaceSetup != null)
            {
                string filename = SETUPS_PATH+spaceSetup._name + SETUP_EXTENSION;
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }
            }
            LoadSetups();
        }

        public void LoadSetups()
        {
            _setups.Clear();
            if (!Directory.Exists(SETUPS_PATH))
            {
                Directory.CreateDirectory(SETUPS_PATH);
            }
            foreach (string f in Directory.EnumerateFiles(SETUPS_PATH))
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(f, FileMode.Open, FileAccess.Read, FileShare.Read);
                PhysicalSpace s = (PhysicalSpace)formatter.Deserialize(stream);
                _setups.Add(s);
                stream.Close();
            }
            if (SetupListChanged != null)
            {
                SetupListChanged(this,new EventArgs());
            }
        }

        public void ActivateSetup(PhysicalSpace physicalSpaceSetup)
        {
            _client.ActiveSpaceSetup = physicalSpaceSetup;
            if (ActiveSetupChanged != null)
            {
                ActiveSetupChanged(this,new EventArgs());
            }
        }

        private static void CheckSetupsDirectoryExists()
        {
            if (!Directory.Exists(SETUPS_PATH))
            {
                Directory.CreateDirectory(SETUPS_PATH);
            }
        }

        public PhysicalSpace GetActiveSetup()
        {
            return _client.ActiveSpaceSetup;
        }


        public void SwitchGazeTarget(string state)
        {
            GazeTarget newState = GazeTarget.Random;
            if (Enum.TryParse<GazeTarget>(state, out newState))
            {
                _client.GazeManager.SwitchGazeTarget(newState);
            }
        }


        internal void SetYTrackingCompensation(double angle)
        {
            _client.YTrackingCompensation = angle;
        }

        internal void SetZTrackingCompensation(double z)
        {
            _client.ZTrackingCompensation = z;
        }
    
        internal void Dispose()
        {
 	        _client.Dispose();
        }
    }
}

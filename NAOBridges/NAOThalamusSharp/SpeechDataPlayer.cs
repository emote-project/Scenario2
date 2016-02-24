using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpeechSoundDataStructures;
using System.Threading;

namespace NAOThalamus
{
    public class SpeechDataPlayer
    {
        public class SpeechCharacteristicReachedEventArgs : EventArgs
        {
            public SpeechData.SpeechCharacteristic characteristic;
            public SpeechCharacteristicReachedEventArgs(SpeechData.SpeechCharacteristic characteristic)
            {
                this.characteristic = characteristic;
            }
        }
        public class SpeechDataPlayerEventArgs : EventArgs
        {
            public SpeechData speechData;
            public SpeechDataPlayerEventArgs(SpeechData data)
            {
                this.speechData = data;
            }
        }

        public event EventHandler<SpeechDataPlayerEventArgs> Finished;
        public event EventHandler<SpeechDataPlayerEventArgs> Started;
        public event EventHandler<SpeechCharacteristicReachedEventArgs> CharacteristicReached;

        SpeechData _data;

        //used to check when to send a "Bookmark Reached" event
        DateTime _timeStarted;
        List<SpeechData.SpeechCharacteristic> _characteristicsBeingChecked;
        Thread _checkingCharacteristicsThread;

        public SpeechDataPlayer(SpeechData data)
        {
            _data = data;
        }

        public SpeechDataPlayer(string serialized_data)
        {
            _data = SpeechData.Deserialize(serialized_data);
        }

        public void Play()
        {
            string filename = @"temp.wav";
            string path = System.IO.Path.GetFullPath(@"./") + filename;
            AudioUtilities.Serializer.DeserializeToFile(_data.dataString, path);
            Console.WriteLine("Should play the file now");

            WavePlayer wp = new WavePlayer(path);
            wp.play += delegate(Object o, EventArgs args)
            {
                Console.WriteLine("Starting playing sound");
                if (Started != null) Started(this, new SpeechDataPlayerEventArgs(_data));
                StartCheckForCharacteristics();
            };
            wp.stopped += delegate(Object o, EventArgs args)
            {
                if (Finished != null) Finished(this, new SpeechDataPlayerEventArgs(_data));
                _checkingCharacteristicsThread.Abort();
                Console.WriteLine("Play ended");
            };
            wp.Play();
        }

        private void CheckForCharacteristics()
        {
            while (_characteristicsBeingChecked != null && _characteristicsBeingChecked.Count > 0)
            {
                List<SpeechData.SpeechCharacteristic> charToFire = (from c in _characteristicsBeingChecked where c.time <= DateTime.Now.Subtract(_timeStarted).TotalMilliseconds select c).ToList<SpeechData.SpeechCharacteristic>();
                foreach (SpeechData.SpeechCharacteristic c in charToFire)
                {
                    _characteristicsBeingChecked.Remove(c);
                    if (CharacteristicReached != null) CharacteristicReached(this, new SpeechCharacteristicReachedEventArgs(c));
                }
            }
        }

        private void StartCheckForCharacteristics()
        {
            _characteristicsBeingChecked = new List<SpeechData.SpeechCharacteristic>();
            foreach (var b in _data.bookmarks)
                _characteristicsBeingChecked.Add((SpeechData.SpeechCharacteristic)b);
            foreach (var v in _data.visemes)
                _characteristicsBeingChecked.Add((SpeechData.SpeechCharacteristic)v);

            _timeStarted = DateTime.Now;
            _checkingCharacteristicsThread = new Thread(new ThreadStart(CheckForCharacteristics));
            _checkingCharacteristicsThread.Start();
        }

    }

    internal class WavePlayer
    {
        public event EventHandler stopped;
        public event EventHandler play;
        string _path;

        public WavePlayer(string path)
        {
            _path = path;
        }

        public void Play()
        {
            Thread t = new Thread(new ThreadStart(PlayTask));
            t.Start();
        }

        private void PlayTask()
        {
#if DEBUG
            using (System.Media.SoundPlayer simpleSound = new System.Media.SoundPlayer(_path))
            {
                if (play != null) play(this, null);
                simpleSound.PlaySync();
                if (stopped != null) stopped(this, null);
            }
#else
            if (play != null) play(this, null);
            PlayCommand("play",_path);
            if (stopped != null) stopped(this, null);
#endif 
        }

        private string PlayCommand(string command, string arg)
        {
            if (Thalamus.ThalamusClient.IsLinux)
            {
                string retMessage = String.Empty;
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                System.Diagnostics.Process p = new System.Diagnostics.Process();

                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardInput = true;

                startInfo.UseShellExecute = false;
                startInfo.Arguments = arg;
                startInfo.FileName = command;

                p.StartInfo = startInfo;
                p.Start();

                p.WaitForExit();
                string s = p.StandardOutput.ReadToEnd();
                Console.WriteLine(s);
                return s;
            }
            else
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(@arg);
                player.PlaySync();
                return "";
            }
        }
    }
}

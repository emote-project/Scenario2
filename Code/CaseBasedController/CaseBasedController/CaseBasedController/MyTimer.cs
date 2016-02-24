using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CaseBasedController
{
    /// <summary>
    /// This is a Timer class to be used during the simulation. 
    /// This class uses the simulated log time-stamp to simulate the current time during the simulation.
    /// </summary>
    public static class MyTimer
    {
        public delegate void TimeElapsedHandler();
        public class TimerData{
            public TimeElapsedHandler Handler;
            public double Delay;
            public double CreationTime;
            public bool Repeating = false;
        }
        static List<TimerData> _timers = new List<TimerData>();

        static double _seconds = 0;
        static Timer _realTimeTimer;

        static bool _usingRealTime = true;

        static object locker = new object();

        /// <summary>
        /// This funcion needs to be called from the simulation controller, to constantly set the current time of the simulation
        /// </summary>
        /// <param name="currentTime">current time of the simulation in seconds</param>
        public static void SetCurrentTime(double currentTime)
        {
            if (!_usingRealTime)
            {
                SetCurrentTime_(currentTime);
            }else 
            {
                throw new Exception("Can't set current time if not in Simulation Mode");
            }
        }

        private static void SetCurrentTime_(double currentTime)
        {
            lock (locker)
            {
                _seconds = currentTime;
                var toFire = _timers.Where(t => _seconds - t.CreationTime >= t.Delay).ToList<TimerData>();
                foreach (TimerData timer in toFire)
                {
                    if (timer.Handler != null)
                    {
                        timer.Handler();
                        if (!timer.Repeating)
                            _timers.Remove(timer);
                        else
                            timer.CreationTime = _seconds;
                    }
                    else
                    {
                        _timers.Remove(timer);
                    }
                }
            }
        }

        /// <summary>
        /// Reset the timer to a clean state
        /// </summary>
        /// <param name="startTime">The starting time (usually equal to the time of the first event on the log) in seconds</param>
        public static void Initialize(double startTime)
        {
            lock (locker)
            {
                _seconds = startTime;
                _timers.Clear();
            }
        }
        
        public static void UseWithSimulationTime()
        {
            lock (locker)
            {
                _usingRealTime = false;
                if (_realTimeTimer != null)
                {
                    _realTimeTimer.Elapsed -= _realTimeTimer_Elapsed;
                    _realTimeTimer.Dispose();
                }
                foreach (TimerData timer in _timers)
                {
                    timer.CreationTime = 0;
                }
                _seconds = -999;
            }
        }

        public static void UseWithRealTime()
        {
            lock (locker)
            {
                _usingRealTime = true;
                if (_realTimeTimer != null)
                {
                    _realTimeTimer.Elapsed -= _realTimeTimer_Elapsed;
                    _realTimeTimer.Dispose();
                }
                foreach (TimerData timer in _timers)
                {
                    timer.CreationTime = 0;
                }
                _seconds = -999;
                _realTimeTimer = new Timer(100);
                _realTimeTimer.Enabled = true;
                _realTimeTimer.AutoReset = true;
                _realTimeTimer.Elapsed += _realTimeTimer_Elapsed;
            }
        }

        static void _realTimeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            double seconds = (double)(DateTime.Now.Ticks / (decimal)TimeSpan.TicksPerSecond);
            SetCurrentTime_(seconds);
        }

        public static TimeSpan GetCurrentTime()
        {
            int hour = (int)(_seconds / 3600);
            int minutes = (int)(_seconds % 3600) / 60;
            int seconds = (int)(_seconds % 60);
            return new TimeSpan(hour,minutes,seconds);
        }

        /// <summary>
        /// Create a new timer firing after x seconds
        /// </summary>
        /// <param name="delay"><b>seconds</b> to wait before firing</param>
        /// <param name="handler">the handler called when the timer fires</param>
        public static TimerData RegisterTimer(double delay, TimeElapsedHandler handler)
        {
            lock (locker)
            {
                var timer = new TimerData() { Delay = delay, Handler = handler, CreationTime = _seconds };
                _timers.Add(timer);
                return timer;
            }
        }

        public static void RemoveTimer(TimerData timerData)
        {
            lock (locker)
            {
                _timers.RemoveAll(x => x.Equals(timerData));
            }
        }

        /// <summary>
        /// Create a new timer firing every x seconds
        /// </summary>
        /// <param name="delay">the interval at witch the timer will fire, in <b>seconds</b></param>
        /// <param name="handler">the handler called every time the timer fires</param>
        /// <returns></returns>
        public static TimerData RegisterRepeatingTimer(double delay, TimeElapsedHandler handler)
        {
            lock (locker)
            {
                var timer = new TimerData() { Delay = delay, Handler = handler, CreationTime = _seconds, Repeating = true };
                _timers.Add(timer);
                return timer;
            }
        }

        public static void DeregisterTimer(TimeElapsedHandler handler)
        {
            lock (locker)
            {
                _timers.RemoveAll(timer => timer.Handler == handler);
            }
        }

        public static void Reset()
        {
            lock (locker)
            {
                _seconds = -9999;
                _timers.Clear();
            }
        }
    }
}

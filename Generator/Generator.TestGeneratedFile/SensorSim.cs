using System;
using System.Threading;

namespace TestGeneratedFile
{
    /// <summary>
    /// Simulates a real sensor generating random values at a random time moments
    /// </summary>
    public class SensorSim
    {
        private readonly Timer timer;
        private readonly Random rnd = new Random();

        public event EventHandler<int> Event;
        public int Index { get; set; }

        public SensorSim()
        {
            int period = rnd.Next(0, 10);
            this.timer = new Timer(NewValue, null, period * 1000, period * 1000);
        }

        private void NewValue(object o)
        {
            int period = rnd.Next(0, 10);
            int value = rnd.Next(30);
            var args = new SensorEventArgs
            {
                SensorIndex = this.Index,
                SensorValue = value
            };
            Console.WriteLine();
            Console.WriteLine($"Sensor{this.Index} new value : {value}");

            this.Event?.Invoke(this, value);

            timer.Change(period * 1000, period * 1000);

        }

        public class SensorEventArgs : EventArgs
        {
            public int SensorIndex { get; set; }
            public int SensorValue { get; set; }
        }
    }
}
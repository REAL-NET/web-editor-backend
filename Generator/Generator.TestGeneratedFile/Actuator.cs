using System;

namespace TestGeneratedFile
{
    public class Actuator
    {
        public int Num { get; }
        public int? Value { get; }

        public event EventHandler<int> Event;

        public Actuator(int num, int? value = null)
        {
            this.Num = num;
            this.Value = value;
        }

        public void Action(int value)
        {
            if (this.Value != null)
            {
                Console.WriteLine($"----- Actuator {Num}: value {value}!!! -----");
                //this.Event?.Invoke(this, 1);
            }
            else
            {
                Console.WriteLine($"----- Actuator {Num} turned down!!! -----");
                //this.Event?.Invoke(this, 0);
            }
        }
    }
}
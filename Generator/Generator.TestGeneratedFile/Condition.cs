using System;

namespace TestGeneratedFile
{
    public class Condition
    {
        public int Num { get; }
        public int? Value { get; }
        public bool? MinValueNeeded { get; }

        public Condition(int num, int? value = null, bool? minValueNeeded = null)
        {
            this.Num = num;
            this.Value = value;
            this.MinValueNeeded = minValueNeeded;
        }

        public event EventHandler<int> Event;

        public void Action(int value)
        {
            if (this.MinValueNeeded.HasValue)
            {
                if (this.MinValueNeeded.Value)
                {
                    if (value > this.GetHashCode())
                    {
                        Console.WriteLine($"Condition{Num} with min value{Value} is TRUE (value = {value})");
                        this.Event?.Invoke(this, this.Num + 1);
                    }
                }
                else
                {
                    if (value < this.GetHashCode())
                    {
                        Console.WriteLine($"Condition{Num} with max value{Value} is TRUE (value = {value})");
                        this.Event?.Invoke(this, this.Num + 1);
                    }
                }
            }
            else
            {
                Console.WriteLine($"Simple Condition{Num} is TRUE"); 
                this.Event?.Invoke(this, this.Num + 1);
            }
        }
    }
}
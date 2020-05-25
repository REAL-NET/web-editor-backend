using System;
using System.Collections.Generic;

namespace TestGeneratedFile
{
    public class AndOperation
    {
        public int Num { get; }

        public Dictionary<int, bool?> IncomingValues { get; set; }

        public AndOperation(int num)
        {
            this.Num = num;
            this.IncomingValues = new Dictionary<int, bool?>();
        }

        public event EventHandler<int> Event;

        public void Action(int value)
        {
            int val;
            bool result;

            if (value < 0)
            {
                val = -value - 1;
                this.IncomingValues[val] = false;
                result = false;
            }
            else
            {
                val = value - 1;
                this.IncomingValues[val] = true;
                result = true;
            }


            foreach (bool? v in this.IncomingValues.Values)
            {
                if (v.HasValue)
                {
                    result = result && v.Value;
                }
                else
                {
                    result = false;
                    break;
                }
            }

            if (result)
            {
                this.Event?.Invoke(this, this.Num + 1);
            }
        }
    }
}
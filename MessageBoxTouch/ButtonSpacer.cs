using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBoxTouch
{
    public class ButtonSpacer
    {
        public ButtonSpacer()
        {

        }

        public ButtonSpacer(double length)
        {
            this.length = length;
        }

        private double length = -1;

        public double GetLength()
        {
            return length;
        }
    }
}

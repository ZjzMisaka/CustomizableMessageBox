using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CustomizableMessageBox
{
    public class ButtonSpacer
    {
        private double value = -1;
        private GridUnitType gridUnitType = GridUnitType.Pixel;
        private bool isForSpan = false;
        public double Value { get => value; set => this.value = value; }
        public GridUnitType GridUnitType { get => gridUnitType; set => gridUnitType = value; }
        public bool IsForSpan { get => isForSpan; set => isForSpan = value; }

        public ButtonSpacer()
        {
            this.Value = 1;
            this.GridUnitType = GridUnitType.Star;
        }

        public ButtonSpacer(bool isForSpan)
        {
            this.IsForSpan = isForSpan;
            this.Value = 1;
            this.GridUnitType = GridUnitType.Star;
        }

        public ButtonSpacer(double length)
        {
            this.Value = length;
            this.GridUnitType = GridUnitType.Pixel;
        }
        public ButtonSpacer(double length, bool isForSpan)
        {
            this.IsForSpan = isForSpan;
            this.Value = length;
            this.GridUnitType = GridUnitType.Pixel;
        }

        public ButtonSpacer(double value, GridUnitType gridUnitType)
        {
            if (gridUnitType == GridUnitType.Pixel)
            {
                this.Value = value;
                this.GridUnitType = GridUnitType.Pixel;
            }
            else if (gridUnitType == GridUnitType.Star)
            {
                this.Value = value;
                this.GridUnitType = GridUnitType.Star;
            }
            else if (gridUnitType == GridUnitType.Auto)
            {
                this.GridUnitType = GridUnitType.Auto;
            }
        }

        public ButtonSpacer(double value, GridUnitType gridUnitType, bool isForSpan)
        {
            this.IsForSpan = isForSpan;
            if (gridUnitType == GridUnitType.Pixel)
            {
                this.Value = value;
                this.GridUnitType = GridUnitType.Pixel;
            }
            else if (gridUnitType == GridUnitType.Star)
            {
                this.Value = value;
                this.GridUnitType = GridUnitType.Star;
            }
            else if (gridUnitType == GridUnitType.Auto)
            {
                this.GridUnitType = GridUnitType.Auto;
            }
        }
    }
}

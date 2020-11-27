using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MissileCommand
{
    class City : GameElement
    {
        public City()
        {
            var rect = new Rectangle();

            rect.Fill = new SolidColorBrush(Color.FromArgb(60, 255, 255, 255));
            rect.Width = 64;
            rect.Height = 64;
            rect.RadiusX = 10;
            rect.RadiusY = 10;

            Add(rect);
        }
    }
}

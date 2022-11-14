using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns
{
    public class Point
    {
        public int X, Y;
    }

    public class Line
    {
        public Point Start, End;

        public Line DeepCopy()
        {
            return new Line
            {
                Start = new Point() { X = Start?.X ?? default, Y = Start?.Y ?? default },
                End = new Point() { X = End?.X ?? default, Y = End?.Y ?? default },
            };
        }
    }
}

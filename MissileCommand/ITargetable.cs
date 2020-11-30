using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Text;
using System.Threading.Tasks;

namespace MissileCommand
{
    interface ITargetable
    {
        void Explode();
        Vector TargetPosition { get; }
    }
}

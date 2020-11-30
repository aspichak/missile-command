using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MissileCommand
{
    class Silo : GameElement, ITargetable
    {
        private bool infiniteAmmo;
        public Vector TargetPosition { get; }
        private Vector Position { get; set; }
        public void Explode()
        {
            // does that boom boom thing
        }
        public void Rebuild()
        {
            // gets it back up
        }

        public Silo(Vector position, bool infAmmo = false)
        {
            Position = position;
            infiniteAmmo = infAmmo;
        }
    }
}

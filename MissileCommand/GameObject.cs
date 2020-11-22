using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MissileCommand
{
    class GameObject
    {
        private bool destroyed = false;

        private readonly static List<GameObject> ObjectsToAdd = new List<GameObject>();
        protected readonly static List<GameObject> Objects = new List<GameObject>();
        protected static Canvas Canvas { get; private set; }

        public static void Initialize(Canvas canvas)
        {
            GameObject.Canvas = canvas;
        }

        public static void UpdateAll(double dt)
        {
            Objects.ForEach(o => o.Update(dt));
            Objects.RemoveAll(o => o.destroyed);
            Objects.AddRange(ObjectsToAdd);
            ObjectsToAdd.Clear();
        }

        public GameObject()
        {
            ObjectsToAdd.Add(this);
        }

        public virtual void Update(double dt) { }

        public void Destroy()
        {
            destroyed = true;
        }
    }
}

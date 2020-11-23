using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MissileCommand
{
    class GameObject
    {
        private bool destroyed = false;
        private List<UIElement> elements = new List<UIElement>();

        private readonly static List<GameObject> objectsToAdd = new List<GameObject>();

        protected readonly static List<GameObject> Objects = new List<GameObject>();
        protected static Canvas Canvas { get; private set; }

        public GameObject()
        {
            objectsToAdd.Add(this);
        }

        public static void Initialize(Canvas canvas)
        {
            GameObject.Canvas = canvas;
        }

        public static void UpdateAll(double dt)
        {
            Objects.ForEach(o => o.Update(dt));
            Objects.RemoveAll(o => o.destroyed);
            Objects.AddRange(objectsToAdd);
            objectsToAdd.Clear();
        }

        public void Add(UIElement element)
        {
            elements.Add(element);
            Canvas.Children.Add(element);
        }

        public virtual void Update(double dt) { }

        public void Destroy()
        {
            destroyed = true;
            elements.ForEach(e => Canvas.Children.Remove(e));
            elements.Clear();
        }
    }
}

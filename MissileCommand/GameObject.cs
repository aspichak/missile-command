using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MissileCommand
{
    abstract class GameObject
    {
        private List<UIElement> elements = new List<UIElement>();

        protected static Canvas Canvas { get; set; }
        protected static Game Game { get; set; }

        public bool Destroyed { get; private set; }

        public GameObject()
        {
            if (!(this is Game)) Game.Add(this);
        }

        protected void Add(UIElement element)
        {
            elements.Add(element);
            Canvas.Children.Add(element);
        }

        internal virtual void Update(double dt) { }

        public void Destroy()
        {
            Destroyed = true;
            elements.ForEach(e => Canvas.Children.Remove(e));
            elements.Clear();
        }
    }
}

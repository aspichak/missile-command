using MissileCommand.Screens;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace MissileCommand
{
    abstract class GameObject
    {
        private static List<GameObject> objectsToAdd = new List<GameObject>();
        private static List<GameObject> objects = new List<GameObject>();

        protected static ReadOnlyCollection<GameObject> Objects => objects.AsReadOnly();

        public static Grid Grid { get; set; }
        public static Canvas Canvas { get; set; }
        public static GameScreen Game { get; set; }

        private List<UIElement> elements = new List<UIElement>();

        public bool Destroyed { get; private set; }

        public GameObject()
        {
            objectsToAdd.Add(this);
        }

        public static void UpdateAll(double dt)
        {
            objects.ForEach(o => o.Update(dt));
            objects.RemoveAll(o => o.Destroyed);
            objects.AddRange(objectsToAdd);
            objectsToAdd.Clear();
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

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MissileCommand
{
    class GameObject
    {
        private bool destroyed = false;
        private Canvas canvas;
        private List<GameObject> children = new List<GameObject>();
        private List<GameObject> childrenToAdd = new List<GameObject>();
        private List<UIElement> elements = new List<UIElement>();

        protected Canvas Canvas {
            get => canvas;
            set
            {
                canvas = value;
                elements.ForEach((e) => canvas?.Children.Add(e));
                children.ForEach((c) => c.Canvas = canvas);
                childrenToAdd.ForEach((c) => c.Canvas = canvas);
            }
        }
        protected Game Game { get; set; }
        public List<GameObject> Children => children;
        public GameObject Parent { get; private set; }

        public GameObject()
        {
        }

        public virtual void Update(double dt)
        {
            Children.ForEach((c) => c.Update(dt));
            Children.RemoveAll((c) => c.destroyed);
            Children.AddRange(childrenToAdd);
            childrenToAdd.Clear();
        }

        public void Destroy()
        {
            destroyed = true;
            elements.ForEach((e) => Canvas.Children.Remove(e));
            elements.Clear();
            children.ForEach((c) => c.Destroy());
            childrenToAdd.ForEach((c) => c.Destroy());
        }

        public void Add(GameObject gameObject)
        {
            gameObject.Game = Game;
            gameObject.Parent = this;
            gameObject.Canvas = Canvas;

            childrenToAdd.Add(gameObject);
        }

        public void Add(UIElement element)
        {
            elements.Add(element);
            Canvas?.Children.Add(element);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MissileCommand
{
    class Game : GameObject, IEnumerable<GameObject>
    {
        private readonly List<GameObject> objectsToAdd = new List<GameObject>();
        private readonly List<GameObject> Objects = new List<GameObject>();

        public int Score { get; set; }
        public int Round { get; set; }
        public bool Paused { get; set; }

        public List<GameObject> Enemies;
        public List<GameObject> Silos;
        public List<GameObject> Cities;

        public Game(Canvas canvas)
        {
            GameObject.Canvas = canvas;
            GameObject.Game = this;
        }

        internal void Add(GameObject gameObject)
        {
            objectsToAdd.Add(gameObject);
        }

        public new void Update(double dt)
        {
            Objects.ForEach(o => o.Update(dt));
            Objects.RemoveAll(o => o.Destroyed);
            Objects.AddRange(objectsToAdd);
            objectsToAdd.Clear();
        }

        public GameObject NearestSilo(Vector vector)
        {
            throw new NotImplementedException();
        }

        public new void Destroy()
        {
            Objects.Union(objectsToAdd).ForEach(o => o.Destroy());
            Objects.Clear();
            objectsToAdd.Clear();
        }

        public IEnumerator<GameObject> GetEnumerator()
        {
            return ((IEnumerable<GameObject>)Objects).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Objects).GetEnumerator();
        }
    }
}

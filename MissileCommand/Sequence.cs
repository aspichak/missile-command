using System;

namespace MissileCommand
{
    class Sequence : GameElement
    {
        public double Duration { get; protected set; }
        public event Action Completed;

        public Sequence Then(Sequence sequence)
        {
            var newSequence = new Sequence();
            newSequence.Duration = this.Duration + sequence.Duration;

            this.Completed += () => newSequence.Add(sequence);

            sequence.Completed += () =>
            {
                newSequence.OnCompleted();
                newSequence.Destroy();
            };

            newSequence.Add(this);

            return newSequence;
        }

        public Sequence Then(Action action) => Then(Timer.At(0, action));
        public Sequence ThenDelay(double delay, Action action = null) => Then(Timer.At(delay, action));

        public Sequence And(Sequence sequence)
        {
            var newSequence = new Sequence();
            var longestSequence = (sequence.Duration > this.Duration) ? sequence : this;
            newSequence.Duration = longestSequence.Duration;

            longestSequence.Completed += () =>
            {
                newSequence.OnCompleted();
                newSequence.Destroy();
            };

            newSequence.Add(this);
            newSequence.Add(sequence);

            return newSequence;
        }

        public Sequence And(Action action) => And(Timer.At(0, action));

        public static Sequence operator +(Sequence a, Sequence b) => a.Then(b);
        public static Sequence operator +(Sequence a, Action b) => a.Then(b);
        public static Sequence operator *(Sequence a, Sequence b) => a.And(b);
        public static Sequence operator *(Sequence a, Action b) => a.And(b);

        protected virtual void OnCompleted()
        {
            Completed?.Invoke();
        }
    }
}

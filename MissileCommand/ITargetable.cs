using System.Windows;

namespace MissileCommand
{
    interface ITargetable
    {
        void Explode();
        void Rebuild();
        bool IsDestroyed { get; }
        Vector TargetPosition { get; }
    }
}


namespace Galaga
{
    public abstract class Enemy : PoolableGameObject, IDestroyable
    {
        protected int _health = 2;

        public abstract void Hit();
    }
}


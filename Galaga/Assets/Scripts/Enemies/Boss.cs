using UnityEngine;

namespace Galaga
{
    namespace AI
    {
        public class Boss : Enemy
        {
            private static readonly int _damagedId = Animator.StringToHash("Damaged");

            public override void Hit()
            {
                // boss' first health point
                if (!_hit)
                {
                    _animator.SetBool(_damagedId, true);
                    _hit = true;
                }
                // boss already took damage; dies normally
                else
                    base.Hit();
            }
        }
    }

}
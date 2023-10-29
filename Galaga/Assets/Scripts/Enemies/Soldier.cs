using UnityEngine;

namespace Galaga
{
    public class Soldier : Enemy
    {

        public override void Hit()
        {
            Debug.Log($"{transform.name}'s health is: {_health}");
        }
    }
}
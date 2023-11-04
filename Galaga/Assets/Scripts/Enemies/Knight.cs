using UnityEngine;

namespace Galaga
{
    namespace AI
    {
        public class Knight : Enemy
        {
            public override void OnSpawn()
            {
                _hit = false;
                _collider.enabled = true;

                // will eventually be replaced by actual equations for each type of enemy depending on round
                _diveThreshold = Random.Range(3.5f, 7.75f);
                //_missileThreshold = Random.Range(.5f, 2.5f);

                RoundManager.Singleton.SpawnedEnemies.Add(this);
            }
        }
    }   
}
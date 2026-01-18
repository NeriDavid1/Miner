using UnityEngine;


namespace MinerGameMode
{
    public class PlayerVfxPoints : MonoBehaviour
    {
        [SerializeField] private Transform spawnPointA;
        [SerializeField] private Transform spawnPointB;
    
        public Transform SpawnPointA
        {
            get { return spawnPointA; }
        }
    
        public Transform SpawnPointB
        {
            get { return spawnPointB; }
        }
    }
}

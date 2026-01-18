using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MinerGameMode
{
    [CreateAssetMenu(menuName = "GoldMiner/LevelDataBase")]
    
    public class LevelDataBase : ScriptableObject
    {
        public List<LevelDataSO> dataSO = new List<LevelDataSO>();
    
        public LevelDataSO GetLevel(int levelID)
        {
            return dataSO[levelID];
        }
    
    }
}

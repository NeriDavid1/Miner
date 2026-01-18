using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;


namespace MinerGameMode
{
    [CreateAssetMenu(menuName = "GoldMiner/LevelDataSO")]
    public class LevelDataSO : ScriptableObject
    {
        public string levelName;
    
        public Sprite background;          
        public string mainLetter;
        public int mainLetterCount;
        public List<LetterDataSO> lettersPool;
        public GameObject playerPrefab;
        public GameObject backgroundPrefab;
        public string targetWord;
    
        public string endStageText;
    
        public ParticleSystem endStageVfxPrefab;
    
        //SPAWNS
        public int totalLettersToSpawn = 5;
    }
}

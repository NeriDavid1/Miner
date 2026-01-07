using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

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

    //SPAWNS
    public int totalLettersToSpawn = 5;
}

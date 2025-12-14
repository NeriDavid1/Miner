using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public LevelDataBase LevelData;

    public int currentLevel;

    void Start()
    {
        StartGame();
    }


    public void StartGame()
    {

        LevelDataSO data = LevelData.GetLevel(currentLevel);
        SpawnManager.instance.CreateLevel(data);
        Instantiate(data.backgroundPrefab);

    }

    public void StartNextLevel()
    {
        currentLevel++;
        if (currentLevel >= LevelData.dataSO.Count)
        {
            return;
        }
        StartGame();
    }
}


//TODO: *Test OnCollected* 1. Count how many main letters in level.  2.Clear level if 0(or if collectedMain = MainLettersInLevel)  3.Move to next stage
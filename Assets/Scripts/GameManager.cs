using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public LevelDataBase LevelDataBase;
    public LevelDataSO level;
    public int letterCounter;

    public int currentLevel;
    

    public static GameManager instance;

    private void Awake()
    {
        if (instance != null && instance != this) 
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        StartGame();
    }


    public void StartGame()
    {

        level = LevelDataBase.GetLevel(currentLevel);
        SpawnManager.instance.CreateLevel(level);
        Instantiate(level.backgroundPrefab);

    }

    public void StartNextLevel()
    {
        currentLevel++;
        if (currentLevel >= LevelDataBase.dataSO.Count)
        {
            return;
        }
        StartGame();
    }

    public bool TryGetLetter(string letterID)
    {
        if (letterID == level.mainLetter)
        {
            if (letterCounter < level.mainLetterCount)
            {
                letterCounter++;
                if (letterCounter == level.mainLetterCount) 
                {
                    Debug.Log("STAGE DONE");
                    //INDITACTION FOR STAGE COMPLETE
                    currentLevel++;
                }
                return true;
            } 

        }
        return false;
    }
}


//TODO: *Test OnCollected* 1. Count how many main letters in level(LIST?).  2.Clear level if 0(or if collectedMain = MainLettersInLevel)  3.Move to next stage
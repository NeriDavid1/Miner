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

    private void OnEnable()
    {
        Player.OnMainLetterDelivered += MainLetterDelivered;
    }

    private void OnDisable()
    {
        Player.OnMainLetterDelivered -= MainLetterDelivered;
    }
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
        letterCounter = 0;
        StartGame();
        
    }


    public void StartGame()
    {

        level = LevelDataBase.GetLevel(currentLevel);
        letterCounter = 0;

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
        if (level == null)
        {
            return false;
        }
        if (letterID != level.mainLetter)
        {
            return false;
        }

        return letterCounter < level.mainLetterCount;
    }

    private void MainLetterDelivered(string letterID)
    {
        if (level == null)
        {
            return;
        }

        if (letterID != level.mainLetter)
        {
            return;
        }

        if (letterCounter >= level.mainLetterCount)
        {
            return;
        }

        letterCounter++;

        if (letterCounter >= level.mainLetterCount)
        {
            Debug.Log("STAGE DONE");
            StartNextLevel();
        }
    }

}


//TODO: *Test OnCollected* 1. Count how many main letters in level(LIST?).  2.Clear level if 0(or if collectedMain = MainLettersInLevel)  3.Move to next stage
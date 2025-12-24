using System;
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
    public string ExpectedLetter;
    private int wordIndex;

    public static event Action<LevelDataSO> OnLevelStarted;
    public static event Action<string> OnExpectedLetterChanged;

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
        //letterCounter = 0;
        
        StartGame();
        
    }


    public void StartGame()
    {

        level = LevelDataBase.GetLevel(currentLevel);
        letterCounter = 0;
        wordIndex = 0;

        OnLevelStarted?.Invoke(level);

        SpawnManager.instance.CreateLevel(level);
      //  Instantiate(level.backgroundPrefab);

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

    private bool IsWordMode()
    {
        if (level == null)
        {
            return false;
        }
        if (string.IsNullOrWhiteSpace(level.targetWord))
        {
            return false;
        }
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



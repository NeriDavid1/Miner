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

    public static event Action<LevelDataSO> OnLevelStarted; // UI + SPAWN MANAGER
    public static event Action<int> OnWordIndexChanged; // UI

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
        StartGame();
    }

    public void StartGame()
    {
        level = LevelDataBase.GetLevel(currentLevel);
        letterCounter = 0;
        wordIndex = 0;

        OnWordIndexChanged?.Invoke(wordIndex);

        SetExpectedLetter();

        OnLevelStarted?.Invoke(level);

        SpawnManager.instance.CreateLevel(level);
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
        if (level == null || level.targetWord == null || level.targetWord == "")
        {
            return false;
        }
        return true;
    }


    public void SetExpectedLetter()
    {
        if (level == null)
        {
            ExpectedLetter = null;
            return;
        }

        if (IsWordMode())
        {
            if (wordIndex >= level.targetWord.Length)
            {
                ExpectedLetter = null;
                return;
            }
            // wordIndex++ BY MainLetterDelivered 
            ExpectedLetter = level.targetWord.Substring(wordIndex, 1);
            return;
        }
        ExpectedLetter = level.mainLetter;
    }

    private void MainLetterDelivered(string letterID)
    {
        if (level == null)
        {
            return;
        }

        if (ExpectedLetter == null)
        {
            return;
        }

        //Full Word Mode
        if (IsWordMode())
        {
            if (letterID != ExpectedLetter)
            {
                return;
            }
            wordIndex++;
            OnWordIndexChanged?.Invoke(wordIndex); //UI
            SetExpectedLetter();

            //Next Level
            if (ExpectedLetter == null)
            {
                Debug.Log("WORD DONE - STAGE DONE");
                StartNextLevel();
            }
            return;
        }

        //Main Letter Mode
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



    public bool TryGetLetter(string letterID)
    {
        if (level == null)
        {
            return false;
        }

        if (ExpectedLetter == null)
        {
            return false;
        }

        //Full Word Mode
        if (IsWordMode())
        {
            return letterID == ExpectedLetter;
        }

        //Main Letter Mode
        if (letterID != level.mainLetter)
        {
            return false;
        }

        return letterCounter < level.mainLetterCount;
    }

}



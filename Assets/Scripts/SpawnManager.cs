using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private LevelDataSO currentLevel;
    [SerializeField] private GameObject LetterPrefab;
    [SerializeField] private LettersDataBase lettersDataBase;
    [SerializeField] private Transform levelRoot;


    //SPAWN POINTS
    [SerializeField] private float innerSpawnRadius = 2f;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float minLetterDistance = 1f;
    [SerializeField] private Transform playerStartPoint;

    private GameObject spawnedPlayer;

    public static SpawnManager instance;



    private void Awake()
    {
        
        if (instance != null && instance != this) 
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void CreateLevel(LevelDataSO level)
    {
        if (level == null)
        {
            Debug.Log("Create Level was called with missing level");
            return;
        }

        currentLevel = level;
        ClearLetters();
        SpawnPlayer();
        SpawnLetters();
        

    }

    private void SpawnPlayer()
    {
        if (playerStartPoint == null || currentLevel.playerPrefab == null)
        {
            Debug.Log("PlayerStartPoint or PlayerPrefab are empty");
            return;
        } 

        if (spawnedPlayer != null)
        {
            spawnedPlayer.transform.position = playerStartPoint.position;
            return;
        }
        spawnedPlayer = Instantiate(currentLevel.playerPrefab, playerStartPoint.position, Quaternion.identity);

    }

    private void SpawnOneLetter(LetterDataSO chosenData)
    {
        if (chosenData == null)
        {
            return;
        }

        Vector3 pos = Vector3.zero;
        bool availableSpot = false;

        int trySpawn = 0;
        int maxTries = 50;

        while (trySpawn < maxTries && availableSpot == false)
        {
            pos = GetRandomPointInCameraBounds();
            if (IsFarEnough(pos))
            {
                availableSpot = true;
            }
            trySpawn++;
        }

        if (availableSpot == false)
        {
            return;
        }

        Sprite spriteToUse = lettersDataBase.GetSpriteByName(chosenData.id);

        if (spriteToUse == null)
        {
            return;
        }

        GameObject newLetterObj = Instantiate(LetterPrefab, pos, Quaternion.identity);
        newLetterObj.transform.SetParent(levelRoot);

        Letter letterScript = newLetterObj.GetComponent<Letter>();
        if (letterScript != null)
        {
            letterScript.Init(chosenData, spriteToUse, false);
        }
    }


    private void SpawnLetters()
    {
        // TotalLettersToSpawn - MainLetterCount = regularCount
        int total = currentLevel.totalLettersToSpawn;
        int mainCount = currentLevel.mainLetterCount;
        int regularCount = total - mainCount;

        //Spawn mains and regulars
        SpawnMainLetters(mainCount);
        SpawnRegularLetters(regularCount);
    }

    private void SpawnMainLetters(int count)
    {
        if (count <= 0)
        {
            return;
        }

        LetterDataSO mainData = GetMainLetterData();

        if (mainData == null)
        {
            return;
        }
        for (int i = 0; i < count; i++)
        {
            SpawnOneLetter(mainData);
        }
    }

    private void SpawnRegularLetters(int count)
    {
        if (count <= 0)
        {
            return;
        }
        List<LetterDataSO> nonMainPool = new List<LetterDataSO>();

        for (int i = 0; i < currentLevel.lettersPool.Count; i++)
        {
            LetterDataSO letterData = currentLevel.lettersPool[i];

            if (letterData != null && letterData.id != currentLevel.mainLetter)
            {
                nonMainPool.Add(letterData);
            }
        }

        if (nonMainPool.Count == 0)
        {
            return;
        }

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, nonMainPool.Count);
            SpawnOneLetter(nonMainPool[index]);
        }


    }

    private bool IsFarEnough(Vector3 letter)
    {
        // IF PLAYER HAS NO COLLIDER
        if (playerStartPoint != null)
        {
            float distFromPlayer = Vector3.Distance(letter, playerStartPoint.position);
            if (distFromPlayer < innerSpawnRadius)
            {
                return false;
            }
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll((Vector2)letter, minLetterDistance);
        Debug.Log($"{hits.Length}");
        if (hits.Length > 0)
        {
            return false;
        }

        return true;
    }

    private Vector3 GetRandomPointInCameraBounds()
    {
        Camera cam;

        if (mainCamera != null)
        {
            cam = mainCamera;
        }
        else
        {
            cam = Camera.main;
            Debug.Log($"{cam} was not defined in SpawnManager");
        }

        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0f, 0f, cam.nearClipPlane));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1f, 1f, cam.nearClipPlane));

        float x = Random.Range(bottomLeft.x, topRight.x);
        float y = Random.Range(bottomLeft.y, topRight.y);

        return new Vector3(x, y, 0f);
    }

    private LetterDataSO GetMainLetterData()
    {
        if (currentLevel == null || currentLevel.lettersPool == null)
        {
            return null;
        }

        foreach (var data in currentLevel.lettersPool)
        {
            if (data != null && data.id == currentLevel.mainLetter)
            {
                return data;
            }
        }
        return null;
    }

    private void ClearLetters()
    {
        if (levelRoot == null)
        {
            return;
        }

        for (int i = levelRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(levelRoot.GetChild(i).gameObject);
        }
    }



    //GIZMOZ
    private void OnDrawGizmosSelected()
    {
        if (playerStartPoint == null) return;

        // INNER
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerStartPoint.position, innerSpawnRadius);
    }
}

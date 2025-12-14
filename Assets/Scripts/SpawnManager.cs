using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private LevelDataSO currentLevel;
    [SerializeField] private GameObject LetterPrefab;
  //  [SerializeField] private GameObject playerPrefab;
    [SerializeField] private LettersDataBase lettersDataBase;


    //SPAWN POINTS
    [SerializeField] private float innerSpawnRadius = 2f;
   // [SerializeField] private float OutterSpawnRadius = 6f;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float minLetterDistance = 1f;
    [SerializeField] private Transform playerStartPoint;
   // [SerializeField] private Transform lettersPlaceHolder;



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
        else
        {
            Instantiate(currentLevel.playerPrefab, playerStartPoint.position, Quaternion.identity);

        }
    }


    private void SpawnLetters()
    {

        if (LetterPrefab == null || playerStartPoint == null || currentLevel == null)
        {
            Debug.Log("LetterPrefab or playerStartPoint was not found");
            return;
        }

        if (currentLevel.lettersPool == null)
        {
            Debug.Log("Level has no lettersPool");
            return;
        }

        int lettersToSpawn = currentLevel.totalLettersToSpawn;

        for (int i = 0; i < lettersToSpawn; i++)
        {
            Vector3 pos = Vector3.zero;
            bool availableSpot = false;

            int trySpawn = 0;
            int maxTries = 50;


            while (trySpawn < maxTries && availableSpot == false)
            {
                pos = GetRandomPointInCameraBounds();
                // CHECKS IF SPAWN POINT IS FAR ENOUGH FROM USED POSITIONS
                if (IsFarEnough(pos))
                {
                    availableSpot = true;
                }
                trySpawn++;
            }
            Debug.Log($"spawn at {trySpawn}");

            //SPAWN IF FOUND A SPOT
            if (availableSpot == true)
            {
                int poolIndex = Random.Range(0, currentLevel.lettersPool.Count);
                LetterDataSO chosenData = currentLevel.lettersPool[poolIndex];
                Sprite spriteToUse = lettersDataBase.GetSpriteByName(chosenData.id);

                if (spriteToUse != null)
                {
                    GameObject newLetterObj = Instantiate(LetterPrefab, pos, Quaternion.identity);

                    //INIT LETTER + MARK IF MAIN
                    Letter letterScript = newLetterObj.GetComponent<Letter>();
                    if (letterScript != null)
                    {
                        bool isMain = chosenData.id == currentLevel.mainLetter;
                        letterScript.Init(chosenData, spriteToUse, isMain);
                    }
                }
            }
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


    //GIZMOZ
    private void OnDrawGizmosSelected()
    {
        if (playerStartPoint == null) return;

        // INNER
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerStartPoint.position, innerSpawnRadius);
    }
}

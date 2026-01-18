using System.Collections;
using UnityEngine;
using TMPro;


namespace MinerGameMode
{
    public class LevelCompleteUI : MonoBehaviour
    {
        [SerializeField] private GameObject endLevelPanel;
        [SerializeField] private LevelGenerator levelGenerator;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private float delayBeforeNextLevel = 5f;
    
        [SerializeField] private int fireworksSpawns = 4;
        [SerializeField] private float spawnInterval = 0.3f;
    
        private GameObject fireworksGroup; //HOLDS ALL VFX
    
        private void OnEnable()
        {
            MinerMode.OnLevelCompleted += HandleLevelCompleted;
            endLevelPanel.SetActive(false);
        }
    
        private void OnDisable()
        {
            MinerMode.OnLevelCompleted -= HandleLevelCompleted;
        }
    
        private void HandleLevelCompleted(string message)
        {
            statusText.text = message;
            StartCoroutine(ShowSequence());
        }
    
        private ParticleSystem GetEndStageVfxPrefab()
        {
            if (MinerMode.instance == null)
            {
                return null; 
            }
            if (MinerMode.instance.level == null) 
            {
                return null; 
            }
            return MinerMode.instance.level.endStageVfxPrefab;
        }
    
    
        private IEnumerator ShowSequence()
        {
            endLevelPanel.SetActive(true);
    
            ParticleSystem vfxPrefab = GetEndStageVfxPrefab();
    
            // USE RANDOMPOSITION FROM SPAWN MANAGER
            if (vfxPrefab != null && levelGenerator != null)
            {
                fireworksGroup = new GameObject("EndLevelFireworksGroup");
    
                float timeSpentSpawning = 0f;
    
                for (int i = 0; i < fireworksSpawns; i++)
                {
                    Vector3 randomPosition = levelGenerator.GetRandomPointInCameraBoundsForVfx();
                    randomPosition.z = 0f;
    
                    ParticleSystem fireworkInstance = Instantiate(vfxPrefab, randomPosition, Quaternion.identity, fireworksGroup.transform);
    
                    fireworkInstance.Play(true);
    
                    yield return new WaitForSeconds(spawnInterval);
                    timeSpentSpawning += spawnInterval;
                }
    
                float remainingTime = delayBeforeNextLevel - timeSpentSpawning;
    
                if (remainingTime > 0f)
                {
                    yield return new WaitForSeconds(remainingTime);
                }
            }
            else
            {
                yield return new WaitForSeconds(delayBeforeNextLevel);
            }
    
            // DESTROY AFTER MESSAGE TURNS OFF
            if (fireworksGroup != null)
            {
                Destroy(fireworksGroup);
                fireworksGroup = null;
            }
    
            endLevelPanel.SetActive(false);
        }
    }
}

using System.Collections;
using UnityEditor.EditorTools;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class WrongLetterVfxSpawner : MonoBehaviour
{
    private PlayerVfxPoints cachedPoints;

    private void Start()
    {
        Player.OnWrongLetterHit += HandleWrongLetterHit;
    }

    private void OnDisable()
    {
        Player.OnWrongLetterHit -= HandleWrongLetterHit;
    }

    private void HandleWrongLetterHit()
    {
        if (VfxPoolManager.Instance == null)
        {
            return;
        }

        if (cachedPoints == null)
        {
            Player playerInScene = FindObjectOfType<Player>();
            if (playerInScene == null)
            {
                return;
            }

            cachedPoints = playerInScene.GetComponent<PlayerVfxPoints>();
            if (cachedPoints == null)
            {
                return;
            }
        }

        SpawnAtPoint(cachedPoints.SpawnPointA);
        SpawnAtPoint(cachedPoints.SpawnPointB);
    }

    private void SpawnAtPoint(Transform spawnPoint)
    {
        if (spawnPoint == null)
        {
            return;
        }

        GameObject pooledParticle = VfxPoolManager.Instance.GetParticle();
        if (pooledParticle == null)
        {
            return;
        }

        pooledParticle.transform.position = spawnPoint.position;
        pooledParticle.SetActive(true);
        StartCoroutine(ActivateParticle(pooledParticle));
    }

    IEnumerator ActivateParticle(GameObject particle)
    {
        float time = 0;
        if (particle.TryGetComponent<ParticleSystem>(out var ps))
        {
            var curv = ps.main.startLifetime;
            time = curv.constantMax;
        }
        yield return new WaitForSeconds(time);
        particle.SetActive(false);
        VfxPoolManager.Instance.Release(particle);
    }
}

using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;


namespace MinerGameMode
{
    public class VfxPoolManager : MonoBehaviour
    {
        public static VfxPoolManager Instance { get; private set; }
    
        [SerializeField] private GameObject particlePrefab;
        [SerializeField] private int initialSize = 4;
        [SerializeField] private Transform poolRoot;
    
        private Queue<GameObject> availableParticles = new Queue<GameObject>();
    
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
    
            Instance = this;
    
    
            WarmUp();
        }
    
        private void WarmUp()
        {
            if (particlePrefab == null)
            {
                return;
            }
    
            for (int i = 0; i < initialSize; i++)
            {
                GameObject newParticle = CreateNew();
                Release(newParticle);
            }
        }
    
        private GameObject CreateNew()
        {
            GameObject particleGameObject = Instantiate(particlePrefab, poolRoot);
            particleGameObject.SetActive(false);
    
            return particleGameObject;
        }
    
    
    
        public GameObject GetParticle()
        {
            if (availableParticles.Count > 0)
            {
                return availableParticles.Dequeue();
            }
    
            if (particlePrefab == null)
            {
                return null;
            }
    
            return CreateNew();
        }
    
        public void Release(GameObject pooledParticle)
        {
            if (pooledParticle == null)
            {
                return;
            }
    
            pooledParticle.transform.SetParent(poolRoot, true);
            pooledParticle.gameObject.SetActive(false);
    
            availableParticles.Enqueue(pooledParticle);
        }
    }
}

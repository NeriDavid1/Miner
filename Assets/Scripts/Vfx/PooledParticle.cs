

namespace MinerGameMode
{
    //using UnityEngine;
    
    //public class PooledParticle : MonoBehaviour
    //{
    //    private ParticleSystem particleSystemComponent;
    //    private bool isInUse;
    //    private VfxPoolManager poolManager;
    
    //    //private void Awake()
    //    //{
    //    //    particleSystemComponent = GetComponent<ParticleSystem>();
    //    //}
    
    //    public void Bind(VfxPoolManager ownerPoolManager)
    //    {
    //        poolManager = ownerPoolManager;
    //    }
    
    //    public void PlayAtPosition(Vector3 worldPosition)
    //    {
    //        if (particleSystemComponent == null)
    //        {
    //            particleSystemComponent = GetComponent<ParticleSystem>();
    //        }
    
    //        transform.position = worldPosition;
    //        gameObject.SetActive(true);
    
    //        particleSystemComponent.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    //        particleSystemComponent.Play(true);
    
    //        isInUse = true;
    //    }
    
    //    private void Update()
    //    {
    //        if (!isInUse || particleSystemComponent == null)
    //        {
    //            return;
    //        }
    
    //        if (!particleSystemComponent.IsAlive(true))
    //        {
    //            isInUse = false;
    
    //            if (poolManager != null)
    //            {
    //                poolManager.Release(this);
    //            }
    //        }
    //    }
    //}
}

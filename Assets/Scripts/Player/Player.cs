using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MinerGameMode
{
    public class Player : MonoBehaviour
    {
        private LevelGenerator levelGenerator;
    
        public void Init(LevelGenerator generator)
        {
            levelGenerator = generator;
        }
    
        [SerializeField] private Transform catchAnchor;
        [SerializeField] private Transform rotateTransform;
        [SerializeField] private Transform ropeStretchBone;
    
        //ROPE
        private Vector3 ropeRestLocalPos;
        private float currentRopeLength;
        public float hookSpeed;
        public float rotationSpeed = 10f;
        private bool isRotating = true;
        private bool isShooting = false;
        private bool isReturning = false;
        [SerializeField] private Transform deliverTarget;
    
        public static event Action<string> OnMainLetterDelivered;
        public static event Action OnWrongLetterHit;
    
        private string carriedMainLetterId = null;
    
        private Transform carriedLetterTransform = null;
    
        [SerializeField] private Animator animator;
    
        [SerializeField] private float viewportMargin = 0.02f;
    
        private string pendingDeliveredId = null;
        private Transform pendingDeliveredTransform = null;
        private Coroutine pendingReportRoutine = null;
        private float animatorSpeedBeforeFreeze = 1f;
    
        void Start()
        {
    
            // RECORD ROPE POSITION
            if (ropeStretchBone != null)
            {
                ropeRestLocalPos = ropeStretchBone.localPosition;
                currentRopeLength = 0f;
            }
        }
    
        private void UpdateAnimator()
        {
            if (animator == null)
            {
                return;
            }
            animator.SetBool("isRotating", isRotating);
            animator.SetBool("isShooting", isShooting);
            animator.SetBool("isReturning", isReturning);
        }
    
    
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ActivateHook();
            }
    
            //ROTATION
            if (isRotating == true && rotateTransform != null)
            {
                rotateTransform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime); 
            }
            HookExit();
            HookReturn();
            UpdateAnimator();
        }
    
         private void ActivateHook()
         {
        
             if (isReturning == false && isRotating == true)
             {
               isRotating = false;
               isShooting = true;
             }
         }
    
        private void ReturnHook()
        {
            isShooting = false;
            isReturning = true;
        }
    
        private void ApplyRopeLength()
        {
            if (ropeStretchBone == null)
            {
                return;
            }
            ropeStretchBone.localPosition = ropeRestLocalPos + Vector3.right * currentRopeLength;
        }
    
        private void HookExit()
        {
            if (!isShooting || ropeStretchBone == null)
            {
                return;
            }
    
            currentRopeLength += hookSpeed * Time.deltaTime;
    
            //CHECK IF HOOK INSIDE CAMERA BONDS
            if (IsHookInsideScreen() == false)
            {
                ReturnHook();
                ApplyRopeLength();
                return;
            }
            ApplyRopeLength();
        }
        private void HookReturn()
        {
            if (!isReturning || ropeStretchBone == null)
            {
                return;
            }
    
            currentRopeLength -= hookSpeed * Time.deltaTime;
    
            if (currentRopeLength <= 0f)
            {
                currentRopeLength = 0f;
                isReturning = false;
                isRotating = true;
    
    
                // DELIVER LETTER + FX
                Transform deliveredTransform = null;
    
                if (carriedLetterTransform != null)
                {
                    deliveredTransform = carriedLetterTransform;
                    carriedLetterTransform = null;
    
                    if (levelGenerator != null)
                    {
                         deliveredTransform.SetParent(levelGenerator.LevelRoot, true); // BACK TO LevelRoot
                    }
                    else
                    {
                         // Fallback or legacy handling if needed, but we expect levelGenerator to be set
                         // Maybe destroy or just disable?
                         // Or check SpawnManager.instance just in case? No, we are removing it.
                         Destroy(deliveredTransform.gameObject);
                    }
    
                    Letter deliveredLetter = deliveredTransform.GetComponent<Letter>();
                    if (deliveredLetter != null)
                    {
                        deliveredLetter.PlayFxAndDisable(deliverTarget);
                    }
                    else
                    {
                        deliveredTransform.gameObject.SetActive(false);
                    }
                }
    
                if (carriedMainLetterId == null)
                {
                    return;
                }
    
                pendingDeliveredId = carriedMainLetterId;
                carriedMainLetterId = null;
    
                pendingDeliveredTransform = deliveredTransform;
    
                pendingReportRoutine = StartCoroutine(ReportWhenLetterDisabled());
            }
            ApplyRopeLength();
        }
    
        private IEnumerator ReportWhenLetterDisabled()
        {
            FreezePlayerVisual();
    
            // WAIT FOR LETTER TO TURN OFF
            while (pendingDeliveredTransform != null && pendingDeliveredTransform.gameObject.activeSelf == true)
            {
                yield return null;
            }
    
            // Unfreeze
            UnfreezePlayerVisual();
    
            // REPORT TO GAME MANAGER
            if (pendingDeliveredId != null)
            {
                OnMainLetterDelivered?.Invoke(pendingDeliveredId);
            }
    
            pendingDeliveredId = null;
            pendingDeliveredTransform = null;
            pendingReportRoutine = null;
        }
    
    
        private void FreezePlayerVisual()
        {
            isRotating = false;
    
            if (animator != null)
            {
                animatorSpeedBeforeFreeze = animator.speed;
                animator.speed = 0f;
            }
        }
    
        private void UnfreezePlayerVisual()
        {
            if (animator != null)
            {
                animator.speed = animatorSpeedBeforeFreeze;
            }
    
            if (isShooting == false && isReturning == false)
            {
                isRotating = true;
            }
        }
    
        private bool IsHookInsideScreen()
        {
            Camera cameraToUse = Camera.main;
    
            if (cameraToUse == null || catchAnchor == null)
            {
                return true;
            }
    
            Vector3 viewportPos = cameraToUse.WorldToViewportPoint(catchAnchor.position);
    
            if (viewportPos.x < viewportMargin || viewportPos.x > 1f - viewportMargin || viewportPos.y < viewportMargin || viewportPos.y > 1f - viewportMargin)
            {
                return false;
            }
            return true;
        }
    
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isShooting) 
            {
                return;
            };
    
            Letter letter = other.GetComponent<Letter>();
    
            if (letter == null)
            {
                return;
            }
            //CHECK IF MAIN LETTER
            bool shouldCollectAsMain = MinerMode.instance.TryGetLetter(letter.letterID);
    
            if (shouldCollectAsMain == true)
            {
                letter.transform.SetParent(catchAnchor, false);
                letter.transform.localPosition = Vector3.zero;
                letter.transform.localRotation = Quaternion.identity;
    
                carriedLetterTransform = letter.transform;
                carriedMainLetterId = letter.letterID;
            }
            else
            {
                letter.gameObject.SetActive(false);
                Debug.Log("Wrong Letter Hit");
                OnWrongLetterHit?.Invoke();
            }
            ReturnHook();
        }
    }
}

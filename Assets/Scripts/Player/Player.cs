using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform catchAnchor;
    [SerializeField] private Transform rotateTransform;
    [SerializeField] private Transform ropeStretchBone;
    [SerializeField] private float maxHookDistance = 5f;

    //ROPE
    private Vector3 ropeRestLocalPos;
    private float currentRopeLength;
    public float hookSpeed;
    public float rotationSpeed = 10f;
    private bool isRotating = true;
    private bool isShooting = false;
    private bool isReturning = false;
    //private Vector3 hookStartLocalPos;
    //private Vector3 shootDirection;

    public static event Action<string> OnMainLetterDelivered;
    private string carriedMainLetterId = null;

    private Transform carriedLetterTransform = null;

    [SerializeField] private Animator animator;




    void Start()
    {
        // RECORD START POSITION
        //if (hookTransform != null)
        //{
        //    hookStartLocalPos = hookTransform.localPosition;
        //}

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

        //CHANGE BOOLIANS IF DIDNT CATCH A LETTER & CROSSED MaxHookDistance
        if (currentRopeLength >= maxHookDistance)
        {
            currentRopeLength = maxHookDistance;
            ReturnHook();
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

            //TO GAME MANAGER -  *AFTER* RETURNED
            if (carriedMainLetterId != null)
            {
                OnMainLetterDelivered?.Invoke(carriedMainLetterId);
                carriedMainLetterId = null;

                if (carriedLetterTransform != null)
                {
                    Destroy(carriedLetterTransform.gameObject);
                    carriedLetterTransform = null;
                }
            }
        }

        ApplyRopeLength();
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
        bool shouldCollectAsMain = GameManager.instance.TryGetLetter(letter.letterID);

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
            Debug.Log("Wrong Letter Hit");
        }

        ReturnHook();
    }


}
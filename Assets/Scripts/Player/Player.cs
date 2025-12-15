using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private Transform hookTransform;
    [SerializeField] private float maxHookDistance = 5f;
    [SerializeField] private LineRenderer ropeLine;

    private Letter caughtLetter;
    public float hookSpeed;
    public float rotationSpeed = 10f;

    private bool isRotating = true;
    private bool isShooting = false;
    private bool isReturning = false;

    private Vector3 hookStartLocalPos;
    private Vector3 shootDirection;

    void Start()
    {
        // RECORD START POSITION
        if (hookTransform != null)
        {
            hookStartLocalPos = hookTransform.localPosition;
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ActivateHook();
        }

        //ROTATION
        if (isRotating == true && hookTransform != null)
        {
            hookTransform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime);
            
        }

        //HOOK EXIT
        if (isShooting == true && hookTransform != null)
        {
            hookTransform.position += shootDirection * hookSpeed * Time.deltaTime;

            //CHANGE BOOLIANS IF DIDNT CATCH A LETTER & CROSSED MaxHookDistance
            float dist = Vector3.Distance(hookTransform.position, transform.position);
            if (dist >= maxHookDistance && caughtLetter == null)
            {
                isShooting = false;
                isReturning = true;
            }
        }


        //RETURN
        if (isReturning == true && hookTransform != null)
        {
            hookTransform.localPosition = Vector3.MoveTowards(hookTransform.localPosition, hookStartLocalPos, hookSpeed * Time.deltaTime);

            if (hookTransform.position == hookStartLocalPos)
            {
                hookTransform.localPosition = hookStartLocalPos;
                isReturning = false;
                isRotating = true;

                if (caughtLetter != null)
                {
                    caughtLetter.Collect();
                    caughtLetter = null;
                }
            }
        }
        // ROPE BY LINE RENDERER
        if (ropeLine != null)
        {
            ropeLine.SetPosition(0, transform.position);
            ropeLine.SetPosition(1, hookTransform.position);

        }
    }

    private void ActivateHook()
    {
        if (isReturning == false && isRotating == true)
        {
            isRotating = false;
            isShooting = true;
            ShootHook();
        }
    }

    private void ShootHook()
    {
        shootDirection = -hookTransform.up;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isShooting) 
        {
            return;
        };


        Letter letter = other.GetComponent<Letter>();
        {
            if (letter == null) return;
        }
        

        //if (!letter.isMainLetter)
        //{
        //    return;
        //}
        bool isMainLetter = GameManager.instance.TryGetLetter(letter.letterID);
        GameObject letterObj = other.gameObject;
        if (caughtLetter == null)
        {
            caughtLetter = letter;
            caughtLetter.transform.SetParent(hookTransform);

            isShooting = false;
            isReturning = true;
        }
    }


}

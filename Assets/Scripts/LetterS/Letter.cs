using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letter : MonoBehaviour
{
    public LetterDataSO data;

    public SpriteRenderer spriteRenderer;

    public bool isMainLetter;
    public string letterID;

    private Coroutine deliveryRoutine;

    [SerializeField] private float riseDistance = 0.7f;
    [SerializeField] private float riseDuration = 0.25f;

    //public static event Action<Letter> OnCollected;
    private void Awake()
    {

    }
    void Start()
    {

    }

    void Update()
    {

    }

    public void Init(LetterDataSO data, Sprite sprite, bool isMainLetter)
    {
        this.data = data;
        this.isMainLetter = isMainLetter;
        this.letterID = data.id;
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;

            gameObject.AddComponent<PolygonCollider2D>();
        }
    }
    public void PlayFxAndDisable()
    {        
        //if (deliveryRoutine != null)
        //{
        //    StopCoroutine(deliveryRoutine);
        //}

        deliveryRoutine = StartCoroutine(DeliverRoutine());
    }

    private IEnumerator DeliverRoutine()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) 
        {
            col.enabled = false;
        }

        transform.rotation = Quaternion.identity;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + Vector3.up * riseDistance;

        float elapsedTime = 0f;

        while (elapsedTime < riseDuration)
        {
            elapsedTime += Time.deltaTime;

            float normalizedProgress = elapsedTime / riseDuration;
            if (normalizedProgress > 1f)
            {
                normalizedProgress = 1f;
            }

            transform.position = Vector3.Lerp(startPosition, endPosition, normalizedProgress);

            yield return null;
        }

        gameObject.SetActive(false);

        deliveryRoutine = null;
    }

}



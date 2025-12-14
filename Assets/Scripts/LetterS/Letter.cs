using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letter : MonoBehaviour
{
    public LetterDataSO data;

    public SpriteRenderer spriteRenderer;

    public bool isMainLetter;

    public static event Action<Letter> OnCollected;


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
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;

            gameObject.AddComponent<PolygonCollider2D>();
        }
    }

    public void Collect()
    {
        OnCollected?.Invoke(this);
        Destroy(gameObject);
    }
}

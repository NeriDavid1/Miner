using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinerGameMode
{
    public class Letter : MonoBehaviour
    {
        public LetterDataSO data;

        public SpriteRenderer spriteRenderer;

        public bool isMainLetter;
        public string letterID;

        private Coroutine deliveryRoutine;

        [SerializeField] private AudioSource audioSource;

        [SerializeField] private float riseDistance = 0.7f;
        [SerializeField] private float riseDuration = 0.25f;

        [SerializeField] private float scaleMultiplier = 1.8f;
        [SerializeField] private float scaleDuration = 0.18f;
        [SerializeField] private float moveToTargetDuration = 0.12f;

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

        private void PlayVoiceClip()
        {
            if (audioSource == null)
            {
                return;
            }

            if (data == null)
            {
                return;
            }

            if (data.voiceClip == null)
            {
                return;
            }

            audioSource.PlayOneShot(data.voiceClip);
        }

        public void PlayFxAndDisable(Transform deliverTarget)
        {
            deliveryRoutine = StartCoroutine(DeliverRoutine(deliverTarget));
        }



        private IEnumerator DeliverRoutine(Transform deliverTarget)
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = false;
            }

            transform.rotation = Quaternion.identity;

            // SCALE
            Vector3 startScale = transform.localScale;
            Vector3 endScale = startScale * scaleMultiplier;

            //MOVE TO CENTER
            if (deliverTarget != null)
            {
                if (moveToTargetDuration > 0f)
                {
                    Vector3 moveStartPosition = transform.position;
                    Vector3 moveEndPosition = deliverTarget.position;

                    float moveElapsedTime = 0f;

                    while (moveElapsedTime < moveToTargetDuration)
                    {
                        moveElapsedTime += Time.deltaTime;

                        float moveProgress = moveElapsedTime / moveToTargetDuration;
                        if (moveProgress > 1f) moveProgress = 1f;

                        transform.position = Vector3.Lerp(moveStartPosition, moveEndPosition, moveProgress);

                        yield return null;
                    }
                }
                transform.position = deliverTarget.position;
            }

            //SCALE ONLY IN CENTER
            if (scaleDuration > 0f)
            {
                float scaleElapsedTime = 0f;

                while (scaleElapsedTime < scaleDuration)
                {
                    scaleElapsedTime += Time.deltaTime;

                    float scaleProgress = scaleElapsedTime / scaleDuration;

                    if (scaleProgress > 1f) 
                    { 
                        scaleProgress = 1f;
                    }

                    if (deliverTarget != null)
                    {
                        transform.position = deliverTarget.position;
                    }
                    transform.localScale = Vector3.Lerp(startScale, endScale, scaleProgress);
                    yield return null;
                }
            }

            // ENSURE SCALE BEFORE RISE
            transform.localScale = endScale;

            PlayVoiceClip();


            //RISE
            Vector3 riseStartPosition = transform.position;
            Vector3 riseEndPosition = riseStartPosition + Vector3.up * riseDistance;

            float riseElapsedTime = 0f;

            while (riseElapsedTime < riseDuration)
            {
                riseElapsedTime += Time.deltaTime;

                float riseProgress = riseElapsedTime / riseDuration;
                if (riseProgress > 1f) riseProgress = 1f;

                transform.position = Vector3.Lerp(riseStartPosition, riseEndPosition, riseProgress);

                yield return null;
            }
            gameObject.SetActive(false);
            deliveryRoutine = null;
        }
    }
}


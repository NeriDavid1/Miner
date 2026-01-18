using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace MinerGameMode
{
    public class MainLetterUI : MonoBehaviour
    {
        [SerializeField] private Transform wordPanel;
        [SerializeField] private Image letterImagePrefab;
        [SerializeField] private LettersDataBase lettersDataBase;
    
        private List<Image> letterImages = new List<Image>();
    
        private void OnEnable()
        {
            MinerMode.OnLevelStarted += HandleLevelStarted;
            MinerMode.OnWordIndexChanged += PaintColor;
    
            if (MinerMode.instance != null)
            {
                if (MinerMode.instance.level != null)
                {
                    HandleLevelStarted(MinerMode.instance.level);
                }
            }
        }
    
        private void OnDisable()
        {
            MinerMode.OnLevelStarted -= HandleLevelStarted;
            MinerMode.OnWordIndexChanged -= PaintColor;
        }
    
        private void HandleLevelStarted(LevelDataSO level)
        {
    
            if (lettersDataBase == null || wordPanel == null || letterImagePrefab == null || level == null)
            {
                Debug.Log("MainLetterUI OR level missing references.");
                return;
            }
    
            ClearUI();
    
            // SPAWN IMAGE PER LETTER IF TargetWord IS FILLED
            if (level.targetWord != null)
            {
                if (level.targetWord != "")
                {
                    BuildWordUI(level.targetWord);
                    return;
                }
            }
            // SPAWN SINGLE LETTER IF TargetWord IS NOT FILLED
            CreateLetterImage(level.mainLetter);
        }
    
        private void BuildWordUI(string targetWord)
        {
            // CALLS CreateLetterImage() ONCE PER LETTER IN TargetWord
            for (int i = 0; i < targetWord.Length; i++)
            {
                //Substring TO EXTRACT SINGLE LETTER AS ID
                string letterId = targetWord.Substring(i, 1);
                CreateLetterImage(letterId);
            }
        }
    
        private void CreateLetterImage(string letterId)
        {
            Sprite letterSprite = lettersDataBase.GetSpriteByName(letterId);
    
            Image newImage = Instantiate(letterImagePrefab, wordPanel);
            newImage.sprite = letterSprite;
            newImage.enabled = (letterSprite != null);
    
            newImage.color = Color.black;
    
            letterImages.Add(newImage);
        }
    
        private void PaintColor(int wordIndex)
        {
            int collectedIndex = wordIndex - 1;
    
            if (collectedIndex < 0)
            {
                return;
            }
    
            if (collectedIndex >= letterImages.Count)
            {
                return;
            }
    
            Image collectedImage = letterImages[collectedIndex];
    
            if (collectedImage == null)
            {
                return;
            }
    
            collectedImage.color = Color.white;
        }
    
        private void ClearUI()
        {
            letterImages.Clear();
    
            for (int i = wordPanel.childCount - 1; i >= 0; i--)
            {
                Destroy(wordPanel.GetChild(i).gameObject);
            }
        }
    }
}

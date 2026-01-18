using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MinerGameMode
{
    [CreateAssetMenu(menuName = "GoldMiner/LettersDataBase")]
    
    public class LettersDataBase : ScriptableObject
    {
    
        public List<Sprite> lettersSprites;
    
        private Dictionary<string, Sprite> spriteMap;
    
        public void Init()
        {
            if (spriteMap != null) return;
    
            spriteMap = new Dictionary<string, Sprite>();
            foreach (var sprite in lettersSprites)
            {
                string key = sprite.name.Replace("L_", "").Replace("U_", "").Trim();
                if (!spriteMap.ContainsKey(key))
                {
                    spriteMap.Add(key, sprite);
                }
                else
                {
                    Debug.LogWarning($"Duplicate sprite name: {key}");
                }
            }
        }
    
        public Sprite GetSpriteByName(string name)
        {
            Init();
    
            Debug.Log($"Looking for sprite with name: '{name}'");
    
            if (spriteMap.TryGetValue(name, out Sprite sprite))
            {
                Debug.Log($"Found sprite: {name}");
                return sprite;
            }
    
            Debug.LogWarning($"No sprite found for letter: '{name}'");
            return null;
        }
    
    }
}

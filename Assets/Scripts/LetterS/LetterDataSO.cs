using UnityEngine;


namespace MinerGameMode
{
    [CreateAssetMenu(menuName = "GoldMiner/LetterDataSO")]
    public class LetterDataSO : ScriptableObject
    {
        public string id;
        public AudioClip voiceClip;
    }
}

using UnityEngine;

[CreateAssetMenu(menuName = "GoldMiner/LetterDataSO")]
public class LetterDataSO : ScriptableObject
{
    public string id;
    public AudioClip voiceClip;
    public float beatTime = 0.2f;

    //public Sprite sprite;
    //public float waight;
    //public int score;
}
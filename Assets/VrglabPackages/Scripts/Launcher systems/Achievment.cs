using UnityEngine;

[CreateAssetMenu(fileName = "Achievment", menuName = "launcher systems/Achivment")]
public class Achievment : ScriptableObject
{
    public Sprite Image;
    public string name, description;

    [Header("Id types")]
    public string steam_id, googleplay_id;
    public int gameJolt_id;


    public bool unlocked { get; set; }
}

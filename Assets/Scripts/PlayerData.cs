using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int PlayerLevel;
    public int PlayerScore;
    public int PersonalBest;

    public bool ShowTutorial;
    public bool BetweenLevelFlag;
    public bool MusicMuted;
    public bool SoundMuted;
    public float MusicLevel;
    public float SoundLevel;
    public int GameMode;
    public int ArcadeDifficulty;
    public int ArcadePuzzleSize;
    public int ArcadeQuestWords;



    public PlayerData()
    {
        PlayerLevel = GameManager.Instance.PlayerLevel;
        PlayerScore = GameManager.Instance.PlayerScore;
        PersonalBest = GameManager.Instance.PersonalBest;

        GameMode = (int)GameManager.Instance.Mode;
        if (GameManager.Instance.Mode == GameManager.GameMode.Arcade)
        {
            ArcadeDifficulty = (int)GameManager.Instance.ArcadeSettings.WordDifficulty;
            ArcadePuzzleSize = GameManager.Instance.ArcadeSettings.NumPuzzleLetters;
            ArcadeQuestWords = GameManager.Instance.ArcadeSettings.MaxQuestWords;
        }

        ShowTutorial = GameManager.Instance.ShowTutorial;
        MusicMuted = GameManager.Instance.MusicMuted;
        SoundMuted = GameManager.Instance.SoundMuted;
        MusicLevel = GameManager.Instance.MusicLevel;
        
        SoundLevel = GameManager.Instance.SoundLevel;
        BetweenLevelFlag = GameManager.Instance.BetweenLevelFlag;
    }
}

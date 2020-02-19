using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class GameManager : MonoBehaviour
{
    private static GameManager _inst;
    public static GameManager Instance { get { return _inst; } }

    // all clearable, player-specific data default constants
    public const int DEFAULT_PLAYERLEVEL = 1;
    public const int DEFAULT_PLAYERPOINTS = 0;
    public const int DEFAULT_PERSONAL_BEST = 0;
    public const bool DEFAULT_BETWEEN_LEVELS = true; // between levels == true means that player keeps progress
    public const bool DEFAULT_TUTORIAL_ACTIVE_FLAG = false; // between levels == true means that player keeps progress


    public DictionaryManager dictionaryManager;

    // Game Mode
    public enum GameMode { Normal, Arcade};
    public GameMode Mode { get; set; } = GameMode.Normal;
    public LevelSettings ArcadeSettings { get; set; } = null;


    // Custom Settings
    public bool ShowTutorial { get; set; } = true;
    public bool MusicMuted { get; set; } = false;
    public bool SoundMuted { get; set; } = false;
    public float MusicLevel { get; set; } = 0f;
    public float SoundLevel { get; set; } = 0f;

    // These all count as clearable, player-specific data
    public int PlayerLevel { get; set; } = DEFAULT_PLAYERLEVEL;
    public int PlayerScore { get; set; } = DEFAULT_PLAYERPOINTS;
    public int PersonalBest { get; set; } = DEFAULT_PERSONAL_BEST;
    public bool BetweenLevelFlag { get; set; } = DEFAULT_BETWEEN_LEVELS;
    public bool TutorialActiveFlag { get; set; } = DEFAULT_TUTORIAL_ACTIVE_FLAG;
    public Results PlayerResults { get; set; } = null;


    // And this checks to see if it's special in any way
    public bool PlayerDataExists { get { // check to make sure all relevant variables are their default values
            return !(PlayerLevel == DEFAULT_PLAYERLEVEL && PersonalBest == DEFAULT_PERSONAL_BEST);

        } }

    // Difficulty
    public enum DifficultyLevel { Easy, Normal, Hard, Expert }
   // public DifficultyLevel difficulty;
    public float[] DifficultyQuantiles { get; } = { 0.125f, 0.25f, 0.5f, 1f };
    public string[] DifficultyNames { get; } = { "Easy", "Normal", "Hard", "Expert" };
    //public float DifficultyQuantile
    //{
    //    get
    //    {
    //        return DifficultyQuantiles[(int)difficulty];
    //    }
    //}

    public class LevelSettings
    {
        public string Name { get; set; } = "Some Level Settings";
        public DifficultyLevel WordDifficulty { get; set; } = DifficultyLevel.Easy;
        public int MaxQuestWords { get; set; } = 1;
        public int NumPuzzleLetters { get; set; } = 7;

        public LevelSettings(string name, DifficultyLevel wordDifficulty, int numPuzzleLetters, int maxQuestWords)
        {
            Name = name;
            WordDifficulty = wordDifficulty;
            MaxQuestWords = maxQuestWords;
            NumPuzzleLetters = numPuzzleLetters;
        }
    }

    public class Results
    {
        public int Level { get; set; }
        public Results(int level)
        {
            Level = level;
        }
    }

    #region NORMALLEVELSETTINGS

    public LevelSettings[] normalLevelSettings = {
        new LevelSettings("Level 01", DifficultyLevel.Easy, 5, 1),
        new LevelSettings("Level 02", DifficultyLevel.Easy, 5, 1),
        new LevelSettings("Level 03", DifficultyLevel.Easy, 6, 1),
        new LevelSettings("Level 04", DifficultyLevel.Easy, 6, 1),
        new LevelSettings("Level 05", DifficultyLevel.Easy, 6, 1),
        new LevelSettings("Level 06", DifficultyLevel.Easy, 7, 1),
        new LevelSettings("Level 07", DifficultyLevel.Easy, 7, 1),
        new LevelSettings("Level 08", DifficultyLevel.Easy, 7, 1),
        new LevelSettings("Level 09", DifficultyLevel.Easy, 7, 1),
        new LevelSettings("Level 10", DifficultyLevel.Easy, 7, 3),
        new LevelSettings("Level 11", DifficultyLevel.Easy, 7, 3),
        new LevelSettings("Level 12", DifficultyLevel.Easy, 7, 3),
        new LevelSettings("Level 13", DifficultyLevel.Easy, 7, 3),
        new LevelSettings("Level 14", DifficultyLevel.Easy, 7, 3),
        new LevelSettings("Level 15", DifficultyLevel.Normal, 7, 3),
        new LevelSettings("Level 16", DifficultyLevel.Normal, 7, 3),
        new LevelSettings("Level 17", DifficultyLevel.Normal, 7, 3),
        new LevelSettings("Level 18", DifficultyLevel.Normal, 7, 3),
        new LevelSettings("Level 19", DifficultyLevel.Normal, 7, 3),
        new LevelSettings("Level 20", DifficultyLevel.Normal, 7, 3),
        new LevelSettings("Level 21", DifficultyLevel.Normal, 7, 3),
        new LevelSettings("Level 22", DifficultyLevel.Normal, 7, 3),
        new LevelSettings("Level 23", DifficultyLevel.Normal, 7, 3),
        new LevelSettings("Level 24", DifficultyLevel.Normal, 7, 3),
        new LevelSettings("Level 25", DifficultyLevel.Normal, 7, 3),
        new LevelSettings("Level 26", DifficultyLevel.Hard, 7, 3)
    };

    #endregion NORMALLEVELSETTINGS

    private void Awake()
    {
        if (_inst != null && _inst != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _inst = this;
            DontDestroyOnLoad(this.gameObject);
        }

        LoadPlayerData();
    }

    public void Start()
    {
        dictionaryManager.LoadDictionary();


        //  If player wasn't betwen levels, then they quit during a level, and lose progress.
        if (!BetweenLevelFlag)
            PlayerLevel = DEFAULT_PLAYERLEVEL; // DROP THE RESET HAMMER1!!!!!!
    }

    public void ResetPlayerData()
    {
        PlayerLevel = DEFAULT_PLAYERLEVEL;
        PersonalBest = DEFAULT_PERSONAL_BEST;
        SavePlayerData();
    }

    public float GetLevelTime(int level)
    {
        return 120f;
    }

    public bool IsLastLevel(int level)
    {
        return false;
    }

    public LevelSettings GetLevelSettings(int level)
    {
        if(Mode == GameMode.Arcade)
        {
            return ArcadeSettings;
        }
        else
        {
            if (level - 1 < normalLevelSettings.Length)
                return normalLevelSettings[level - 1];
            else
                return normalLevelSettings[normalLevelSettings.Length - 1];
        }
    }

    //public void SaveLevel(int level)
    //{
    //    PlayerLevel = level;
    //    SavePlayerData();
    //}

    //public void SaveLevel() // we call this override to reset the save
    //{
    //    PlayerLevel = DEFAULT_PLAYERLEVEL;
    //    SavePlayerData();
    //}

    //public void SaveBetwenLevelFlag(bool value)
    //{
    //    BetweenLevelFlag = value;
    //    SavePlayerData();
    //}

    public void ResetPlayerLevel()
    {
        PlayerLevel = DEFAULT_PLAYERLEVEL;
        PlayerScore = DEFAULT_PLAYERPOINTS;
    }

    public void SaveResults()
    {
        PlayerResults = new Results(PlayerLevel);
    }

    public void SavePlayerData(int? level = null, int? score = null, bool? betweenLevels = null)
    {

        PlayerLevel = level ?? PlayerLevel;
        PlayerScore = score ?? PlayerScore;
        BetweenLevelFlag = betweenLevels ?? BetweenLevelFlag;

        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "ascend.data";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(); // PlayerData class is smart enough to grab most options from GameManager namespace
        formatter.Serialize(stream, data);
        stream.Close();
    }

    private void LoadPlayerData()
    {
        string path = Application.persistentDataPath + "ascend.data";
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            PlayerData data;
            try
            {
                data = formatter.Deserialize(stream) as PlayerData;
            }catch (SerializationException)
            {
                Debug.LogWarning("SerializationException: Couldn't load save data");
                return;
            }
            finally
            {
                stream.Close();
            }




            ShowTutorial = data.ShowTutorial;
            MusicLevel = data.MusicLevel;
            SoundLevel = data.SoundLevel;
            MusicMuted = data.MusicMuted;
            SoundMuted = data.SoundMuted;
            PlayerLevel = data.PlayerLevel;
            PlayerScore = data.PlayerScore;
            PersonalBest = data.PersonalBest;
            BetweenLevelFlag = data.BetweenLevelFlag;
            Mode = (GameMode)data.GameMode;
            if(Mode == GameMode.Arcade)
            {
                ArcadeSettings = new LevelSettings(
                    "Arcade Load", 
                    (DifficultyLevel)data.ArcadeDifficulty,
                    data.ArcadePuzzleSize,
                    data.ArcadeQuestWords);
            }
        }
        else
        {
            Debug.Log("No saved player data; sticking with defaults");
        }
    }
}

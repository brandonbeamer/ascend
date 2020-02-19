using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class LevelManager : MonoBehaviour
{
    private DictionaryManager dm;
    private SoundFX sfx;
    public LetterManager entry;
    public LetterManager puzzle;
    public QuestTextManager qtm;


    public int Score { get; private set; }
    public int Chain { get; private set; } = 1;
    private string lastSubmission = "";

    public int Level { get; private set; } = GameManager.DEFAULT_PLAYERLEVEL;
    public float ExpireTime { get; private set; } = 0;

    //public Vector3 SimulatedCameraVelocity { get; private set; } = new Vector3(0, 1);

    public enum LevelState { Playing, Idle }
    public LevelState State { get; set; } = LevelState.Idle;
    public bool Playing {   get { return State == LevelState.Playing; }      }

    private string[] allLetters = new string[26];
    private List<string> previousSubmissions = new List<string>();

    private void Awake()
    {
        int i = 0;
        foreach (char c in "abcdefghijklmnopqrstuvwxyz")
        {
            allLetters[i++] = c.ToString();
        }

        dm = GameManager.Instance.dictionaryManager;
        sfx = FindObjectOfType<SoundFX>();
        if(sfx == null)
        {
            Debug.LogError("Couldn't find 'SoundFX' object.");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Level = GameManager.Instance.PlayerLevel;
        Score = GameManager.Instance.PlayerScore;
        
        StageDirector.Instance.DoSceneIntro();
    }
    void Update()
    {
        UpdateInput();

        if (ExpireTime != 0 && Time.time > ExpireTime)
        {
            if(qtm.QuestComplete)
                EndLevel(success: true);
            else
                EndLevel(success: false);
        }

    }

    // Level State Changers
    public void StartNewLevel()
    {
        GameManager.Instance.BetweenLevelFlag = false; // We are IN a level now, not between them, the player is committed.

        GameManager.LevelSettings settings = GameManager.Instance.GetLevelSettings(Level);

        Debug.Log(settings.NumPuzzleLetters);

        string newWord = dm.GetRandomWord(settings.NumPuzzleLetters, GameManager.Instance.DifficultyQuantiles[(int)settings.WordDifficulty]);

        entry.SetLetters("");
        puzzle.SetLetters(newWord);
        puzzle.Shuffle();
        SetQuestWords(newWord, settings.MaxQuestWords);
        previousSubmissions.Clear();

        ExpireTime = Time.time + GameManager.Instance.GetLevelTime(Level);

        State = LevelState.Playing;
    }

    void EndLevel(bool success = false)
    {
        entry.SetLetters("");
        puzzle.SetLetters("");
        ExpireTime = 0;
        Chain = 1;

        if (success) { // move on
            if(GameManager.Instance.IsLastLevel(Level))
            {
                // WINNING CONDITION?
            }
            else
            {
                Level++;

                GameManager.Instance.SavePlayerData(level: Level, score: Score, betweenLevels: true);
                State = LevelState.Idle;
                StageDirector.Instance.DoLevelTransition();
            }
                
        }
        else // lose
        {
            GameManager.Instance.SaveResults();

            GameManager.Instance.ResetPlayerLevel();
            GameManager.Instance.SavePlayerData(betweenLevels:true);
            State = LevelState.Idle;
            StageDirector.Instance.DoLoseGame();
        }

    }

    void EndGameplay()
    {
        entry.SetLetters("");
        puzzle.SetLetters("");
        Chain = 1;
        State = LevelState.Idle;

        GameManager.Instance.ResetPlayerLevel();
        GameManager.Instance.SavePlayerData(betweenLevels:true);
        StageDirector.Instance.DoLeaveGame();
    }

    // Smaller Tasks

    
    void SetQuestWords(string newWord, int maxQuestWords)
    {
        // Get Quest Words Setup
        List<string> anagrams = dm.GetAnagramsOf(newWord);
        anagrams.RemoveAll(x => x.Length < 5);

        List<float> quantiles = new List<float>();
        List<string> questWords = new List<string>();
        foreach (string w in anagrams)
        {
            quantiles.Add(dm.GetQuantile(w));
        }
        questWords.Add(newWord);
        float maxQuantile = dm.GetQuantile(newWord);
        
        for (int i = 0; i < anagrams.Count; i++)
        {
            if (questWords.Count == maxQuestWords)
                break;

            if (anagrams[i] == newWord)
                continue;

            if (quantiles[i] <= maxQuantile)
            {
                questWords.Add(anagrams[i]);
            }
        }

        qtm.SetKeyWords(questWords);
    }

    int ScoreSubmission(string submission)
    {
        if (dm.WordToKey(lastSubmission) == dm.WordToKey(submission))
            Chain++;
        else
            Chain = 1;

        lastSubmission = submission;
        return dm.ScoreWord(submission) * Chain;
    }

    public void SpendScore(int amount)
    {
        Score -= amount;
    }

    // Input related
    void UpdateInput()
    {
        if (!Playing || GameManager.Instance.TutorialActiveFlag) return;

        foreach(string c in allLetters)
        {
            if(Input.GetKeyDown(c))
            {
                TypeLetter(c);
            }
        }

        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            RemoveLetter();
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(entry.IsEmpty()) 
            {
                EndGameplay();
            }
            else
            {
                CancelEntry();
            }
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            if(!entry.IsEmpty())
            {
                SubmitEntry();
            }
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            ShufflePuzzle();
        }

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (qtm.QuestComplete)
            {
                EndLevel(success:true);
            }
        }
    }

    void TypeLetter(string c)
    {
        if(puzzle.UseLetter(c[0]))
        {
            entry.AddLetter(c[0]);
        }
    }

    void RemoveLetter()
    {
        char c = entry.RemoveLastLetter();
        if(c != '\0')
        {
            puzzle.FreeLetter(c);
        }
    }

    void ShufflePuzzle()
    {
        puzzle.Shuffle();
    }

    void SubmitEntry()
    {
        string submission = entry.GetString();
        if(previousSubmissions.Contains(submission))    // repeat entry
        {
            //entry.DramaticallySubmit(submission.Length, fizzle: true);
            entry.DramaticallyClear();
            puzzle.FreeAllLetters();
            Chain = 1;
            Debug.Log("Repeat: chain reset");
        }
        else                                            // new entry
        {
            if(dm.IsAnagramOf(submission, puzzle.GetString()))  // success!
            {
                int thisScore = ScoreSubmission(submission);
                Score += thisScore;
                previousSubmissions.Add(submission);
                entry.DramaticallySubmit(thisScore);
                puzzle.FreeAllLetters();
                bool foundQuestWord = qtm.RevealWord(submission);

                if(foundQuestWord)
                {
                    // play the arpeggio
                    sfx.PlayArp();
                }
                else
                {
                    // play a note
                    
                    sfx.PlayScaleNote(Mathf.Min(Chain, 8));
                }

            }
            else                                                //nope
            {
                entry.DramaticallyClear();
                puzzle.FreeAllLetters();
                Chain = 1;
                Debug.Log("Non-anagram: chain reset");
            }
        }
    }

    void CancelEntry()
    {
        entry.DramaticallyClear();
        puzzle.FreeAllLetters();
    }


}

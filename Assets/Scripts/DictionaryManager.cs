using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictionaryManager : MonoBehaviour
{
    public TextAsset dictionaryFile;    // wordprint => anagramList lookup
    public TextAsset[] wordlistFiles;      // wordlists sorted by frequency, all words in lists assumed to be same length
    public TextAsset trigramFile;       // trigram log frequencies

    private Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
    private Dictionary<string, float> trigrams = new Dictionary<string, float>();
    private List<List<string>> wordlists = new List<List<string>>(); // sorted by difficulty, index = 0 -> length 3 words

    private float max_rareness = 0;

    private void Awake()
    {
        for(int i = 0; i < wordlistFiles.Length; i++)
        {
            wordlists.Add(new List<string>());
        }
    }

    // Start is called before the first frame update
    void Start()
    {

       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadDictionary()
    {
        // --------- load the anagram dictionary
        string txt = dictionaryFile.ToString().TrimEnd();
        string[] lines = txt.Split('\n');
        foreach(string line in lines)
        {
            string[] fields = line.Trim().Split('\t');
            string[] anagrams = fields[1].Split(',');
            dic.Add(fields[0], new List<string>(anagrams));


        }

        // ---------- load the trigram dictionary
        txt = trigramFile.ToString().TrimEnd();
        lines = txt.Split('\n');
        foreach(string line in lines)
        {
            string[] fields = line.Trim().Split('\t');
            string trigram = fields[0];
            float rareness = float.Parse(fields[3], System.Globalization.CultureInfo.InvariantCulture);
            trigrams.Add(trigram, rareness);
            if (rareness > max_rareness)
                max_rareness = rareness;
        }

        // ---------- load the wordlists
        foreach(TextAsset file in wordlistFiles)
        {
            txt = file.ToString().TrimEnd();
            lines = txt.Split('\n');
            List<string> wl = GetWordListByLength(lines[0].Trim().Split('\t')[0].Length);
            foreach(string line in lines)
            {
                wl.Add(line.Trim().Split('\t')[0]);
            }
        }

        Debug.Log(dic.Count + " words loaded.");

    }

    public string GetRandomWord(int length = 7, float quantile = 1)       // length = 0 indicates any length
                                                                          // quantile (0 to 1) controls difficulty: higher = harder
    {
        List<string> wl = GetWordListByLength(length);
        string word = wl[Random.Range(0, (int)(quantile * wl.Count))];
        return word;
    }

    public bool IsAnagramOf(string candidate, string word)
    {
        List<string> anagrams;
        if(dic.TryGetValue(WordToKey(word), out anagrams))
        {
            if(anagrams.Contains(candidate))
            {
                return true;
            }
            return false;
        }
        else
        {
            // should never happen
            Debug.LogWarning(word + " is not in the dictionary!");
            return false;
        }
    }

    public List<string> GetAnagramsOf(string word)
    {
        List<string> anagrams;
        dic.TryGetValue(WordToKey(word), out anagrams);
        return new List<string>(anagrams);
    }

    public float GetQuantile(string word)
    {
        List<string> wl = GetWordListByLength(word.Length);
        int index = wl.IndexOf(word);

        if (index < 0) return 1f;

        return (float)index / wl.Count;
    }

    public string WordToKey(string word)
    {
        List<char> lst = new List<char>(word.ToLower().ToCharArray());
        lst.Sort();
        return new string(lst.ToArray());
    }

    public int ScoreWord(string word)
    {
        word = "^" + word + "$";

        int total_score = 0;

        for (int i = 0; i < word.Length - 2; i++)
        {
            float rareness;
            if(!trigrams.TryGetValue(word.Substring(i, 3), out rareness))
            {
                rareness = max_rareness;
            }

            total_score += Mathf.Max((int)(rareness), 1);
        }

        return total_score;
    }

    private List<string> GetWordListByLength(int length)
    {
        return wordlists[length - 3];
    }
}

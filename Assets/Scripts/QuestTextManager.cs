using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestTextManager : MonoBehaviour
{
    public LevelManager lm;
    private Text questText;
    public bool QuestComplete { get; private set; } = false;

    private Animator anim;
    private bool lastPlaying;

    private class QuestWordComparer : IComparer<string>
    {
        public int Compare(string a, string b)
        {
            if (a == b) return 0;
            if (a.Length == b.Length)
            {
                return a.CompareTo(b);
            }
            else
            {
                return a.Length < b.Length ? 1 : -1;
            }
        }
    }
    private SortedDictionary<string, bool[]> quests = new SortedDictionary<string, bool[]>(new QuestWordComparer());

    private void Awake()
    {
        questText = GetComponent<Text>();
        anim = GetComponent<Animator>();
        lastPlaying = anim.GetBool("OnScreen");
    }

    public void Update()
    {
        if(lastPlaying != lm.Playing)
        {
            lastPlaying = lm.Playing;
            anim.SetBool("OnScreen", lm.Playing);
        }
    }

    private void UpdateText()
    {
        string text = "";
        QuestComplete = true;

        foreach (string w in quests.Keys)
        {
            Debug.Log("Updating Quest: " + w);
            bool[] mask;
            quests.TryGetValue(w, out mask);
            string line = "";

            for (int i = 0; i < mask.Length; i++)
            {
                if(mask[i])
                {
                    line += w.Substring(i, 1).ToUpper() + " ";
                }
                else
                {
                    QuestComplete = false;
                    line += "_ ";
                }

            }
            Debug.Log("Complete? " + QuestComplete);

            text += line.TrimEnd() + '\n';
        }
        text = text.TrimEnd();

        questText.text = text;
    }

    public void SetKeyWords(List<string> words)
    {
        quests.Clear();
        foreach (string w in words)
        {
            bool[] mask = new bool[w.Length];
            for (int i = 0; i < mask.Length; i++) mask[i] = false;

            quests.Add(w, mask);
        }
        UpdateText();
    }

    public bool RevealWord(string word)
    {
        Debug.Log("Revealing: " + word);
        bool[] mask;
        bool rt = false;
        if(quests.TryGetValue(word, out mask)) {
            for (int i = 0; i < mask.Length; i++)
                mask[i] = true;
            rt = true;
        }
        UpdateText();
        return rt;
    }

    public void RevealRandomLetter()
    {
        foreach(string k in quests.Keys)
        {
            bool[] mask;
            if(!quests.TryGetValue(k, out mask))
            {
                Debug.Log("Somehow managed to fail to get a key from the key list?!");
                return;
            }

            int i = GetRandomUnrevealedIndex(mask);
            if (i == -1) continue;

            mask[i] = true;
        }
        UpdateText();
    }

    public void RevealBookendLetters()
    {
        foreach (string k in quests.Keys)
        {
            bool[] mask;
            if (!quests.TryGetValue(k, out mask))
            {
                Debug.Log("Somehow managed to fail to get a key from the key list?!");
                return;
            }

            if (mask[0] && mask[mask.Length - 1]) continue; // move on if both are already visible

            mask[0] = mask[mask.Length - 1] = true;

        }
        UpdateText();
    }

    public void RevealFiftyFifty()
    {
        foreach (string k in quests.Keys)
        {
            bool[] mask;
            if (!quests.TryGetValue(k, out mask))
            {
                Debug.Log("Somehow managed to fail to get a key from the key list?!");
                return;
            }

            for(int i = 0; i < mask.Length; i++)
            {
                if (Random.value > 0.5f)
                    mask[i] = true;
            }

        }
        UpdateText();
    }
    
    public void RevealInstancesOfLetter(string letter)
    {
        char l = letter[0];
        foreach(string k in quests.Keys)
        {
            bool[] mask;
            if (!quests.TryGetValue(k, out mask))
            {
                Debug.Log("Somehow managed to fail to get a key from the key list?!");
                return;
            }

            for(int i = 0; i < k.Length; i++)
            {
                if(k[i] == l)
                {
                    mask[i] = true;
                }
            }

        }
        UpdateText();
    }

    public void Clear()
    {
        quests.Clear();
        UpdateText();
    }

    private int GetRandomUnrevealedIndex(bool[] mask)
    {
        int unrevealedCount = 0;
        for(int i = 0; i < mask.Length; i++)
            if (!mask[i]) unrevealedCount++;

        if (unrevealedCount == 0) return -1;

        for(int i = 0; i < mask.Length; i++)
        {
            if (mask[i]) continue;

            if(Random.value <= 1f / (unrevealedCount))
            {
                return i;
            }
            unrevealedCount--;
        }

        Debug.LogWarning("SHOULD NOT HAPPEN!");
        return -1;
    }
}

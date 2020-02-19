using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public LevelManager lm;
    public QuestTextManager qtm;
    public Text chainTextPrefab;
    public GameObject upArrow;

    private Text chainTextInst;


    private int lastChain = -1;
    private bool lastComplete = false;

    private bool lastPlaying;

    private void Awake()
    {
        lastPlaying = lm.Playing;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (lastChain != lm.Chain)
            UpdateChain();

        if (lastComplete != qtm.QuestComplete)
            UpdateQuestComplete();

        if (lastPlaying != lm.Playing)
            UpdatePlaying();
    }

    void UpdatePlaying()
    {
        lastPlaying = lm.Playing;

        if(lm.Playing == false && upArrow.activeInHierarchy)
        {
            upArrow.SetActive(false);
        }
    }

    void UpdateChain()
    {
        if(chainTextInst != null)
        {
            Destroy(chainTextInst);
        }

        if(lm.Chain > 1)
        {
            chainTextInst = Instantiate(chainTextPrefab, transform);
            chainTextInst.GetComponent<Text>().text = lm.Chain.ToString() + "-Chain!";
        }

        lastChain = lm.Chain;
    }

    void UpdateQuestComplete()
    {
        if(qtm.QuestComplete)
        {
            upArrow.SetActive(true);
        }
        else
        {
            upArrow.SetActive(false);
        }

        lastComplete = qtm.QuestComplete;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintAbilityManager : MonoBehaviour
{
    public enum HintType { RandomLetter, Bookend, FiftyFifty }

    public LevelManager lm;
    public QuestTextManager qtm;
    public LetterManager entry;
    public LetterManager puzzle;

    public Text label;
    public Image img;
    public int cost = 100;
    public float dimAlpha = 0.5f;
    public KeyCode activateKey;
    public HintType type;
    //public Color unavailableColor;

    private int score = -1;
    public bool Available { get; private set; } = false;
    

    private Animator anim;

    private void Awake()
    {
       //img = GetComponent<Image>();
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        img.color = new Color(img.color.r, img.color.g, img.color.b, dimAlpha);
        label.color = new Color(label.color.r, label.color.g, label.color.b, dimAlpha);
    }

    // Update is called once per frame
    void Update()
    {
        if (score != lm.Score)
            UpdateScore();

        if (Input.GetKeyDown(activateKey))
        {
            if (!Available)
            {
                anim.SetTrigger("Flash");
            }
            else
            {
                Activate();
            }
            
        }

    }

    private void Activate()
    {
        switch (type)
        {
            case HintType.RandomLetter:
                qtm.RevealRandomLetter();
                break;
            case HintType.Bookend:
                qtm.RevealBookendLetters();
                break;
            case HintType.FiftyFifty:
                qtm.RevealFiftyFifty();
                break;
        }
        lm.SpendScore(cost);
    }

    void UpdateScore()
    {
        score = lm.Score;
        if (lm.Score >= cost)
        {
            // available

            //img.color = new Color(img.color.r, img.color.g, img.color.b, 1f);
            label.color = new Color(label.color.r, label.color.g, label.color.b, 1f);
            Available = true;
        }
        else
        {
            // unavailable
            //img.color = new Color(img.color.r, img.color.g, img.color.b, dimAlpha);
            label.color = new Color(label.color.r, label.color.g, label.color.b, dimAlpha);
            Available = false;
        }
        anim.SetBool("Available", Available);
    }
}

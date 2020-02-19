using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public LevelManager lm;

    private Text scoreText;
    private Animator anim;
    private bool lastPlaying;

    private int lastScore = -1;

    private void Awake()
    {
        scoreText = GetComponent<Text>();
        anim = GetComponent<Animator>();
        lastPlaying = anim.GetBool("OnScreen");
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (lastScore != lm.Score)
            UpdateScore();

        if (lastPlaying != lm.Playing)
            UpdatePlaying();
    }

    void UpdatePlaying()
    {
        lastPlaying = lm.Playing;
        anim.SetBool("OnScreen", lm.Playing);
    }

    void UpdateScore()
    {
        scoreText.text = lm.Score.ToString();
        if (lastScore != -1) // don't animate on initial update
            anim.SetTrigger("DoSwell");
        lastScore = lm.Score;
    }
}

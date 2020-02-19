using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuoteTextManager : MonoBehaviour
{
    public LevelManager lm;

    private int level;
    private LevelManager.LevelState levelState;

    private Text text;
    private Animator anim;

    private void Awake()
    {
        level = lm.Level;
        levelState = lm.State;
        text = GetComponent<Text>();
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (levelState != lm.State)
        {
            levelState = lm.State;

        }

        // Update Text
    }
}

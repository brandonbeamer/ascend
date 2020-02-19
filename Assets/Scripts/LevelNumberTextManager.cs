using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelNumberTextManager : MonoBehaviour
{
    public LevelManager lm;

    private int level = -1;
    private LevelManager.LevelState levelState;

    private Text text;
    private Animator anim;

    private void Awake()
    {
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
        if(level != lm.Level)
        {
            level = lm.Level;
            text.text = level.ToString();
        }

        if(levelState != lm.State)
        {
            levelState = lm.State;
        }
    }


}

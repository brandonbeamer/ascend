using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePanelManager : MonoBehaviour
{
    public LevelManager lm;

    private LevelManager.LevelState state;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        state = lm.State;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (state != lm.State)
            UpdateState();
    }

    private void UpdateState()
    {
        state = lm.State;
        if(state == LevelManager.LevelState.Playing)
        {
            anim.SetBool("OnScreen", true);
        }
        else
        {
            anim.SetBool("OnScreen", false);
        }
    }
}

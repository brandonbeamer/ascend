using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionPanelManager : MonoBehaviour
{
    public LevelManager lm;

    private LevelManager.LevelState lmState;

    private void Awake()
    {
        lmState = lm.State;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(lmState != lm.State)
        {
            lmState = lm.State;

            //
        }

        /*if(lm.State == LevelManager.LevelState.Transition)
        {
            // We only accept inputs during this state

            if(Input.anyKeyDown)
            {
                lm.State = LevelManager.LevelState.TransitionOut;
            }
        }*/
    }
}

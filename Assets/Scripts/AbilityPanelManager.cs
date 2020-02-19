using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPanelManager : MonoBehaviour
{
    public LevelManager lm;

    private Animator anim;
    private bool lastPlaying;

    private void Awake()
    {
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
        if (lm.Playing != lastPlaying)
        {
            lastPlaying = lm.Playing;
            anim.SetBool("OnScreen", lm.Playing);
        }

    }
}

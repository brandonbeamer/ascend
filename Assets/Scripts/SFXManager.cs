using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{

    private Dictionary<string, AudioSource> lib = new Dictionary<string, AudioSource>();

    // Start is called before the first frame update
    void Start()
    {
        // Get and organize all children
        foreach(Transform child in transform)
        {
            lib.Add(child.gameObject.name, child.gameObject.GetComponent<AudioSource>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySFX(string name)
    {
        AudioSource src;
        if (!lib.TryGetValue(name, out src))
            return;
        src.Play();
    }
}

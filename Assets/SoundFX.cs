using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFX : MonoBehaviour
{
    public AudioSource[] scaleNotes = new AudioSource[8];
    public AudioSource arp;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayScaleNote(int degree)
    {
        degree--; // note array is 0-based where scale degrees are 1-based
        if(degree < 0 || degree >= scaleNotes.Length)
        {
            Debug.LogError("No note for scale degree " + (degree + 1));
            return;
        }

        scaleNotes[degree].Play();
    }

    public void PlayArp()
    {
        arp.Play();
    } 
}

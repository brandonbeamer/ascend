using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartBGM : MonoBehaviour
{
    public AudioSource bgm1;
    public AudioSource bgm2;
    public float startDelay;
    public float fadeInTime;
    public float loopTime;

    private float startTime;

    private AudioSource currentlyPlaying;

    // Start is called before the first frame update
    void Start()
    {
        bgm1.Stop();
        bgm2.Stop();
        currentlyPlaying = bgm1;
        bgm1.loop = false;
        bgm2.loop = false;
        startTime = Time.time;
        StartCoroutine(StartCR());
    }

    IEnumerator StartCR()
    {
        if (startDelay > 0)
        {
            while (Time.time < startTime + startDelay)
                yield return null;
        }

        bgm1.Play();

        if (fadeInTime > 0)
        {
            AnimationCurve volCurve = new AnimationCurve();
            volCurve.AddKey(new Keyframe(0f, 0f));
            volCurve.AddKey(new Keyframe(fadeInTime, 1f));

            float fadeInStartTime = Time.time;
            while(Time.time < fadeInStartTime + fadeInTime)
            {
                bgm1.volume = volCurve.Evaluate(Time.time - fadeInStartTime);
                yield return null;
            }

        }




    }

    private void FixedUpdate()
    {
        if(currentlyPlaying.time >= loopTime)
        {
            if(currentlyPlaying == bgm1)
            {
                bgm2.Play();
                currentlyPlaying = bgm2;
            }
            else
            {
                bgm1.Play();
                currentlyPlaying = bgm1;
            }

        }
    }
}

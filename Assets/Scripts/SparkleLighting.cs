using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkleLighting : MonoBehaviour
{
    public GameObject sparkle;
    public float maxIntensity = 2;
    public float minIntensity = 1;

    private Light lt;
    private SpriteRenderer sr;
    private SparkleMotion motion;

    private void Awake()
    {
        lt = GetComponent<Light>();
        sr = sparkle.GetComponent<SpriteRenderer>();
        motion = sparkle.GetComponent<SparkleMotion>();
    }

    // Update is called once per frame
    void Update()
    {
        float normalizedScale = (sparkle.transform.localScale.x - motion.minScale) / (motion.maxScale - motion.minScale);
        float normalizedAlpha = (sr.color.a - motion.minAlpha) / (motion.maxAlpha - motion.minAlpha);
        float normalizedIntensity = (normalizedAlpha + normalizedScale) / 2;

        lt.intensity = minIntensity + normalizedIntensity * (maxIntensity - minIntensity);
    }
}

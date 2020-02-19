using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogMachine : MonoBehaviour
{
    public Camera cam;
    public GameObject prefab;
   // public LevelManager lm;
    public bool Active { get; set; } = true;
    //public float startDensity; // how much fog at Start()

    public bool ignoreStageDirector = false;

    public float meanSpeed;
    public float sdSpeed;

    public float meanSpawnRate;
    public float sdSpawnRate;
    public int   initialFogCount = 50;
    public float meanScale;
    public float sdScale;

    public float minSpeed;
    public float minScale;

    private Vector3 cameraVelocity;

    // Start is called before the first frame update
    void Start()
    {
        if(!ignoreStageDirector)
            cameraVelocity = StageDirector.Instance.SimulatedCameraVelocity;

        StartCoroutine(MakeFog());
        Bounds cBounds = cam.GetComponent<CameraTools>().GetOrthographicBounds();
        float killY = prefab.GetComponent<FogSprite>().killY;

        Vector2 max = new Vector2(cBounds.max.x, transform.position.y + cBounds.extents.y);
        Vector2 min = new Vector2(cBounds.min.x, killY);

        Bounds spawnBounds = new Bounds(
                new Vector3((max.x + min.x) / 2, (max.y + min.y) / 2, transform.position.z), // center
                new Vector3(max.x - min.x, max.y - min.y, 0)                                 // size
        );

        for (int i = 0; i < initialFogCount; i++)
        {
            SpawnFog(spawnBounds);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!ignoreStageDirector && cameraVelocity != StageDirector.Instance.SimulatedCameraVelocity)
        {
            cameraVelocity = StageDirector.Instance.SimulatedCameraVelocity;
            StopAllCoroutines();
            StartCoroutine(MakeFog());
        }
    }

    private IEnumerator MakeFog()
    {
        while(true)
        {
            Bounds cBounds = cam.GetComponent<CameraTools>().GetOrthographicBounds();
            if(Active) SpawnFog(new Bounds(transform.position, cBounds.size));
            float secs = 1f / ((ignoreStageDirector ? 1 : cameraVelocity.y) * randomNormal(meanSpawnRate, sdSpawnRate));
            yield return new WaitForSeconds(Mathf.Abs(secs));

        }
    }

    private void SpawnFog(Bounds bnds) // if provided, will spawn within given bounds
    { 
        float x, y, z;
        y = Random.Range(bnds.min.y, bnds.max.y);
        x = Random.Range(bnds.min.x, bnds.max.x);
        z = transform.position.z + Random.Range(-.5f, .5f);


        float scale = randomNormal(meanScale, sdScale);
        float speed = randomNormal(meanSpeed, sdSpeed);

        if (scale < minScale)
            scale = minScale * 2 - scale;

        if (speed < minSpeed)
            speed = minSpeed * 2 - speed;

        GameObject go = Instantiate(prefab, new Vector3(x, y, z), Quaternion.Euler(new Vector3(0,0,Random.value * 360)), transform);
        go.GetComponent<FogSprite>().IgnoreStageDirector = ignoreStageDirector;
        go.transform.localScale = new Vector3(scale, scale, 1);
        go.GetComponent<FogSprite>().Velocity = new Vector3(0, -speed, 0);
    }

    private float randomNormal(float mean, float sd) // Generates pseudo-random number from standard normal distribution
                                                     // 
    {
        float u1, u2;
        while (true) {
            u1 = Random.value;
            u2 = Random.value;
            if (u1 != 0 && u2 != 0) break;
        }

        float z = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2); //random normal(0,1)
        return Mathf.Clamp(mean + sd * z, -3f * sd, 3f * sd); //random scaled
    }

}

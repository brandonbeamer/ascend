using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTools : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Bounds GetOrthographicBounds()
    {
        Camera cam = GetComponent<Camera>();
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = cam.orthographicSize * 2;
        Bounds bounds = new Bounds(
            cam.transform.position,
            new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
        return bounds;
    }
}

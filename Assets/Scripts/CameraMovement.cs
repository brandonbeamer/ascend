using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraMovement : MonoBehaviour
{
    public LevelManager lm;

    private Camera cam;
    private LevelManager.LevelState state;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void UpdateState()
    {
    }

    public void CameraPanEnd()
    {
    }

}

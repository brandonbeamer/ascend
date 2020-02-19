using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogSprite : MonoBehaviour
{
    private LevelManager lm;
    public float killY = -20;
    public bool IgnoreStageDirector { get; set; } = false;

    //private Vector3 _velocity = new Vector3();
    private Rigidbody2D rb;
    public Vector3 Velocity { get; set; }
    //{
    //    get => _velocity;
    //    set
    //    {
    //        _velocity = value;
    //        rb.velocity = value - StageDirector.SimulatedCameraVelocity;
    //    }
    //}

    //private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        lm = FindObjectOfType<LevelManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = Velocity;
        if (!IgnoreStageDirector)
        {
            rb.velocity = rb.velocity - new Vector2(StageDirector.Instance.SimulatedCameraVelocity.x,
                                                    StageDirector.Instance.SimulatedCameraVelocity.y);
        }

        if (transform.position.y < killY)
            Destroy(gameObject);
    }
}

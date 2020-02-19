using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkleMotion : MonoBehaviour
{
    public float maxSpeed; // units per second
    public float homingForce;
    public float steeringForce;
    public float leavingForce;
    public float arrivalDistance;
    public float flickerSpeed;
    public float maxScale = 1.5f;
    public float minScale = 0.5f;
    public float maxAlpha = 0.6f;
    public float minAlpha = 0f;
    public float startDelay = 0f;   // time before sparkle starts moving
                                    // public float minTargetDistance = 2f;

    public Bounds manualBounds;
    public bool getBoundsFromCamera = true;

    [SerializeField]
    private Vector3 targetPoint;
    
    private float targetAlpha = 1;
    //private float targetScale = 1;
    private bool powered = true;
    private bool leaving = false;
    private float startTime;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Bounds bounds;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetNewRandomTargetPoint();
        SetNewRandomTargetAlpha();
//        SetNewRandomTargetScale();
//        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, Random.value);

        if(getBoundsFromCamera) {
            GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            bounds = camera.GetComponent<CameraTools>().GetOrthographicBounds();
        }
        else
        {
            bounds = manualBounds;
        }
        //Debug.Log("Bounds: " + bounds.min.x + " " + bounds.max.x);

        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - startTime < startDelay)
            return;

        if (powered)
        {
            UpdateMovement();
            UpdateAlpha();
            //UpdateScale();
        }
    }

    void UpdateMovement()
    {
        Vector2 toTarget = (targetPoint - transform.localPosition);
        float targetDist = toTarget.magnitude;
        toTarget = toTarget.normalized;

        float angle = Vector3.SignedAngle(rb.velocity, toTarget, Vector3.forward);
        if (angle < 2f)             // turn right
        {
            rb.AddForce(Quaternion.Euler(0, 0, -90f) * toTarget * steeringForce * Time.deltaTime);
        }
        else if (angle > 2f)        // turn left
        {
            rb.AddForce(Quaternion.Euler(0, 0, 90f) * toTarget * steeringForce * Time.deltaTime);
        }

        if (leaving || rb.velocity.magnitude < maxSpeed || Mathf.Abs(angle) > 90)
        {
            rb.AddForce(toTarget * homingForce * Time.deltaTime);
        }

        if (targetDist < arrivalDistance)
        {
            if (leaving)
                Destroy(gameObject);
            else
                SetNewRandomTargetPoint();
        }

        if(rb.angularVelocity == 0)
            rb.AddTorque(Random.Range(-100f, 100f));
    }

    void UpdateAlpha()
    {
        float alpha = sr.color.a;
        int sign = targetAlpha > alpha ? 1 : -1;
        alpha += sign * flickerSpeed * Time.deltaTime;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
        float scale = minScale + alpha * (maxScale - minScale); // scale is tied to alpha
        transform.localScale = new Vector3(scale, scale, 1);

        if (Mathf.Abs(alpha - targetAlpha) < 0.1) SetNewRandomTargetAlpha();
    }

    //void UpdateScale()
    //{
    //    //float scale = transform.localScale.x;
    //    //int sign = targetScale > scale ? 1 : -1;
    //    //scale += sign * flickerSpeed * Time.deltaTime;
    //    //transform.localScale = new Vector3(scale, scale, scale);

    //    //if (Mathf.Abs(scale - targetScale) < 0.1) SetNewRandomTargetScale();
    //}

    void SetNewRandomTargetPoint()
    {
        // preserve z value
        targetPoint = new Vector3(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y), transform.localPosition.z);
    }

    void SetNewRandomTargetAlpha()
    {
        if (leaving) return;
        targetAlpha = Random.Range(minAlpha, maxAlpha);
    }

    //void SetNewRandomTargetScale()
    //{
    //    //targetScale = Random.Range(minScale, maxScale);
    //}

    public void Leave()
    {
        targetPoint = new Vector3(transform.position.x, bounds.max.y + 3, transform.position.z);
        targetAlpha = .1f;
        leaving = true;
    }
}

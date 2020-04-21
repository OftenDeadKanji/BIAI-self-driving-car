using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] public float spd = 0f;
    [SerializeField] public float acc = .01f;
    [SerializeField] public float maxSpdFwd = 5f;
    [SerializeField] public float maxSpdBck = -2f;
    [SerializeField] private float leftDist;
    [SerializeField] private float rightDist;
    [SerializeField] private float leftfwdDist;
    [SerializeField] private float rightfwdDist;
    [SerializeField] private float fwdDist;
    SpriteRenderer render;


    // Start is called before the first frame update
    void Start()
    {
        render = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition += spd * transform.up * Time.deltaTime;
        spd += Input.GetAxis("Drive") * acc;
        if (spd > maxSpdFwd) spd = maxSpdFwd;
        if (spd < maxSpdBck) spd = maxSpdBck;

        if (spd != 0) transform.Rotate(transform.forward, -Input.GetAxis("Turn")/3 * spd/Mathf.Abs(spd));

        RaycastHit2D seen;
        if (seen = Physics2D.Raycast((Vector2)transform.position, (Vector2)transform.up, 3.0f))
        {
            fwdDist = (seen.point - (Vector2)transform.position).magnitude;
        }
        else
        {
            fwdDist = 3.0f;
        }
        if (seen = Physics2D.Raycast((Vector2)transform.position, (Vector2)transform.right, 3.0f))
        {
            rightDist = (seen.point - (Vector2)transform.position).magnitude;
        }
        else
        {
            rightDist = 3.0f;
        }
        if (seen = Physics2D.Raycast((Vector2)transform.position, (Vector2)(-transform.right), 3.0f))
        {
            leftDist = (seen.point - (Vector2)transform.position).magnitude;
        }
        else
        {
            leftDist = 3.0f;
        }
        if (seen = Physics2D.Raycast((Vector2)transform.position, (Vector2)(transform.right + transform.up), 3.0f))
        {
            rightfwdDist = (seen.point - (Vector2)transform.position).magnitude;
        }
        else
        {
            rightfwdDist = 3.0f;
        }
        if (seen = Physics2D.Raycast((Vector2)transform.position, (Vector2)(transform.up - transform.right), 3.0f))
        {
            leftfwdDist = (seen.point - (Vector2)transform.position).magnitude;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        render.color = Color.red;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        render.color = Color.white;
    }

    void OnDrawGizmosSelected ()
    {
        Vector3 target = 3 * transform.right + transform.position;
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, target);
        target = -3 * transform.right + transform.position;
        Gizmos.DrawLine(transform.position, target);
        target = 3 * transform.up + transform.position;
        Gizmos.DrawLine(transform.position, target);
        target = 3 * Vector3.Normalize(transform.right + transform.up) + transform.position;
        Gizmos.DrawLine(transform.position, target);
        target = 3 * Vector3.Normalize(-transform.right + transform.up) + transform.position;
        Gizmos.DrawLine(transform.position, target);
    }
}

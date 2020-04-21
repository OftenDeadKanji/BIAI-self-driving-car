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
    SpriteRenderer renderer;


    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        renderer.color = Color.white;
        transform.localPosition += spd * transform.up * Time.deltaTime;
        spd += Input.GetAxis("Drive") * acc;
        if (spd > maxSpdFwd) spd = maxSpdFwd;
        if (spd < maxSpdBck) spd = maxSpdBck;

        if (spd != 0) transform.Rotate(transform.forward, -Input.GetAxis("Turn")/4 * spd/Mathf.Abs(spd));

    }

    void OnCollisionEnter(Collision col)
    {
        renderer.color = Color.red;
        Debug.Log("Col dec");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTest : MonoBehaviour
{
    public float speed = 5.0f;
    public float lifeTime = 3.0f;
    public float time = 0;
    Vector3 dir;

    void Start()
    {
        dir = GameObject.Find("target").transform.forward;
    }

    void Update()
    {
        time += Time.deltaTime;
        transform.position += dir * speed * Time.deltaTime;
        if(lifeTime<time)
            Destroy(gameObject);
    }
}

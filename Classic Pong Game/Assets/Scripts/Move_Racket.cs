using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_Racket : MonoBehaviour
{
    public float speed = 30.0f;
    void FixedUpdate()
    {
        float v = Input.GetAxisRaw("Vertical");
        GetComponent<Rigidbody2D>().velocity = new Vector2(0,v)*speed;
    }
}

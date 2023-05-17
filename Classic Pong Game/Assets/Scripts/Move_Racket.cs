using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_Racket : MonoBehaviour
{
    public bool isOnRight;
    public float speed = 30.0f;

    public GameObject rightSideGameObject;
    private RightSide rightSideVariable;

    void Start()
    {
        rightSideVariable = rightSideGameObject.GetComponent<RightSide>();
    }
    void FixedUpdate()
    {
        if(rightSideVariable.isPlayerOnRight == isOnRight)
        {
            float v = Input.GetAxisRaw("Vertical");
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, v) * speed;
        }   
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour
{
    public float gravity = 3f;

    void Start()
    {
        Physics.gravity = new Vector3(0, -gravity, 0);
    }
}


﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCubeID : MonoBehaviour
{

    [SerializeField]
    float speed = 10;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movementVector = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        movementVector *= speed;

    }
}

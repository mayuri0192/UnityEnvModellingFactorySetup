using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System.Collections.Specialized;
using System.Diagnostics;
using System;

public class OperatorScript : MonoBehaviour
{
    public Rigidbody rBody;
    public int wall = 0;

    void Start()
    {
        gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
        rBody = GetComponent<Rigidbody>();
    }

    //Track Wall Hit
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("Wall"))
        {
            if (this.wall == 0)
            { this.wall = 1; }
            else
            { this.wall = 0; }
        }
    }
    //Move Operator
    void FixedUpdate()
    {
        if (this.wall == 1)
        {
            this.rBody.AddForce(5 * Vector3.right);
        }
        else
        {
            this.rBody.AddForce(5 * Vector3.left);
        }
    }
}


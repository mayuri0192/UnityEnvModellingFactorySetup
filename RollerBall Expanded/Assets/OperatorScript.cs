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

    void Start()
    {
        gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
        rBody = GetComponent<Rigidbody>();
    }

    //Move Operator
    void FixedUpdate()
    {
        if (Convert.ToInt32(Time.time / 5) % 2 == 0) //Periodicity
        {
            this.rBody.AddForce(10 * Vector3.right);
        }
        else
        {
            this.rBody.AddForce(10 * Vector3.left);
        }
    }
}


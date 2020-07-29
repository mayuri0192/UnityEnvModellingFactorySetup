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
        this.transform.localPosition = new Vector3(0, 0.5f, 0);
    }

    void FixedUpdate()
    {
        if (Convert.ToInt32(Time.time/2) % 2 == 0)
        {
            this.rBody.AddForce(10*Vector3.forward);
        }
        else
        {
            this.rBody.AddForce(10*Vector3.back);
        }
    }
}


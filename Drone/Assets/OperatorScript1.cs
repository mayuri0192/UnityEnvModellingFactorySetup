using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System.Collections.Specialized;
using System.Diagnostics;
using System;

public class OperatorScript1 : MonoBehaviour
{
    public Rigidbody rBody;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        this.transform.localPosition = new Vector3(3, 0.5f, 5);
    }

    void FixedUpdate()
    {
        if (Convert.ToInt32(Time.time/5) % 2 == 0)
        {
            this.rBody.AddForce(10*Vector3.forward);
        }
        else
        {
            this.rBody.AddForce(10*Vector3.back);
        }
    }
}


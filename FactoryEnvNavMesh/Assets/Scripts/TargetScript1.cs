﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class TargetScript1 : MonoBehaviour
{
    public Rigidbody rBody;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        this.transform.localPosition = new Vector3(0, 0, 8.5f);
    }

}
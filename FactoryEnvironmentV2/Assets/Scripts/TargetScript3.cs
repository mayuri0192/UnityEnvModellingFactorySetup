using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class TargetScript3 : MonoBehaviour
{
    public Rigidbody rBody;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        this.transform.localPosition = new Vector3(1, 0, -9);
    }

}
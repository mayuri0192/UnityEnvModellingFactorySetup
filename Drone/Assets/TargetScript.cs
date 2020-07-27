using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class TargetScript : MonoBehaviour
{
    public Rigidbody rBody;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        this.transform.localPosition = new Vector3(8.5f, 0.5f, 4.5f);
    }

}
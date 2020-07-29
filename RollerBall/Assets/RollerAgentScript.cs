using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System.Collections.Specialized;
using System.CodeDom;
using System.Diagnostics;
using System;

public class RollerAgentScript : Agent
{
    Rigidbody rBody;

    public Game_Master Master;
    public int k;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        int rand = UnityEngine.Random.Range(0, 4);
        if (rand == 0)
        {
            this.transform.localPosition = new Vector3(UnityEngine.Random.Range(-3.75f, -2.0f), 0.5f, UnityEngine.Random.Range(-3.75f, 3.75f));
        }

        if (rand == 1)
        {
            this.transform.localPosition = new Vector3(UnityEngine.Random.Range(2.0f, 3.75f), 0.5f, UnityEngine.Random.Range(-3.75f, 3.75f));
        }

        if (rand == 2)
        {
            this.transform.localPosition = new Vector3(UnityEngine.Random.Range(-0.75f, 0.75f), 0.5f, UnityEngine.Random.Range(2.25f, 3.75f));
        }

        if (rand == 3)
        {
            this.transform.localPosition = new Vector3(UnityEngine.Random.Range(-0.75f, 0.75f), 0.5f, UnityEngine.Random.Range(-3.75f, -2.25f));
        }
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        Master.Start();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
        sensor.AddObservation(Master.Flags[0]);
        sensor.AddObservation(Master.Flags[1]);
        sensor.AddObservation(Master.Flags[2]);
        sensor.AddObservation(Master.Flags[3]);
        //sensor.AddObservation(Master.Target1.localPosition);
        //sensor.AddObservation(Master.Target2.localPosition);
        //sensor.AddObservation(Master.Target3.localPosition);
        //sensor.AddObservation(Master.Target4.localPosition);
    }

    public float speed = 10;
    public override void OnActionReceived(float[] vectorAction)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        this.rBody.AddForce(controlSignal * speed);
        //AddReward(- 0.0001f);
    }

    void OnCollisionEnter(Collision collision)
    {       
        if (collision.gameObject.tag != "Floor")
        {
            if (collision.gameObject.name.Contains("Target"))
            {
                collision.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
                string s = collision.gameObject.name;
                string subs = s.Substring(s.Length - 1);
                k = Int32.Parse(subs) - 1;
                if (Master.Flags[k] == 0)
                {
                    AddReward(0.5f);
                    Master.Count(k);
                }
            }
            else
            {
                AddReward(-0.5f);
                UnityEngine.Debug.Log("Collision");
                Master.EndAll();
            }
        }    
    }

    public void End()
    {
        EndEpisode();
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }
}
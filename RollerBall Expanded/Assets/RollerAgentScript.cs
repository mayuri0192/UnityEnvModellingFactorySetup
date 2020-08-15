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
    public OperatorNavMeshScript Operator1;
    public OperatorNavMeshScript Operator2;
    public int k;
    public int c = 0; //Action Count
    public int e = 0; //Episode Count
    public int r_count = 1; //Reward Count

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        this.r_count = 1; //Reset Reward Count
        this.e = e + 1; //Count Episode
        this.c = 0; //Reset Action Count
        
        //Initialize Agent to Random Location
        this.transform.localPosition = new Vector3(UnityEngine.Random.Range(-13f, 13f), 0.5f, UnityEngine.Random.Range(-16f, 16f));
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = new Vector3(0f, 0f, 0f);

        //Master Calls
        Master.Start();

        Master.Check(); //End Episode if all are targets randomly initialized to 1 already
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //Self location and velocity
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);

        //Other agent's location and velocity - Should be scripted better for generalization

        if (this.name == "Agent 1")
        {
            sensor.AddObservation(Master.roller2.transform.localPosition);
            sensor.AddObservation(Master.roller2.rBody.velocity.x);
            sensor.AddObservation(Master.roller2.rBody.velocity.z);
        }

        else
        {
            sensor.AddObservation(Master.roller.transform.localPosition);
            sensor.AddObservation(Master.roller.rBody.velocity.x);
            sensor.AddObservation(Master.roller.rBody.velocity.z);
        }

        //Target Flags
        sensor.AddObservation(Master.Flags[0]);
        sensor.AddObservation(Master.Flags[1]);
        sensor.AddObservation(Master.Flags[2]);
        sensor.AddObservation(Master.Flags[3]);
    }

    public override void OnActionReceived(float[] vectorAction)//Discrete Action Space
    {
        var movement = (int)vectorAction[0];

        if (movement == 0)
        {
            this.rBody.velocity = (new Vector3(10f, 0f, 0f));
        }

        else if (movement == 1)
        {
            this.rBody.velocity = (new Vector3(-10f, 0f, 0f));
        }

        else if (movement == 2)
        {
            this.rBody.velocity = (new Vector3(0f, 0f, 10f));
        }

        else if (movement == 3)
        {
            this.rBody.velocity = (new Vector3(0f, 0f, -10f));
        }

        else if (movement == 4)
        {
            this.rBody.velocity = (new Vector3(0f, 0f, 0f));
        }

        c = c + 1; //Increment Action Count

        //Action Count Limit
        if (c == 2048)
        {
            Master.EndAll();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
    
        if (collision.gameObject.tag != "Floor")
        {
            if (collision.gameObject.name.Contains("Target"))
            {
                //Change Target Color
                collision.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;

                //Change Target Flag, Tag, Add and Count Reward
                string s = collision.gameObject.name;
                string subs = s.Substring(s.Length - 1);
                k = Int32.Parse(subs) - 1;
                if (Master.Flags[k] == 0)
                {
                    collision.gameObject.tag = "Null";
                    Master.Flags[k] = 1;
                    AddReward(0.33f*this.r_count);
                    this.r_count = this.r_count + 1;
                    Master.Check(); //Check if all targets are met
                }

                //Negative Reward if hitting already met target
                if (Master.Flags[k] == 1)
                {
                    AddReward(-0.1f);
                }
            }

            //Negative Reward for other collisions
            else
            {
                AddReward(-0.00625f);
            }
        }
    }

    //Negative Reward if Operator is hit and End Episode
    public void OpCollision()
    {
        AddReward(-0.1f);
        Master.EndAll();
    }

    //End Episode
    public void End()
    {
        EndEpisode();
    }

    //Play Manually
    public override void Heuristic(float[] actionsOut)
    {
        if (Input.GetKey(KeyCode.A))
        {
            actionsOut[0] = 1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            actionsOut[0] = 0;
        }

        if (Input.GetKey(KeyCode.W))
        {
            actionsOut[0] = 2;
        }

        if (Input.GetKey(KeyCode.S))
        {
            actionsOut[0] = 3;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            actionsOut[0] = 4;
        }
    }
}
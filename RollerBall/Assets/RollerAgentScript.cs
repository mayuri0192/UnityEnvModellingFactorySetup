using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System.Collections.Specialized;
using System.CodeDom;
using System.Diagnostics;

public class RollerAgentScript : Agent
{
    Rigidbody rBody;

    public OperatorScript Operator;
    //float reward=0.0f;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public Transform Target;
    public override void OnEpisodeBegin()
    {
        //reward = 0.0f;
        Operator.transform.localPosition = new Vector3(UnityEngine.Random.Range(-3, 3), 0.5f, 0);
        Operator.rBody.angularVelocity = Vector3.zero;
        Operator.rBody.velocity = Vector3.zero;
        //UnityEngine.Debug.Log(reward);
        //if (this.transform.localPosition.y < 0)
        //{
        //    // If the Agent fell, zero its momentum
        //    this.rBody.angularVelocity = Vector3.zero;
        //    this.rBody.velocity = Vector3.zero;
        //    this.transform.localPosition = new Vector3(0, 0.5f, 0);
        //}

        // Move the target to a new spot
        //Operator.localPosition = new Vector3(-1.0f, 0.5f, -1.0f);
        //Operator.rBody2.velocity = new Vector3(0, 0, 3.0f);

        Target.localPosition = new Vector3(3.0f, 0.5f, 3.0f);
        this.transform.localPosition = new Vector3(UnityEngine.Random.Range(-3, 3), 0.5f, UnityEngine.Random.Range(-3, 3));
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        //sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }

    public float speed = 10;
    public override void OnActionReceived(float[] vectorAction)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        rBody.AddForce(controlSignal * speed);
        // Rewards
        //float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);
        //float distanceToOperator = Vector3.Distance(this.transform.localPosition, Operator.transform.localPosition);

        // Reached target
        //if (distanceToOperator < 1.6f)
        //{
            //UnityEngine.Debug.Log("Operator Nearby");
            //AddReward(-0.05f);
            //reward = reward-0.5f;
            //UnityEngine.Debug.Log(reward);
        //}

            //AddReward(-1.0f);
            //reward = reward - 1.0f;
            //UnityEngine.Debug.Log(reward);
            
            //reward = reward + 750.0f;
            //UnityEngine.Debug.Log(reward);

        // Fell off platform
        //if (this.transform.localPosition.y < 0)
        //{
        //    EndEpisode();
        //}
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Walls"))
        {
            UnityEngine.Debug.Log("Wall Hit");
            AddReward(-0.005f);
            EndEpisode();
        }

        if (collision.gameObject.CompareTag("Interactive"))
        {
            UnityEngine.Debug.Log("Operator Hit");
            AddReward(-0.005f);
            EndEpisode();
        }

        if (collision.gameObject.CompareTag("Interactive2"))
        {
            UnityEngine.Debug.Log("Target Hit");
            AddReward(1.0f);
            EndEpisode();
        }
    }
}

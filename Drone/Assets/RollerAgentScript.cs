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

    //public RayPerceptionSensor(string name, RayPerceptionInput rayInput);
    public Drone Drone1;

    List<Vector3> opeLoc = new List<Vector3>();
    List<Vector3> targLoc = new List<Vector3>();
    List<Vector3> wallLoc = new List<Vector3>();
    List<Vector3> endLoc = new List<Vector3>();
    List<Vector3> agentLoc = new List<Vector3>();
    List<Vector3> groundLoc = new List<Vector3>();

    //Vector3 agentOldPos;
    //Vector3 agentNewPos;
    Vector3 agentPos;
    float oldX;
    float oldZ;
    

    public OperatorScript1 Operator1;
    public OperatorScript2 Operator2;
    


    public float speed = 20;


    public override void Initialize()
    {

        rBody = GetComponent<Rigidbody>();

    }

    public Transform Target;

    public override void OnEpisodeBegin()

    {
        this.transform.localPosition = new Vector3(1.5f, 0.5f, -3);
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;

        Drone1.transform.localPosition = new Vector3(6, 5, 2.5f);
        Drone1.rBody.angularVelocity = Vector3.zero;
        Drone1.rBody.velocity = Vector3.zero;
        Operator1.transform.localPosition = new Vector3(3, 0.5f, 5);
        Operator1.rBody.angularVelocity = Vector3.zero;
        Operator1.rBody.velocity = Vector3.zero;
        Operator2.transform.localPosition = new Vector3(6, 0.5f, -3);
        Operator2.rBody.angularVelocity = Vector3.zero;
        Operator2.rBody.velocity = Vector3.zero;

        Target.localPosition = new Vector3(8.5f, 0.5f, 8.5f);

        oldX = 20;
        oldZ = 20;
    }

    //public void droneInfo()
    //{
    //    Drone1.RayCastUpdate(out agentLoc, out endLoc, out wallLoc, out opeLoc, out targLoc);
    //}

    public override void CollectObservations(VectorSensor sensor)
    {
        
        Drone1.RayCastUpdate(out agentLoc, out endLoc, out wallLoc, out opeLoc, out targLoc, out groundLoc);
        //agentOldPos = agentLoc[0];
        foreach (var agentLocation in agentLoc)
        {
            sensor.AddObservation(agentLocation);
            agentPos = agentLocation;
        }
        foreach (var endLocation in endLoc)
        {
            sensor.AddObservation(endLocation);
        }
        foreach (var wallLocation in wallLoc)
        {
            sensor.AddObservation(wallLocation);
            //UnityEngine.Debug.Log(wallLocation);
        }
        foreach (var opeLocation in opeLoc)
        {
            sensor.AddObservation(opeLocation);
        }
        foreach (var targLocation in targLoc)
        {
            sensor.AddObservation(targLocation);
            if ((targLocation.x - agentPos.x) < oldX)
            {
                AddReward(0.1f);
                oldX = targLocation.x - agentPos.x;
                //UnityEngine.Debug.Log(oldX);
                sensor.AddObservation(oldX);
            }
            if ( (targLocation.z - agentPos.z) < oldZ)
            {
                AddReward(0.1f);
                oldZ = targLocation.z - agentPos.z;
                //UnityEngine.Debug.Log(oldX);
                sensor.AddObservation(oldZ);
            }

            //sensor.AddObservation(targLocation- this.transform.localPosition);
        }
        //foreach (var groundLocation in groundLoc)
        //{
        //    sensor.AddObservation(groundLocation);
        //    //UnityEngine.Debug.Log(groundLocation);
        //}
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
        //UnityEngine.Debug.Log(this.transform.localPosition);
        //Drone1.RayCastUpdate(out agentLoc, out endLoc, out wallLoc, out opeLoc, out targLoc);
        //agentNewPos = agentLoc[0];
        //agentVel = (agentNewPos - agentOldPos);
        //UnityEngine.Debug.Log(agentVel);
    }


    public override void OnActionReceived(float[] vectorAction)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        rBody.AddForce(controlSignal * speed);
        AddReward(-0.0005f);
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Walls"))
        {
            UnityEngine.Debug.Log("Wall Hit");
            AddReward(-0.05f);
            //EndEpisode();
        }

        if (collision.collider.CompareTag("EndGame"))
        {
            UnityEngine.Debug.Log("Operator Hit Game End");
            AddReward(-0.05f);
            EndEpisode();
        }

        if (collision.collider.CompareTag("Interactive"))
        {
            UnityEngine.Debug.Log("Operator Hit");
            AddReward(-0.05f);
            EndEpisode();
        }

        if (collision.collider.CompareTag("Interactive2"))
        {
            UnityEngine.Debug.Log("Target Hit Game End");
            AddReward(1.0f);
            EndEpisode();
        }
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }
}

using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System.Collections.Specialized;
using System.CodeDom;
using System.Diagnostics;
using System.Linq;
using System;

//Drone agent do not use NavMesh to travel
//Refer to comment on Roller Agent for better understand of the whole environmnet
//Will comment only on different areas between two agents

public class Drone : Agent
{
    Rigidbody rBody;

    Vector3 agent_agentLoc2;
    Vector3 agent_opeLoc1;
    Vector3 agent_targLoc1;
    Vector3 agent_targLoc2;
    Vector3 agent_targLoc3;
    Vector3 agent_targLoc4;
    Vector3 agent_targLoc5;
    Vector3 agent_targLoc6;
    Vector3 agent_targLoc7;

    public int newPriority;

    public Agent2 RobotAgent2;

    public OperatorScript1 Operator1;

    public TargetScript1 Target1;
    public TargetScript2 Target2;
    public TargetScript3 Target3;
    public TargetScript4 Target4;
    public TargetScript5 Target5;
    public TargetScript6 Target6;
    public TargetScript7 Target7;

    private const int numDirection = 100;
    //For Drone Agent numDirection of at least 180 is recommened
    //Lower numdirection will require Drone to move more to notice the target
    private const int maxDistance = 20;

    private float speed = 5;

    public override void Initialize()
    {

        rBody = GetComponent<Rigidbody>();

    }


    public override void OnEpisodeBegin()

    {
        this.transform.localPosition = new Vector3(0, 3, 5);
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;


        agent_agentLoc2 = Vector3.zero;
        agent_opeLoc1 = Vector3.zero;
        agent_targLoc1 = Vector3.zero;
        agent_targLoc2 = Vector3.zero;
        agent_targLoc3 = Vector3.zero;
        agent_targLoc4 = Vector3.zero;
        agent_targLoc5 = Vector3.zero;
        agent_targLoc6 = Vector3.zero;
        agent_targLoc7 = Vector3.zero;

        newPriority = 1;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        RaycastHit wallHit;
        if (Physics.Raycast(transform.position, Vector3.forward, out wallHit, maxDistance))
        {
            if (wallHit.collider.tag == "Walls")
            {
                sensor.AddObservation(wallHit.point);
            }
        }
        if (Physics.Raycast(transform.position, Vector3.back, out wallHit, maxDistance))
        {
            if (wallHit.collider.tag == "Walls")
            {
                sensor.AddObservation(wallHit.point);
            }
        }
        if (Physics.Raycast(transform.position, Vector3.left, out wallHit, maxDistance))
        {
            if (wallHit.collider.tag == "Walls")
            {
                sensor.AddObservation(wallHit.point);
            }
        }
        if (Physics.Raycast(transform.position, Vector3.right, out wallHit, maxDistance))
        {
            if (wallHit.collider.tag == "Walls")
            {
                sensor.AddObservation(wallHit.point);
            }
        }
        if (Physics.Raycast(transform.position, Vector3.up, out wallHit, maxDistance))
        {
            if (wallHit.collider.tag == "Walls")
            {
                sensor.AddObservation(wallHit.point);
            }
        }
        if (Physics.Raycast(transform.position, Vector3.down, out wallHit, maxDistance))
        {
            if (wallHit.collider.tag == "Walls")
            {
                sensor.AddObservation(wallHit.point);
            }
        }

        RayCastUpdate(out agent_agentLoc2, out agent_opeLoc1, out agent_targLoc1, out agent_targLoc2, out agent_targLoc3, out agent_targLoc4, out agent_targLoc5, out agent_targLoc6, out agent_targLoc7);
        if (agent_agentLoc2 != Vector3.zero)
        {
            sensor.AddObservation(agent_agentLoc2);
            AddReward(0.000005f);//Add reward when Drone Agent notice Roller Agent
        }
        if (agent_opeLoc1 != Vector3.zero)
        {
            sensor.AddObservation(agent_opeLoc1);
            AddReward(0.000005f);
        }
        if (agent_targLoc1 != Vector3.zero && newPriority == 1)
        {
            sensor.AddObservation(agent_targLoc1);
            AddReward(0.000005f);
        }
        if (agent_targLoc2 != Vector3.zero && newPriority == 2)
        {
            sensor.AddObservation(agent_targLoc2);
            AddReward(0.000005f);
        }
        if (agent_targLoc3 != Vector3.zero && newPriority == 3)
        {
            sensor.AddObservation(agent_targLoc3);
            AddReward(0.000005f);
        }
        if (agent_targLoc4 != Vector3.zero && newPriority == 4)
        {
            sensor.AddObservation(agent_targLoc4);
            AddReward(0.000005f);
        }
        if (agent_targLoc5 != Vector3.zero && newPriority == 5)
        {
            sensor.AddObservation(agent_targLoc5);
            AddReward(0.000005f);
        }
        if (agent_targLoc6 != Vector3.zero && newPriority == 6)
        {
            sensor.AddObservation(agent_targLoc6);
            AddReward(0.000005f);
        }
        if (agent_targLoc7 != Vector3.zero && newPriority == 7)
        {
            sensor.AddObservation(agent_targLoc7);
            AddReward(0.000005f);
        }
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(this.rBody.velocity);

        agent_agentLoc2 = Vector3.zero;
        agent_opeLoc1 = Vector3.zero;
        agent_targLoc1 = Vector3.zero;
        agent_targLoc2 = Vector3.zero;
        agent_targLoc3 = Vector3.zero;
        agent_targLoc4 = Vector3.zero;
        agent_targLoc5 = Vector3.zero;
        agent_targLoc6 = Vector3.zero;
        agent_targLoc7 = Vector3.zero;
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        rBody.AddForce(controlSignal * speed);

    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Walls"))
        {
            UnityEngine.Debug.Log("Drone Wall Hit");
            AddReward(-0.5f);//Subtract reward when Drone Agent collids to the wall
            RobotAgent2.EndEpisode();
            Operator1.EndEpisode();
            EndEpisode();
        }

    }
    public void RayCastUpdate(out Vector3 return1, out Vector3 return2, out Vector3 return3, out Vector3 return4, out Vector3 return5, out Vector3 return6, out Vector3 return7, out Vector3 return8, out Vector3 return9)
    {
        foreach (var direction in SphereDirections(numDirection))
        {
            //Debug.DrawRay(transform.position, direction, Color.blue);
            //Uncomment the debug code to see diection of the RayCast rays

            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, maxDistance))
            {
                if (hit.collider.tag == "Agent2")//Agent2
                {
                    agent_agentLoc2 = hit.point;
                }
                if (hit.collider.tag == "Operator1")
                {
                    agent_opeLoc1 = hit.point;
                }
                if (hit.collider.tag == "Target1")
                {
                    agent_targLoc1 = hit.point;
                }
                if (hit.collider.tag == "Target2")
                {
                    agent_targLoc2 = hit.point;
                }
                if (hit.collider.tag == "Target3")
                {
                    agent_targLoc3 = hit.point;
                }
                if (hit.collider.tag == "Target4")
                {
                    agent_targLoc4 = hit.point;
                }
                if (hit.collider.tag == "Target5")
                {
                    agent_targLoc5 = hit.point;
                }
                if (hit.collider.tag == "Target6")
                {
                    agent_targLoc6 = hit.point;
                }
                if (hit.collider.tag == "Target7")
                {
                    agent_targLoc7 = hit.point;
                }
            }
        }
        return1 = agent_agentLoc2;
        return2 = agent_opeLoc1;
        return3 = agent_targLoc1;
        return4 = agent_targLoc2;
        return5 = agent_targLoc3;
        return6 = agent_targLoc4;
        return7 = agent_targLoc5;
        return8 = agent_targLoc6;
        return9 = agent_targLoc7;
    }


    private Vector3[] SphereDirections(int numDirections)
    //I have used solution provided by titan68
    //This method allowes to create directions in all directions
    //Have to modify this method soon since Human agent will have limited field of view
    //credit: titan68
    //https://answers.unity.com/questions/43812/how-to-get-raycasts-in-all-directionsvector3.html
    {
        var pts = new Vector3[numDirections];
        var il = Math.PI * (3 - Math.Sqrt(5));
        var idd = 2f / numDirections;

        foreach (var i in Enumerable.Range(0, numDirections))
        {
            var y = i * idd - 1 + (idd / 2);
            var r = Math.Sqrt(1 - y * y);
            var phi = i * il;
            var x = (float)(Math.Cos(phi) * r);
            var z = (float)(Math.Sin(phi) * r);
            pts[i] = new Vector3(x, y, z);
        }

        return pts;
    }
}


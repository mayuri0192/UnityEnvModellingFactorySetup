using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System.Collections.Specialized;
using System.CodeDom;
using System.Diagnostics;
using System.Linq;
using System;

public class Drone : Agent
{
    Rigidbody rBody;

    Vector3 agent_agentLoc;
    Vector3 agent_endLoc;
    Vector3 agent_opeLoc;
    Vector3 agent_firstTargLoc;
    Vector3 agent_secondTargLoc;
    Vector3 agent_finalTargLoc;
    
    public RollerAgentScript RollerAgent;
    public OperatorScript1 Operator1;
    public OperatorScript2 Operator2;
    public TargetScript Target;
    public TargetScript1 Target1;
    public TargetScript2 Target2;

    private const int numDirection = 180;
    private const int maxDistance = 20;

    private float speed = 10;

    public override void Initialize()
    {

        rBody = GetComponent<Rigidbody>();

    }


    public override void OnEpisodeBegin()

    {
        this.transform.localPosition = new Vector3(6, 2.5f, 2.5f);
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;

        Operator1.transform.localPosition = new Vector3(3, 0.5f, 5);
        Operator1.rBody.angularVelocity = Vector3.zero;
        Operator1.rBody.velocity = Vector3.zero;
        Operator2.transform.localPosition = new Vector3(6, 0.5f, -3);
        Operator2.rBody.angularVelocity = Vector3.zero;
        Operator2.rBody.velocity = Vector3.zero;

        Target.transform.localPosition = new Vector3(8.5f, 0.5f, 4.5f);
        Target.rBody.angularVelocity = Vector3.zero;
        Target.rBody.velocity = Vector3.zero;
        Target1.transform.localPosition = new Vector3(8.5f, 0.5f, -3.5f);
        Target1.rBody.angularVelocity = Vector3.zero;
        Target1.rBody.velocity = Vector3.zero;
        Target2.transform.localPosition = new Vector3(1.5f, 0.5f, -3.5f);
        Target2.rBody.angularVelocity = Vector3.zero;
        Target2.rBody.velocity = Vector3.zero;

        agent_agentLoc = Vector3.zero;
        agent_opeLoc = Vector3.zero;
        agent_firstTargLoc = Vector3.zero;
        agent_secondTargLoc = Vector3.zero;
        agent_finalTargLoc = Vector3.zero;
        agent_endLoc = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {

        RayCastUpdate(out agent_agentLoc, out agent_endLoc, out agent_opeLoc, out agent_firstTargLoc, out agent_secondTargLoc, out agent_finalTargLoc);
        if (agent_agentLoc != Vector3.zero)
        {
            sensor.AddObservation(agent_agentLoc);
            AddReward(0.005f);
            agent_agentLoc = Vector3.zero;
        }
        if (agent_endLoc != Vector3.zero)
        {
            sensor.AddObservation(agent_endLoc);
            //AddReward(0.00005f);
            agent_endLoc = Vector3.zero;
        }
        if (agent_opeLoc != Vector3.zero)
        {
            sensor.AddObservation(agent_opeLoc);
            //AddReward(0.00005f);
            agent_opeLoc = Vector3.zero;
        }
        if (agent_firstTargLoc != Vector3.zero)
        {
            sensor.AddObservation(agent_firstTargLoc);
            //AddReward(0.0005f);
            agent_firstTargLoc = Vector3.zero;
        }
        if (agent_secondTargLoc != Vector3.zero)
        {
            sensor.AddObservation(agent_secondTargLoc);
            //AddReward(0.0005f);
            agent_secondTargLoc = Vector3.zero;
        }
        if (agent_finalTargLoc != Vector3.zero)
        {
            sensor.AddObservation(agent_finalTargLoc);
            AddReward(0.005f);
            agent_finalTargLoc = Vector3.zero;
        }
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(this.rBody.velocity);
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
            AddReward(-0.05f);
            EndEpisode();
            RollerAgent.EndEpisode();
        }

    }
    public void RayCastUpdate(out Vector3 return1, out Vector3 return2, out Vector3 return3, out Vector3 return4, out Vector3 return5, out Vector3 return6)
    {
        foreach (var direction in SphereDirections(numDirection))
        {
            //Debug.DrawRay(transform.position, direction, Color.blue);


            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, maxDistance))
            {
                if (hit.collider.tag == "RollerAgent")
                {
                    agent_agentLoc = hit.point;
                }
                if (hit.collider.tag == "EndGame")
                {
                    agent_endLoc = hit.point;
                }
                if (hit.collider.tag == "Interactive")
                {
                    agent_opeLoc = hit.point;
                }
                if (hit.collider.tag == "firstTarget")
                {
                    agent_firstTargLoc = hit.point;
                }
                if (hit.collider.tag == "secondTarget")
                {
                    agent_secondTargLoc = hit.point;
                }
                if (hit.collider.tag == "finalTarget")
                {
                    agent_finalTargLoc = hit.point;
                }
            }
        }
        return1 = agent_agentLoc;
        return2 = agent_endLoc;
        return3 = agent_opeLoc;
        return4 = agent_firstTargLoc;
        return5 = agent_secondTargLoc;
        return6 = agent_finalTargLoc;
    }


    private Vector3[] SphereDirections(int numDirections)
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



//using System.Collections.Generic;
//using UnityEngine;
//using Unity.MLAgents;
//using Unity.MLAgents.Sensors;
//using System.Collections.Specialized;
//using System.CodeDom;
//using System.Diagnostics;
//using System.Linq;
//using System;

//public class Drone : Agent
//{
//    Rigidbody rBody;

//    //public RayPerceptionSensor(string name, RayPerceptionInput rayInput);
//    //public Drone Drone1;

//    //List<Vector3> opeLoc = new List<Vector3>();
//    //List<Vector3> targLoc = new List<Vector3>();
//    //List<Vector3> wallLoc = new List<Vector3>();
//    //List<Vector3> endLoc = new List<Vector3>();
//    //List<Vector3> agentLoc = new List<Vector3>();
//    //List<Vector3> groundLoc = new List<Vector3>();

//    List<Vector3> agent_opeLoc = new List<Vector3>();
//    List<Vector3> agent_firsttargLoc = new List<Vector3>();
//    List<Vector3> agent_secondtargLoc = new List<Vector3>();
//    List<Vector3> agent_finaltargLoc = new List<Vector3>();
//    List<Vector3> agent_wallLoc = new List<Vector3>();
//    List<Vector3> agent_endLoc = new List<Vector3>();
//    List<Vector3> agent_agentLoc = new List<Vector3>();
//    List<Vector3> agent_groundLoc = new List<Vector3>();

//    //Vector3 agentOldPos;
//    //Vector3 agentNewPos;
//    Vector3 agentPos;
//    //float oldX;
//    //float oldZ;

//    public RollerAgentScript RollerAgent;
//    public OperatorScript1 Operator1;
//    public OperatorScript2 Operator2;

//    private const int numDirection = 180;
//    private const int maxDistance = 20;

//    public float speed = 1;


//    public override void Initialize()
//    {

//        rBody = GetComponent<Rigidbody>();

//    }

//    public Transform Target;
//    public Transform Target1;
//    public Transform Target2;

//    public override void OnEpisodeBegin()

//    {
//        this.transform.localPosition = new Vector3(6, 5, 2.5f);
//        this.rBody.angularVelocity = Vector3.zero;
//        this.rBody.velocity = Vector3.zero;

//        Operator1.transform.localPosition = new Vector3(3, 0.5f, 5);
//        Operator1.rBody.angularVelocity = Vector3.zero;
//        Operator1.rBody.velocity = Vector3.zero;
//        Operator2.transform.localPosition = new Vector3(6, 0.5f, -3);
//        Operator2.rBody.angularVelocity = Vector3.zero;
//        Operator2.rBody.velocity = Vector3.zero;

//        Target.localPosition = new Vector3(8.5f, 0.5f, 4.5f);
//        Target1.localPosition = new Vector3(8.5f, 0.5f, -3.5f);
//        Target2.localPosition = new Vector3(1.5f, 0.5f, -3.5f);

//        //oldX = 20;
//        //oldZ = 20;
//    }

//    //public void droneInfo()
//    //{
//    //    Drone1.RayCastUpdate(out agentLoc, out endLoc, out wallLoc, out opeLoc, out targLoc);
//    //}

//    public override void CollectObservations(VectorSensor sensor)
//    {

//        RayCastUpdate(out agent_agentLoc, out agent_endLoc, out agent_wallLoc, out agent_opeLoc, out agent_firsttargLoc, out agent_groundLoc, out agent_finaltargLoc, out agent_secondtargLoc);
//        //agentOldPos = agentLoc[0];
//        foreach (var agentLocation in agent_agentLoc)
//        {
//            sensor.AddObservation(agentLocation);
//            agentPos = agentLocation;
//            AddReward(0.00005f);
//            //AddReward(0.000005f);
//        }
//        foreach (var endLocation in agent_endLoc)
//        {
//            sensor.AddObservation(endLocation);
//            AddReward(0.00005f);
//        }
//        foreach (var wallLocation in agent_wallLoc)
//        {
//            sensor.AddObservation(wallLocation);
//            //AddReward(0.00005f);
//            //UnityEngine.Debug.Log(wallLocation);
//        }
//        foreach (var opeLocation in agent_opeLoc)
//        {
//            sensor.AddObservation(opeLocation);
//            AddReward(0.00005f);
//        }
//        foreach (var firsttargLocation in agent_firsttargLoc)
//        {
//            sensor.AddObservation(firsttargLocation);
//            AddReward(0.00005f);

//        }
//        foreach (var secondtargLocation in agent_secondtargLoc)
//        {
//            sensor.AddObservation(secondtargLocation);
//            AddReward(0.00005f);

//        }
//        foreach (var finaltargLocation in agent_finaltargLoc)
//        {
//            sensor.AddObservation(finaltargLocation);
//            AddReward(0.00005f);
//            //AddReward(0.000005f);
//            //UnityEngine.Debug.Log(finaltargLocation);

//        }
//        sensor.AddObservation(this.transform.localPosition);
//        sensor.AddObservation(this.rBody.velocity);

//    }


//    public override void OnActionReceived(float[] vectorAction)
//    {
//        Vector3 controlSignal = Vector3.zero;
//        controlSignal.x = vectorAction[0];
//        controlSignal.z = vectorAction[1];
//        rBody.AddForce(controlSignal * speed);

//    }


//    void OnCollisionEnter(Collision collision)
//    {
//        if (collision.collider.CompareTag("Walls"))
//        {
//            UnityEngine.Debug.Log("Drone Wall Hit");
//            AddReward(-0.05f);
//            EndEpisode();
//            RollerAgent.EndEpisode();
//        }



//    }
//    //public void InfoProcessor(out Vector3 return1)
//    //{
//    //    RayCastUpdate(out agent_agentLoc, out agent_endLoc, out agent_wallLoc, out agent_opeLoc, out agent_firsttargLoc, out agent_groundLoc, out agent_finaltargLoc, out agent_secondtargLoc);
//    //    //agentOldPos = agentLoc[0];
//    //    foreach (var agentLocation in agent_agentLoc)
//    //    {
//    //        agentPos = agentLocation;
//    //    }
//    //    return1 = agentPos;
//    //}
//    public void RayCastUpdate(out List<Vector3> return1, out List<Vector3> return2, out List<Vector3> return3, out List<Vector3> return4, out List<Vector3> return5, out List<Vector3> return6, out List<Vector3> return7, out List<Vector3> return8)
//    {
//        agent_opeLoc = new List<Vector3>();
//        agent_firsttargLoc = new List<Vector3>();
//        agent_secondtargLoc = new List<Vector3>();
//        agent_finaltargLoc = new List<Vector3>();
//        agent_wallLoc = new List<Vector3>();
//        agent_endLoc = new List<Vector3>();
//        agent_agentLoc = new List<Vector3>();
//        agent_groundLoc = new List<Vector3>();
//        foreach (var direction in SphereDirections(numDirection))
//        {
//            //Debug.DrawRay(transform.position, direction, Color.blue);


//            RaycastHit hit;
//            if (Physics.Raycast(transform.position, direction, out hit, maxDistance))
//            {
//                if (hit.collider.tag == "RollerAgent")
//                {
//                    //agent_agentLoc.Add(hit.normal);
//                    agent_agentLoc.Add(hit.point);
//                    //UnityEngine.Debug.Log("Agent detected" + agentLoc);
//                }
//                if (hit.collider.tag == "EndGame")
//                {
//                    //agent_endLoc.Add(hit.normal);
//                    agent_endLoc.Add(hit.point);
//                    //UnityEngine.Debug.Log("Ender detected" + endLoc);
//                }
//                if (hit.collider.tag == "Walls")
//                {
//                    //agent_wallLoc.Add(hit.normal);
//                    agent_wallLoc.Add(hit.point); // adding an example Vector3
//                    //wall = hit.point;
//                    //UnityEngine.Debug.Log("Walls detected" + wall);
//                }
//                if (hit.collider.tag == "Interactive")
//                {
//                    //agent_opeLoc.Add(hit.normal);
//                    agent_opeLoc.Add(hit.point);
//                    //UnityEngine.Debug.Log("Operator detected"+ opeLoc);
//                }
//                if (hit.collider.tag == "firstTarget")
//                {
//                    //agent_firsttargLoc.Add(hit.normal);
//                    agent_firsttargLoc.Add(hit.point);
//                    //UnityEngine.Debug.Log("target detected"+ targLoc);
//                }
//                if (hit.collider.tag == "secondTarget")
//                {
//                    //agent_secondtargLoc.Add(hit.normal);
//                    agent_secondtargLoc.Add(hit.point);
//                    //UnityEngine.Debug.Log("target detected"+ targLoc);
//                }
//                if (hit.collider.tag == "finalTarget")
//                {
//                    //agent_finaltargLoc.Add(hit.normal);
//                    agent_finaltargLoc.Add(hit.point);
//                    //UnityEngine.Debug.Log("target detected"+ targLoc);
//                }
//                if (hit.collider.tag == "Ground")
//                {

//                    //agent_groundLoc.Add(hit.normal);
//                    agent_groundLoc.Add(hit.point);
//                    //UnityEngine.Debug.Log("target detected"+ targLoc);
//                }
//            }
//        }
//        return1 = agent_agentLoc;
//        return2 = agent_endLoc;
//        return3 = agent_wallLoc;
//        return4 = agent_opeLoc;
//        return5 = agent_firsttargLoc;
//        return6 = agent_groundLoc;
//        return7 = agent_finaltargLoc;
//        return8 = agent_secondtargLoc;
//    }


//    private Vector3[] SphereDirections(int numDirections)
//    {
//        var pts = new Vector3[numDirections];
//        var il = Math.PI * (3 - Math.Sqrt(5));
//        var idd = 2f / numDirections;

//        foreach (var i in Enumerable.Range(0, numDirections))
//        {
//            var y = i * idd - 1 + (idd / 2);
//            var r = Math.Sqrt(1 - y * y);
//            var phi = i * il;
//            var x = (float)(Math.Cos(phi) * r);
//            var z = (float)(Math.Sin(phi) * r);
//            pts[i] = new Vector3(x, y, z);
//        }

//        return pts;
//    }
//}



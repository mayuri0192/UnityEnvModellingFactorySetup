using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System.Collections.Specialized;
using System.CodeDom;
using System.Diagnostics;
using System.Linq;
using System;

public class RollerAgentScript : Agent
{
    Rigidbody rBody;
    NavMeshAgent agent;

    Vector3 drone_endLoc;
    Vector3 drone_agentLoc;
    Vector3 drone_opeLoc;
    Vector3 drone_firstTargLoc;
    Vector3 drone_secondTargLoc;
    Vector3 drone_finalTargLoc;
    
    Vector3 agent_endLoc;
    Vector3 agent_opeLoc;
    Vector3 agent_firstTargLoc;
    Vector3 agent_secondTargLoc;
    Vector3 agent_finalTargLoc;

    bool firstTargetHit;
    bool secondTargetHit;

    public Drone agent_Drone;
    public OperatorScript1 Operator1;
    public OperatorScript2 Operator2;
    public TargetScript Target;
    public TargetScript1 Target1;
    public TargetScript2 Target2;

    private const int numDirection = 180;
    private const int maxDistance = 20;

    private float speed = 10;

    private NavMeshPath path;

    public override void Initialize()
    {
        rBody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
    }

    public override void OnEpisodeBegin()

    {
        path = new NavMeshPath();

        this.transform.localPosition = new Vector3(1.5f, 0.5f, 8);
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;

        //this.agent.updatePosition = false;
        //this.agent.updateRotation = false;


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

        firstTargetHit = false;
        secondTargetHit = false;

        agent_endLoc = Vector3.zero;
        agent_opeLoc = Vector3.zero;
        agent_firstTargLoc = Vector3.zero;
        agent_secondTargLoc = Vector3.zero;
        agent_finalTargLoc = Vector3.zero;
        
        drone_agentLoc = Vector3.zero;
        drone_endLoc = Vector3.zero;
        drone_opeLoc = Vector3.zero;
        drone_firstTargLoc = Vector3.zero;
        drone_secondTargLoc = Vector3.zero;
        drone_finalTargLoc = Vector3.zero;
        
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        RayCastUpdate(out agent_endLoc, out agent_opeLoc, out agent_firstTargLoc, out agent_secondTargLoc, out agent_finalTargLoc);
        if (agent_endLoc != Vector3.zero)
        {
            sensor.AddObservation(agent_endLoc);
        }
        if (agent_opeLoc != Vector3.zero)
        {
            sensor.AddObservation(agent_opeLoc);
        }
        if (firstTargetHit == false && secondTargetHit == false)
        {
            NavMesh.CalculatePath(this.transform.localPosition, agent_firstTargLoc, NavMesh.AllAreas, path);
            foreach (var pathToTarget in path.corners)
            {
                sensor.AddObservation(pathToTarget);
            }
            this.agent.SetDestination(agent_firstTargLoc);
        }
        if (firstTargetHit == true && secondTargetHit == false)
        {
            NavMesh.CalculatePath(this.transform.localPosition, agent_secondTargLoc, NavMesh.AllAreas, path);
            foreach (var pathToTarget in path.corners)
            {
                sensor.AddObservation(pathToTarget);
            }
            this.agent.SetDestination(agent_secondTargLoc);
        }
        if (secondTargetHit == true)
        {
            NavMesh.CalculatePath(this.transform.localPosition, agent_finalTargLoc, NavMesh.AllAreas, path);
            foreach (var pathToTarget in path.corners)
            {
                sensor.AddObservation(pathToTarget);
            }
            this.agent.SetDestination(agent_finalTargLoc);
        }
        //Raycast info from drone
        agent_Drone.RayCastUpdate(out drone_agentLoc, out drone_endLoc, out drone_opeLoc, out drone_firstTargLoc, out drone_secondTargLoc, out drone_finalTargLoc);
        if (drone_endLoc != Vector3.zero && agent_endLoc == Vector3.zero)
        {
            sensor.AddObservation(drone_endLoc);
        }
        if (drone_opeLoc != Vector3.zero && agent_opeLoc == Vector3.zero)
        {
            sensor.AddObservation(drone_opeLoc);
        }
        if (firstTargetHit == false && secondTargetHit == false && agent_firstTargLoc == Vector3.zero)
        {
            NavMesh.CalculatePath(drone_agentLoc, drone_firstTargLoc, NavMesh.AllAreas, path);
            foreach (var pathToTarget in path.corners)
            {
                sensor.AddObservation(pathToTarget);
            }
            this.agent.SetDestination(drone_firstTargLoc);
        }
        if (firstTargetHit == true && secondTargetHit == false && agent_secondTargLoc == Vector3.zero)
        {
            NavMesh.CalculatePath(drone_agentLoc, drone_secondTargLoc, NavMesh.AllAreas, path);
            foreach (var pathToTarget in path.corners)
            {
                sensor.AddObservation(pathToTarget);
            }
            this.agent.SetDestination(drone_secondTargLoc);
        } 
        if(secondTargetHit == true && agent_finalTargLoc == Vector3.zero)
        {
            NavMesh.CalculatePath(drone_agentLoc, drone_finalTargLoc, NavMesh.AllAreas, path);
            foreach (var pathToTarget in path.corners)
            {
                sensor.AddObservation(pathToTarget);
            }
            this.agent.SetDestination(drone_finalTargLoc);
        }
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(rBody.velocity);

        agent_endLoc = Vector3.zero;
        agent_opeLoc = Vector3.zero;
        agent_firstTargLoc = Vector3.zero;
        agent_secondTargLoc = Vector3.zero;
        agent_finalTargLoc = Vector3.zero;

        drone_agentLoc = Vector3.zero;
        drone_endLoc = Vector3.zero;
        drone_opeLoc = Vector3.zero;
        drone_firstTargLoc = Vector3.zero;
        drone_secondTargLoc = Vector3.zero;
        drone_finalTargLoc = Vector3.zero;
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        //rBody.AddForce(controlSignal * speed);
        //agent.nextPosition = rBody.position;
        AddReward(-0.00001f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Walls"))
        {
            UnityEngine.Debug.Log("Wall Hit");
            AddReward(-0.05f);
            //EndEpisode();
            //agent_Drone.EndEpisode();
        }
        if (collision.collider.CompareTag("EndGame"))
        {
            AddReward(-0.05f);
            UnityEngine.Debug.Log("Operator Hit Game End");
            EndEpisode();
            agent_Drone.EndEpisode();
        }
        if (collision.collider.CompareTag("Interactive"))
        {
            AddReward(-0.05f);
            UnityEngine.Debug.Log("Operator Hit");
            EndEpisode();
            agent_Drone.EndEpisode();
        }
        if (firstTargetHit != true)
        {
            if (collision.collider.CompareTag("firstTarget"))
            {
                AddReward(0.5f);
                UnityEngine.Debug.Log("First Target Hit");
                firstTargetHit = true;
            }
        }
        if (secondTargetHit != true)
        {
            if (collision.collider.CompareTag("secondTarget"))
            {
                AddReward(1.0f);
                UnityEngine.Debug.Log("Second Target Hit");
                secondTargetHit = true;
            }
        }
        if (collision.collider.CompareTag("finalTarget"))
        {
            AddReward(1.5f);
            UnityEngine.Debug.Log("Target Hit Game End");
            EndEpisode();
            agent_Drone.EndEpisode();
        }
    }


    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }


    public void RayCastUpdate(out Vector3 return1, out Vector3 return2, out Vector3 return3, out Vector3 return4, out Vector3 return5)
    {
        foreach (var direction in SphereDirections(numDirection))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, maxDistance))
            {
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
        return1 = agent_endLoc;
        return2 = agent_opeLoc;
        return3 = agent_firstTargLoc;
        return4 = agent_secondTargLoc;
        return5 = agent_finalTargLoc;
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
//using UnityEngine.AI;
//using Unity.MLAgents;
//using Unity.MLAgents.Sensors;
//using System.Collections.Specialized;
//using System.CodeDom;
//using System.Diagnostics;
//using System.Linq;
//using System;

//public class RollerAgentScript : Agent
//{
//    Rigidbody rBody;
//    NavMeshAgent agent;

//    //public RayPerceptionSensor(string name, RayPerceptionInput rayInput);
//    //public Drone Drone1;

//    List<Vector3> drone_opeLoc = new List<Vector3>();
//    List<Vector3> drone_firsttargLoc = new List<Vector3>();
//    List<Vector3> drone_secondtargLoc = new List<Vector3>();
//    List<Vector3> drone_finaltargLoc = new List<Vector3>();
//    List<Vector3> drone_wallLoc = new List<Vector3>();
//    List<Vector3> drone_endLoc = new List<Vector3>();
//    List<Vector3> drone_agentLoc = new List<Vector3>();
//    List<Vector3> drone_groundLoc = new List<Vector3>();

//    List<Vector3> agent_opeLoc = new List<Vector3>();
//    List<Vector3> agent_firsttargLoc = new List<Vector3>();
//    List<Vector3> agent_secondtargLoc = new List<Vector3>();
//    List<Vector3> agent_finaltargLoc = new List<Vector3>();
//    List<Vector3> agent_wallLoc = new List<Vector3>();
//    List<Vector3> agent_endLoc = new List<Vector3>();
//    List<Vector3> agent_agentLoc = new List<Vector3>();
//    List<Vector3> agent_groundLoc = new List<Vector3>();

//    Vector3 direction;// not sure if i will use at this point
//    //https://forum.unity.com/threads/moving-to-navmesh-path-corners-individually-gets-stuck-on-the-first-corner.466008/
//    //i might have to use above to fix kinemetic issue

//    bool firstTargetHit;
//    bool secondTargetHit;

//    Vector3 drone_agentPos;

//    public Drone agent_Drone;
//    public OperatorScript1 Operator1;
//    public OperatorScript2 Operator2;
//    public TargetScript Target;
//    public TargetScript1 Target1;
//    public TargetScript2 Target2;

//    private const int numDirection = 180;
//    private const int maxDistance = 20;

//    public float speed = 20;

//    private NavMeshPath path;


//    public override void Initialize()
//    {
//        rBody = GetComponent<Rigidbody>();
//        agent = GetComponent<NavMeshAgent>();
//        //path = new NavMeshPath();
//    }



//    public override void OnEpisodeBegin()

//    {
//        path = new NavMeshPath();
//        this.transform.localPosition = new Vector3(1.5f, 0.5f, 8);
//        this.rBody.angularVelocity = Vector3.zero;
//        this.rBody.velocity = Vector3.zero;
//        //this.agent.velocity = Vector3.zero;


//        this.agent.updatePosition = false;
//        this.agent.updateRotation = false;


//        //agent = this.GetComponent<NavMeshAgent>();
//        //agent.transform.localPosition = new Vector3(1.5f, 0.5f, 8);
//        //agent.rBody.angularVelocity = Vector3.zero;
//        //agent.rBody.velocity = Vector3.zero;

//        Operator1.transform.localPosition = new Vector3(3, 0.5f, 5);
//        Operator1.rBody.angularVelocity = Vector3.zero;
//        Operator1.rBody.velocity = Vector3.zero;
//        Operator2.transform.localPosition = new Vector3(6, 0.5f, -3);
//        Operator2.rBody.angularVelocity = Vector3.zero;
//        Operator2.rBody.velocity = Vector3.zero;

//        Target.transform.localPosition = new Vector3(8.5f, 0.5f, 4.5f);
//        Target.rBody.angularVelocity = Vector3.zero;
//        Target.rBody.velocity = Vector3.zero;
//        Target1.transform.localPosition = new Vector3(8.5f, 0.5f, -3.5f);
//        Target1.rBody.angularVelocity = Vector3.zero;
//        Target1.rBody.velocity = Vector3.zero;
//        Target2.transform.localPosition = new Vector3(1.5f, 0.5f, -3.5f);
//        Target2.rBody.angularVelocity = Vector3.zero;
//        Target2.rBody.velocity = Vector3.zero;

//        firstTargetHit = false;
//        secondTargetHit = false;
//        direction = Vector3.zero;

//    }

//    //public void droneInfo()
//    //{
//    //    Drone1.RayCastUpdate(out agentLoc, out endLoc, out wallLoc, out opeLoc, out targLoc);
//    //}

//    public override void CollectObservations(VectorSensor sensor)
//    {
//        //AddReward(-0.0000005f);
//        RayCastUpdate(out agent_agentLoc, out agent_endLoc, out agent_wallLoc, out agent_opeLoc, out agent_firsttargLoc, out agent_groundLoc, out agent_finaltargLoc, out agent_secondtargLoc);
//        //agentOldPos = agentLoc[0];
//        //foreach (var agentLocation in agent_agentLoc)
//        //{
//        //    sensor.AddObservation(agentLocation);
//        //    //agentPos = agentLocation;
//        //}
//        foreach (var endLocation in agent_endLoc)
//        {
//            sensor.AddObservation(endLocation);
//        }
//        //foreach (var wallLocation in agent_wallLoc)
//        //{
//        //    sensor.AddObservation(wallLocation);
//        //    //UnityEngine.Debug.Log(wallLocation);
//        //}
//        foreach (var opeLocation in agent_opeLoc)
//        {
//            sensor.AddObservation(opeLocation);
//        }
//        foreach (var firsttargLocation in agent_firsttargLoc)
//        {

//            if (firstTargetHit == false && secondTargetHit == false)
//            {
//                //sensor.AddObservation(firsttargLocation);
//                NavMesh.CalculatePath(this.transform.localPosition, firsttargLocation, NavMesh.AllAreas, path);
//                foreach (var pathToTarget in path.corners)
//                {
//                    sensor.AddObservation(pathToTarget);
//                    //UnityEngine.Debug.Log(pathToTarget);
//                }
//                //direction = path.corners[1];
//                //UnityEngine.Debug.Log(this.transform.localPosition);
//                //UnityEngine.Debug.Log(path.corners[0]);
//                //UnityEngine.Debug.Log(path.corners[1]);
//                ////this.agent.SetDestination(drone_firsttargLocation);
//            }
//        }
//        foreach (var secondtargLocation in agent_secondtargLoc)
//        {
//            if (firstTargetHit == true && secondTargetHit == false)
//            {
//                sensor.AddObservation(secondtargLocation);
//                NavMesh.CalculatePath(this.transform.localPosition, secondtargLocation, NavMesh.AllAreas, path);
//            }
//        }
//        foreach (var finaltargLocation in agent_finaltargLoc)
//        {
//            if (secondTargetHit == true)
//            {
//                sensor.AddObservation(finaltargLocation);
//                NavMesh.CalculatePath(this.transform.localPosition, finaltargLocation, NavMesh.AllAreas, path);
//            }
//        }
//        //Raycast info from drone
//        agent_Drone.RayCastUpdate(out drone_agentLoc, out drone_endLoc, out drone_wallLoc, out drone_opeLoc, out drone_firsttargLoc, out drone_groundLoc, out drone_finaltargLoc, out drone_secondtargLoc);
//        foreach (var drone_agentLocation in drone_agentLoc)
//        {
//            //sensor.AddObservation(drone_agentLocation);
//            drone_agentPos = drone_agentLocation;
//            //UnityEngine.Debug.Log(drone_agentPos);
//        }
//        //agent_Drone.InfoProcessor(out drone_agentPos);
//        //UnityEngine.Debug.Log(drone_agentPos);

//        //foreach (var drone_endLocation in drone_endLoc)
//        //{
//        //    sensor.AddObservation(drone_endLocation);
//        //}
//        //foreach (var drone_wallLocation in drone_wallLoc)
//        //{
//        //    sensor.AddObservation(drone_wallLocation);
//        //    //UnityEngine.Debug.Log(wallLocation);
//        //}
//        //foreach (var drone_opeLocation in drone_opeLoc)
//        //{
//        //    sensor.AddObservation(drone_opeLocation);
//        //}

//        foreach (var drone_firsttargLocation in drone_firsttargLoc)
//        {
//            if (firstTargetHit == false && secondTargetHit == false)
//            {

//                //sensor.AddObservation(drone_firsttargLocation);
//                NavMesh.CalculatePath(drone_agentPos, drone_firsttargLocation, NavMesh.AllAreas, path);
//                //UnityEngine.Debug.Log(path.corners.Length);
//                foreach (var pathToTarget in path.corners)
//                {
//                    sensor.AddObservation(pathToTarget);
//                    //UnityEngine.Debug.Log(pathToTarget);
//                }
//            }
//        }
//        foreach (var drone_secondtargLocation in drone_secondtargLoc)
//        {
//            //Drone_secondTargLoc = drone_secondtargLocation;
//            if (firstTargetHit == true && secondTargetHit == false)
//            {
//                sensor.AddObservation(drone_secondtargLocation);
//                NavMesh.CalculatePath(drone_agentPos, drone_secondtargLocation, NavMesh.AllAreas, path);
//                //this.agent.SetDestination(drone_secondtargLocation);
//            }
//        }
//        foreach (var drone_finaltargLocation in drone_finaltargLoc)
//        {
//            //Drone_finalTargLoc = drone_finaltargLocation;

//            if (secondTargetHit == true)
//            {

//                sensor.AddObservation(drone_finaltargLocation);
//                NavMesh.CalculatePath(drone_agentPos, drone_finaltargLocation, NavMesh.AllAreas, path);
//                //this.agent.SetDestination(drone_finaltargLocation);
//            }
//        }
//        sensor.AddObservation(this.transform.localPosition);
//        sensor.AddObservation(rBody.velocity);
//    }


//    public override void OnActionReceived(float[] vectorAction)
//    {
//        Vector3 controlSignal = Vector3.zero;
//        controlSignal.x = vectorAction[0];
//        controlSignal.z = vectorAction[1];
//        rBody.AddForce(controlSignal * speed);
//        //if (path.corners.Length > 1)
//        //{
//        //    direction = path.corners[1];
//        //    direction.y = 0;
//        //    //UnityEngine.Debug.Log(direction);
//        //    rBody.AddForce(direction * speed);
//        //    path = new NavMeshPath();
//        //}
//        //else
//        //{
//        //    rBody.AddForce(controlSignal * speed);
//        //}
//        //rBody.AddForce(controlSignal * speed);
//        //rBody.AddForce(direction * speed);
//        agent.nextPosition = rBody.position;
//        //agent.Move(controlSignal * speed);
//        //AddReward(-0.00000005f);
//        AddReward(-0.00001f);
//    }


//    void OnCollisionEnter(Collision collision)
//    {
//        if (collision.collider.CompareTag("Walls"))
//        {
//            UnityEngine.Debug.Log("Wall Hit");
//            AddReward(-0.05f);
//            //EndEpisode();
//            //agent_Drone.EndEpisode();
//        }

//        if (collision.collider.CompareTag("EndGame"))
//        {

//            AddReward(-0.05f);
//            UnityEngine.Debug.Log("Operator Hit Game End");
//            EndEpisode();
//            agent_Drone.EndEpisode();
//        }

//        if (collision.collider.CompareTag("Interactive"))
//        {

//            AddReward(-0.05f);
//            UnityEngine.Debug.Log("Operator Hit");
//            EndEpisode();
//            agent_Drone.EndEpisode();
//        }
//        if (firstTargetHit != true)
//        {
//            if (collision.collider.CompareTag("firstTarget"))
//            {
//                UnityEngine.Debug.Log("First Target Hit");
//                //AddReward(0.1f);
//                firstTargetHit = true;
//                //EndEpisode();
//                //agent_Drone.EndEpisode();
//            }
//        }
//        if (secondTargetHit != true)
//        {
//            if (collision.collider.CompareTag("secondTarget"))
//            {
//                UnityEngine.Debug.Log("Second Target Hit");
//                //AddReward(2.5f);
//                secondTargetHit = true;
//                //EndEpisode();
//                //agent_Drone.EndEpisode();
//            }
//        }

//        if (collision.collider.CompareTag("finalTarget"))
//        {

//            AddReward(1.0f);
//            UnityEngine.Debug.Log("Target Hit Game End");
//            EndEpisode();
//            agent_Drone.EndEpisode();
//        }
//    }

//    //public void Collision()
//    //{

//    //}

//    public override void Heuristic(float[] actionsOut)
//    {
//        actionsOut[0] = Input.GetAxis("Horizontal");
//        actionsOut[1] = Input.GetAxis("Vertical");
//    }


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
//                if (hit.collider.tag == "Drone")
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

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
    Rigidbody rBody;//Assigned Rigidbody component as rBody
    NavMeshAgent agent;//Assigned NavMeshAgent as agent

    //Following variables store raycast hit info of the Vector3 position each tag was hit
    //Information collected from Drone Agent
    Vector3 drone_agentLoc;//Rollar Agent (Start at: North-West)
    Vector3 drone_endLoc;//Operator 1 (Left)
    Vector3 drone_opeLoc;//Operator 2 (Right)
    Vector3 drone_firstTargLoc;//Target 1 (South-West)
    Vector3 drone_secondTargLoc;//Target 2 (South-East)
    Vector3 drone_finalTargLoc;//Target 3 (North-East)
    //Information collected from Rollar Agent
    Vector3 agent_endLoc;//Operator 1 (Left)
    Vector3 agent_opeLoc;//Operator 2 (Right)
    Vector3 agent_firstTargLoc;//Target 1 (South-West)
    Vector3 agent_secondTargLoc;//Target 2 (South-East)
    Vector3 agent_finalTargLoc;//Target 3 (North-East)

    bool firstTargetHit;//Varification for collide status between the Target 1 and the Roller Agent
    bool secondTargetHit;//Varification for collide status between the Target 2 and the Roller Agent

    public Drone agent_Drone;//Calling agent_Drone from the Dron script

    public OperatorScript1 Operator1;//Calling Operator 1 from OperatorScript1
    public OperatorScript2 Operator2;//Calling Operator 1 from OperatorScript1
    
    public TargetScript1 Target1;//Calling Target 1 from TargetScript1
    public TargetScript2 Target2;//Calling Target 2 from TargetScript2
    public TargetScript Target;//Calling Target 3 from TargetScript

    private const int numDirection = 180;//Setting number of direction that Roller Agent can see
    //Direction of which the Roller Agent can see can decrease
    //Decreasing direction will make traing slower as it is harder for agent to spot object
    //Direction is in 3D which is different than 1D of RayPerception Sensor
    //However since the ray is going on all around the agent, it will be better to change the code 
    //so that we could narrow down the angle
    private const int maxDistance = 20;//Setting maximum distance that ray can perceive
    //Unlike ray perception sensor, ray distance does not matter as much

    private float speed = 10;//Speed of Roller Agent

    private NavMeshPath path;//NavMeshPath stores path information on path variable

    public override void Initialize()
    {
        rBody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
    }

    public override void OnEpisodeBegin()

    {
        path = new NavMeshPath();//Renew memorized path for beginning of the every new episode

        this.transform.localPosition = new Vector3(1.5f, 0.5f, 8);//RollerAgent start location
        this.rBody.angularVelocity = Vector3.zero;//RollerAgent start angular velocity
        this.rBody.velocity = Vector3.zero;//RollerAgent start veloicty

        //this.agent.updatePosition = false;//updatePosition: false will not allow NavMesh to move the RollerAgent
        //this.agent.updateRotation = false;//updateRotation: false will not allow NavMesh to rotate the RollerAgent

        //Above code is commented because this code currently uses SetDestination to move the agent
        //If we want to only use the Rigidbody to move the agent than we need to uncomment the code
        //However, as described in the power point, there are methods to combine both

        Operator1.transform.localPosition = new Vector3(3, 0.5f, 5);//Operator 1 start location
        Operator1.rBody.angularVelocity = Vector3.zero;
        Operator1.rBody.velocity = Vector3.zero;
        Operator2.transform.localPosition = new Vector3(6, 0.5f, -3);//Operator 2 start location
        Operator2.rBody.angularVelocity = Vector3.zero;
        Operator2.rBody.velocity = Vector3.zero;

        Target.transform.localPosition = new Vector3(8.5f, 0.5f, 4.5f);//Target 3 start location
        Target.rBody.angularVelocity = Vector3.zero;
        Target.rBody.velocity = Vector3.zero;
        Target1.transform.localPosition = new Vector3(8.5f, 0.5f, -3.5f);//Target 1 start location
        Target1.rBody.angularVelocity = Vector3.zero;
        Target1.rBody.velocity = Vector3.zero;
        Target2.transform.localPosition = new Vector3(1.5f, 0.5f, -3.5f);//Target 2 start location
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
        //RayCast from Roller Agent
        //agent_endLoc: Operator 1
        //agent_opeLoc: Operator 2
        //agent_firstTargLoc: Target 1
        //agent_secondTargLoc: Target 2
        //agent_finalTargLoc: Target 3
        if (agent_endLoc != Vector3.zero)//if there are hit info Vector3 will be not zero
        {
            sensor.AddObservation(agent_endLoc);//Observe Operator 1
        }
        if (agent_opeLoc != Vector3.zero)
        {
            sensor.AddObservation(agent_opeLoc);//Observe Operator 2
        }
        if (firstTargetHit == false && secondTargetHit == false)
        {
            NavMesh.CalculatePath(this.transform.localPosition, agent_firstTargLoc, NavMesh.AllAreas, path);
            //Calculate ideal route from RollerAgent's current position to the Target location and saves array of vectors data
            //Access the array of vector data by using path.corners
            foreach (var pathToTarget in path.corners)//pathToTarget represents each Vector 3 location info in path.corners
            {
                sensor.AddObservation(pathToTarget);//Observe each path point
            }
            //Above code can be commented from start of NavMesch.CalculatePath since we are using SetDestination
            this.agent.SetDestination(agent_firstTargLoc);//Method that automatically finds ideal route and automatically move agent to Target 1
        }
        if (firstTargetHit == true && secondTargetHit == false)
        {
            NavMesh.CalculatePath(this.transform.localPosition, agent_secondTargLoc, NavMesh.AllAreas, path);
            foreach (var pathToTarget in path.corners)
            {
                sensor.AddObservation(pathToTarget);//Observe each path point
            }
            //Above code can be commented from start of NavMesch.CalculatePath since we are using SetDestination
            this.agent.SetDestination(agent_secondTargLoc);//Method that automatically finds ideal route and automatically move agent to Target 2
        }
        if (secondTargetHit == true)//So that even if Agent does not reach the first target and go straight to second target,
            //it will be able to reach the Target 3
        {
            NavMesh.CalculatePath(this.transform.localPosition, agent_finalTargLoc, NavMesh.AllAreas, path);
            foreach (var pathToTarget in path.corners)
            {
                sensor.AddObservation(pathToTarget);//Observe each path point
            }
            //Above code can be commented from start of NavMesch.CalculatePath since we are using SetDestination
            this.agent.SetDestination(agent_finalTargLoc);//Method that automatically finds ideal route and automatically move agent to Target 3
        }
        
        agent_Drone.RayCastUpdate(out drone_agentLoc, out drone_endLoc, out drone_opeLoc, out drone_firstTargLoc, out drone_secondTargLoc, out drone_finalTargLoc);
        //Raycast from Drone Agent
        //drone_agentLoc: Roller Agent
        //drone_endLoc: Operator 1
        //drone_opeLoc: Operator 2
        //drone_firstTargLoc: Target 1
        //drone_secondTargLoc: Target 2
        //drone_finalTargLoc: Target 3
        if (drone_endLoc != Vector3.zero && agent_endLoc == Vector3.zero)
        //Check 1: if Operator 1 has been spotted by Drone Agent. Continue if True (!= Vector3.zero)
        //Check 2: if Operator 1 has been spotted by Roller Agent before. Continue if False (Vector3.zero)
        {
            sensor.AddObservation(drone_endLoc);//Observe Operator 1
        }
        if (drone_opeLoc != Vector3.zero && agent_opeLoc == Vector3.zero)
        //Check 1: if Operator 2 has been spotted by Drone Agent. Continue if True (!= Vector3.zero)
        //Check 2: if Operator 2 has been spotted by Roller Agent before. Continue if False (Vector3.zero)
        {
            sensor.AddObservation(drone_opeLoc);//Observe Operator 2
        }
        if (firstTargetHit == false && secondTargetHit == false && agent_firstTargLoc == Vector3.zero)
        {
            NavMesh.CalculatePath(drone_agentLoc, drone_firstTargLoc, NavMesh.AllAreas, path);
            //Calculate ideal route from RollerAgent's position observed by Drone Agent to the Target location observed by Drone Agent and saves array of vectors data
            //Access the array of vector data by using path.corners
            foreach (var pathToTarget in path.corners)//pathToTarget represents each Vector 3 location info in path.corners
            {
                sensor.AddObservation(pathToTarget);//Observe each path point
            }
            this.agent.SetDestination(drone_firstTargLoc);//Method that automatically finds ideal route and automatically move agent to Target 1
        }
        if (firstTargetHit == true && secondTargetHit == false && agent_secondTargLoc == Vector3.zero)
        {
            NavMesh.CalculatePath(drone_agentLoc, drone_secondTargLoc, NavMesh.AllAreas, path);
            foreach (var pathToTarget in path.corners)
            {
                sensor.AddObservation(pathToTarget);//Observe each path point
            }
            this.agent.SetDestination(drone_secondTargLoc);//Method that automatically finds ideal route and automatically move agent to Target 2
        } 
        if(secondTargetHit == true && agent_finalTargLoc == Vector3.zero)
        {
            NavMesh.CalculatePath(drone_agentLoc, drone_finalTargLoc, NavMesh.AllAreas, path);
            foreach (var pathToTarget in path.corners)
            {
                sensor.AddObservation(pathToTarget);//Observe each path point
            }
            this.agent.SetDestination(drone_finalTargLoc);//Method that automatically finds ideal route and automatically move agent to Target 3
        }
        sensor.AddObservation(this.transform.localPosition);//Observe current position of Roller Agent
        sensor.AddObservation(rBody.velocity);//Observe velocity of Roller Agent
        //Initialize all the variable to false
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

    public override void OnActionReceived(float[] vectorAction)//Method that commands next movement.
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        //rBody.AddForce(controlSignal * speed);//Commented because we are using SetDestination to move Roller Agent
        //agent.nextPosition = rBody.position;//Commented because we have commented the line 72 and 73
        AddReward(-0.00001f);//Negative reward to increse time performence
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Walls"))
        {
            AddReward(-0.05f);
            UnityEngine.Debug.Log("Wall Hit");
            //EndEpisode();//Commented to improve result and shorten learning time
            //agent_Drone.EndEpisode();//Commented to improve result and shorten learning time
        }
        if (collision.collider.CompareTag("EndGame"))//Operator 1 has tag of EndGame
        {
            AddReward(-0.05f);
            UnityEngine.Debug.Log("Operator Hit Game End");
            EndEpisode();//End episode of Roller Agent
            agent_Drone.EndEpisode();//End episode of Drone Agent
        }
        if (collision.collider.CompareTag("Interactive"))//Operator 2 has tag of EndGame
        {
            AddReward(-0.05f);
            UnityEngine.Debug.Log("Operator Hit");
            EndEpisode();
            agent_Drone.EndEpisode();
        }
        if (firstTargetHit != true)//If Target 1 has not been hit
        {
            if (collision.collider.CompareTag("firstTarget"))//Target 1 has tag of firstTarget
            {
                AddReward(0.5f);
                UnityEngine.Debug.Log("First Target Hit");
                firstTargetHit = true;
            }
        }
        if (secondTargetHit != true)//If Target 2 has not been hit
        {
            if (collision.collider.CompareTag("secondTarget"))//Target 2 has tag of secondTarget
            {
                AddReward(1.0f);
                UnityEngine.Debug.Log("Second Target Hit");
                secondTargetHit = true;
            }
        }
        if (collision.collider.CompareTag("finalTarget"))//Target 3 has tag of finalTarget
        {
            AddReward(1.5f);
            UnityEngine.Debug.Log("Target Hit Game End");
            EndEpisode();
            agent_Drone.EndEpisode();
        }
    }


    public override void Heuristic(float[] actionsOut)//Control Heuristically with arrows
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }


    public void RayCastUpdate(out Vector3 return1, out Vector3 return2, out Vector3 return3, out Vector3 return4, out Vector3 return5)
    {
        //RayCast from Roller Agent
        //Changed from Vector3[] to Vector3 for each returns
        //Such change will result in saving only the last ray hit for each object
        //If we want to notice diverse location under same tag such as walls, we need to use Vector3 and use forloop in collect observation
        foreach (var direction in SphereDirections(numDirection))
        //gets all the directions from SphereDirections method
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, maxDistance))
            {
                if (hit.collider.tag == "EndGame")//EndGame: Operator 1
                {
                    agent_endLoc = hit.point;//hit.point stores location where ray was hit.
                }
                if (hit.collider.tag == "Interactive")//Interactive: Operator 2
                {
                    agent_opeLoc = hit.point;
                }
                if (hit.collider.tag == "firstTarget")//firstTarget: Target 1
                {
                    agent_firstTargLoc = hit.point;
                }
                if (hit.collider.tag == "secondTarget")//secondTarget: Target 2
                {
                    agent_secondTargLoc = hit.point;
                }
                if (hit.collider.tag == "finalTarget")//finalTarget: Target 3
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
//
//End of current version of the code
//




//Following is the old code that I kept for refernces in case we have to use some features that we threw out on the current version
//WARNING: Follwing code has not been coded cleanly. Proceed if you can withstand bad syntax.
//
//
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

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
using System.Security.Cryptography;

public class OperatorScript1 : Agent
{
    Rigidbody rBody;//Assigned Rigidbody component as rBody
    NavMeshAgent nAgent;//Assigned NavMeshAgent as agent

    //Following variables store raycast hit info of the Vector3 position each tag was hit

    //Information collected from Drone Agent
    Vector3 drone_agentLoc2;//Robot Agent 2
    Vector3 drone_opeLoc1;//Operator 1 
    Vector3 drone_targLoc1;//Target 1 
    Vector3 drone_targLoc2;//Target 2 
    Vector3 drone_targLoc3;//Target 3 
    Vector3 drone_targLoc4;//Target 4
    Vector3 drone_targLoc5;//Target 5
    Vector3 drone_targLoc6;//Target 6 
    Vector3 drone_targLoc7;//Target 7

    //Information collected from Operator Agent 1
    Vector3 agent_agentLoc2;//Robot Agent 2
    Vector3 agent_targLoc1;//Target 1 
    Vector3 agent_targLoc2;//Target 2 
    Vector3 agent_targLoc3;//Target 3 
    Vector3 agent_targLoc4;//Target 4 
    Vector3 agent_targLoc5;//Target 5 
    Vector3 agent_targLoc6;//Target 6 
    Vector3 agent_targLoc7;//Target 7

    Vector3 operatorDestination1;//Destination for Operator
    bool workStatus_operator1;//Working status for Operator
    public int newPriority;
    public int priority;
    public int reach;

    public Vector3 targLoc1;
    public Vector3 targLoc2;
    public Vector3 targLoc3;
    public Vector3 targLoc4;
    public Vector3 targLoc5;
    public Vector3 targLoc6;
    public Vector3 targLoc7;

    public Drone agent_Drone;//Calling agent_Drone from the Dron script
    public Agent2 RobotAgent2;//Calling RobotAgent3 from the Agent3 script

    public TargetScript1 Target1;//Calling Target 1 from TargetScript1
    public TargetScript2 Target2;//Calling Target 2 from TargetScript2
    public TargetScript3 Target3;//Calling Target 3 from TargetScript3
    public TargetScript4 Target4;//Calling Target 4 from TargetScript4
    public TargetScript5 Target5;//Calling Target 5 from TargetScript5
    public TargetScript6 Target6;//Calling Target 6 from TargetScript6
    public TargetScript7 Target7;//Calling Target 7 from TargetScript7

    private const int numDirection = 100;//Setting number of direction that Roller Agent can see
    //Direction of which the Robot Agent2 can see can decrease
    //Decreasing direction will make traing slower as it is harder for agent to spot object
    //Direction is in 3D which is different than 1D of RayPerception Sensor
    //However since the ray is going on all around the agent, it will be better to change the code 
    //so that we could narrow down the angle
    private const int maxDistance = 20;//Setting maximum distance that ray can perceive
    //Unlike ray perception sensor, ray distance does not matter as much
    private float speed = 5;//Speed of Robot Agent2

    public override void Initialize()
    {
        rBody = GetComponent<Rigidbody>();
        nAgent = GetComponent<NavMeshAgent>();
    }

    public override void OnEpisodeBegin()

    {
        //this.transform.localPosition = new Vector3(-6, 1, 0);//RollerAgent start location
        //this.nAgent.transform.position = new Vector3(-6, 1, 0);
        //this.rBody.angularVelocity = Vector3.zero;//RollerAgent start angular velocity
        //this.rBody.velocity = Vector3.zero;//RollerAgent start veloicty
        nAgent.transform.position = new Vector3(-6, 1, 10);
        nAgent.ResetPath();
        rBody.position = nAgent.transform.position;
        rBody.velocity = Vector3.zero;//RollerAgent start veloicty



        //Information collected from Drone
        drone_agentLoc2 = Vector3.zero;//Robot Agent 3
        drone_opeLoc1 = Vector3.zero;//Operator 1 
        drone_targLoc1 = Vector3.zero;//Target 1 
        drone_targLoc2 = Vector3.zero;//Target 2 
        drone_targLoc3 = Vector3.zero;//Target 3 
        drone_targLoc4 = Vector3.zero;//Target 4
        drone_targLoc5 = Vector3.zero;//Target 5
        drone_targLoc6 = Vector3.zero;//Target 6 
        drone_targLoc7 = Vector3.zero;//Target 7

        //Information collected from Operator
        agent_agentLoc2 = Vector3.zero;//Robot Agent 2
        agent_targLoc1 = Vector3.zero;//Target 1 
        agent_targLoc2 = Vector3.zero;//Target 2 
        agent_targLoc3 = Vector3.zero;//Target 3 
        agent_targLoc4 = Vector3.zero;//Target 4 
        agent_targLoc5 = Vector3.zero;//Target 5 
        agent_targLoc6 = Vector3.zero;//Target 6 
        agent_targLoc7 = Vector3.zero;//Target 7

        operatorDestination1 = Vector3.zero;
        workStatus_operator1 = false;//Working status for Operator
        newPriority = 0;
        priority = 0;
        reach = 0;

        targLoc1 = Vector3.zero;
        targLoc2 = Vector3.zero;
        targLoc3 = Vector3.zero;
        targLoc4 = Vector3.zero;
        targLoc5 = Vector3.zero;
        targLoc6 = Vector3.zero;
        targLoc7 = Vector3.zero;
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        //nAgent.Stop();
        RayCastUpdate(out agent_agentLoc2, out agent_targLoc1, out agent_targLoc2, out agent_targLoc3, out agent_targLoc4, out agent_targLoc5, out agent_targLoc6, out agent_targLoc7);
        //RayCast from Robot Agent 2
        //agent_agentLoc3: Agent 3
        //agent_opeLoc1: Operator 1
        //agent_opeLoc2: Operator 2
        //agent_opeLoc3: Operator 3
        //agent_targLoc1: Target 1
        //agent_targLoc2: Target 2
        //agent_targLoc3: Target 3
        //agent_targLoc4: Target 4
        //agent_targLoc5: Target 5
        //agent_targLoc6: Target 6
        //agent_targLoc7: Target 7
        agent_Drone.RayCastUpdate(out drone_agentLoc2, out drone_opeLoc1, out drone_targLoc1, out drone_targLoc2, out drone_targLoc3, out drone_targLoc4, out drone_targLoc5, out drone_targLoc6, out drone_targLoc7);
        //Raycast from Drone Agent
        //drone_agentLoc2: Agent 2
        //drone_agentLoc3: Agent 3
        //drone_opeLoc1: Operator 1
        //drone_opeLoc2: Operator 2
        //drone_opeLoc3: Operator 3
        //drone_targLoc1: Target 1
        //drone_targLoc2: Target 2
        //drone_targLoc3: Target 3
        //drone_targLoc4: Target 4
        //drone_targLoc5: Target 5
        //drone_targLoc6: Target 6
        //drone_targLoc7: Target 7
        if (agent_agentLoc2 == Vector3.zero && drone_agentLoc2 != Vector3.zero)
        {
            agent_agentLoc2 = drone_agentLoc2;
        }
        
        if (drone_targLoc1 != Vector3.zero)
        {
            targLoc1 = drone_targLoc1;
        }
        if (agent_targLoc1 != Vector3.zero)
        {
            targLoc1 = agent_targLoc1;
        }
        
        if (drone_targLoc2 != Vector3.zero)
        {
            targLoc2 = drone_targLoc2;
        }
        if (agent_targLoc2 != Vector3.zero)
        {
            targLoc2 = agent_targLoc2;
        }
        
        if (drone_targLoc3 != Vector3.zero)
        {
            targLoc3 = drone_targLoc3;
        }
        if (agent_targLoc3 != Vector3.zero)
        {
            targLoc3 = agent_targLoc3;
        }
        
        if (drone_targLoc4 != Vector3.zero)
        {
            targLoc4 = drone_targLoc4;
        }
        if (agent_targLoc4 != Vector3.zero)
        {
            targLoc4 = agent_targLoc4;
        }
        
        if (drone_targLoc5 != Vector3.zero)
        {
            targLoc5 = drone_targLoc5;
        }
        if (agent_targLoc5 != Vector3.zero)
        {
            targLoc5 = agent_targLoc5;
        }
        
        if (drone_targLoc6 != Vector3.zero)
        {
            targLoc6 = drone_targLoc6;
        }
        if (agent_targLoc6 != Vector3.zero)
        {
            targLoc6 = agent_targLoc6;
        }
        
        if (drone_targLoc7 != Vector3.zero)
        {
            targLoc7 = drone_targLoc7;
        }
        if (agent_targLoc7 != Vector3.zero)
        {
            targLoc7 = agent_targLoc7;
        }
        


        if (agent_agentLoc2 != Vector3.zero)//if there are hit info Vector3 will be not zero
        {
            sensor.AddObservation(agent_agentLoc2);//Observe Agent 3
        }
        if (newPriority -1 > reach)
        {
            //workStatus_operator1 = false;
            priority = reach + 1;
        }
        if (workStatus_operator1 == false)
        {
            if (priority == 1 && targLoc1 != Vector3.zero)
            {
                operatorDestination1 = targLoc1;//set Destination to  Target 1
                workStatus_operator1 = true;
                sensor.AddObservation(operatorDestination1);
            }
            if (priority == 2 && targLoc2 != Vector3.zero)
            {
                operatorDestination1 = targLoc2;//set Destination to  Target 2
                workStatus_operator1 = true;
                sensor.AddObservation(operatorDestination1);
            }
            if (priority == 3 && targLoc3 != Vector3.zero)
            {
                operatorDestination1 = targLoc3;//set Destination to  Target 3
                workStatus_operator1 = true;
                sensor.AddObservation(operatorDestination1);
            }
            if (priority == 4 && targLoc4 != Vector3.zero)
            {
                operatorDestination1 = targLoc4;//set Destination to  Target 4
                workStatus_operator1 = true;
                sensor.AddObservation(operatorDestination1);
            }
            if (priority == 5 && targLoc5 != Vector3.zero)
            {
                operatorDestination1 = targLoc5;//set Destination to  Target 5
                workStatus_operator1 = true;
                sensor.AddObservation(operatorDestination1);
            }
            if (priority == 6 && targLoc6 != Vector3.zero)
            {
                operatorDestination1 = targLoc6;//set Destination to  Target 6
                workStatus_operator1 = true;
                sensor.AddObservation(operatorDestination1);
            }
            if (priority == 7 && targLoc7 != Vector3.zero)
            {
                operatorDestination1 = targLoc7;//set Destination to  Target 7
                workStatus_operator1 = true;
                sensor.AddObservation(operatorDestination1);
            }
            if (priority == 8)
            {
                workStatus_operator1 = false;
            }
        }
        else
        {
            sensor.AddObservation(operatorDestination1);
            if (Mathf.Abs(operatorDestination1.x - nAgent.transform.position.x) < 1 && Mathf.Abs(operatorDestination1.z - nAgent.transform.position.z) < 1)
            {
                nAgent.ResetPath();
                workStatus_operator1 = false;
            }
        }
        sensor.AddObservation(this.transform.localPosition);//Observe current position of Roller Agent
        sensor.AddObservation(rBody.velocity);//Observe velocity of Roller Agent
        //Initialize all the variable to false
        //Information collected from Drone
        drone_agentLoc2 = Vector3.zero;//Robot Agent 2
        drone_opeLoc1 = Vector3.zero;//Operator 1 
        drone_targLoc1 = Vector3.zero;//Target 1 
        drone_targLoc2 = Vector3.zero;//Target 2 
        drone_targLoc3 = Vector3.zero;//Target 3 
        drone_targLoc4 = Vector3.zero;//Target 4
        drone_targLoc5 = Vector3.zero;//Target 5
        drone_targLoc6 = Vector3.zero;//Target 6 
        drone_targLoc7 = Vector3.zero;//Target 7
        //Information collected from Operator
        agent_agentLoc2 = Vector3.zero;//Robot Agent 2
        agent_targLoc1 = Vector3.zero;//Target 1 
        agent_targLoc2 = Vector3.zero;//Target 2 
        agent_targLoc3 = Vector3.zero;//Target 3 
        agent_targLoc4 = Vector3.zero;//Target 4 
        agent_targLoc5 = Vector3.zero;//Target 5 
        agent_targLoc6 = Vector3.zero;//Target 6 
        agent_targLoc7 = Vector3.zero;//Target 7
    }

    public override void OnActionReceived(float[] vectorAction)//Method that commands next movement.
    {
        if (workStatus_operator1 == false)
        {
            Vector3 controlSignal = Vector3.zero;
            controlSignal.x = vectorAction[0];
            controlSignal.z = vectorAction[1];
            rBody.AddForce(controlSignal * speed);
            nAgent.nextPosition = rBody.position;
        }
        else
        {
            this.nAgent.SetDestination(operatorDestination1);//Method that automatically finds ideal route and automatically move agent to Destination
            //rBody.position = nAgent.nextPosition;
        }
        //if (priority == 0)
        //{
        //    AddReward(-0.0001f);//Negative reward for not getting to priority quickly
        //}
        //if (priority == 1)
        //{
        //    AddReward(-0.00001f);//Negative reward for not getting to priority quickly
        //}
        //if (priority == 2)
        //{
        //    AddReward(-0.000001f);//Negative reward for not getting to priority quickly
        //}
        //if (priority == 3)
        //{
        //    AddReward(-0.0000001f);//Negative reward for not getting to priority quickly
        //}
        //if (priority == 4)
        //{
        //    AddReward(-0.00000001f);//Negative reward for not getting to priority quickly
        //}
        //if (priority == 5)
        //{
        //    AddReward(-0.000000001f);//Negative reward for not getting to priority quickly
        //}
        //if (priority == 6)
        //{
        //    AddReward(-0.0000000001f);//Negative reward for not getting to priority quickly
        //}
        //if (priority == 7)
        //{
        //    AddReward(-0.00000000001f);//Negative reward for not getting to priority quickly
        //}
        //AddReward(-0.0000001f);//Negative reward to increse time performence
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Walls"))
        {
            AddReward(-0.05f);
            UnityEngine.Debug.Log("Operator 1 - Wall Hit");
            if (workStatus_operator1 == true)
            {
                nAgent.ResetPath();
            }
            agent_Drone.EndEpisode();//End episode of Drone Agent
            RobotAgent2.EndEpisode();
            EndEpisode();
        }
        if (collision.collider.CompareTag("Agent2"))//Operator 2 has tag of EndGame
        {
            AddReward(-0.025f);
            UnityEngine.Debug.Log("Operator 1 - Agent 2 Hit");
            if (workStatus_operator1 == true)
            {
                nAgent.ResetPath();
            }
            agent_Drone.EndEpisode();//End episode of Drone Agent
            RobotAgent2.EndEpisode();
            EndEpisode();
        }
        if (priority == 1)//If Target 1 has not been hit
        {
            if (collision.collider.CompareTag("Target1"))//Target 1 has tag of Target1
            {
                AddReward(0.5f);
                UnityEngine.Debug.Log("Operator 1 - Target 1 Hit");
                if (workStatus_operator1 == true)
                {
                    nAgent.ResetPath();
                }
                reach = 1;
                priority = 8;
                //RobotAgent2.newPriority = 2;
                //RobotAgent3.newPriority = 2;
                //Operator2.newPriority = 2;
                //Operator3.newPriority = 2;
                workStatus_operator1 = false;
            }
        }
        if (priority == 2)//If Target 2 has not been hit
        {
            if (collision.collider.CompareTag("Target2"))//Target 2 has tag of Target2
            {
                AddReward(1.0f);
                UnityEngine.Debug.Log("Operator 1 - Target 2 Hit");
                if (workStatus_operator1 == true)
                {
                    nAgent.ResetPath();
                }
                reach = 2;
                priority = 8;
                //RobotAgent2.newPriority = 3;
                //RobotAgent3.newPriority = 3;
                //Operator2.newPriority = 3;
                //Operator3.newPriority = 3;
                workStatus_operator1 = false;
            }
        }
        if (priority == 3)//If Target 2 has not been hit
        {
            if (collision.collider.CompareTag("Target3"))//Target 2 has tag of Target2
            {
                AddReward(1.5f);
                UnityEngine.Debug.Log("Operator 1 - Target 3 Hit");
                if (workStatus_operator1 == true)
                {
                    nAgent.ResetPath();
                }
                reach = 3;
                priority = 8;
                //RobotAgent2.newPriority = 4;
                //RobotAgent3.newPriority = 4;
                //Operator2.newPriority = 4;
                //Operator3.newPriority = 4;
                workStatus_operator1 = false;
            }
        }
        if (priority == 4)//If Target 2 has not been hit
        {
            if (collision.collider.CompareTag("Target4"))//Target 2 has tag of Target2
            {
                AddReward(2.0f);
                UnityEngine.Debug.Log("Operator 1 - Target 4 Hit");
                if (workStatus_operator1 == true)
                {
                    nAgent.ResetPath();
                }
                reach = 4;
                priority = 8;
                //RobotAgent2.newPriority = 5;
                //RobotAgent3.newPriority = 5;
                //Operator2.newPriority = 5;
                //Operator3.newPriority = 5;
                workStatus_operator1 = false;
            }
        }
        if (priority == 5)//If Target 2 has not been hit
        {
            if (collision.collider.CompareTag("Target5"))//Target 2 has tag of Target2
            {
                AddReward(2.5f);
                UnityEngine.Debug.Log("Operator 1 - Target 5 Hit");
                if (workStatus_operator1 == true)
                {
                    nAgent.ResetPath();
                }
                reach = 5;
                priority = 8;
                //RobotAgent2.newPriority = 6;
                //RobotAgent3.newPriority = 6;
                //Operator2.newPriority = 6;
                //Operator3.newPriority = 6;
                workStatus_operator1 = false;
            }
        }
        if (priority == 6)//If Target 2 has not been hit
        {
            if (collision.collider.CompareTag("Target6"))//Target 2 has tag of Target2
            {
                AddReward(3.0f);
                UnityEngine.Debug.Log("Operator 1 - Target 6 Hit");
                if (workStatus_operator1 == true)
                {
                    nAgent.ResetPath();
                }
                reach = 6;
                priority = 8;
                //RobotAgent2.newPriority = 7;
                //RobotAgent3.newPriority = 7;
                //Operator2.newPriority = 7;
                //Operator3.newPriority = 7;
                workStatus_operator1 = false;
            }
        }
        if (priority == 7)//If Target 2 has not been hit
        {
            if (collision.collider.CompareTag("Target7"))//Target 2 has tag of Target2
            {
                AddReward(3.5f);
                UnityEngine.Debug.Log("Operator 1 - Target 7 Hit");
                if (workStatus_operator1 == true)
                {
                    nAgent.ResetPath();
                }
                agent_Drone.EndEpisode();//End episode of Drone Agent
                RobotAgent2.EndEpisode();
                EndEpisode();
            }
        }
    }
    


    public override void Heuristic(float[] actionsOut)//Control Heuristically with arrows
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }


    public void RayCastUpdate(out Vector3 return1, out Vector3 return2, out Vector3 return3, out Vector3 return4, out Vector3 return5, out Vector3 return6, out Vector3 return7, out Vector3 return8)
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
                if (hit.collider.tag == "Agent2")//EndGame: Agent 3
                {
                    agent_agentLoc2 = hit.point;//hit.point stores location where ray was hit.
                }
                if (hit.collider.tag == "Target1")//firstTarget: Target 1
                {
                    agent_targLoc1 = hit.point;
                }
                if (hit.collider.tag == "Target2")//secondTarget: Target 2
                {
                    agent_targLoc2 = hit.point;
                }
                if (hit.collider.tag == "Target3")//secondTarget: Target 3
                {
                    agent_targLoc3 = hit.point;
                }
                if (hit.collider.tag == "Target4")//secondTarget: Target 4
                {
                    agent_targLoc4 = hit.point;
                }
                if (hit.collider.tag == "Target5")//secondTarget: Target 5
                {
                    agent_targLoc5 = hit.point;
                }
                if (hit.collider.tag == "Target6")//secondTarget: Target 6
                {
                    agent_targLoc6 = hit.point;
                }
                if (hit.collider.tag == "Target7")//secondTarget: Target 7
                {
                    agent_targLoc7 = hit.point;
                }
            }
        }
        return1 = agent_agentLoc2;
        return2 = agent_targLoc1;
        return3 = agent_targLoc2;
        return4 = agent_targLoc3;
        return5 = agent_targLoc4;
        return6 = agent_targLoc5;
        return7 = agent_targLoc6;
        return8 = agent_targLoc7;
    }


    private Vector3[] SphereDirections(int numDirections)
    //I have used solution provided by titan68
    //This method allowes to create directions in all directions
    //Have to modify this method soon since Human agent will have limited field of view
    //credit: titan68
    //https://answers.unity.com/questions/43812/how-to-get-raycasts-in-all-directionsvector3.html
    {
        var pts = new Vector3[numDirections];
        foreach (var i in Enumerable.Range(0, numDirections))
        {
            float angle = i * Mathf.PI * 2f / numDirections;
            var x = (float)(Math.Cos(angle));
            var z = (float)(Math.Sin(angle));
            pts[i] = new Vector3(x, transform.position.y, z);
        }
        return pts;
        //var pts = new Vector3[numDirections];
        //var il = Math.PI * (3 - Math.Sqrt(5));
        //var idd = 2f / numDirections;

        //foreach (var i in Enumerable.Range(0, numDirections))
        //{
        //    var yi = i * idd - 1 + (idd / 2);
        //    var r = Math.Sqrt(1 - yi * yi);
        //    var phi = i * il;
        //    var x = (float)(Math.Cos(phi) * r);
        //    var z = (float)(Math.Sin(phi) * r);
        //    pts[i] = new Vector3(x, transform.position.y, z);
        //}

        //return pts;
    }
}
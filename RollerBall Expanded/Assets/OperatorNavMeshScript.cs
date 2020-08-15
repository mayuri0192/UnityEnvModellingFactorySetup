using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OperatorNavMeshScript : MonoBehaviour
{
    public RollerAgentScript roller;
    public RollerAgentScript roller2;
    public Transform Target1;
    public Transform Target2;
    public Transform Target3;
    public Transform Target4;

    //Called on start of episode. Reset Operator and Set Destination to Target 1.
    public void Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (this.name == "Operator 1")
        {
            agent.Warp(new Vector3(UnityEngine.Random.Range(-13f, 13f), 0.5f, 6.75f));
        }
        else
        {
            agent.Warp(new Vector3(UnityEngine.Random.Range(-13f, 13f), 0.5f, -6.75f));
        }

        agent.destination = Target1.position;
    }

    //Loop Operator Destinations and Trigger Agent Collision
    void OnTriggerEnter(Collider collision)
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (collision.gameObject.name == "Target 1")
        {
            agent.destination = Target2.position;
        }

        if (collision.gameObject.name == "Target 2")
        {
            agent.destination = Target3.position;
        }

        if (collision.gameObject.name == "Target 3")
        {
            agent.destination = Target4.position;
        }

        if (collision.gameObject.name == "Target 4")
        {
            agent.destination = Target1.position;
        }

        if (collision.gameObject.name == "Agent 1")
        {
            roller.OpCollision();
        }

        if (collision.gameObject.name == "Agent 2")
        {
            roller2.OpCollision();
        }
    }
}

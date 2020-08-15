using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using UnityEngine;

//Use rollerball_config_2.yaml for training

public class Game_Master : MonoBehaviour
{
    //Instantiate Agents, Targets and Operators
    public RollerAgentScript roller;
    public RollerAgentScript roller2;
    public Transform Target1;
    public Transform Target2;
    public Transform Target3;
    public Transform Target4;
    public OperatorNavMeshScript Operator1;
    public OperatorNavMeshScript Operator2;
    
    //Flags for Targets
    public int[] Flags = {0, 0, 0, 0};
 
    //Color for Targets
    public Color Red = new Color(1f, 0f, 0f, 1f);
    public Color Green = new Color(0f, 1f, 0f, 1f);

    //Called on Episode Beginning
    public void Start()
    {
        //Initialize Flags Randomly
        Flags[0] = (int)UnityEngine.Random.Range(0.01f, 1.99f);
        Flags[1] = (int)UnityEngine.Random.Range(0.01f, 1.99f);
        Flags[2] = (int)UnityEngine.Random.Range(0.01f, 1.99f);
        Flags[3] = (int)UnityEngine.Random.Range(0.01f, 1.99f);
        
        //Assign Color to Targets
        if (Flags[0] == 0)
        {
            Target1.GetComponent<MeshRenderer>().material.color = Green;
            Target1.tag = "Machine A";
        }
        else
        {
            Target1.GetComponent<MeshRenderer>().material.color = Red;
            Target1.tag = "Null";
        }

        if (Flags[1] == 0)
        {
            Target2.GetComponent<MeshRenderer>().material.color = Green;
            Target2.tag = "Machine B";
        }
        else
        {
            Target2.GetComponent<MeshRenderer>().material.color = Red;
            Target2.tag = "Null";
        }

        if (Flags[2] == 0)
        {
            Target3.GetComponent<MeshRenderer>().material.color = Green;
            Target3.tag = "Machine C";
        }
        else
        {
            Target3.GetComponent<MeshRenderer>().material.color = Red;
            Target3.tag = "Null";
        }

        if (Flags[3] == 0)
        {
            Target4.GetComponent<MeshRenderer>().material.color = Green;
            Target4.tag = "Machine D";
        }
        else
        {
            Target4.GetComponent<MeshRenderer>().material.color = Red;
            Target4.tag = "Null";
        }

        //Initialize Operators
        Operator1.Start();
        Operator2.Start();
    }
    
    //Flag Checker
    public void Check()
    {
        if (Flags[0] == 1 && Flags[1] == 1 && Flags[2] == 1 && Flags[3] == 1) //If all Targets are met,
        {
            EndAll();//End Episodes
        }
    }

    //End Episodes for All Agents
    public void EndAll()
    {
        roller.End();
        roller2.End();
    }
}
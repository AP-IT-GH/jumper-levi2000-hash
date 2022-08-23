using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

public class JumperAgent : Agent
{
    public Transform obstacle_position;
    public Obstacle obstacle;
    private float jumpPower = 4f;
    private Rigidbody rb;

    public override void Initialize()
    {
        transform.localPosition = new Vector3(-8.02f, 6.36f, -3.88f);
    }

    public override void OnEpisodeBegin()
    {
        rb = GetComponent<Rigidbody>();
        obstacle.SetObstacle();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(obstacle.speed);
        sensor.AddObservation(obstacle_position.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 controlSignal = Vector3.zero;

        controlSignal.y = actions.ContinuousActions[0];

        rb.AddForce(controlSignal * jumpPower, ForceMode.Impulse);
        
        if(transform.localPosition.y < 0f || transform.position.y < 0f)
        {
            SetReward(-1);
            EndEpisode();
        }

        if (obstacle.transform.localPosition.z >= 13f && obstacle.CompareTag("bad"))
        {
            SetReward(1f);
            EndEpisode();
        }
        if (obstacle.transform.localPosition.z >= 13f && obstacle.CompareTag("good"))
        {
            SetReward(-1f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continousActions = actionsOut.ContinuousActions;
        if (Input.GetKey(KeyCode.Space))
        {
            continousActions[0] = 1;
            Vector3 move = new Vector3(0, continousActions[0], 0);
            rb.AddForce(move * jumpPower, ForceMode.Force);
        }
    }

    private void OnTriggerEnter(Collider other)
    { 
        if( other.gameObject.CompareTag("good"))
        {
           SetReward(1f);
           EndEpisode();
        }

       if (other.gameObject.CompareTag("bad"))
       {
           SetReward(-1);
           EndEpisode();
       }
    }


    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            jumpPower = 0f;
        }
    }

    private void OnCollisionStay(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Ground"))
        {
            jumpPower = 4f;
        }
    }
}

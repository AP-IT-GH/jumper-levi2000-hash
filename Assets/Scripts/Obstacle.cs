using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Obstacle : MonoBehaviour
{
    public int speed;
    private ObstacleType type;
    
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * Time.deltaTime * speed);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            this.SetObstacle();
        }
    }

    public void SetObstacle()
    {
        transform.localPosition = new Vector3(11.05f, 6.37f, -3.22f); 
        speed = Random.Range(5, 15);
        int choice = Random.Range(1, 100);
        if (choice % 2 == 0)
            type = ObstacleType.GOOD;
        else if(choice % 2 != 0)
        {
            type = ObstacleType.BAD;
        }

        var obstacleRenderer = this.GetComponent<Renderer>();
        if (type == ObstacleType.BAD)
        {
            obstacleRenderer.material.SetColor("_Color", Color.red);
            this.tag = "bad";
        }

        else if (type == ObstacleType.GOOD)
        {
            obstacleRenderer.material.SetColor("_Color", Color.green);
            this.transform.localPosition += new Vector3(0, 4f, 0);
            this.tag = "good";
        }
    }
}

public enum ObstacleType
{
    GOOD,
    BAD
}

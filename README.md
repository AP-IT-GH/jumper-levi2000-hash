# Jumper Assignment
## Introduction
I created and ai that jumps over obstacles.
There are 2 types of obstacles, the good one(green) and the bad one(red).
The agent needs to avoid the red one and hit the green one.
I also gave the jumper agent some rays (eyes) to give extra help.

If you take a look at the configuration file you can see at the bottom that I trained the agent for 1000000 steps. 
The agent still hits the bad obstacle sometimes.
This happens because the agent first does a small jump and then attempts the big jump and hits the obstacle or because the agent is still in the air and the second obstacle is already on its way moving at a higher speed.
## JumperAgent

The variables of the agent, htey speak for themselves
```c#
    public Transform obstacle_position;
    public Obstacle obstacle;
    private float jumpPower = 4f;
    private Rigidbody rb;
```

During initialization I make sure the agent stands on the right place. 
Then when the episode begins I take the rigidbody of the agent (for jumping) and spawn the obstacle.
```c#
public override void Initialize()
{
	transform.localPosition = new Vector3(0, 0.7f, 6f);
}

public override void OnEpisodeBegin()
{
	rb = GetComponent<Rigidbody>();
	obstacle.SetObstacle();
}
```

I gave the agent 2 observations, the speed of the obstacle and the position. This helped a lot for the agent to learn.
```c#
public override void CollectObservations(VectorSensor sensor)
{
	sensor.AddObservation(obstacle.speed);
	sensor.AddObservation(obstacle_position.localPosition);
}
```

In the OnActionReceived method I handled most of my rewards.
I made sure that if the agent goes below the platform it automatically ends.
I also check if the obstacle passed the agent and hit the wall and what type of obstacle it was. If it was a good one it loses points, and if it was bad it gains points.
I chose to use tags as identifiers for this since it was very easy to use in Unity.
```c#
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
```

Here I handled the collision with the obstacles.

```c#
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
```

## Obstacle
The variables of the obstacle object
```c#
public int speed;
private ObstacleType type;
public GameObject wall;
```

This makes the obstacle move towards the agent at a certain speed that is randomly generated in the setObstacle method
```c# 
void Update()
{
	transform.Translate(Vector3.right * Time.deltaTime * speed);
}
```

Here I check if the obstacle hits the wall on the end. If it does I reset the obstacle.
```c#
private void OnTriggerEnter(Collider other)
{
	if (other.gameObject.CompareTag("Wall"))
	{
		SetObstacle();
	}
}
```

This is the method that sets the obstacle up.
First I make sure that the obstacle is set on its starting position relative to the environment.
Then I generate a random speed value.
I also generate a random choice value that helps deciding what type of obstacle it will be.
If it is a whole number it will be a good obstacle.
If it is a bad one it will be a bad obstacle.
Then I get the Renderer component of my obstacle to change its color this is based of of the type it got.
When this happens I also set the tag of the obstacle to be used in the collision detection with the agent.
```c#
 public void SetObstacle()
{
	transform.localPosition = new Vector3(0, 0.5f, -10f); 
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
		this.tag = "good";
	}
}
```

The enum ObstacleType
```c#
public enum ObstacleType
{
    GOOD,
    BAD
}
```

using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.UI;
using UnityEngine.Events;

public class TanukiAgent : Agent
{
    float giro = 0.0f;
    bool accel = false;
    bool drift = false;
    Vector3 initialPosition;
    Quaternion initialRotation;
    LongBoardControls longBoardControls;
    float tiempo = 0.0f;
	float maxAngularSpeed = 5.0f; //normalize rotation
    public UnityEvent onResetTrails;
		
    [System.Serializable]
    public class Rewards
    {
        public float fall = -1.0f;
        public float speed = 0.01f;
        public float drift = 0.002f;
        public float giro = -0.001f;
        public float step = -0.0001f;
        public float collision = -1.0f;
        public float finish = 2.0f;
    }
    public Rewards rewards = new Rewards();

    private void Update()
    {
        if (!longBoardControls.grounded) tiempo += Time.deltaTime;
        if (longBoardControls.grounded) tiempo = 0.0f;
        if (tiempo >= 0.15f)
        {
            SetReward(rewards.fall);
            EndEpisode();
            tiempo = 0.0f;
        }
    }

    private float n(float current, float min, float max)
    {
        float value = (current - min) / (max - min);
        return value;
    }
		
    public override void Initialize()
    {  //Start method for agent
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        longBoardControls = GetComponent<LongBoardControls>();
    }
    public override void OnEpisodeBegin()
    {  // Actions to do at the start of training episode
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        onResetTrails.Invoke();

        Physics.SyncTransforms(); // reset phyisics and lowering longboard height
        longBoardControls.EnableGameplay();
    }
		
    public override void CollectObservations(VectorSensor sensor) //using the unity sensor defaults (ray and body)
    {
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {   //Does the recieved actions
        giro = actionBuffers.ContinuousActions[0];
        accel = actionBuffers.DiscreteActions[0] == 1;      
        drift = actionBuffers.DiscreteActions[1] == 1;

        float normalizedSpeed = longBoardControls.speed0to1;
        bool goingForward = longBoardControls.dirDot > 0;

        float angularVelocityY = Mathf.Abs(longBoardControls.rb.angularVelocity.y);
        float normalizedAngular = Mathf.Clamp01(angularVelocityY / maxAngularSpeed);

        if (longBoardControls.grounded)
        {
            if (goingForward && normalizedSpeed > 0.05f)
            {
                AddReward(normalizedSpeed * rewards.speed);
            }
            if (longBoardControls.drifting && normalizedSpeed > 0.3f && normalizedAngular > 0.1f)
            {
                AddReward(normalizedAngular * rewards.drift);
            }
        }
        if (Mathf.Abs(giro) > 0.0f)
        {
            AddReward(rewards.giro * Mathf.Abs(giro));
        }

        AddReward(rewards.step);
        longBoardControls.SendActions(giro, accel, drift);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") || (collision.gameObject.CompareTag("Car")) || (collision.gameObject.CompareTag("Cone")))
        {
            SetReward(rewards.collision);
            EndEpisode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("TT_FinishTrigger"))
        {
            onResetTrails.Invoke();
            SetReward(rewards.finish);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    { // Manual control during testing
        var continuousActionsOut = actionsOut.ContinuousActions;
        var discreteActionsOut = actionsOut.DiscreteActions;

        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        discreteActionsOut[0] = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) ? 1 : 0;
        discreteActionsOut[1] = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? 1 : 0;

    }
}


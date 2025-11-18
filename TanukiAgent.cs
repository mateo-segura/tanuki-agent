using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.UI;

public class TanukiAgent : Agent
{
    //public Transform sensores;
    float giro = 0.0f;
    bool accel = false;
    bool drift = false;
    Vector3 initialPosition;
    Quaternion initialRotation;
    public Image accelImage;
    LongBoardControls longBoardControls;
    float tiempo = 0.0f;


    private void Update()
    {
        if (!longBoardControls.grounded) tiempo += Time.deltaTime;
        if (longBoardControls.grounded) tiempo = 0.0f;
        if (tiempo >= 2.0f)
        {
            SetReward(-1.0f);
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
    {  // Es como el método Start del agente
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        longBoardControls = GetComponent<LongBoardControls>();
    }
    public override void OnEpisodeBegin()
    {  // Acciones a realizar al inicio de un episodio de entrenamiento
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        Physics.SyncTransforms(); //reseteando las fisicas y bajando el longboard
        longBoardControls.EnableGameplay();
    }
    public override void CollectObservations(VectorSensor sensor)
    {  // Recolecta observaciones y arma el vector
        /*
        for (int k = 0; k < sensores.childCount; k++) { 
            Transform sensorK = sensores.GetChild(k);
            RaycastHit hit;
            if (Physics.Raycast(sensores.transform.position,
                                sensorK.forward,
                                out hit,
                                100.0f))
            {
                sensor.AddObservation(n(hit.distance, 0.0f, 100.0f));
                Debug.DrawLine(sensores.transform.position,
                    hit.point, Color.yellow, 0.5f);
            }
            else {
                sensor.AddObservation(1.0f);
            }
            
        }
        */
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {   //Ejecuta las acciones recibidas

        // Acción continua 
        giro = actionBuffers.ContinuousActions[0];

        // Acciones discretas
        accel = actionBuffers.DiscreteActions[0] == 1;        // Rama 0: accel (0 = no accel, 1 = accel)
        drift = actionBuffers.DiscreteActions[1] == 1;        // Rama 1: drift (0 = no drift, 1 = drift)

        // Recomensa por ir hacia adelante (para que no se quede girando como trompo)
        float forwardVelocity = Vector3.Dot(longBoardControls.rb.linearVelocity, transform.forward);
        if (forwardVelocity > 1.0f)
        {
            AddReward(forwardVelocity * 0.001f);
        }

        // Recompensa por drift+giro
        float giroAbs = Mathf.Abs(giro); //quitar signo negativo
        float angularVelocityY = Mathf.Abs(longBoardControls.rb.angularVelocity.y); //Velocidad angular real (qué tan rápido gira)

        if (drift && giroAbs > 0.3f && angularVelocityY > 0.8f)
        {
            AddReward(angularVelocityY * 0.1f);
        }

        AddReward(-0.0001f);


        if (accelImage != null)
        {
            accelImage.fillAmount = n(giro, -1.0f, 1.0f);
        }

        // Enviar las acciones al controlador
        longBoardControls.SendActions(giro, accel, drift);
    }

    private void OnCollisionEnter(Collision collision)
    {
        /*

        if (collision.gameObject.CompareTag("Car")||)
        {
            SetReward(-0.5f);
        }
        */
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    { // Para control manual durante pruebas
        var continuousActionsOut = actionsOut.ContinuousActions;
        var discreteActionsOut = actionsOut.DiscreteActions;

        continuousActionsOut[0] = Input.GetAxis("Horizontal");        // Giro con teclado
        discreteActionsOut[0] = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) ? 1 : 0;         // Accel con tecla W o flecha arriba
        discreteActionsOut[1] = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? 1 : 0;        // Drift con tecla Shift o botón del gamepad

    }
}

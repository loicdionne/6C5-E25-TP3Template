using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class EscapeAgent : Agent
{
    [SerializeField] private Transform switchTransform;
    [SerializeField] private Transform doorTransform;
    [SerializeField] private Transform exitZone;
    [SerializeField] private Material doorOpenMaterial;
    [SerializeField] private Material doorClosedMaterial;
    [SerializeField] private Renderer doorRenderer;
    [SerializeField] private float moveSpeed = 5f;

    private bool doorIsOpen = false;
    private Vector3 agentStartPos;

    public override void Initialize()
    {
        agentStartPos = transform.localPosition;
    }

    public override void OnEpisodeBegin()
    {
        // Remet seulement l'agent à sa position de départ (fixe)
        transform.localPosition = agentStartPos;

        // Réinitialise l’état de la porte
        doorIsOpen = false;
        doorRenderer.material = doorClosedMaterial;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observations des positions (fixes mais nécessaires pour apprendre)
        sensor.AddObservation(transform.localPosition);         // Agent
        sensor.AddObservation(switchTransform.localPosition);   // Interrupteur
        sensor.AddObservation(doorTransform.localPosition);     // Porte
        sensor.AddObservation(exitZone.localPosition);          // Sortie
        sensor.AddObservation(doorIsOpen ? 1f : 0f);            // État porte
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        Vector3 move = new Vector3(moveX, 0, moveZ);
        transform.Translate(move * moveSpeed * Time.deltaTime);

        // Punition légère par step
        AddReward(-0.001f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Switch"))
        {
            ToggleDoor();
            AddReward(0.5f); // Récompense pour avoir trouvé l'interrupteur
        }
        else if (other.CompareTag("Exit"))
        {
            if (doorIsOpen)
            {
                AddReward(1f); // Réussite
            }
            else
            {
                AddReward(-1f); // Sortie interdite
            }
            EndEpisode();
        }
        else if (other.CompareTag("Wall"))
        {
            AddReward(-0.5f);
            EndEpisode();
        }
    }

    private void ToggleDoor()
    {
        doorIsOpen = !doorIsOpen;
        doorRenderer.material = doorIsOpen ? doorOpenMaterial : doorClosedMaterial;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var contActions = actionsOut.ContinuousActions;
        contActions[0] = Input.GetAxisRaw("Horizontal");
        contActions[1] = Input.GetAxisRaw("Vertical");
    }
}

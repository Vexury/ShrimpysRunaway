using TMPro;
using UnityEngine;

public class ControllerSwapper : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private TopDownController topDownController;
    [SerializeField] private VehicleController vehicleController;
    [SerializeField] private GameObject topDownGeometry;
    [SerializeField] private GameObject carGeometry;
    [SerializeField] private TMP_Text controllerLabel;

    [Header("Labels")]
    [SerializeField] private string topDownLabel = "Top Down";
    [SerializeField] private string vehicleLabel = "Vehicle";

    [Header("TopDown Capsule")]
    [SerializeField] private float topDownHeight = 2f;
    [SerializeField] private float topDownRadius = 0.4f;
    [SerializeField] private Vector3 topDownCenter = new Vector3(0f, 1f, 0f);

    [Header("Vehicle Capsule")]
    [SerializeField] private float vehicleHeight = 1.2f;
    [SerializeField] private float vehicleRadius = 1f;
    [SerializeField] private Vector3 vehicleCenter = new Vector3(0f, 0.6f, 0f);

    private bool isTopDown = true;

    private void Start()
    {
        Apply();
    }

    private void OnEnable()
    {
        inputReader.NextEvent += Cycle;
    }

    private void OnDisable()
    {
        inputReader.NextEvent -= Cycle;
    }

    private void Cycle()
    {
        isTopDown = !isTopDown;
        Apply();
    }

    private void Apply()
    {
        topDownController.enabled = isTopDown;
        vehicleController.enabled = !isTopDown;
        topDownGeometry.SetActive(isTopDown);
        carGeometry.SetActive(!isTopDown);

        if (isTopDown)
        {
            characterController.height = topDownHeight;
            characterController.radius = topDownRadius;
            characterController.center = topDownCenter;
        }
        else
        {
            characterController.height = vehicleHeight;
            characterController.radius = vehicleRadius;
            characterController.center = vehicleCenter;
        }

        if (controllerLabel != null)
            controllerLabel.text = isTopDown ? topDownLabel : vehicleLabel;
    }
}

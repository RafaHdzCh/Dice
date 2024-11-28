using UnityEngine;

public class Dice : MonoBehaviour
{
    private Rigidbody diceRigidbody;
    private bool isDragging = false;
    private Camera mainCamera;
    private Vector3 mouseDragOffset;
    private Vector3 initialMousePosition;
    private Vector3 lastMousePosition;

    public float shakeForce = 500f; 
    public float launchForce = 15f;

    [SerializeField] private GameObject platform;

    void Start()
    {
        diceRigidbody = GetComponent<Rigidbody>();
        mainCamera = Camera.main;

        if (diceRigidbody == null)
        {
            Debug.LogError("Rigidbody missing.");
        }

        if (mainCamera == null)
        {
            Debug.LogError("Camera missing.");
        }
    }

    void FixedUpdate()
    {
        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                isDragging = true;
                initialMousePosition = Input.mousePosition;
                lastMousePosition = Input.mousePosition;

                mouseDragOffset = hit.point - transform.position;

                diceRigidbody.isKinematic = true;
                platform.SetActive(false);
            }
        }

        if (isDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                transform.position = hit.point - mouseDragOffset;
                Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
                lastMousePosition = Input.mousePosition;
                Vector3 rotationDelta = new Vector3(-mouseDelta.y, mouseDelta.x, 0);
                transform.Rotate(rotationDelta * (shakeForce * Time.fixedDeltaTime * 0.1f), Space.World);
            }
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;

            diceRigidbody.isKinematic = false;

            Vector3 mouseDelta = Input.mousePosition - initialMousePosition;
            Vector3 launchDirection = new Vector3(mouseDelta.x, Mathf.Abs(mouseDelta.y), mouseDelta.y).normalized;

            diceRigidbody.AddForce(launchDirection * launchForce, ForceMode.Impulse);
            diceRigidbody.AddTorque(Random.insideUnitSphere * launchForce, ForceMode.Impulse);
        }
    }
}

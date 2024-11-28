using UnityEngine;

public class Dice : MonoBehaviour
{
    private Rigidbody diceRigidbody;
    private bool isDragging;
    private Camera mainCamera;
    private Vector3 mouseDragOffset;
    private Vector3 initialMousePosition;
    private float shakeForce = 800f; 
    private float launchForce = 50f;
    private bool isThrown;
    private bool isStopped;
    private float stopCheckDelay = 0.5f;
    private float throwTime;
    private Vector3 randomRotationAxis;
    private float randomRotationSpeed;

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
        CheckIfStopped();
    }

    private void HandleMouseInput()
    {
        if (isThrown || isStopped) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                isDragging = true;
                initialMousePosition = Input.mousePosition;

                mouseDragOffset = hit.point - transform.position;

                diceRigidbody.isKinematic = true;
                platform.SetActive(false);

                randomRotationAxis = Random.onUnitSphere; 
                randomRotationSpeed = Random.Range(60f, 120f);
            }
        }

        if (isDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                transform.position = hit.point - mouseDragOffset;
                transform.Rotate(randomRotationAxis, randomRotationSpeed * Time.fixedDeltaTime, Space.World);
            }
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            isThrown = true;
            throwTime = Time.time;

            diceRigidbody.isKinematic = false;

            Vector3 mouseDelta = Input.mousePosition - initialMousePosition;
            Vector3 launchDirection = new Vector3(mouseDelta.x, Mathf.Abs(mouseDelta.y), mouseDelta.y).normalized;

            diceRigidbody.AddForce(launchDirection * launchForce, ForceMode.Impulse);
            diceRigidbody.AddTorque(Random.insideUnitSphere * (launchForce * 6), ForceMode.Impulse);
        }
    }

    private void CheckIfStopped()
    {
        if (!isThrown || isStopped) return;
        if (Time.time - throwTime < stopCheckDelay) return;

        if (diceRigidbody.linearVelocity.magnitude < 0.1f && diceRigidbody.angularVelocity.magnitude < 0.1f)
        {
            isStopped = true;
            diceRigidbody.linearVelocity = Vector3.zero;
            diceRigidbody.angularVelocity = Vector3.zero;

            Debug.Log("El dado se ha detenido.");
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    private Rigidbody diceRigidbody;
    private bool isDragging;
    private Camera mainCamera;
    private Vector3 mouseDragOffset;
    private Vector3 initialMousePosition;
    private float shakeForce = 20000f; 
    private float launchForce = 1000f;
    private bool isThrown;
    private bool isStopped;
    private float stopCheckDelay = 0.5f;
    private float throwTime;
    private Vector3 randomRotationAxis;
    private float randomRotationSpeed;
    private AudioSource audioSource;
    [SerializeField] private GameObject resetButton;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        diceRigidbody = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        diceRigidbody.useGravity = false;

        if (diceRigidbody == null)
        {
            Debug.LogError("Rigidbody missing.");
        }

        if (mainCamera == null)
        {
            Debug.LogError("Camera missing.");
        }
    }
    
    private void OnCollisionEnter()
    {
        audioSource.Play();
    }

    void FixedUpdate()
    {
        HandleMouseInput();
        CheckIfStopped();
        if (diceRigidbody.useGravity && !isStopped)
        {
            diceRigidbody.AddForce(Vector3.down * 98.1f, ForceMode.Acceleration);
        }
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

                diceRigidbody.useGravity = true;
                initialMousePosition = Input.mousePosition;

                mouseDragOffset = hit.point - transform.position;

                diceRigidbody.isKinematic = true;

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
            throwTime = Time.fixedTime;

            diceRigidbody.isKinematic = false;

            Vector3 mouseDelta = Input.mousePosition - initialMousePosition;
            Vector3 launchDirection = new Vector3(mouseDelta.x, Mathf.Abs(mouseDelta.y), mouseDelta.y).normalized;

            diceRigidbody.AddForce(launchDirection * launchForce, ForceMode.Impulse);
            diceRigidbody.AddTorque(Random.insideUnitSphere * (launchForce * 6000), ForceMode.Impulse);
        }
    }

    private void CheckIfStopped()
    {
        if (!isThrown || isStopped) return;
        if (Time.fixedTime - throwTime < stopCheckDelay) return;

        if (Mathf.Abs(diceRigidbody.linearVelocity.x) < 0.2f &&
            Mathf.Abs(diceRigidbody.linearVelocity.z) < 0.2f &&
            diceRigidbody.angularVelocity.magnitude < 10f)
        {
            isStopped = true;
            diceRigidbody.linearVelocity = Vector3.zero;
            diceRigidbody.angularVelocity = Vector3.zero;
            diceRigidbody.useGravity = false;
            diceRigidbody.isKinematic = true;
            
            resetButton.gameObject.SetActive(true);
        }
    }

}

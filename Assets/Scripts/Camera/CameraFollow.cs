using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerTransform;
    public float followSpeed = 2f;
    // public float distanceFromPlayer = 2f;
    public float screenEdgeBuffer = 0.1f;

    private Camera mainCamera;
    public float minX, maxX, minY, maxY;
    public float pixelToUnits = 40f;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();

        // Calculate the min and max positions of the camera based on the screen size
        var screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        minX = screenBounds.x * -1f + screenEdgeBuffer;
        maxX = screenBounds.x - screenEdgeBuffer;
        minY = screenBounds.y * -1f + screenEdgeBuffer;
        maxY = screenBounds.y - screenEdgeBuffer;
    }

    private void FixedUpdate()
    {
        // Calculate the target position for the camera based on the player's position
        var targetPosition = playerTransform.position;
        targetPosition.z = RoundToNearestPixel(transform.position.z);
        targetPosition.x = Mathf.Clamp(RoundToNearestPixel(targetPosition.x), minX, maxX);
        targetPosition.y = Mathf.Clamp(RoundToNearestPixel(targetPosition.y), minY, maxY);

        // Smoothly move the camera towards the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
    }
 
    private float RoundToNearestPixel(float unityUnits)
    {
            float valueInPixels = unityUnits * pixelToUnits;
            valueInPixels = Mathf.Round(valueInPixels);
            float roundedUnityUnits = valueInPixels * (1 / pixelToUnits);
            return roundedUnityUnits;
    }
}

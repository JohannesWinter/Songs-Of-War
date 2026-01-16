using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public GameObject cameraObject;
    public PlayerController playerController;
    public float cameraSpeed; // (0,inf)

    public Vector2 currentCameraPosition { get; private set; } //local position
    public Vector2 targetCameraPosition { get; private set; } //local position

    void Update()
    {

        UpdateCurrentCameraPosition();
        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        //approch target position from current positon
        Vector2 direction = targetCameraPosition - currentCameraPosition;
        float distance = direction.magnitude;

        Vector2 toTravel = (Vector3)direction * cameraSpeed * Time.deltaTime;

        //end travel if calculated distance > actual distance
        if (toTravel.magnitude < distance)
        {
            cameraObject.transform.Translate(toTravel, Space.Self);
        }
        else
        {
            cameraObject.transform.localPosition = targetCameraPosition;
        }
    }
    void UpdateCurrentCameraPosition()
    {
        currentCameraPosition = cameraObject.transform.localPosition;
    }
}

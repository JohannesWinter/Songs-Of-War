using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public GameObject cameraObject;
    public PlayerController playerController;
    public float cameraSpeed; // (0,inf)
    public float cameraOffsetUp, cameraOffsetDown, cameraOffsetRight, cameraOffsetLeft;

    public Vector2 standardCameraOffset;
    public Vector2 currentCameraPosition { get; private set; } //local position
    public Vector2 targetCameraPosition { get; private set; } //local position

    void Update()
    {
        UpdateCurrentCameraPosition();
        UpdateTargetCameraPosition();
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
    void UpdateTargetCameraPosition()
    {
        //updates targetCameraPosition based on movement
        Vector2 newTargetPosition = standardCameraOffset;

        //horizontal
        if ((playerController.holding || playerController.slipping) == false)
        {
            //not holding on wall
            switch (playerController.playerMovementDirection)
            {
                case PlayerMovementDirection.Right:
                    newTargetPosition += Vector2.right * cameraOffsetRight;
                    break;
                case PlayerMovementDirection.Left:
                    newTargetPosition += Vector2.left * cameraOffsetLeft;
                    break;
            }
        }
        else
        {
            //holding on wall -> inverted
            switch (playerController.playerMovementDirection)
            {
                case PlayerMovementDirection.Right:
                    newTargetPosition += Vector2.left * cameraOffsetRight;
                    break;
                case PlayerMovementDirection.Left:
                    newTargetPosition += Vector2.right * cameraOffsetLeft;
                    break;
            }
        }

        //vertical
        if ((Manager.m.playerManager.up.hold && Manager.m.playerManager.down.hold) == false)
        {
            //does not change if up and down are pressed together
            if (Manager.m.playerManager.up.hold)
            {
                newTargetPosition += Vector2.up * cameraOffsetUp * 0.5f;
            }
            if (Manager.m.playerManager.down.hold)
            {
                newTargetPosition += Vector2.down * cameraOffsetDown * 0.5f;
            }
        }
        if (playerController.velocity.y > 0)
        {
            newTargetPosition += Vector2.up * Mathf.Min(cameraOffsetUp * playerController.velocity.y / 10 * 0.5f, cameraOffsetUp * 0.5f);
        }
        else if (playerController.velocity.y > 0)
        {
            newTargetPosition += Vector2.down * Mathf.Min(cameraOffsetDown * playerController.velocity.y / 10 * 0.5f, cameraOffsetDown * 0.5f);
        }

        //set calculated target position
        targetCameraPosition = newTargetPosition;
    }
    
}

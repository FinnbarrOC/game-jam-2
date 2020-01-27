﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 10f;
    public float turnSpeed = 20f;
    public float canMoveRotationThreshold = 0.1f;
    public float consideredMovementThreshold = 0.1f;

    private Rigidbody rb;
    private PlayerManager playerManager;

    private Vector3 movement;

    void Start()
    {
        rb = Utils.GetRequiredComponent<Rigidbody>(this);
        playerManager = Utils.GetRequiredComponent<PlayerManager>(this);
    }

    void FixedUpdate()
    {
        Vector3 relativeForward = CleanForwardVector(CameraManager.Instance.GetActiveCameraTransform().forward);
        Vector3 relativeRight = CalculateRightVector(relativeForward);

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        movement = relativeForward * vertical + relativeRight * horizontal;
        movement.Normalize();
        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;    // to be used later for animations and such

        HandleControl(movement);
    }

    private Vector3 CleanForwardVector(Vector3 forwardVector)
    {
        forwardVector.y = 0f;
        return forwardVector.normalized;
    }

    private Vector3 CalculateRightVector(Vector3 forwardVector)
    {
        // The vector perpendicular to the forward vector and this transform's up, must be the relative right vector
        return -Vector3.Cross(forwardVector, transform.up).normalized;
    }

    void HandleControl(Vector3 movementDirection)
    {
        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, movementDirection, turnSpeed * Time.fixedDeltaTime, 0f);

        HandleRotation(desiredForward);

        // Only start moving once we are close enough to our desired final rotation (which we are smoothly rotating towards)
        if ((movementDirection - desiredForward).magnitude < canMoveRotationThreshold)
        {
            HandleMovement();
        }
    }

    void HandleRotation(Vector3 desiredForward)
    {
        rb.MoveRotation(Quaternion.LookRotation(desiredForward));
    }

    void HandleMovement()
    {
        rb.MovePosition(rb.position + movement * movementSpeed * Time.fixedDeltaTime);

        // We only want to decrease stamina if the input is over a certain threshold (gets buggy otherwise)
        // TODO: Change this to sqrMagnitude for efficency increase, but this is easier to conceptualize for now
        if (movement.magnitude > consideredMovementThreshold)
        {
            playerManager.HandleDecreaseStamina();
        }
    }
}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DoorBehaviour : MonoBehaviour
{
    public float closedDelta = 5f;
    public bool shakeCameraOnClose = true;
    public bool startLocked = false;
    public Conversation lockedConversation;

    private float closedYRotation;
    private bool isClosed = true;
    private bool isLocked = false;

    private HingeJoint hinge;
    private JointLimits startHingeLimits;
    private JointLimits lockedHingeLimits;
    private CinemachineImpulseSource impulseSource;

    public event Action OnDoorOpen;
    public event Action OnDoorClose;
    public event Action OnPlayerCollideWithDoor;

    [Header("FMOD Audio")]
    [FMODUnity.EventRef]
    public string DoorSqueak;
    [FMODUnity.EventRef]
    public string DoorSlam;

    private void Awake()
    {
        hinge = Utils.GetRequiredComponent<HingeJoint>(this);
        hinge.useLimits = true;
        startHingeLimits = hinge.limits;
        lockedHingeLimits = startHingeLimits;
        lockedHingeLimits.min = 0f;
        lockedHingeLimits.max = 0f;

        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Start()
    {
        closedYRotation = transform.rotation.eulerAngles.y;

        SetLocked(startLocked);
    }

    public void SetLocked(bool locked)
    {
        hinge.limits = locked ? lockedHingeLimits : startHingeLimits;
        isLocked = locked;
    }

    public void SetMinLimit(float min)
    {
        hinge.limits = new JointLimits() { min = min, max = hinge.limits.max };
    }

    private void Update()
    {
        if (Mathf.Abs(closedYRotation - transform.rotation.eulerAngles.y) <= closedDelta)
        {
            if (!isClosed)
            {
                OnDoorClose?.Invoke();
                isClosed = true;

                if (shakeCameraOnClose && impulseSource)
                {
                    impulseSource.GenerateImpulse();
                    FMODUnity.RuntimeManager.PlayOneShot(DoorSlam, transform.position);
                }
            }
        }
        else
        {
            if (isClosed)
            {
                OnDoorOpen?.Invoke();
                FMODUnity.RuntimeManager.PlayOneShot(DoorSqueak, transform.position);
                isClosed = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(Constants.TAG_PLAYER))
        {
            if (isLocked && lockedConversation)
            {
                DialogueManager.Instance.StartConversation(lockedConversation);
            }

            OnPlayerCollideWithDoor?.Invoke();
        }
    }
}

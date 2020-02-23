﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoisePing : MonoBehaviour
{
    public GameObject pingPrefab;
    public float pingFloorOffset = 1.0f;
    public float pingDuration = 1.0f;
    public float pingGrowthScale = 0.1f;
    
    private GameObject pingInstance;
    
    public void SpawnNoisePing(Vector3 hitPoint, Vector3 hitNormal)
    {
        pingInstance = Instantiate(pingPrefab, hitPoint + (pingFloorOffset * hitNormal), 
            Quaternion.LookRotation(hitPoint));
        Destroy(pingInstance, pingDuration);
    }

    private void FixedUpdate()
    {
        if (!pingInstance) return;

        pingInstance.transform.localScale += new Vector3(pingGrowthScale, 0f, pingGrowthScale);
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekComponent : MonoBehaviour
{
    public float AlertRadius = 25.0f;
    public float VisionRadius = 15.0f;
    public float NightVisionRadius = 8.0f;

    public float LungeDistance = 8.0f;
    public float WaitTime = 2.0f;
    public Transform Target;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, AlertRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, VisionRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, NightVisionRadius);
    }
}

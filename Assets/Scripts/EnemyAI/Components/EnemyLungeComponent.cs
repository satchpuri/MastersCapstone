﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLungeComponent : MonoBehaviour
{

    public float LungeValue = 5.0f;
    //public float PrelungeValue = 5.0f;
    public float PrelungeTime = 0.25f;
    public float LungeTime = 0.25f;
    public float CurrentTimeForPrelunging = 0.0f;
    public float CurrentTimeForLunging = 0.0f;
    //public float LungingSpeed = Random.Range(20, 40);

    public bool IsLunging;
    public bool IsPrelunging;


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, LungeValue);
        //Gizmos.color = Color.grey;
        //Gizmos.DrawWireSphere(transform.position, PrelungeValue);
    }

}
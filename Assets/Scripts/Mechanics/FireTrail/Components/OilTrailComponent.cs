﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilTrailComponent : MonoBehaviour
{
    public LineRenderer LineRenderer; // Oil slick using line renderer
    public int TrailLimit = 5; // Number of oil trail points
    public int CurrentTrailCount = 0; // Current oil trail count
    public float TrailMinimumDistance = 1.0f; // Minumum distance between oil trail points
    public float TrailMaximumDistance = 10.0f; // Minumum distance between oil trail points
    public List<Vector3> TrailPoints; // List of oil trail positions
    public bool IsEquipped = false; // If oil cannister or item is equipped
    public GameObject OilTrialPrefab = null;
    private GameObject OilTrailInstance = null;

    private void Start()
    {
        OilTrailInstance = Instantiate(OilTrialPrefab);
        TrailPoints = new List<Vector3>();
        LineRenderer = OilTrailInstance.GetComponent<LineRenderer>();// GetComponentInChildren<LineRenderer>();
        LineRenderer.positionCount = 0;
    }
}
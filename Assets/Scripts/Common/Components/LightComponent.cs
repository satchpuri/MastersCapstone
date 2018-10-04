﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightComponent : MonoBehaviour
{
    public bool LightIsOn;

    public void ToggleLightOn()
    {
        var lightSource = GetComponentInChildren<Light>();
        lightSource.enabled = !lightSource.enabled;
    }

    public string GetParent()
    {
        if(transform.parent != null)
            return transform.parent.name.ToString();
        return null;
    }

}
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CheckpointManager : MonoBehaviour
{

    public static string latestCheckpoint;
    public static Vector3 cameraOffset;
    public GameObject Player;
    public CameraController CamController;
    private Vector3 defaultCamOffset = new Vector3(5.4F, 20.5F, -17.1F);
    private static bool created = false;
    public static InventoryComponent latestPlayerInventory;
    public static string leftHandComponent;

    Dictionary<string, Transform> checkpointMap;

    private void Start()
    {
        if (CamController == null)
        {
            var cam = GameObject.Find("Main Camera");
            if (cam != null)
            {
                CamController = cam.GetComponent<CameraController>();
            }
        }
        checkpointMap = new Dictionary<string, Transform>();
        var checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        foreach (var checkpoint in checkpoints)
        {
            var name = checkpoint.GetComponent<CheckpointTrigger>().CheckpointName;
            var transform = checkpoint.GetComponent<Transform>();
            if (!checkpointMap.ContainsKey(name))
                checkpointMap.Add(name, transform);
        }

        if (!string.IsNullOrEmpty(latestCheckpoint))
        {
            GoToLatestCheckpoint();
        }
    }

    void Awake()
    {
        checkpointMap = new Dictionary<string, Transform>();
        var checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        foreach (var checkpoint in checkpoints)
        {
            var name = checkpoint.GetComponent<CheckpointTrigger>().CheckpointName;
            var transform = checkpoint.GetComponent<Transform>();
            if (!checkpointMap.ContainsKey(name))
                checkpointMap.Add(name, transform);
        }

        if (!created)
        {
            DontDestroyOnLoad(gameObject);
            cameraOffset = defaultCamOffset;
            created = true;

            Debug.Log("Awake: " + gameObject);
        }
    }

    public void SetLatestCheckpoint(string checkpoint)
    {
        if (Player == null)
        {
            Player = GameObject.Find("Player");
        }
        latestCheckpoint = checkpoint;
        cameraOffset = CamController.offset;
        latestPlayerInventory = Player.GetComponent<InventoryComponent>();
        var equippedItem = Player.GetComponentInChildren<EquipComponent>().EquipedItem;
        if (equippedItem != null)
            leftHandComponent = equippedItem.name;
    }

    public void GoToLatestCheckpoint()
    {
        if (Player == null)
        {
            Player = GameObject.Find("Player");
        }
        CamController.SetOffset(cameraOffset);
        Player.transform.position = checkpointMap[latestCheckpoint].position;
        var lhc = GameObject.Find(leftHandComponent);
        var lamp = GameObject.Find("lamp");
        var pickup = lamp.GetComponent<Pickup>();
        var lantern = lamp.GetComponent<Lantern>();
        pickup.IsInteracting = true;
        pickup.IsEquiped = true;   // equip to left hand
        pickup.IsInteractable = false;
        lantern.EquipRightHand();
        lamp.GetComponent<LightComponent>().ToggleLightOn();

        lhc.GetComponent<InventoryItemComponent>().AddToInventory = true;
    }
}
﻿using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// Creates oil trail using Line renderer attached to the player. 
/// </summary>
public class OilTrailSystem : ComponentSystem
{
    private struct Group
    {
        public OilTrailComponent OilTrail;
        public Pickup pickup;
        //  public InputComponent Input;
        public Transform Transform;
    }

    private struct PlayerData
    {
        public readonly int Length;
        public ComponentArray<Transform> Transforms;
        public ComponentArray<InputComponent> Inputs;
        public ComponentArray<FireComponent> FireComponent;
    }

    [Inject] private PlayerData Player;

    /// <summary>
    /// Left hand data
    /// </summary>
    private struct LeftHandData
    {
        readonly public int Length;
        public ComponentArray<EquipComponent> EquipComp;
        public ComponentArray<LeftHandComponent> data;
    }

    [Inject] private LeftHandData leftHandData;

    private struct HUD
    {
        public HUDUpdate props;
    }

    /// <summary>
    /// Player inventory data
    /// </summary>
    struct InventoryData
    {
        readonly public int Length;
        public ComponentArray<InventoryComponent> Inventory;
    }

    [Inject] private InventoryData inventoryData;

    bool spilling = false;
    /// <summary>
    /// If equipped, holding down left mouse button will create oil trail on ground. Oil trail is rendered using line renderer where holding down the 
    /// the left mouse button will create "points" on the ground for the oil to be drawn on to. 
    /// </summary>
    protected override void OnUpdate()
    {
        //Debug.Log(spilling);
        var entityHUD = GetEntities<HUD>()[0];
        var lhComponent = leftHandData.data[0];
        var transform = Player.Transforms[0];
        var fireComponent = Player.FireComponent[0];
        var entities = GetEntities<Group>();
        foreach (var entity in entities)
        {
            if(entity.OilTrail.usedOil >= entity.OilTrail.TrailLimit && entity.pickup.IsEquiped)
            {
                //GameObject.Destroy(entity.OilTrail.gameObject.GetComponent<InteractUIComponent>());//.enabled = false;
                entity.OilTrail.gameObject.GetComponent<InteractUIComponent>().Show = false;
                lhComponent.DropItem();
                entityHUD.props.Disable();
                AkSoundEngine.PostEvent("Stop_SpillingGas", entity.OilTrail.gameObject);
                //Equip next item
                if (inventoryData.Inventory[0].PlayerInventory.Items.Count > 0)
                {
                    leftHandData.EquipComp[0].EquipItem(inventoryData.Inventory[0].PlayerInventory.Items[0].Prefab);
                    inventoryData.Inventory[0].PlayerInventory.Remove(inventoryData.Inventory[0].PlayerInventory.Items[0]);
                }
            }

            if (entity.OilTrail.IsEquipped && entity.pickup.IsEquiped)
            {
                var position = entity.Transform.position;// transform.position;
                                                         // entity.Transform.position = position;
                var lineRenderer = entity.OilTrail.LineRenderer;
                var minDistance = entity.OilTrail.TrailMinimumDistance;
                if(Input.GetKeyDown(KeyCode.Joystick1Button2) || Input.GetKeyDown(KeyCode.Joystick1Button0))
                {
                    AkSoundEngine.PostEvent("Play_SpillingGas", entity.OilTrail.gameObject);
                }
                
                if (Player.Inputs[0].Control("OilTrail"))
                {
                    spilling = true;
                    //entityHUD.props.ShowNoFade();
                    if (entity.OilTrail.CurrentTrailCount == 0)
                    {
                        entity.OilTrail.TrailPoints.Add(position);
                        lineRenderer.positionCount = 2;
                        //position.y += 0.10f;
                        lineRenderer.SetPosition(0, position);

                        position.x += 0.1f;
                        lineRenderer.SetPosition(1, position);
                        entity.OilTrail.CurrentTrailCount = 2;
                    }
                    else
                    {
                        var distance = float.MaxValue;
                        Vector3 closestPoint = new Vector3();
                        for (int i = 0; i < lineRenderer.positionCount; ++i)
                        {
                            var trailDistance = Vector3.Distance(lineRenderer.GetPosition(i), position);
                            if (trailDistance < distance)
                            {
                                distance = trailDistance;
                                closestPoint = lineRenderer.GetPosition(i);
                            }
                        }

                        if (entity.OilTrail.usedOil < entity.OilTrail.TrailLimit && distance >= minDistance)
                        {
                            entity.OilTrail.TrailPoints.Add(position);
                            lineRenderer.positionCount = entity.OilTrail.CurrentTrailCount;
                            //position.y += 0.10f;
                            lineRenderer.SetPosition(entity.OilTrail.CurrentTrailCount - 1, position);

                            entity.OilTrail.CurrentTrailCount++;
                            entity.OilTrail.usedOil++;
                        }

                        // Reset the oil trail if the oil can isn't used up. Makes the mechanic a little easier to use. 
                        if (distance > entity.OilTrail.TrailMaximumDistance && entity.OilTrail.usedOil < entity.OilTrail.TrailLimit)
                        {
                            entity.OilTrail.TrailPoints.Clear();
                            entity.OilTrail.TrailPoints.Add(position);
                            Vector3[] pos = { };
                            lineRenderer.SetPositions(pos);
                            lineRenderer.positionCount = 2;
                            //position.y += 0.10f;
                            lineRenderer.SetPosition(0, position);

                            position.x += 0.1f;
                            lineRenderer.SetPosition(1, position);
                            entity.OilTrail.CurrentTrailCount = 2;
                        }
                    }
                }
                else
                {
                    spilling = false;
                }

                if(lineRenderer.positionCount > 0)
                {
                    fireComponent.OilTrail = entity.OilTrail;
                }
                else
                {
                    fireComponent.OilTrail = null;
                }

                if (spilling)
                {
                    entityHUD.props.ShowNoFade();
                }
                else if(Input.GetKeyUp(KeyCode.Joystick1Button0))
                {
                    AkSoundEngine.PostEvent("Stop_SpillingGas", entity.OilTrail.gameObject);
                    entityHUD.props.Disable();
                }
            }
            else
            {
                spilling = false;
            }
            if (Input.GetKeyUp(KeyCode.Joystick1Button2))
            {
                AkSoundEngine.PostEvent("Stop_SpillingGas", entity.OilTrail.gameObject);
            }
        }
    }

}

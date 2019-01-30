﻿using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// System to instantiate fire at oil trail points. 
/// </summary>
public class FireSystem : ComponentSystem
{
    private struct Group
    {
        public FireComponent Fire;
        public Transform Transform;
    }

    private struct IgniterGroup
    {
        public Pickup PickupItem;
        public FireIgniterComponent Igniter;
        public Transform Transform;
    }

    private struct OilCanInstanceGroup
    {
        public ComponentArray<OilTrailComponent> OilTrail;
    }

    [Inject] private OilCanInstanceGroup OilCanInstance;

    /// <summary>
    /// Player data
    /// </summary>
    private struct Player
    {
        readonly public int Length;
        public ComponentArray<InputComponent> InputComponents;
        public ComponentArray<Transform> Transforms;
    }
    [Inject] private Player playerData;

    protected override void OnUpdate()
    {
        var playerTransform = playerData.Transforms[0];
        var entities = GetEntities<Group>();

        foreach (var entity in entities)
        {
            HandleFireQueue(entity.Fire);
        }

        foreach (var entity in GetEntities<Group>())
        {
            if (entity.Fire.OilTrail != null)
            {
                var oilTrail = entity.Fire.OilTrail;
                var firePrefab = entity.Fire.FirePrefab;
                var points = oilTrail.TrailPoints.ToArray();
                int closestPointIndex;
                int igniterIndex;
                bool isPlayerClose = IsClose(points, playerTransform.position, entity.Fire.OilTrailDistanceThreshold, out closestPointIndex);
                bool isIgniterClose = IsIgniterClose(points, out igniterIndex);
                if (isIgniterClose)
                    closestPointIndex = igniterIndex;

                // Allow burning of oil on ground only if player there is oil to burn and player is close to oil trail
                if ((playerData.InputComponents[0].Control("LightFire") && points.Length > 0 && isPlayerClose) || isIgniterClose)
                {
                    entities[0].Fire.FireSoundList.Enqueue(GameObject.Instantiate(entities[0].Fire.FireSoundPrefab,points[0],Quaternion.identity));
                    entity.Fire.IsFireStopped = false;
                    oilTrail.LineRenderer.positionCount = 0;
                    //oilTrail.CurrentTrailCount = 0;
                    for (int i = closestPointIndex; i >= 0; --i)
                    {
                        entity.Fire.FireDownQueue.Enqueue(points[i]);
                    }

                    for (int i = closestPointIndex + 1; i < points.Length; ++i)
                    {
                        entity.Fire.FireUpQueue.Enqueue(points[i]);
                    }

                    oilTrail.TrailPoints.Clear(); //Clear out Oil Trail Component once fire has been instantiated. 
                }
            }

        }

    }

    void HandleFireQueue(FireComponent fireComponent)
    {
        if (fireComponent.CurrentTime > fireComponent.PropogationTimeStep)
        {
            fireComponent.CurrentTime = 0;
            if (fireComponent.FireDownQueue.Count > 0)
            {
                DequeueAndInstantiateFire(fireComponent.FireDownQueue, fireComponent.FirePrefab, fireComponent);
            }

            if (fireComponent.FireUpQueue.Count > 0)
            {
                DequeueAndInstantiateFire(fireComponent.FireUpQueue, fireComponent.FirePrefab, fireComponent);
            }
        }

        fireComponent.CurrentTime += Time.deltaTime;
    }

    void DequeueAndInstantiateFire(Queue<Vector3> queue, GameObject firePrefab, FireComponent fireComponent)
    {
        var pos = queue.Dequeue();
        pos.x += 0.2F;
        var instance = PrefabPool.Spawn(firePrefab, pos, new Quaternion());
        instance.GetComponent<ParticleSystem>().Play();
        var main = instance.GetComponent<ParticleSystem>().main;
        main.loop = true;
        fireComponent.Instances.Add(instance);
    }

    /// <summary>
    /// Returns true if player is close to given position. 
    /// </summary>
    /// <param name="points"></param>
    /// <param name="position"></param>
    /// <param name="distanceThreshold"></param>
    /// <returns></returns>
    private bool IsPlayerClose(Vector3[] points, Vector3 position, float distanceThreshold, out int closestIndex)
    {
        int index = -1;
        var minDistance = float.MaxValue;
        for (int i = 0; i < points.Length; ++i)
        {
            var distance = Vector3.Distance(points[i], position);
            if (distance < minDistance)
            {
                minDistance = distance;
                index = i;
            }
        }

        closestIndex = index;
        return (minDistance <= distanceThreshold);
    }

    bool IsIgniterClose(Vector3[] points, out int closestIndex)
    {
        bool isClose = false;
        closestIndex = -1;
        var igniters = GetEntities<IgniterGroup>();
        foreach(var igniter in igniters)
        {
            if (!igniter.PickupItem.IsEquiped)
            {
                isClose = IsClose(points, igniter.Transform.position, igniter.Igniter.IgnitionDistance, out closestIndex);
                if (isClose) break;
            }
        }

        return isClose;
    }

    /// <summary>
    /// Returns true if given position is close to one of the points in the array. 
    /// </summary>
    /// <param name="points"></param>
    /// <param name="position"></param>
    /// <param name="distanceThreshold"></param>
    /// <returns></returns>
    bool IsClose(Vector3[] points, Vector3 position, float distanceThreshold, out int closestIndex)
    {
        int index = -1;
        var minDistance = float.MaxValue;
        for (int i = 0; i < points.Length; ++i)
        {
            var distance = Vector3.Distance(points[i], position);
            if (distance < minDistance)
            {
                minDistance = distance;
                index = i;
            }
        }
        closestIndex = index;
        return (minDistance <= distanceThreshold);
    }

}

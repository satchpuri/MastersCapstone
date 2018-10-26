﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class PatrolSystem : ComponentSystem
{
    private struct EnemyPatrolData
    {
        public NavAgentComponent AgentComponent;
        public Transform EnemyTransform;
        public WayPointComponent PatrolComponent;

    }

    protected override void OnUpdate()
    {

        foreach (var enemy in GetEntities<EnemyPatrolData>())
        {
            if (enemy.PatrolComponent.IsWandering == true)
            {
                if (enemy.PatrolComponent.Waypoints.Length == 0)
                {
                    return;
                }
                else
                {
                    enemy.AgentComponent.Agent.SetDestination(enemy.PatrolComponent.Waypoints[enemy.PatrolComponent.currentWaypointIndex].position);

                    if (Vector3.Distance(enemy.EnemyTransform.position, enemy.PatrolComponent.Waypoints[enemy.PatrolComponent.currentWaypointIndex].position) <= 2)
                    {
                        enemy.PatrolComponent.currentWaypointIndex = Random.Range(0, enemy.PatrolComponent.Waypoints.Length);
                        enemy.AgentComponent.Agent.SetDestination(enemy.PatrolComponent.Waypoints[enemy.PatrolComponent.currentWaypointIndex].position);
                    }
                }

            }
           
        }


    }


}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tantan
{
    public class PlayerPhysics : MonoBehaviour
    {
        [SerializeField] private WorldGrid m_worldGrid;
        [SerializeField] private CameraController m_cameraController;

        private void Update()
        {
            if(m_cameraController.IsRotating)
                return;
            // Check if standing on block
        }

        private void OnDrawGizmos(){
            if(m_cameraController.IsRotating)
                return;
            var playerPos = GetPlayerPosition();
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(new Vector3(playerPos.x, playerPos.y, 0.3f), new Vector3(1f, 1f,1f));
        }

        private Vector2Int GetPlayerPosition()
        {
            Vector3Int worldPos = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
            switch (m_cameraController.GetRotation())
            {
                case Rotation.R_0:
                    return new Vector2Int(worldPos.x, worldPos.y);
                case Rotation.R_90:
                    return new Vector2Int(WorldGrid.DEFAULT_WORLD_SIZE - worldPos.z - 1, worldPos.y);
                case Rotation.R_180:
                    return new Vector2Int(WorldGrid.DEFAULT_WORLD_SIZE - worldPos.x - 1, worldPos.y);
                case Rotation.R_270:
                    return new Vector2Int(worldPos.z, worldPos.y);
            }
            return Vector2Int.zero;
        }
    }
}
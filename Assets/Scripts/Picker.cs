using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tantan
{
    public class Picker : MonoBehaviour
    {
        [SerializeField] private WorldGrid m_worldGrid;
        [SerializeField] private Camera m_currentCamera;
        [SerializeField] private CameraController m_cameraController;
        private void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
                Vector3 worldPos = m_currentCamera.ScreenToWorldPoint(Input.mousePosition);
                Debug.Log("worldpos: "+ worldPos);
                Vector3 gridPosition = m_cameraController.ConvertToVisualPosition(worldPos);
                Debug.Log(gridPosition);
                PhysicsObject physicsObject = m_worldGrid.GetPhysicsObjectAtLocation(new Vector2Int((int)(gridPosition.x + 0.5f), (int)(gridPosition.y + 0.5f)));
                if(physicsObject == null)
                    return;
                physicsObject.SetRed();
            }
        }
    }
}
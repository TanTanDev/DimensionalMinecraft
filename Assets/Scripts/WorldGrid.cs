using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tantan
{
    public class WorldGrid : MonoBehaviour
    {
        [SerializeField] private CameraController m_cameraController;
        public const int DEFAULT_WORLD_SIZE = 16;
        // Loop through world objects and put them into the worldgrid vectors
        private List<List<List<PhysicsObject>>> m_worldGrid;
        // generated grid based on camera rotation

        public struct SideViewData
        {
            public SideViewData(PhysicsObject a_physicsObject, int a_depth) 
            {
                PhysicsObject = a_physicsObject;
                Depth = a_depth;
            }
            public void Set(PhysicsObject a_physicsObject, int a_depth)
            {
                PhysicsObject = a_physicsObject;
                Depth = a_depth;
            }
            public PhysicsObject PhysicsObject;
            public int Depth;
        }
        private List<List<SideViewData>> m_currentGrid;

        public void Construct()
        {
            m_worldGrid = new List<List<List<PhysicsObject>>>(DEFAULT_WORLD_SIZE);
            m_currentGrid = new List<List<SideViewData>>(DEFAULT_WORLD_SIZE);
            for(int x = 0; x < m_currentGrid.Capacity; x++)
            {
                m_currentGrid.Add(new List<SideViewData>(DEFAULT_WORLD_SIZE));
                for(int y = 0; y < m_currentGrid[x].Capacity; y++)
                    m_currentGrid[x].Add(new SideViewData(null, 0));
            }
            for(int x = 0; x < m_worldGrid.Capacity; x++)
            {
                m_worldGrid.Add(new List<List<PhysicsObject>>(DEFAULT_WORLD_SIZE));
                for(int y = 0; y < m_worldGrid.Capacity; y++)
                {
                    m_worldGrid[x].Add(new List<PhysicsObject>(DEFAULT_WORLD_SIZE));
                    for(int z = 0; z < m_worldGrid[x][y].Capacity; z++)
                        m_worldGrid[x][y].Add(null);
                }
            }
        }

        public void RemoveAt(Vector3Int a_worldIndex)
        {
            m_worldGrid[a_worldIndex.x][a_worldIndex.y][a_worldIndex.z] = null;
            GenerateCurrentGrid();
        }

        public void AddAt(Vector3Int a_worldIndex, PhysicsObject a_physicsObject)
        {
            m_worldGrid[a_worldIndex.x][a_worldIndex.y][a_worldIndex.z] = a_physicsObject;
            GenerateCurrentGrid();
        }

        public void Generate() {
            PhysicsObject[] physicsObjects = GameObject.FindObjectsOfType<PhysicsObject>();
            // int extentsX, extentsY, extentsZ = 0;
            // Zero everything
            for(int x = 0; x < m_worldGrid.Count; x++)
                for(int z = 0; z < m_worldGrid[x].Count; z++)
                    for(int y = 0; y < m_worldGrid.Count; y++)
                        m_worldGrid[x][z][y] = null;

            for(int i = 0; i < physicsObjects.Length; i++)
            {
                PhysicsObject physicsObject = physicsObjects[i];
                Vector3 positionF = physicsObject.transform.position;
                Vector3Int position = new Vector3Int((int)positionF.x, (int)positionF.y, (int)positionF.z);
                m_worldGrid[position.x][position.y][position.z] = physicsObject;
            }
        }

        private void GenerateCurrentGrid()
        {
            // reset
            for(int x = 0; x < m_currentGrid.Count; x++)
                for(int y = 0; y < m_currentGrid[x].Count; y++)
                    m_currentGrid[x][y] = new SideViewData(null, 0);

            // calculate based camera rotation
            switch(m_cameraController.GetRotation())
            {
                case Rotation.R_0:
                {
                    for(int x = 0; x < m_worldGrid.Count; x++)
                    {
                        for(int y = 0; y < m_worldGrid[x].Count; y++)
                        {
                            for(int z = m_worldGrid[x][y].Count-1; z > 0 ; z--)
                            {
                                PhysicsObject physicsObject = m_worldGrid[x][y][z];
                                if(physicsObject != null && physicsObject.PhysicsType != PhysicsType.None)
                                {
                                    m_currentGrid[x][y] = new SideViewData(physicsObject, z);
                                    break;
                                }
                            }
                        }
                    }
                    break;
                }
                case Rotation.R_90:
                {
                    //for(int x = m_worldGrid.Count-1; x > 0; x--)
                    for(int x = 0; x < m_worldGrid.Count; x++)
                    {
                        for(int y = 0; y < m_worldGrid[x].Count; y++)
                        {
                            for(int z = 0; z < m_worldGrid[x][y].Count; z++)
                            {
                                PhysicsObject physicsObject = m_worldGrid[(m_worldGrid[x][y].Count -1) - z][y][(m_worldGrid.Count-1) - x];
                                if(physicsObject != null && physicsObject.PhysicsType != PhysicsType.None)
                                {
                                    m_currentGrid[x][y] = new SideViewData(physicsObject, (m_worldGrid[x][y].Count -1) - z);
                                    break;
                                }
                            }
                        }
                    }
                    break;
                }
                case Rotation.R_180:
                {
                    for(int x = 0; x < m_worldGrid.Count; x++)
                    {
                        for(int y = 0; y < m_worldGrid[x].Count; y++)
                        {
                            for(int z = 0; z < m_worldGrid[x][y].Count ; z++)
                            {
                                PhysicsObject physicsObject = m_worldGrid[(m_worldGrid.Count-1)-x][y][z];
                                if(physicsObject != null && physicsObject.PhysicsType != PhysicsType.None)
                                {
                                    PhysicsType physicsType = physicsObject.PhysicsType;
                                    m_currentGrid[x][y] = new SideViewData(physicsObject, z);
                                    break;
                                }
                            }
                        }
                    }
                    break;
                }
                case Rotation.R_270:
                {
                    for(int x = 0; x < m_worldGrid.Count; x++)
                    {
                        for(int y = 0; y < m_worldGrid[x].Count; y++)
                        {
                            for(int z = 0; z < m_worldGrid[x][y].Count; z++)
                            {
                                PhysicsObject physicsObject = m_worldGrid[z][y][x];
                                if(physicsObject != null && physicsObject.PhysicsType != PhysicsType.None)
                                {
                                    PhysicsType physicsType = physicsObject.PhysicsType;
                                    m_currentGrid[x][y] = new SideViewData(physicsObject, z);
                                    break;
                                }
                            }
                        }
                    }
                    break;
                }
            } 
        }

        private void Awake()
        {
            m_cameraController.OnRotationChanged += GenerateCurrentGrid;
            Construct();
            Generate();
            GenerateCurrentGrid();
        }

        public PhysicsType? GetPhysicsTypeAtLocation(Vector2Int a_location) 
        {
            SideViewData sideViewData = m_currentGrid[a_location.x][a_location.y];
            if(sideViewData.PhysicsObject == null)
                return null;
            return sideViewData.PhysicsObject.PhysicsType;
        }
        
        public PhysicsObject GetPhysicsObjectAtLocation(Vector2Int a_location) 
        {
            return m_currentGrid[a_location.x][a_location.y].PhysicsObject;
        }

        public int GetPhysicsDepthAtLocation(Vector2Int a_location) 
        {
            return m_currentGrid[a_location.x][a_location.y].Depth;
        }

        private void OnDrawGizmos()
        {
            if(!Application.isPlaying)
                return;
            float scale = 0.3f;
            Vector3 scaleVector = new Vector3(scale, scale, scale);
            for(int x = 0; x < m_currentGrid.Count; x++)
            {
                for(int y = 0; y < m_currentGrid[x].Count; y++)
                {
                    Color color = Color.black;
                    SideViewData sideViewData = m_currentGrid[x][y];
                    if(sideViewData.PhysicsObject != null && sideViewData.PhysicsObject.PhysicsType == PhysicsType.Solid)
                        color = Color.red;
                    if(sideViewData.PhysicsObject != null && sideViewData.PhysicsObject.PhysicsType == PhysicsType.Platform)
                        color = Color.yellow;
                    Gizmos.color = color;
                    Gizmos.DrawWireCube(new Vector3(x,y,0f), scaleVector);
                }
            }
        }
    }
}

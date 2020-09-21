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
        private List<List<List<PhysicsType>>> m_worldGrid;
        // generated grid based on camera rotation
        private List<List<PhysicsType>> m_currentGrid;

        public void Construct()
        {
            m_worldGrid = new List<List<List<PhysicsType>>>(DEFAULT_WORLD_SIZE);
            m_currentGrid = new List<List<PhysicsType>>(DEFAULT_WORLD_SIZE);
            for(int x = 0; x < m_currentGrid.Capacity; x++)
            {
                m_currentGrid.Add(new List<PhysicsType>(DEFAULT_WORLD_SIZE));
                for(int y = 0; y < m_currentGrid[x].Capacity; y++)
                    m_currentGrid[x].Add(PhysicsType.None);
            }
            for(int x = 0; x < m_worldGrid.Capacity; x++)
            {
                m_worldGrid.Add(new List<List<PhysicsType>>(DEFAULT_WORLD_SIZE));
                for(int y = 0; y < m_worldGrid.Capacity; y++)
                {
                    m_worldGrid[x].Add(new List<PhysicsType>(DEFAULT_WORLD_SIZE));
                    for(int z = 0; z < m_worldGrid[x][y].Capacity; z++)
                        m_worldGrid[x][y].Add(PhysicsType.None);
                }
            }
        }

        public void Generate() {
            PhysicsObject[] physicsObjects = GameObject.FindObjectsOfType<PhysicsObject>();
            // int extentsX, extentsY, extentsZ = 0;
            // Zero everything
            for(int x = 0; x < m_worldGrid.Count; x++)
                for(int z = 0; z < m_worldGrid[x].Count; z++)
                    for(int y = 0; y < m_worldGrid.Count; y++)
                        m_worldGrid[x][z][y] = PhysicsType.None;

            for(int i = 0; i < physicsObjects.Length; i++)
            {
                PhysicsObject physicsObject = physicsObjects[i];
                Vector3 positionF = physicsObject.transform.position;
                Vector3Int position = new Vector3Int((int)positionF.x, (int)positionF.y, (int)positionF.z);
                Debug.Log(position);
                m_worldGrid[position.x][position.y][position.z] = physicsObject.PhysicsType;
            }
        }

        private void GenerateCurrentGrid()
        {
            // teset
            for(int x = 0; x < m_currentGrid.Count; x++)
                for(int y = 0; y < m_currentGrid[x].Count; y++)
                    m_currentGrid[x][y] = PhysicsType.None;

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
                                PhysicsType physicsType = m_worldGrid[x][y][z];
                                if(physicsType != PhysicsType.None)
                                {
                                    m_currentGrid[x][y] = physicsType;
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
                                PhysicsType physicsType = m_worldGrid[(m_worldGrid[x][y].Count -1 ) - z][y][(m_worldGrid.Count-1) - x];
                                if(physicsType != PhysicsType.None)
                                {
                                    m_currentGrid[x][y] = physicsType;
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
                                PhysicsType physicsType = m_worldGrid[(m_worldGrid.Count-1)-x][y][z];
                                if(physicsType != PhysicsType.None)
                                {
                                    m_currentGrid[x][y] = physicsType;
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
                                PhysicsType physicsType = m_worldGrid[(m_worldGrid[x][y].Count -1 ) - z][y][x];
                                if(physicsType != PhysicsType.None)
                                {
                                    m_currentGrid[x][y] = physicsType;
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

        public PhysicsType GetPhysicsTypeAtLocation(Vector2Int a_location) 
        {
            return m_currentGrid[a_location.x][a_location.y];
        }

        private void OnDrawGizmos()
        {
            float scale = 0.3f;
            Vector3 scaleVector = new Vector3(scale, scale, scale);
            for(int x = 0; x < m_currentGrid.Count; x++)
            {
                for(int y = 0; y < m_currentGrid[x].Count; y++)
                {
                    Color color = Color.black;
                    if(m_currentGrid[x][y] == PhysicsType.Solid)
                        color = Color.red;
                    if(m_currentGrid[x][y] == PhysicsType.Platform)
                        color = Color.yellow;
                    Gizmos.color = color;
                    Gizmos.DrawWireCube(new Vector3(x,y,0f), scaleVector);
                }
            }
        }
    }
}

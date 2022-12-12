using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tantan
{
    public class WorldGrid : MonoBehaviour
    {
        [SerializeField] private CameraController m_cameraController;
        [SerializeField] private WorldGridScriptable m_worldScriptScriptable;
        public const int DEFAULT_WORLD_SIZE = 64;
        // Loop through world objects and put them into the worldgrid vectors
        //private List<List<List<PhysicsObject>>> m_worldGrid;
        private Dictionary<Vector3Int, PhysicsObject> m_worldGrid;
        private Vector3Int m_worldBoundMax;
        private Vector3Int m_worldBoundMin;
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
        //private List<List<SideViewData>> m_currentGrid;
        private Dictionary<Vector2Int, SideViewData> m_currentGrid;

        public void Construct()
        {
            //m_worldGrid = new List<List<List<PhysicsObject>>>(DEFAULT_WORLD_SIZE);
            m_worldGrid = new Dictionary<Vector3Int, PhysicsObject>();
            //m_currentGrid = new List<List<SideViewData>>(DEFAULT_WORLD_SIZE);
            m_currentGrid = new Dictionary<Vector2Int, SideViewData>();     
        }

        public void RemoveAt(Vector3Int a_worldIndex)
        {
            m_worldGrid.Remove(a_worldIndex);
            GenerateCurrentGrid();
        }

        public void AddAt(Vector3Int a_worldIndex, PhysicsObject a_physicsObject)
        {
            m_worldGrid.Add(a_worldIndex, a_physicsObject);
            GenerateCurrentGrid();
        }

        public void Generate() {
            PhysicsObject[] physicsObjects = GameObject.FindObjectsOfType<PhysicsObject>();
            // int extentsX, extentsY, extentsZ = 0;
            // Zero everything
            m_worldGrid.Clear();

            for(int i = 0; i < physicsObjects.Length; i++)
            {
                PhysicsObject physicsObject = physicsObjects[i];
                Vector3 positionF = physicsObject.transform.position;
                Vector3Int position = new Vector3Int((int)positionF.x, (int)positionF.y, (int)positionF.z);
                m_worldGrid.Add(position, physicsObject);
            }
        }

        private void calculate_depth(KeyValuePair<Vector3Int, PhysicsObject> entry, Vector2Int gridPos, int depth, bool want_low_depth) {
            //foreach(KeyValuePair<Vector3Int, PhysicsObject> entry in worldgrid) {
             //var gridPos = new Vector2Int(entry.Key.z, entry.Key.y);
             if (!m_currentGrid.ContainsKey(gridPos)) {
                m_currentGrid.Add(gridPos, new SideViewData(entry.Value, depth));
             } else {
                bool is_lower = m_currentGrid[gridPos].Depth < depth;
                bool is_higher = m_currentGrid[gridPos].Depth > depth;
                bool check = is_lower && want_low_depth || is_higher && !want_low_depth;
                if(check) {
                    m_currentGrid[gridPos] = new SideViewData(entry.Value, depth);
                }
             }
        }

        private void GenerateCurrentGrid()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            // reset
            m_currentGrid.Clear();
            //for(int x = 0; x < m_currentGrid.Count; x++)
            //    for(int y = 0; y < m_currentGrid[x].Count; y++)
            //        m_currentGrid[x][y] = new SideViewData(null, 0);

            // calculate based camera rotation
            switch(m_cameraController.GetRotation())
            {
                case Rotation.R_0:
                {
                    foreach(KeyValuePair<Vector3Int, PhysicsObject> entry in m_worldGrid) {
                        var gridPos = new Vector2Int(entry.Key.x, entry.Key.y);
                        this.calculate_depth(entry, gridPos, entry.Key.z, true);
                    }
                    break;
                }
                case Rotation.R_90:
                {
                    foreach(KeyValuePair<Vector3Int, PhysicsObject> entry in m_worldGrid) {
                        var gridPos = new Vector2Int(entry.Key.z, entry.Key.y);
                        this.calculate_depth(entry, gridPos, entry.Key.x,true);
                    }
                    break;
                }
                case Rotation.R_180:
                {
                    foreach(KeyValuePair<Vector3Int, PhysicsObject> entry in m_worldGrid) {
                        var gridPos = new Vector2Int(entry.Key.x, entry.Key.y);
                        this.calculate_depth(entry, gridPos, entry.Key.z,false);
                    }
                    break;
                }
                case Rotation.R_270:
                {
                    foreach(KeyValuePair<Vector3Int, PhysicsObject> entry in m_worldGrid) {
                        var gridPos = new Vector2Int(entry.Key.z, entry.Key.y);
                        this.calculate_depth(entry, gridPos, entry.Key.x,false);
                    }
                    break;
                }
            } 
            watch.Stop();
            var elapsed = watch.ElapsedMilliseconds;
            Debug.Log("elapsed: " + elapsed);
        }

        private void Awake()
        {
            m_worldScriptScriptable.WorldGrid = this;
            m_cameraController.OnRotationChanged += GenerateCurrentGrid;
            Construct();
            Generate();
            GenerateCurrentGrid();
        }

        public PhysicsType? GetPhysicsTypeAtLocation(Vector2Int a_location) 
        {
            SideViewData sideViewData;
            if(!m_currentGrid.TryGetValue(a_location, out sideViewData)) {
                return null;
            }

            if(sideViewData.PhysicsObject == null)
                return null;
            return sideViewData.PhysicsObject.PhysicsType;
        }
        
        public PhysicsObject GetPhysicsObjectAtLocation(Vector2Int a_location) 
        {
            SideViewData o;
            if(m_currentGrid.TryGetValue(a_location, out o)) {
                return o.PhysicsObject;
            }
            return null;
        }

        public int GetPhysicsDepthAtLocation(Vector2Int a_location) 
        {
            return m_currentGrid[a_location].Depth;
        }

        private void OnDrawGizmos()
        {
            if(!Application.isPlaying)
                return;
            float scale = 0.3f;
            Vector3 scaleVector = new Vector3(scale, scale, scale);
            foreach(KeyValuePair<Vector2Int, SideViewData> entry in m_currentGrid) {
                    Color color = Color.black;
                    Vector2Int pos = entry.Key;
                    SideViewData sideViewData = entry.Value;
                    if(sideViewData.PhysicsObject != null && sideViewData.PhysicsObject.PhysicsType == PhysicsType.Solid)
                        color = Color.red;
                    if(sideViewData.PhysicsObject != null && sideViewData.PhysicsObject.PhysicsType == PhysicsType.Platform)
                        color = Color.yellow;
                    Gizmos.color = color;
                    Gizmos.DrawWireCube(new Vector3(pos.x,pos.y,0f), scaleVector);

            }
        }
    }
}

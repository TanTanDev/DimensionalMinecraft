using UnityEngine;
using Zenject;
using TanTanTools.Audio;

namespace Tantan
{
    public class Picker : MonoBehaviour
    {
        [SerializeField] private WorldGrid m_worldGrid;
        [SerializeField] private Camera m_currentCamera;
        [SerializeField] private CameraController m_cameraController;
        [SerializeField] private GameObject m_grassPrefab;
        [SerializeField] private InventoryScriptable m_inventoryScriptable;
        [SerializeField] private Animator m_currentAnimator;
        [SerializeField] private ScriptableAudioAsset m_breakAudio;
        [SerializeField] private ScriptableAudioAsset m_placeAudio;
        [SerializeField] private ScriptableAudioAsset m_noAudio;
        [Inject] private IAudioManager m_audioManager;

        public void SetAnimator(Animator a_animator)
        {
            m_currentAnimator = a_animator;
        }

        private void PerformBreak()
        {
            Vector3 worldPos = m_currentCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 gridPosition = m_cameraController.ConvertToVisualPosition(worldPos);
            PhysicsObject physicsObject = m_worldGrid.GetPhysicsObjectAtLocation(new Vector2Int((int)(gridPosition.x + 0.5f), (int)(gridPosition.y + 0.5f)));
            if(physicsObject == null)
            {
                //m_audioManager.StopAudio(m_breakAudio);
                return;
            }
            physicsObject.OnHold();
            m_currentAnimator.SetTrigger("Place");
            m_audioManager.PlayAudio(m_breakAudio/*, true*/);
        }

        private void PerformPlace()
        {
            if(!m_inventoryScriptable.Inventory.CanPlace())
            {
                m_audioManager.PlayAudio(m_noAudio);
                return;
            }
            Vector3 worldPos = m_currentCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 gridPosition = m_cameraController.ConvertToVisualPosition(worldPos);
            Vector3 spawnPosWorld = new Vector3(gridPosition.x, gridPosition.y, 0f);
            PhysicsObject physicsObject = m_worldGrid.GetPhysicsObjectAtLocation(new Vector2Int((int)(gridPosition.x + 0.5f), (int)(gridPosition.y + 0.5f)));

            // if occupied place in front
            if(physicsObject != null)
            {
                // Place in front
                int blockDepth = m_worldGrid.GetPhysicsDepthAtLocation(new Vector2Int((int)(gridPosition.x + 0.5f), (int)(gridPosition.y + 0.5f)));
                spawnPosWorld = worldPos;
                var cameraRotation = m_cameraController.GetRotation();
                int blockDelta = 0;
                // Don't ask...
                if(cameraRotation == Rotation.R_0 || cameraRotation == Rotation.R_90)
                    blockDelta = 1;
                else
                    blockDelta = -1;

                spawnPosWorld = m_cameraController.GetWorldFromDepth(spawnPosWorld, blockDepth+blockDelta);
            }
            else 
            {
                Vector3 playerPosWorld = transform.position;
                Vector3 playerPosScreen = m_cameraController.ConvertToVisualPosition(playerPosWorld);
                spawnPosWorld = worldPos;
                spawnPosWorld = m_cameraController.GetWorldFromDepth(spawnPosWorld, playerPosScreen.z);
            }
            spawnPosWorld = new Vector3(Mathf.Floor(spawnPosWorld.x+0.5f), Mathf.Floor(spawnPosWorld.y+0.5f), Mathf.Floor(spawnPosWorld.z+0.5f));
            GameObject block_GO = Instantiate(m_grassPrefab, spawnPosWorld, Quaternion.identity); 
            PhysicsObject newPhysicsObject = block_GO.GetComponent<PhysicsObject>();
            ItemScriptable holdingItem = m_inventoryScriptable.Inventory.GetHoldingItem();
            newPhysicsObject.Setup(holdingItem);
            m_inventoryScriptable.Inventory.DecreaseHoldingItem();
            m_worldGrid.AddAt(new Vector3Int((int)spawnPosWorld.x, (int)spawnPosWorld.y, (int)spawnPosWorld.z), block_GO.GetComponent<PhysicsObject>());
            m_currentAnimator.SetTrigger("Place");
            m_audioManager.PlayAudio(m_placeAudio);
        }

        private void Update()
        {
            if(Input.GetMouseButton(0))
                PerformBreak();
            // if(Input.GetMouseButtonUp(0))
            //     m_audioManager.StopAudio(m_breakAudio);
            // Left click spawn grass block for now
            if(Input.GetMouseButtonDown(1))
                PerformPlace();
        }
    }
}
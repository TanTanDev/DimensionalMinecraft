﻿using UnityEngine;
using UnityEngine.Events;
using TanTanTools.Audio;

namespace Tantan
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float m_speed = 180.0f;
        [SerializeField] private CameraControllerScriptable m_cameraControllerScriptable;
        [SerializeField] private Transform m_followTransform;
        [SerializeField] private float m_distanceFromTarget = 200.0f;
        [Zenject.Inject] private IAudioManager m_audioManager;
        [SerializeField] private ScriptableAudioAsset m_dimensionChangeSound;
        public UnityAction OnRotationChanged;
        private float m_rotateProgress;

        public bool IsRotating{get{return m_rotateProgress < 1.0f;}}

        enum RotationDirection {
            Left,
            Right,
        }

        enum CameraState
        {
            Following,
            Rotating,
        }

        private CameraState m_cameraState;
        private Quaternion m_currentRotation;
        private Quaternion m_startRotation;
        private Rotation m_rotation = Rotation.R_0;
        private Vector3 m_lerpedFollowPosition;
        public Rotation GetRotation()
        {
            return m_rotation;
        }

        private void Awake()
        {
            m_cameraControllerScriptable.CameraController = this;
        }

        private void ChangeRotation(RotationDirection a_direction)
        {
            m_audioManager.PlayAudio(m_dimensionChangeSound);
            m_rotateProgress = 0f;
            m_cameraState = CameraState.Rotating;
            m_startRotation = transform.rotation * Quaternion.Euler(0f,180,0f);
            m_lerpedFollowPosition = transform.position + transform.forward * m_distanceFromTarget;
            switch (m_rotation)
            {
                case Rotation.R_0:
                    m_rotation = (a_direction==RotationDirection.Left)?Rotation.R_270: Rotation.R_90;
                    break;
                case Rotation.R_90:
                    m_rotation = (a_direction==RotationDirection.Left)?Rotation.R_0: Rotation.R_180;
                    break;
                case Rotation.R_180:
                    m_rotation = (a_direction==RotationDirection.Left)?Rotation.R_90: Rotation.R_270;
                    break;
                case Rotation.R_270:
                    m_rotation = (a_direction==RotationDirection.Left)?Rotation.R_180: Rotation.R_0;
                    break;
            }
            OnRotationChanged.Invoke();
        }

        public float GetAngle()
        {
            if(m_rotation == Rotation.R_0)
                return 0f;
            if(m_rotation == Rotation.R_90)
                return 90f;
            if(m_rotation == Rotation.R_180)
                return 180f;
            else
                return 270f;
        }

        private void Update() {
            if(Input.GetKeyDown(KeyCode.LeftArrow))
                ChangeRotation(RotationDirection.Left);
            if(Input.GetKeyDown(KeyCode.RightArrow))
                ChangeRotation(RotationDirection.Right);

            if(m_cameraState == CameraState.Following)
            {
                Vector3 targetLocation = m_followTransform.position;
                targetLocation -= transform.forward * m_distanceFromTarget;
                transform.position = Vector3.Lerp(transform.position, targetLocation, Time.deltaTime * m_speed);
            }

            if(m_rotateProgress >= 1f)
                return;
            Quaternion targetRotation = Quaternion.AngleAxis(GetAngle(), Vector3.up);

            m_rotateProgress += Time.deltaTime * m_speed;
            if(m_rotateProgress >= 1f)
                m_cameraState = CameraState.Following;
            m_currentRotation = Quaternion.Lerp(m_startRotation, targetRotation, m_rotateProgress);
            Vector3 direction = m_currentRotation * Vector3.forward;

            m_lerpedFollowPosition = Vector3.Lerp(m_lerpedFollowPosition, m_followTransform.position, Time.deltaTime * m_speed); 
            transform.position = m_lerpedFollowPosition + direction * m_distanceFromTarget;

            Vector3 relativePos = m_lerpedFollowPosition - transform.position;
            Quaternion cameraRotation = Quaternion.LookRotation(relativePos);
            transform.rotation = cameraRotation;
        }

        public Vector3 ConvertToVisualPosition(Vector3 a_current)
        {
            switch (m_rotation)
            {
                case Rotation.R_0:
                {
                    a_current = new Vector3(a_current.x, a_current.y, a_current.z);
                    break;
                }
                case Rotation.R_90:
                {
                    a_current = new Vector3(a_current.z, a_current.y, a_current.x);
                    break;
                }
                case Rotation.R_180:
                {
                    a_current = new Vector3(a_current.x, a_current.y, a_current.z);
                    break;
                }
                case Rotation.R_270:
                {
                    a_current = new Vector3(a_current.z, a_current.y, a_current.x);
                    break;
                }
            }
            return a_current;
        }

        public Vector3 GetWorldFromDepth(Vector3 a_world, float depth)
        {
            switch (m_rotation)
            {
                case Rotation.R_0:
                {
                    return new Vector3(a_world.x, a_world.y, depth);
                }
                case Rotation.R_90:
                {
                    return new Vector3(depth, a_world.y, a_world.z); 
                }
                case Rotation.R_180:
                {
                    return new Vector3(a_world.x, a_world.y, depth); 
                }
                case Rotation.R_270:
                {
                    return new Vector3(depth, a_world.y, a_world.z); 
                }
            }
            return Vector3.zero;
        }

        public Vector3 ConvertToVisualDelta(Vector3 a_delta)
        {
            switch (m_rotation)
            {
                case Rotation.R_0:
                {
                    a_delta = new Vector3(-a_delta.x, a_delta.y, a_delta.z);
                    break;
                }
                case Rotation.R_90:
                {
                    a_delta = new Vector3(a_delta.z, a_delta.y, a_delta.x);
                    break;
                }
                case Rotation.R_180:
                {
                    a_delta = new Vector3(a_delta.x, a_delta.y, a_delta.z);
                    break;
                }
                case Rotation.R_270:
                {
                    a_delta = new Vector3(a_delta.z, a_delta.y, -a_delta.x);
                    break;
                }
            }
            return a_delta;
        }
    }
}
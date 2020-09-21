using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Tantan
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float m_speed = 180.0f;
        [SerializeField] private Transform m_followTransform;
        [SerializeField] private float m_distanceFromTarget = 200.0f;
        public UnityAction OnRotationChanged;

        enum RotationDirection {
            Left,
            Right,
        }

        private Quaternion m_currentRotation;
        private Rotation m_rotation = Rotation.R_0;
        public Rotation GetRotation()
        {
            return m_rotation;
        }

        private void ChangeRotation(RotationDirection a_direction)
        {
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

        private float GetAngle(){
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

            Quaternion targetRotation = Quaternion.AngleAxis(GetAngle(), Vector3.up);

            m_currentRotation = Quaternion.Lerp(m_currentRotation, targetRotation, Time.deltaTime * m_speed);
            Vector3 direction = m_currentRotation * Vector3.forward;
            transform.position = m_followTransform.position + direction * m_distanceFromTarget;

            Vector3 relativePos = m_followTransform.position - transform.position;
            Quaternion cameraRotation = Quaternion.LookRotation(relativePos);
            transform.rotation = cameraRotation;
        }
    }

}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace com.HtetAungHlaing
{
    public class _3D_Displayer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Header("Scroll")]
        [Tooltip("The speed at which zooming would occur")]
        public float ZoomSpeed;
        [Tooltip("")]
        public Limitations FOV_Limitations;

        [Header("Auto Rotation")]
        [Tooltip("The Speed at which the camera automatically rotates")]
        public float AutoRotation;
        [Tooltip("The time it takes before auto rotation starts")]
        public float AutoRotationTime;

        [Header("Manual Rotation;")]
        [Tooltip("The amount of rotation you want to move on each axis, no Z axis")]
        public Vector2 MovementVector;
        [Tooltip("Rotation limitations"), SerializeField]
        public Limitations rotationLimitations;
        
        private bool isDragging = false;
        private GameObject CameraCentre;
        /// <summary>
        /// The time at which the model is last rotated
        /// </summary>
        private float LastRotationTime;

        #region Unity_Default_Function
        private void Awake()
        {
            CameraCentre = Camera.main.transform.parent.gameObject;
            LastRotationTime = -AutoRotationTime;
        }

        private void Update()
        {

        }

        private void LateUpdate()
        {
            if(Time.time > LastRotationTime + AutoRotationTime && !isDragging)
            {
                //We will rotate with Sin function
                bool RotateUp = Mathf.Sin(Time.time / 4) > 0;
                RotateCamera(new Vector2(AutoRotation * 3,AutoRotation * ((RotateUp)?-1:1)));
            }
        }

        #endregion

        #region Zoom Functions
        public void ZoomIn()
        {
            Camera.main.fieldOfView -= ZoomSpeed;
            ClampZoom();
         
        }

        public void ZoomOut()
        {
            Camera.main.fieldOfView += ZoomSpeed;
            ClampZoom();
        }

        public void ClampZoom()
        {
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, FOV_Limitations.LowerLimitation, FOV_Limitations.UpperLimitation);
        }
        #endregion

        #region Pointer Functions
        public void OnPointerDown(PointerEventData eventData)
        {
            isDragging = true;
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            isDragging = true;
            RotateCamera(eventData.delta);
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false;
            LastRotationTime = Time.time;
        }
        #endregion
        #region RotateCamera
        private void RotateCamera(Vector2 InputVector)
        {
            float x, y;
            GetXandY(InputVector, out x, out y);
            //DevLog.Log("X: " + x + " Y:" + y);

            Vector2 NewMovementVector = new Vector2(-y * MovementVector.y, x* MovementVector.x);
            CameraCentre.transform.Rotate(NewMovementVector.x, NewMovementVector.y, 0);
            ClampRotation();
        }

        private void GetXandY(Vector2 vector, out float x, out float y)
        {
            x = vector.x;
            y = vector.y;
        }

        private void ClampRotation()
        {
            float NewRotationX = CameraCentre.transform.rotation.eulerAngles.x;

            if(NewRotationX > 180)
            {
                NewRotationX = NewRotationX - 360;
            }

            if(NewRotationX < -180)
            {
                NewRotationX = NewRotationX + 360;
            }

            NewRotationX = Mathf.Clamp(NewRotationX, rotationLimitations.LowerLimitation, rotationLimitations.UpperLimitation);
            //Debug.Log(NewRotationX);
            CameraCentre.transform.rotation = Quaternion.Lerp(CameraCentre.transform.rotation,Quaternion.Euler(NewRotationX, CameraCentre.transform.rotation.eulerAngles.y, 0), 0.5f);
        }
        #endregion
    }

    [Serializable]
    public class Limitations
    {
        public float UpperLimitation;
        public float LowerLimitation;
    }
}

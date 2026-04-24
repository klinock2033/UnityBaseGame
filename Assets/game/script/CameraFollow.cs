using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset = new Vector3(0f, 5f, -8f);
    [SerializeField] private float _smoothSpeed = 5f;
    [SerializeField] private float _mouseSensitivity = 10f;
    
    private float _yaw = 0f;
    private float _pitch = 0f;

    private void LateUpdate()
    {
        if (_target == null) return;
        
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;

        _yaw += mouseX;
        _pitch -= mouseY;
        
        //limit pitch
        _pitch = Mathf.Clamp(_pitch, -90f, 90f);
        
        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        
        
        Vector3 desiredPosition = _target.position + rotation * _offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);
        
        transform.LookAt(_target);
        
    }
}

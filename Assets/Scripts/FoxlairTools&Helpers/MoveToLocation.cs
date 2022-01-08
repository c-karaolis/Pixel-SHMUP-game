using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoveToLocation : MonoBehaviour
{
    bool isMoving = false;
    Transform passenger;
    Vector3 targetPosition;
    float speed;
    public UnityEvent onDestinationReached;

    public void MoveTo(Transform _passenger, Vector3 _targetPosition, float _speed)
    {
        passenger = _passenger;
        targetPosition = _targetPosition;
        isMoving = true;
        speed = _speed;
    }

    public void StopMoving() { isMoving = false; }

    void Update()
    {
        if (isMoving)
        {
            float step = speed * Time.deltaTime;
            passenger.position = Vector3.MoveTowards(transform.position, targetPosition, step);
            
            if (Vector3.Distance(passenger.position, targetPosition) < 0.001f)
            {
                onDestinationReached?.Invoke();
                isMoving = false;
            }
        }
    }



}

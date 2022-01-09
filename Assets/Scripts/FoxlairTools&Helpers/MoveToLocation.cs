using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoveToLocation : MonoBehaviour
{
    #region Variables
    bool isMoving = false;
    Transform passenger;
    Vector3 targetPosition;
    float speed;
    public float reachDestinationDistanceThreshold = 0.001f;

    public bool enabledStuckMechanic = false;
    Vector3 positionBeforeStep;
    Vector3 positionAfterStep;
    int stuckStrikes = 0;
    public int stuckStrikesTolerance = 5;
    public float stuckPositionDistanceThreshold = 0.001f;

    #endregion

    public UnityEvent onDestinationReached;
    public UnityEvent onStuckPassenger;

    public void MoveTo(Transform _passenger, Vector3 _targetPosition, float _speed)
    {
        passenger = _passenger;
        targetPosition = _targetPosition;
        speed = _speed;
        isMoving = true;
    }

    public void StopMoving() { isMoving = false; }

    void Update()
    {
        if (isMoving)
        {
            UpdateMovementData();
            if(enabledStuckMechanic){CheckStuckPassenger();}
            CheckPassengerReachedDestination();
        }
    }

    private void UpdateMovementData()
    {
        positionBeforeStep = passenger.position;
        float step = speed * Time.deltaTime;
        passenger.position = Vector3.MoveTowards(transform.position, targetPosition, step);
        positionAfterStep = passenger.position;
    }

    private void CheckStuckPassenger()
    {
        if (Vector3.Distance(positionBeforeStep, positionAfterStep) < stuckPositionDistanceThreshold)
        {
            stuckStrikes++;
            if (stuckStrikes >= stuckStrikesTolerance)
            {
                onStuckPassenger?.Invoke();
            }
        }
        else
        {
            stuckStrikes = 0;
        }
    }

    private void CheckPassengerReachedDestination()
    {
        if (Vector3.Distance(passenger.position, targetPosition) < reachDestinationDistanceThreshold)
        {
            onDestinationReached?.Invoke();
            isMoving = false;
        }
    }

   
}

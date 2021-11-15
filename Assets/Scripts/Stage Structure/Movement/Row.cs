using Foxlair.Enemies;
using Foxlair.Tools.Events;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(RowAnimation))]
public class Row : MonoBehaviour
{
    #region  Fields
    public Formation formation;
    public bool isVertical = true;
    public bool overrideStartingPosition = false;
    public Vector2 overridedPosition;
    public float distanceBetweenEnemies = 1f;
    public int numberOfSlots;
    public Transform startingPosition;
    public Vector2 distanceVector;
    public GameObject slotsGameObject;
    public GameObject slotPrefab;
    public List<GameObject> allSlotGameObjects;
    public List<Slot> slots;
    public RowAnimation rowAnimation;
    #endregion

    #region  Methods
    private void Awake()
    {
        slots = GetComponentsInChildren<Slot>().ToList();
        rowAnimation = GetComponent<RowAnimation>();

    }

    private void Start()
    {
        FoxlairEventManager.Instance.Enemy_OnReachedSlot_Event += OnEnemyReachedSlot;
    }

    void OnEnemyReachedSlot(EnemySpaceship enemySpaceship,Slot slot)
    {
        if(slot == slots[slots.Count-1])
        rowAnimation.StartRowAnimation();
    }
    private void OnDestroy()
    {
        FoxlairEventManager.Instance.Enemy_OnReachedSlot_Event -= OnEnemyReachedSlot;
    }
    #endregion

    #region Generation
    public void GenerateSlots()
    {
        if (slotsGameObject == null)
        {
            CreateSlotsGameObject();
        }
        RemoveAllSlots();
        float distanceOffset = 0f;

        if (overrideStartingPosition)
        {
            distanceVector = overridedPosition;
        }
        else
        {
            distanceVector = startingPosition.position;

        }

        for (int i = 0; i < numberOfSlots; i++)
        {
            distanceOffset += distanceBetweenEnemies;

            if (isVertical)
            {
                distanceVector = new Vector2(transform.position.x, distanceOffset);
            }
            else
            {
                distanceVector = new Vector2(distanceOffset, transform.position.y);
            }

            //TODO: change slots to enemies or add slot in enemies to show which slot is theirs?
            GameObject newSlot = Instantiate(slotPrefab, distanceVector, startingPosition.rotation, slotsGameObject.transform);
           // newSlot.AddComponent<Slot>();
            slots.Add(newSlot.GetComponent<Slot>());

        }
        distanceVector = new Vector2();
    }

    private void CreateSlotsGameObject()
    {
        slotsGameObject = Instantiate(new GameObject(), transform.position, startingPosition.rotation, transform);
        slotsGameObject.transform.position = transform.position;
    }

    private void RetrieveSlotGameObjects()
    {
        foreach (Transform child in slotsGameObject.transform)
        {
            allSlotGameObjects.Add(child.gameObject);
        }
    }
    public void RemoveAllSlots()
    {

        if (slotsGameObject == null)
        {
            CreateSlotsGameObject();
        }
        RetrieveSlotGameObjects();

        for (int i = 0; i < allSlotGameObjects.Count; i++)
        {
            DestroyImmediate(allSlotGameObjects[i]);
        }

        slots = new List<Slot>();
        allSlotGameObjects = new List<GameObject>();
    }
    #endregion
}

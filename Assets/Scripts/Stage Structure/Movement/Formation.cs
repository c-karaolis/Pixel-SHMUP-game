using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Formation : MonoBehaviour
{
    #region  Fields
    public List<Row> rows;
    public List<Slot> slots;
    public bool isVertical = true;
    public int numberOfRows;
    public int overrideRowNumberOfSlots = 0;
    public int overrideDistanceBetweenSlots = 0;
    public Transform startingPosition;
    public float distanceBetweenRows = 1f;
    public Vector2 distanceVector;
    public GameObject rowsGameObject;
    public GameObject rowPrefab;
    public List<GameObject> allRowGameObjects;
    #endregion

    #region  Methods
    private void Awake()
    {
        rows = GetComponentsInChildren<Row>().ToList();
        slots = GetSlots();
    }

    public int GetSlotsCount()
    {
        int count = 0;
        foreach (Row row in rows)
        {
            count += row.slots.Count;
        }
        return count;
    }

    public List<Slot> GetSlots()
    {
        List<Slot> slots = new List<Slot>();
        foreach (Row row in rows)
        {
            slots.AddRange(row.slots);
        }
        return slots;
    }
    #endregion

    #region Generation
    public void GenerateRows()
    {
        if (rowsGameObject == null)
        {
            FindRowsGameObject();
        }
        RemoveAllRows();
        float distanceOffset = 0f;

        distanceVector = startingPosition.position;

        for (int i = 0; i < numberOfRows; i++)
        {
            distanceOffset += distanceBetweenRows;

            if (isVertical)
            {
                distanceVector = new Vector2(0, distanceOffset);
            }
            else
            {
                distanceVector = new Vector2(distanceOffset, 0);
            }

            GameObject newRow = Instantiate(rowPrefab, distanceVector, startingPosition.rotation, rowsGameObject.transform);
            newRow.GetComponent<Row>().isVertical = !isVertical;
            newRow.GetComponent<Row>().overrideStartingPosition = true;
            newRow.GetComponent<Row>().formation = this;
            newRow.GetComponent<Row>().overridedPosition = distanceVector;
            newRow.GetComponent<Row>().slotsGameObject.transform.position = distanceVector;
            if (overrideRowNumberOfSlots != 0)
            {
                newRow.GetComponent<Row>().numberOfSlots = overrideRowNumberOfSlots;
            }
            if (overrideDistanceBetweenSlots != 0)
            {
                newRow.GetComponent<Row>().distanceBetweenEnemies = overrideDistanceBetweenSlots;
            }
            newRow.GetComponent<Row>().GenerateSlots();
            rows.Add(newRow.GetComponent<Row>());

        }
        distanceVector = new Vector2();
    }

    private void FindRowsGameObject()
    {
        foreach (Transform child in transform)
        {
            if (child.name == "Rows")
            {
                rowsGameObject = child.gameObject;
            }
        }
        RetrieveRowGameObjects();

    }

    private void RetrieveRowGameObjects()
    {

        foreach (Transform child in rowsGameObject.transform)
        {
            allRowGameObjects.Add(child.gameObject);
        }
    }

    public void RemoveAllRows()
    {

        if (rowsGameObject == null)
        {
            FindRowsGameObject();
        }
        RetrieveRowGameObjects();

        for (int i = 0; i < allRowGameObjects.Count; i++)
        {
            DestroyImmediate(allRowGameObjects[i]);
        }

        rows = new List<Row>();
        allRowGameObjects = new List<GameObject>();
    }

    #endregion
}

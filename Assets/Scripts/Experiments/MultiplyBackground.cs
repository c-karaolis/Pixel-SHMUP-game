using System.Collections.Generic;
using UnityEngine;

public class MultiplyBackground : MonoBehaviour
{

    public bool shouldGenerate = false;

    // Background scroll speed can be set in Inspector with slider
    [Range(1f, 20f)]
    public float scrollSpeed = 1f;

    // Scroll offset value to smoothly repeat backgrounds movement
    public float scrollOffset;
    // Backgrounds new position
    float newPos;

    // Start position of background movement
    Vector2 startPos;

    public static float xOffset; 
    public static float yOffset;

    public List<GameObject> backgroundLayers;

    string parentName;
    Vector3 parentPosition;

    private void OnValidate()
    {
        //if (!shouldGenerate)
        //    return;

        //foreach (Transform child in transform)
        //{
        //    backgroundLayers.Add(child.gameObject);
        //}

       
    }

    private void OnEnable()
    {
       
    }


    // Start is called before the first frame update
    void Start()
    {
        //startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //newPos = Mathf.Repeat(Time.time * -scrollSpeed, scrollOffset);

        //// Setting new position
        //transform.position = startPos + Vector2.up * newPos;
    }
    void GenerateClones()
    {
        if(transform.childCount != backgroundLayers.Count)
        {
            return;
        }

        foreach (GameObject childGameObject in backgroundLayers)
        {
            xOffset = childGameObject.GetComponent<SpriteRenderer>().size.x;
            yOffset = childGameObject.GetComponent<SpriteRenderer>().size.y;


            parentName = childGameObject.gameObject.name;
            parentPosition = childGameObject.transform.position;

            //LEFT COLUMN
            GameObject cloneMinusX = Instantiate(childGameObject, transform) as GameObject;
            cloneMinusX.name = parentName + " clone";
            cloneMinusX.transform.position = new Vector3(-xOffset, parentPosition.y, parentPosition.z);

            GameObject cloneMinusXPlusY = Instantiate(childGameObject, transform) as GameObject;
            cloneMinusXPlusY.name = cloneMinusX.name + " clone";
            cloneMinusXPlusY.transform.position = new Vector3(cloneMinusX.transform.position.x, yOffset, cloneMinusX.transform.position.z);

            GameObject cloneMinusXMinusY = Instantiate(childGameObject, transform) as GameObject;
            cloneMinusXMinusY.name = cloneMinusX.name + " clone";
            cloneMinusXMinusY.transform.position = new Vector3(cloneMinusX.transform.position.x, -yOffset, cloneMinusX.transform.position.z);



            //MID COLUMN
            GameObject cloneMinusY = Instantiate(childGameObject, transform) as GameObject;
            cloneMinusY.name = parentName + " clone";
            cloneMinusY.transform.position = new Vector3(parentPosition.x, -yOffset, parentPosition.z);

            GameObject clonePlusY = Instantiate(childGameObject, transform) as GameObject;
            clonePlusY.name = parentName + " clone";
            clonePlusY.transform.position = new Vector3(parentPosition.x, yOffset, parentPosition.z);


            //RIGHT COLUMN
            GameObject clonePlusX = Instantiate(childGameObject, transform) as GameObject;
            clonePlusX.name = parentName + " clone";
            clonePlusX.transform.position = new Vector3(xOffset, parentPosition.y, parentPosition.z);

            GameObject clonePlusXPlusY = Instantiate(childGameObject, transform) as GameObject;
            clonePlusXPlusY.name = clonePlusX.name + " clone";
            clonePlusXPlusY.transform.position = new Vector3(clonePlusX.transform.position.x, yOffset, clonePlusX.transform.position.z);

            GameObject clonePlusXMinusY = Instantiate(childGameObject, transform) as GameObject;
            clonePlusXMinusY.name = clonePlusX.name + " clone";
            clonePlusXMinusY.transform.position = new Vector3(clonePlusX.transform.position.x, -yOffset, clonePlusX.transform.position.z);

        }
    }

    void DeleteClones()
    {
        Transform[] transforms;
        transforms = transform.GetComponentsInChildren<Transform>();

        foreach (Transform child in transforms)
        {
            if (child.gameObject.name.Contains("clone"))
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }
}

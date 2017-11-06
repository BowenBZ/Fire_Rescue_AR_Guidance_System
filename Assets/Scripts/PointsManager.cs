using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Academy.HoloToolkit.Unity;
using UnityEngine.VR.WSA;

public class PointsManager : Singleton<PointsManager> {

    public GameObject pointPrefab;
    const int maxPoint = 100;
    GameObject[] pointsCollection = new GameObject[maxPoint];
    int pointsNum = 0;

    public int PointsNum { get { return pointsNum; } }
    public GameObject[] PointsCollection { get { return pointsCollection; } }

    // Add new points to collection
    public void AddPoint()
    {
        pointsCollection[pointsNum] = Instantiate(pointPrefab, Camera.main.transform);
        AddSaveAnchor(pointsCollection[pointsNum]);
        pointsNum++;
    }

    // Whether or not to show points
    public void ShowAllPoints(bool ifShow)
    {
        for (int i = 0; i < maxPoint; i++)
        {
            if (pointsCollection[i] != null)
                pointsCollection[i].SetActive(ifShow);
        }
    }

    #region AnchorMovement
    // Add anchor to the object and save it to anchor store
    void AddSaveAnchor(GameObject obj)
    {
        obj.AddComponent<WorldAnchor>();
        // Save this world anchor to disk

    }

    // Load anchor to the object according to the index
    public void LoadAnchor(GameObject obj, int index)
    {
        // Load anchor from disk according to the index
    }

    // Clear all the points and anchor store
    public void Dispose()
    {
        for (int i = 0; i < maxPoint; i++)
        {
            if (pointsCollection[i] == null)
            {
                break;
            }
            else
            {
                DestroyImmediate(pointsCollection[i].GetComponent<WorldAnchor>());
                DestroyImmediate(pointsCollection[i]);
            }
        }
        // Clear anchor store
    }
    #endregion
}

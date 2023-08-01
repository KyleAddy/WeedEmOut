using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpace
{
    public Vector2 gridSpaceCoords { get;}
    public GameObject gridGameObject { get; }

    public Enumes.GridSpaceTypes gridSpaceType { get; }

    public bool isPoisoned = false;

    public GridSpace(Vector2 _gridSpaceCoords, GameObject _gridGameObject, Enumes.GridSpaceTypes _gridSpaceType)
    {
        gridSpaceCoords = _gridSpaceCoords;
        gridGameObject = _gridGameObject;
        gridSpaceType = _gridSpaceType;
    }

    public GridSpace(Vector2 _gridSpaceCoords)
    {
        gridSpaceCoords = _gridSpaceCoords;
        gridSpaceType = Enumes.GridSpaceTypes.none;
    }

    public void DestroyGridSpaceGameObject()
    {
        if(gridGameObject != null)
        {
            GameObject.Destroy(gridGameObject);
        }
    }
}

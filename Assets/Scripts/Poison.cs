using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : MonoBehaviour
{

    public void PoisionSpace(Vector2 _gridCoord, float _poisonLength)
    {
        StartCoroutine(Poision(_gridCoord,_poisonLength));
    }
    
    private IEnumerator Poision(Vector2 _gridCoord, float _poisonLength)
    {
        GridManager gridManager = GridManager.instance.GetComponent<GridManager>();
        List<GridSpace> neighbors = gridManager.GetNeighborGridSpaces(_gridCoord);
        neighbors.Add(gridManager.GetGridSpace((int)_gridCoord.x, (int)_gridCoord.y));
        foreach (GridSpace gridSpace in neighbors)
        {
            gridSpace.isPoisoned = true;
        }

        yield return new WaitForSeconds(_poisonLength);

        foreach (GridSpace gridSpace in neighbors)
        {
            GridSpace temp = gridManager.GetGridSpace((int)gridSpace.gridSpaceCoords.x, (int)gridSpace.gridSpaceCoords.x);

            temp.isPoisoned = false;
        }
        Destroy(gameObject);
        yield break; // Exit the coroutine                    
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeedManager : MonoBehaviour
{
    GridManager gridManager;

    [SerializeField]
    DKGGameEvent gameWonEvent;

    [SerializeField]
    DKGGameEvent gameLostEvent;

    [SerializeField]
    GameObject weedPrefab;

    [SerializeField]
    GameObject flowerPrefab;

    [SerializeField]
    IntVariable chloroplastCountVar;

    [SerializeField]
    IntVariable plantCostVar;

    [SerializeField]
    IntVariable chloroplastProductionVar;

    [SerializeField]
    IntVariable weedCostVar;

    [SerializeField]
    FloatVariable weedSpreadChanceVar;

    [SerializeField]
    FloatVariable poisonResistanceVar;

    [SerializeField]
    int worldTimerLimit = 1;

    [SerializeField]
    float wroldTimer = 1;

    [SerializeField]
    IntVariable weedCountVar;

    [SerializeField]
    IntVariable flowerCountVar;

    int totalGridSpaces = 0;

    [SerializeField]
    bool debug = false;

    // Start is called before the first frame update
    void Start()
    {
        gridManager = GridManager.instance.GetComponent<GridManager>();
        wroldTimer = worldTimerLimit;

        Vector2 gridSize = gridManager.GetGridSize();
        totalGridSpaces = (int)(gridSize.x * gridSize.y);
    }

    // Update is called once per frame
    void Update()
    {
        wroldTimer -= Time.deltaTime;

        if (wroldTimer <= 0)
        {
            SimulateGarden();
            wroldTimer = worldTimerLimit;
        }

        if (Input.GetMouseButtonDown(0))
        {
            clicked();
        }
    }

    public void IsGameFinished()
    {
        if(weedCountVar.GetValue() <= 0 && chloroplastCountVar.GetValue() < plantCostVar.GetValue())
        {
            Debug.Log("no more money lost");
            gameLostEvent.Raise();
            return;
        }

        float weedCovarage = (float)weedCountVar.GetValue() / (float)totalGridSpaces;

        if(Mathf.Round(weedCovarage * 100) >= 100)
        {
            Debug.Log("full coverage");
            gameWonEvent.Raise();
            return;
        }

        float flowerCoverage = (float)flowerCountVar.GetValue() / (float)totalGridSpaces;

        if (Mathf.Round(flowerCoverage * 100) >= 100)
        {
            Debug.Log("flower full coverage");
            gameLostEvent.Raise();
        }
    }

    private void SimulateGarden()
    {
        Vector2 gridSize = gridManager.GetGridSize();

        int chloroplastGained = 0;
        List<Vector2> weedToGrow = new List<Vector2>();

        List<Vector2> flowerToGrow = new List<Vector2>();

        List<GridSpace> gridSpacesToDelete = new List<GridSpace>();

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                GridSpace gridSpace = gridManager.GetGridSpace(x, y);
                if (debug) Debug.Log(gridSpace.gridSpaceType.ToString());

                if (gridSpace.gridSpaceType == Enumes.GridSpaceTypes.none) continue;

                if(gridSpace.gridSpaceType == Enumes.GridSpaceTypes.weed)
                {

                    if (gridSpace.isPoisoned)
                    {
                        float range = Random.Range(0f, 1f);

                        if (range > poisonResistanceVar.GetValue())
                        {
                            gridSpacesToDelete.Add(gridSpace);
                        }
                        continue;
                    }

                    List<GridSpace> neighbors = gridManager.GetNeighborGridSpaces(new Vector2(x, y));
                    int neighborFlowerCount = 0;

                    foreach (GridSpace neighbor in neighbors)
                    {
                        if (neighbor.gridSpaceType == Enumes.GridSpaceTypes.flower) neighborFlowerCount++;
                    }

                    if (Random.Range(0f, 1f) + (neighborFlowerCount * .05f) <= weedSpreadChanceVar.GetValue())
                    {
                        List<Vector2> emptyNeighbors = gridManager.GetEmptyNeighborCoords(new Vector2(x, y));
                        if (emptyNeighbors.Count == 0) continue;

                       

                        if (emptyNeighbors.Count > 0)
                        {
                            int i = Mathf.RoundToInt(Random.Range(0, emptyNeighbors.Count - 1));
                            if (i >= emptyNeighbors.Count) continue;
                            weedToGrow.Add(emptyNeighbors[i]);
                            continue;
                        }
                    }

                    chloroplastGained += chloroplastProductionVar.GetValue();
                    continue;
                }

                if (gridSpace.gridSpaceType == Enumes.GridSpaceTypes.flower)
                {
                    List<GridSpace> neighbors = gridManager.GetNeighborGridSpaces(gridSpace.gridSpaceCoords);
                    int neighborWeedCount = 0;

                    foreach (GridSpace neighbor in neighbors)
                    {
                        if (neighbor.gridSpaceType == Enumes.GridSpaceTypes.weed) neighborWeedCount++;
                    }

                    if(neighborWeedCount >= 3)
                    {
                        gridSpacesToDelete.Add(gridSpace);
                        continue;
                    }

                    if (Random.Range(0f, 1f) <= .05)
                    {
                        List<Vector2> emptyNeighbors = gridManager.GetEmptyNeighborCoords(new Vector2(x, y));
                        if (emptyNeighbors.Count == 0) continue;

                        if (emptyNeighbors.Count > 0)
                        {
                            int i = Mathf.RoundToInt(Random.Range(0, emptyNeighbors.Count - 1));
                            if (i >= emptyNeighbors.Count) continue;
                            flowerToGrow.Add(emptyNeighbors[i]);
                            continue;
                        }
                    }
                }
            }
        }

        chloroplastCountVar.IncrementValue(chloroplastGained);

        foreach (GridSpace gridSpace in gridSpacesToDelete)
        {
            gridManager.DeleteGridSpace(gridSpace);
        }

        foreach (Vector2 coord in weedToGrow)
        {
            GrowWeed(coord);
        }

        foreach (Vector2 coord in flowerToGrow)
        {
            GrowFlower(coord);
        }

        if(weedToGrow.Count > 0 || flowerToGrow.Count > 0)
        {
            AudioManager.instance.PlayClip(Enumes.AudioClips.pop);
        }

        IsGameFinished();
    }

    private void clicked()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f))
        {

            Vector2? gridCoord = gridManager.WorldToGridCoords(hit.point);

            if (!gridCoord.HasValue) return;

            GridSpace selectgedGridSpace = gridManager.GetGridSpace((int)gridCoord.Value.x, (int)gridCoord.Value.y);

            if (selectgedGridSpace.gridSpaceType != Enumes.GridSpaceTypes.none) return;

            if (chloroplastCountVar.GetValue() - weedCostVar.GetValue() < 0)
            {
                return;
            }

            chloroplastCountVar.DecrementValue(weedCostVar.GetValue());

            GrowWeed(gridCoord.Value);
            AudioManager.instance.PlayClip(Enumes.AudioClips.pop);
        }
    }

    private bool GrowWeed(Vector2 _gridCoord)
    {
        if (!gridManager.IsGridSpaceOpen(_gridCoord)) return false;

        Vector3 worldCoords = gridManager.GridToWorldCoords(_gridCoord);
        Ray yRay = new Ray(worldCoords + Vector3.up * 10, Vector3.down);
        RaycastHit yHit;

        float y = 0f;
        if (Physics.Raycast(yRay, out yHit, float.MaxValue))
        {
            // Return the Y coordinate of the hit point
            y = yHit.point.y;
        }

        GameObject newWeed = Instantiate(weedPrefab, new Vector3(worldCoords.x + Random.Range(-.05f, .05f), y, worldCoords.z + Random.Range(-.05f, .05f)), transform.rotation.normalized);
        //newWeed.transform.rotation.SetEulerAngles(0, Random.Range(0, 360), 0);

        GridSpace newGridSpace = new GridSpace(_gridCoord, newWeed, Enumes.GridSpaceTypes.weed);

        newGridSpace.isPoisoned = gridManager.GetGridSpace((int)_gridCoord.x, (int)_gridCoord.y).isPoisoned;

        gridManager.SetGridSpace(newGridSpace);
        return true;
    }

    private bool GrowFlower(Vector2 _gridCoord)
    {
        if (!gridManager.IsGridSpaceOpen(_gridCoord)) return false;


        Vector3 worldCoords = gridManager.GridToWorldCoords(_gridCoord);
        Ray yRay = new Ray(worldCoords + Vector3.up * 10, Vector3.down);
        RaycastHit yHit;

        float y = 0f;
        if (Physics.Raycast(yRay, out yHit, float.MaxValue))
        {
            // Return the Y coordinate of the hit point
            y = yHit.point.y;
        }

        GameObject newFlower = Instantiate(flowerPrefab, new Vector3(worldCoords.x + Random.Range(-.05f, .05f), y, worldCoords.z + Random.Range(-.05f, .05f)), transform.rotation.normalized);

        Transform childTransform = newFlower.transform.Find("Flowers"); // Find the child object by name
        if (childTransform != null)
        {
            GameObject childObject = childTransform.gameObject;
            childObject.SetActive(true); // Enable the child object
        }
        //newWeed.transform.rotation.SetEulerAngles(0, Random.Range(0, 360), 0);

        GridSpace newGridSpace = new GridSpace(_gridCoord, newFlower, Enumes.GridSpaceTypes.flower);

        newGridSpace.isPoisoned = gridManager.GetGridSpace((int)_gridCoord.x, (int)_gridCoord.y).isPoisoned;

        gridManager.SetGridSpace(newGridSpace);
        return true;
    }
}

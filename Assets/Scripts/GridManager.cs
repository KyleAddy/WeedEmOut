using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GameObject instance;

    [SerializeField]
    private int gridColumnCount;

    [SerializeField]
    private int gridRowCount;

    [SerializeField]
    private float gridSpaceWidth;

    [SerializeField]
    private float gridSpaceHeight;

    //grid is made from bottom left to top right. So (0,0) is the bottom left of the grid
    private GridSpace?[,] grid;

    private GameObject gridMesh;

    [SerializeField]
    public Material gridMeshMaterial;

    [SerializeField]
    private IntVariable weedCountVar;

    [SerializeField]
    private IntVariable flowerCountVar;

    private List<GridSpace> weedSpaceList = new List<GridSpace>();

    private List<GridSpace> flowerSpaceList = new List<GridSpace>();


    private void Awake()
    {
        if (instance == null)
        {
            instance = gameObject;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        grid = new GridSpace[gridColumnCount, gridRowCount];

        for (int x = 0; x < gridColumnCount; x++)
        {
            for (int y = 0; y < gridRowCount; y++)
            {
                grid[x, y] = new GridSpace(new Vector2(x,y));
                
            }
        }
        //CreateGridMesh();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            CreateGridMesh();
        }
    }

    public Vector2 GetGridSize()
    {
        return new Vector2(gridColumnCount, gridRowCount);
    }

    public void CreateGridMesh()
    {
        if(gridMesh != null)
        {
            Destroy(gridMesh);
        }
        // Create an empty game object to hold the mesh
        gridMesh = new GameObject("MeshObject");
        gridMesh.transform.parent = transform;
        // Create a new Mesh instance and assign it to the game object
        Mesh mesh = new Mesh();
        gridMesh.AddComponent<MeshFilter>().mesh = mesh;
        MeshRenderer meshRenderer = gridMesh.AddComponent<MeshRenderer>();

        // Set the mesh material
        meshRenderer.material = gridMeshMaterial;

        // Define the vertices and triangles of the mesh
        Vector3[] vertices = new Vector3[4]
        {
            Vector3.zero,
            new Vector3(0f, 0f, gridRowCount * gridSpaceHeight),
            new Vector3(gridColumnCount * gridSpaceWidth, 0f, gridRowCount * gridSpaceHeight),
            new Vector3(gridColumnCount * gridSpaceWidth, 0f, 0f)
        };

        // Define the triangles that connect the vertices
        int[] triangles = new int[6]
        {
            0, 1, 2, // First triangle
            0, 2, 3  // Second triangle
        };

        // Assign the vertices and triangles to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Position the mesh object in the scene
        gridMesh.transform.position = Vector3.zero; // Set to desired position
        gridMesh.AddComponent<MeshCollider>();

        MoveCameraToGrid();
    }

    public void MoveCameraToGrid()
    {
        // Calculate the desired position and rotation for the camera based on the grid bounds

        // Get the bounds of the grid mesh
        Bounds gridBounds = gridMesh.GetComponent<MeshRenderer>().bounds;

        // Calculate the maximum size (rows or columns) of the grid
        float maxSize = Mathf.Max(gridBounds.size.x, gridBounds.size.z);

        // Calculate the desired distance from the grid based on the maximum size
        float desiredDistance = (maxSize / 2f) / Mathf.Tan(Mathf.Deg2Rad * (Camera.main.fieldOfView / 2f));

        // Calculate the desired position and look-at point
        Vector3 desiredPosition = gridBounds.center - Camera.main.transform.forward * desiredDistance;
        Vector3 lookAtPoint = gridBounds.center;

        // Move the camera to the desired position and set the look-at point
        Camera.main.transform.position = desiredPosition;
        Camera.main.transform.LookAt(lookAtPoint);
    }

    public Vector2? WorldToGridCoords(Vector3 _worldCoords)
    {
        Vector2 gridCoords = new Vector2(Mathf.Floor(_worldCoords.x / gridSpaceWidth), Mathf.Floor(_worldCoords.z / gridSpaceHeight));
        if (gridCoords.x < 0 || gridCoords.x >= gridColumnCount || gridCoords.y < 0 || gridCoords.y >= gridRowCount) return null;

        return new Vector2(Mathf.Floor(_worldCoords.x / gridSpaceWidth), Mathf.Floor(_worldCoords.z / gridSpaceHeight));
    }

    public Vector3 GridToWorldCoords(Vector2 _gridCoords)
    {
        //return new Vector3(Mathf.Floor((_gridCoords.x * gridSpaceWidth) + (gridSpaceWidth / 2)), 0f, Mathf.Floor(_gridCoords.y * gridSpaceHeight));
        return new Vector3((_gridCoords.x * gridSpaceWidth) + (gridSpaceWidth / 2), 0f, (_gridCoords.y * gridSpaceHeight) + (gridSpaceHeight / 2));
    }

    public GridSpace? GetGridSpace(int _gridX, int _gridY)
    {
        if(_gridX < 0 || _gridX >= gridColumnCount || _gridY < 0 || _gridY >= gridRowCount)
        {
            return null;
        }

        return grid[_gridX, _gridY];
    }

    public void DeleteGridSpace(GridSpace _gridSpace)
    {

        switch (_gridSpace.gridSpaceType)
        {
            case Enumes.GridSpaceTypes.weed:
                weedSpaceList.Remove(_gridSpace);
                weedCountVar.DecrementValue();
                break;

            case Enumes.GridSpaceTypes.flower:
                flowerSpaceList.Remove(_gridSpace);
                flowerCountVar.DecrementValue();
                break;

            default:
                break;
        }

        _gridSpace.DestroyGridSpaceGameObject();

        grid[(int)_gridSpace.gridSpaceCoords.x, (int)_gridSpace.gridSpaceCoords.y] = new GridSpace(_gridSpace.gridSpaceCoords);

    }

    public bool SetGridSpace(GridSpace _newGridSpace)
    {
        if (_newGridSpace.gridSpaceCoords.x < 0 || _newGridSpace.gridSpaceCoords.x > gridColumnCount || _newGridSpace.gridSpaceCoords.y < 0 || _newGridSpace.gridSpaceCoords.y > gridRowCount)
        {
            return false;
        }

        if (grid[(int)_newGridSpace.gridSpaceCoords.x, (int)_newGridSpace.gridSpaceCoords.y].gridSpaceType != Enumes.GridSpaceTypes.none) return false;

        grid[(int)_newGridSpace.gridSpaceCoords.x, (int)_newGridSpace.gridSpaceCoords.y] = _newGridSpace;

        switch(_newGridSpace.gridSpaceType)
        {
            case Enumes.GridSpaceTypes.weed:
                weedSpaceList.Add(_newGridSpace);
                weedCountVar.IncrementValue();
                break;

            case Enumes.GridSpaceTypes.flower:
                flowerSpaceList.Add(_newGridSpace);
                flowerCountVar.IncrementValue();
                break;

            default:
                break;
        }

        return true;
    }

    public bool IsGridSpaceOpen(Vector2 _startCoord)
    {
        if (_startCoord.x < 0 || _startCoord.x >= gridColumnCount || _startCoord.y < 0 || _startCoord.y >= gridRowCount) return false;

        if (GetGridSpace((int)_startCoord.x, (int)_startCoord.y).gridSpaceType == Enumes.GridSpaceTypes.none) return true;
        return false;
    }

    public List<GridSpace> GetNeighborGridSpaces(Vector2 _homeCoord)
    {
        List<GridSpace> neighborGridSpaces = new List<GridSpace>();
        if (_homeCoord.x < 0 || _homeCoord.x >= gridColumnCount || _homeCoord.y < 0 || _homeCoord.y >= gridRowCount) return neighborGridSpaces;

        Vector2 neighboorCoord = new Vector2(_homeCoord.x - 1, _homeCoord.y - 1);
        for(int x = 0; x < 3; x++)
        {
            
            for (int y = 0; y < 3; y++)
            {
                if (neighboorCoord.x + x < 0 || neighboorCoord.x + x >= gridColumnCount || neighboorCoord.y + y < 0 || neighboorCoord.y + y >= gridRowCount) continue;
                if (neighboorCoord.x + x == _homeCoord.x && neighboorCoord.y + y == _homeCoord.y) continue;
                neighborGridSpaces.Add(GetGridSpace((int)neighboorCoord.x + x, (int)neighboorCoord.y + y));
            }
        }

        return neighborGridSpaces;
    }

    public List<Vector2> GetEmptyNeighborCoords(Vector2 _homeCoord)
    {
        List<Vector2> neighborCoords = new List<Vector2>();
        if (_homeCoord.x < 0 || _homeCoord.x >= gridColumnCount || _homeCoord.y < 0 || _homeCoord.y >= gridRowCount) return neighborCoords;

        Vector2 neighboorCoord = new Vector2(_homeCoord.x - 1, _homeCoord.y - 1);
        for (int x = 0; x < 3; x++)
        {

            for (int y = 0; y < 3; y++)
            {
                Vector2 temp = new Vector2(neighboorCoord.x + x, neighboorCoord.y + y);
                if (neighboorCoord.x + x < 0 || neighboorCoord.x + x >= gridColumnCount || neighboorCoord.y + y < 0 || neighboorCoord.y + y >= gridRowCount) continue;
                if (neighboorCoord.x + x == _homeCoord.x && neighboorCoord.y + y == _homeCoord.y) continue;
                if (!IsGridSpaceOpen(temp)) continue;
                neighborCoords.Add(temp);
            }
        }

        return neighborCoords;
    }

    public GridSpace GetRandomGridSpace()
    {
        return grid[Random.Range(0, gridColumnCount), Random.Range(0, gridRowCount)];
    }

    public GridSpace GetRandomWeedCoord()
    {
        if (weedSpaceList.Count == 0) return null;

        int index = (int)Random.Range(0f, (float)weedSpaceList.Count);

        return weedSpaceList[index];
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // Generate points for horizontal grid lines
        for (int i = 0; i <= gridRowCount; i++)
        {
            Vector3 startPoint = new Vector3(0f, .5f, i * gridSpaceHeight);
            Vector3 endPoint = new Vector3(gridColumnCount * gridSpaceWidth, .5f, i * gridSpaceHeight);

            Gizmos.DrawLine(startPoint, endPoint);
        }

        // Generate points for vertical grid lines
        for (int i = 0; i <= gridColumnCount; i++)
        {
            Vector3 startPoint = new Vector3(i * gridSpaceWidth, .5f, 0f);
            Vector3 endPoint = new Vector3(i * gridSpaceWidth, .5f, gridRowCount * gridSpaceHeight);

            Gizmos.DrawLine(startPoint, endPoint);
        }
    }
}

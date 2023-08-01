using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Gardener : MonoBehaviour
{
    NavMeshAgent agent;

    GridManager gridManager;

    List<Enumes.GardenerActions> gardenerActions;

    private int totalGridSpaces;

    [SerializeField]
    IntVariable weedCounterVar;

    [SerializeField]
    string currentAction = "";

    [SerializeField]
    bool isPerformingAction = false;

    [SerializeField]
    int actionTimerLimit = 5;

    float actionTimer = 5;

    [SerializeField]
    float actionTimeOutLimit = 30;

    [SerializeField]
    float actionTimeOut = 0;

    [SerializeField]
    int randomMovementTimerLimit = 3;

    float randomMovementTimer = 0;

    [SerializeField]
    float timeToPullWeed = 0;

    [SerializeField]
    GameObject poisionPrefab;

    [SerializeField]
    float timeToSpreadPoison = 0;

    [SerializeField]
    float poisonDuration = 5;

    [SerializeField]
    float timeToPlantFlower = 0;

    [SerializeField]
    GameObject flowerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        gridManager = GridManager.instance.GetComponent<GridManager>();
        agent = GetComponent<NavMeshAgent>();
        randomMovementTimer = randomMovementTimerLimit;
        actionTimer = actionTimerLimit;
        gardenerActions = new List<Enumes.GardenerActions>();
        Vector2 gridSize = gridManager.GetGridSize();
        totalGridSpaces = (int)(gridSize.x * gridSize.y);
        gardenerActions.Add(Enumes.GardenerActions.PlantFlower);

        actionTimeOut = actionTimeOutLimit;
    }

    // Update is called once per frame
    void Update()
    {
        gridManager = GridManager.instance.GetComponent<GridManager>();

        if (isPerformingAction)
        {
            actionTimeOut -= Time.deltaTime;
            return;
        }
        else
        {
            actionTimeOut = actionTimeOutLimit;
        }

        if (randomMovementTimer <= 0)
        {
            randomMovementTimer = randomMovementTimerLimit;
            SetAgentDestination(gridManager.GetRandomGridSpace());
        }
        else
        {
            randomMovementTimer -= Time.deltaTime;
        }

        if (actionTimer <= 0)
        {
            isPerformingAction = true;
            actionTimer = actionTimer = actionTimerLimit;
    
            if (weedCounterVar.GetValue() == 0)
            {
                StartCoroutine(PlantFlower());
                return;
            }
            int i = Random.Range(0, gardenerActions.Count);
            //Debug.Log("Action: " + i + " | count: " + gardenerActions.Count);
            Enumes.GardenerActions action = gardenerActions[i];
            
            switch (action)
            {
                case Enumes.GardenerActions.PlantFlower:
                    StartCoroutine(PlantFlower());
                    break;

                case Enumes.GardenerActions.PullWeed:
                    StartCoroutine(PullWeed());
                    break;

                case Enumes.GardenerActions.SpreadWeedKiller:
                    StartCoroutine(SpreadWeedKiller());
                    break;
            }

        }
        else
        {
            actionTimer -= Time.deltaTime;
        }
    }

    public void UpdateAvailableActions()
    {
        //Debug.Log("Updating actions | coverage: " + ((float)weedCounterVar.GetValue() / (float)totalGridSpaces));

        float weedCovarage = (float)weedCounterVar.GetValue() / (float)totalGridSpaces;
        if ((weedCovarage >= .01 && !gardenerActions.Contains(Enumes.GardenerActions.PullWeed)))
        {
            gardenerActions.Add(Enumes.GardenerActions.PullWeed);
            //Debug.Log("Added: Pull Weed");
        }

        if (((float)weedCounterVar.GetValue() / (float)totalGridSpaces) >= .03 && !gardenerActions.Contains(Enumes.GardenerActions.SpreadWeedKiller))
        {
            gardenerActions.Add(Enumes.GardenerActions.SpreadWeedKiller);
            //Debug.Log("Added: Weed Killer");
        }

        if (((float)weedCounterVar.GetValue() / (float)totalGridSpaces) >= .2 && gardenerActions.Contains(Enumes.GardenerActions.PullWeed))
        {
            gardenerActions.Remove(Enumes.GardenerActions.PullWeed);
            //Debug.Log("Removed: Pull Weed");
        }

        if (((float)weedCounterVar.GetValue() / (float)totalGridSpaces) >= .35 && gardenerActions.Contains(Enumes.GardenerActions.PlantFlower))
        {
            gardenerActions.Remove(Enumes.GardenerActions.PlantFlower);
            //Debug.Log("Removed: Planting Flower");
        }

        if ((weedCovarage >= .8 && !gardenerActions.Contains(Enumes.GardenerActions.PullWeed)))
        {
            gardenerActions.Add(Enumes.GardenerActions.PullWeed);
            //Debug.Log("Added: Pull Weed");
        }

    }

    private Vector3 SetAgentDestination(GridSpace _gridSpace)
    {
        Vector3 destination = gridManager.GridToWorldCoords(_gridSpace.gridSpaceCoords);

        Ray yRay = new Ray(destination + Vector3.up * 10, Vector3.down);
        RaycastHit yHit;

        float y = 0f;
        if (Physics.Raycast(yRay, out yHit, float.MaxValue))
        {
            // Return the Y coordinate of the hit point
            y = yHit.point.y;
        }
        destination = new Vector3(destination.x, y, destination.z);
        agent.SetDestination(destination);
        //Debug.Log("GridSpace: " + _gridSpace.gridSpaceCoords + " | destination: " + destination);
        return destination;
    }

    private IEnumerator PullWeed()
    {
        currentAction = "Pull Weed";
        GridSpace randomWeed = gridManager.GetRandomWeedCoord();
        if (randomWeed.gridSpaceType != Enumes.GridSpaceTypes.weed) yield break;
        SetAgentDestination(randomWeed);

        while (true)
        {
            yield return null;

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + (agent.stoppingDistance / 2))
            {

                var main = randomWeed.gridGameObject.GetComponent<ParticleSystem>().main;
                main.duration = timeToPullWeed;
                randomWeed.gridGameObject.GetComponent<ParticleSystem>().Play();
                yield return new WaitForSeconds(timeToPullWeed);
                // Perform the action once the destination is reached
                gridManager.DeleteGridSpace(randomWeed);
                isPerformingAction = false;
                currentAction = "";
                yield break; // Exit the coroutine
            }
            else
            {
                if (actionTimeOut <= 0)
                {
                    isPerformingAction = false;
                    currentAction = "";
                    yield break; // Exit the coroutine
                }
            }
        }
    }

    private IEnumerator SpreadWeedKiller()
    {
        currentAction = "Spread Weed killer";
        GridSpace randomWeed = gridManager.GetRandomWeedCoord();
        if (randomWeed == null)
        {
            randomWeed = gridManager.GetRandomGridSpace();
        }
        SetAgentDestination(randomWeed);

        while (true)
        {
            yield return null;

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + (agent.stoppingDistance / 2))
            {
                Vector3 worldLocation = gridManager.GridToWorldCoords(randomWeed.gridSpaceCoords);

                Ray yRay = new Ray(worldLocation + Vector3.up * 10, Vector3.down);
                RaycastHit yHit;

                float y = 0f;
                if (Physics.Raycast(yRay, out yHit, float.MaxValue))
                {
                    // Return the Y coordinate of the hit point
                    y = yHit.point.y;
                }
                Vector3 poisonPosition = new Vector3(worldLocation.x, y, worldLocation.z);
                GameObject poison = Instantiate(poisionPrefab, poisonPosition, transform.rotation.normalized);
                poison.GetComponent<Poison>().PoisionSpace(randomWeed.gridSpaceCoords, poisonDuration);

                AudioManager.instance.PlayClip(Enumes.AudioClips.spray);

                isPerformingAction = false;
                currentAction = "";
                yield break; // Exit the coroutine
            }
            else
            {
                if (actionTimeOut <= 0)
                {
                    isPerformingAction = false;
                    currentAction = "";
                    yield break; // Exit the coroutine
                }
            }
        }
    }

    private IEnumerator PlantFlower()
    {
        currentAction = "Plating Flower";
        GridSpace randomGridSpace = gridManager.GetRandomGridSpace();
        if (randomGridSpace.gridSpaceType != Enumes.GridSpaceTypes.none)
        {
            isPerformingAction = false;
            currentAction = "";
            yield break; // Exit the coroutine
        }
        Vector3 worldCoords = SetAgentDestination(randomGridSpace);

        if (actionTimeOut <= 0)
        {
            isPerformingAction = false;
            currentAction = "";
            yield break; // Exit the coroutine
        }

        while (true)
        {
            yield return null;
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + (agent.stoppingDistance / 2))
            {
                if(gridManager.GetGridSpace((int)randomGridSpace.gridSpaceCoords.x, (int)randomGridSpace.gridSpaceCoords.y).gridSpaceType != Enumes.GridSpaceTypes.none)
                {
                    isPerformingAction = false;
                    currentAction = "";
                    yield break; // Exit the coroutine
                }                

                GameObject newFlower = Instantiate(flowerPrefab, worldCoords, transform.rotation.normalized);

                GridSpace newGridSpace = new GridSpace(randomGridSpace.gridSpaceCoords, newFlower, Enumes.GridSpaceTypes.flower);

                gridManager.SetGridSpace(newGridSpace);

                //play planting animation

                var main = newGridSpace.gridGameObject.GetComponent<ParticleSystem>().main;
                main.duration = timeToPlantFlower;
                newGridSpace.gridGameObject.GetComponent<ParticleSystem>().Play();

                yield return new WaitForSeconds(timeToPlantFlower);

                Transform childTransform = newGridSpace.gridGameObject.transform.Find("Flowers"); // Find the child object by name
                if (childTransform != null)
                {
                    GameObject childObject = childTransform.gameObject;
                    childObject.SetActive(true); // Enable the child object
                }

                AudioManager.instance.PlayClip(Enumes.AudioClips.pop);

                isPerformingAction = false;
                currentAction = "";
                yield break; // Exit the coroutine
            }            
        }
    }
}

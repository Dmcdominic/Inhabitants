using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cell_controller : MonoBehaviour
{

    public static cell_controller instance;

    public cell prefabCell;
    public LayerMask treeZoneMask;

    const int WIDTH = 100, HEIGHT = 80;
    const float CELL_WIDTH = 0.2f, CELL_HEIGHT = 0.2f;

    private bool[,] on_map = new bool[HEIGHT, WIDTH];
    protected cell[,] cells = new cell[HEIGHT, WIDTH];

    //Controls speed of entire tree progression system
    public float growth_rate = 0.006f;
    // >1 biases towards growth, <1 biases towards decay
    public float growth_factor = 1.9f;
    // random factor in growth speed
    public float growth_random = 0.05f;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //ContactFilter2D cf = new ContactFilter2D();
        //cf.SetLayerMask(treeZoneMask);
        // Placeholder before having an actual map
        for (int i = 0; i < HEIGHT; i++)
        {
            for (int j = 0; j < WIDTH; j++)
            {
                float x = (j - WIDTH / 2.0f) * CELL_WIDTH + Random.Range(-0.3f, 0.3f);
                float y = (i - HEIGHT / 2.0f) * CELL_HEIGHT + Random.Range(-0.3f, 0.3f);
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(x, y), Vector2.zero, 0.0f, treeZoneMask);
                on_map[i, j] = hit.collider != null;
                if(on_map[i, j])
                {
                    cell c = Instantiate(prefabCell, new Vector3(x, y, 0), Quaternion.identity, transform);
                    c.state = Random.Range(0.0f, 0.8f) * Random.Range(0.0f, 0.8f);
                    cells[i, j] = c;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        float[,] nextState = new float[HEIGHT, WIDTH];
        for (int i = 0; i < HEIGHT; i++)
        {
            for (int j = 0; j < WIDTH; j++)
            {
                if (on_map[i, j])
                {
                    float sum = 0;
                    int numNeighbors = 0;
                    for (int k = -1; k <= 1; k++)
                    {
                        for (int l = -1; l <= 1; l++)
                        {
                            if (!(k == 0 && l == 0) && i + k >= 0 && i + k < HEIGHT && j + l >= 0 && j + l < WIDTH && on_map[i + k, j + l])
                            {
                                sum += cells[i + k, j + l].state * cells[i + k, j + l].state;
                                numNeighbors++;
                            }
                        }
                    }
                    nextState[i, j] = treeLogic(cells[i, j].state, numNeighbors, sum);
                }
            }
        }
        for (int i = 0; i < HEIGHT; i++)
        {
            for (int j = 0; j < WIDTH; j++)
            {
                if (on_map[i, j])
                {
                    cells[i, j].state = nextState[i, j];
                }
            }
        }
    }

    //Computes the next state for a given tree based on its neighbors
    float treeLogic(float self, int numNeighbors, float neighborSum)
    {
        float growth = growth_rate * (neighborSum - numNeighbors * (self * self) / growth_factor);
        growth *= Random.Range(1.0f - growth_random, 1.0f + growth_random);
        float result = self + Time.deltaTime * growth;
        result = Mathf.Clamp01(result);
        if(self >= 0.75f && result < 0.75f) return 0.75f; //clamp without automatic decrease
        return result;
    }

    //Computes tree density in a given area
    public float tree_density(Vector2 pos, float radius)
    {
        int minCol = Mathf.Max(0, (int)Mathf.Floor((pos.x - 0.3f - radius) / CELL_WIDTH + WIDTH / 2.0f));
        int maxCol = Mathf.Min(HEIGHT, (int)Mathf.Ceil((pos.x + 0.3f + radius) / CELL_WIDTH + WIDTH / 2.0f));
        int minRow = Mathf.Max(0, (int)Mathf.Floor((pos.y - 0.3f - radius) / CELL_HEIGHT + HEIGHT / 2.0f));
        int maxRow = Mathf.Min(HEIGHT, (int)Mathf.Ceil((pos.y + 0.3f + radius) / CELL_HEIGHT + HEIGHT / 2.0f));
        float sum = 0.0f;
        int total = 0;
       
        for (int i = minRow; i < maxRow; i++)
        {
            for (int j = minCol; j < maxCol; j++)
            {
                if (on_map[i, j] && Vector2.Distance(cells[i, j].transform.position, pos) < radius)
                {
                    sum += cells[i, j].state;
                    total += 1;
                }
            }
        }
        if (total == 0) {
            Debug.Log("total goes down to zero");
            return 0;
        }
        return sum / total;
    }

    public void spread_trees(Vector2 pos, float radius, float delta) {
        int minCol = Mathf.Max(0, (int)Mathf.Floor((pos.x - 0.3f - radius) / CELL_WIDTH + WIDTH / 2.0f));
        int maxCol = Mathf.Min(HEIGHT, (int)Mathf.Ceil((pos.x + 0.3f + radius) / CELL_WIDTH + WIDTH / 2.0f));
        int minRow = Mathf.Max(0, (int)Mathf.Floor((pos.y - 0.3f - radius) / CELL_HEIGHT + HEIGHT / 2.0f));
        int maxRow = Mathf.Min(HEIGHT, (int)Mathf.Ceil((pos.y + 0.3f + radius) / CELL_HEIGHT + HEIGHT / 2.0f));
        float sum = 0.0f;
        for (int i = minRow; i < maxRow; i++)
        {
            for (int j = minCol; j < maxCol; j++)
            {
                if (on_map[i, j] && Vector2.Distance(cells[i, j].transform.position, pos) < radius)
                {
                    sum += cells[i, j].state;
                }
            }
        }
        if (sum >= 1.0f) {
            random_tree(pos, radius, 0);
        }




    }
    public void random_tree(Vector2 pos, float radius, int counter) {
        if (counter > 100000) //to make sure this random growth doesn't recurse forever 
        {
            return;

        }
        else
        {
            Vector2 random_tree_centre = Random.insideUnitCircle * radius + pos;
            if (tree_density(random_tree_centre, 0.01f) < 0.1f)
            {
                growTrees(random_tree_centre, 0.05f, 0.3f);
                

            }
            else
            {
                random_tree(pos, radius, counter + 1);

            }
        }
    
    
    
    }
    //Modifies the value of trees in a given area (clamped)
    public void growTrees(Vector2 pos, float radius, float delta)
    {
        int minCol = Mathf.Max(0, (int)Mathf.Floor((pos.x - 0.3f - radius) / CELL_WIDTH + WIDTH / 2.0f));
        int maxCol = Mathf.Min(HEIGHT, (int)Mathf.Ceil((pos.x + 0.3f + radius) / CELL_WIDTH + WIDTH / 2.0f));
        int minRow = Mathf.Max(0, (int)Mathf.Floor((pos.y - 0.3f - radius) / CELL_HEIGHT + HEIGHT / 2.0f));
        int maxRow = Mathf.Min(HEIGHT, (int)Mathf.Ceil((pos.y + 0.3f + radius) / CELL_HEIGHT + HEIGHT / 2.0f));
        for (int i = minRow; i < maxRow; i++)
        {
            for (int j = minCol; j < maxCol; j++)
            {
                if (on_map[i, j] && Vector2.Distance(cells[i, j].transform.position, pos) < radius) {
          cells[i, j].state = Mathf.Clamp01(cells[i, j].state + delta);
                }
            }
        }
    }

    public float treeLevel()
    {
        float sum = 0;
        int divisor = 0;
        for (int i = 0; i < HEIGHT; i++)
        {
            for (int j = 0; j < WIDTH; j++)
            {
                if (on_map[i, j])
                {
                    sum += cells[i, j].state;
                    divisor++;
                }
            }
        }
        return sum / divisor;
    }

}

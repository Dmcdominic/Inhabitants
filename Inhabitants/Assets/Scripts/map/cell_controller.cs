using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cell_controller : MonoBehaviour
{

    public cell prefabCell;
    public LayerMask treeZoneMask;

    const int WIDTH = 100, HEIGHT = 80;
    const float CELL_WIDTH = 0.2f, CELL_HEIGHT = 0.2f;

    private bool[,] on_map = new bool[HEIGHT, WIDTH];
    protected cell[,] cells = new cell[HEIGHT, WIDTH];

    protected float time_since_tick = 0;
    protected float time_per_tick = 0.5f;

    //Controls speed of entire tree progression system
    public float growth_rate = 0.006f;
    // >1 biases towards growth, <1 biases towards decay
    public float growth_factor = 0.7f;
    // random factor in growth speed
    public float growth_random = 0.05f;

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
                    cell c = Instantiate(prefabCell, new Vector3(x, y, 0), Quaternion.identity);
                    c.state = Random.Range(0.0f, 1.0f) * Random.Range(0.0f, 1.0f);
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
                                sum += cells[i + k, j + l].state;
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

    float treeLogic(float self, int numNeighbors, float neighborSum)
    {
        float growth = growth_rate * (neighborSum - numNeighbors * (self * self) / growth_factor);
        growth *= Random.Range(1.0f - growth_random, 1.0f + growth_random);
        float result = self + Time.deltaTime * growth;
        result = Mathf.Clamp01(result);
        return result;
    }

    public float tree_density(Vector2 pos, float radius)
    {
        //TODO remove inefficient code :P
        float sum = 0.0f;
        for(int i = 0; i < HEIGHT; i++)
        {
            for(int j = 0; j < WIDTH; j++)
            {
                if(Vector2.Distance(cells[i, j].transform.position, pos) < radius)
                {
                    sum += cells[i, j].state;
                }
            }
        }
        return sum / (Mathf.PI * radius * radius);
    }

    public void growTrees(Vector2 pos, float radius, float delta)
    {
        //TODO remove inefficient code :P
        for (int i = 0; i < HEIGHT; i++)
        {
            for (int j = 0; j < WIDTH; j++)
            {
                if (Vector2.Distance(cells[i, j].transform.position, pos) < radius)
                {
                    cells[i, j].state = Mathf.Clamp01(cells[i, j].state + delta);
                }
            }
        }
    }


}

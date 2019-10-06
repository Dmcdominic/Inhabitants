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
        time_since_tick += Time.deltaTime;
        while(time_since_tick > time_per_tick)
        {
            time_since_tick -= time_per_tick;
            Tick();
        }
    }

    void Tick()
    {
        float[,] nextState = new float[HEIGHT, WIDTH];
        for (int i = 0; i < HEIGHT; i++)
        {
            for (int j = 0; j < WIDTH; j++)
            {
                if (on_map[i, j])
                {
                    float sum = 0;
                    for (int k = -1; k <= 1; k++)
                    {
                        for (int l = -1; l <= 1; l++)
                        {
                            if (!(k == 0 && l == 0) && i + k >= 0 && i + k < HEIGHT && j + l >= 0 && j + l < WIDTH && on_map[i + k, j + l])
                            {
                                sum += cells[i + k, j + l].state;
                            }
                        }
                    }
                    if(sum > 7.0f)
                    {
                        nextState[i, j] = 0.0f;
                    } else if(sum > 4.0f)
                    {
                        nextState[i, j] = cells[i, j].state - 0.04f - Random.Range(0.0f, 0.05f);
                    } else if(sum < 1.0f)
                    {
                        nextState[i, j] = 0.0f;
                    } else
                    {
                        nextState[i, j] += cells[i, j].state + (4 - sum) * Random.Range(0.03f, 0.08f) - 0.05f;
                    }
                }
            }
        }
        for(int i = 0; i < HEIGHT; i++)
        {
            for(int j = 0; j < WIDTH; j++)
            {
                if (on_map[i, j])
                {
                    cells[i, j].state = nextState[i, j];
                }
            }
        }
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
                    cells[i, j].state += delta;
                }
            }
        }
    }


}

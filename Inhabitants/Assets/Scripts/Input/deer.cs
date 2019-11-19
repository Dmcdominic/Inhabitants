using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deer : MonoBehaviour
{
    public deer_reps deer_rep;
    public float colony_num = 100f;
    private static float max_colony_num = 100f;
    public float tree_effect = 1f;
    private CircleCollider2D cc;
    public float radius = 1f;
    private deer_reps[] deer_r = new deer_reps[5];

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CircleCollider2D>();
        this.gameObject.name = "deer";
        for (int i = 0; i < 5; i++) { 
            Vector3 pos = Random.insideUnitCircle * radius;
            deer_reps r = Instantiate(deer_rep,transform.position+pos, Quaternion.identity);
            r.radius = radius;
            r.transform.parent = this.transform;
            r.basestartpoint = transform.position;
            deer_r[i] = r;

        
        
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        //parameters
        float tree_decrease_rate = -Time.deltaTime * colony_num * 0.001f;
        float colony_decrease_rate = 10f * Time.deltaTime;

        //slow down growth of trees
        cell_controller.instance.growTrees(transform.position, radius, tree_decrease_rate);

        //if tree density is too small, gradually decrease # of deers
        float deer_d = colony_num * Time.deltaTime * 10;
        float tree_d = cell_controller.instance.tree_density(transform.position, radius);
        if (deer_d > tree_d)
        {
            colony_num -= colony_decrease_rate;
        }

        //check if there are wolves nearby; if so, decrease size of colony 
        Collider2D[] nearObjects = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D obj in nearObjects)
        {
            if (obj != null && obj.name == "wolf")
            {
                colony_num -= colony_decrease_rate;

            }


        }

        float per = colony_num / max_colony_num;
        if (per < 0.2f)
        {
            for (int i = 1; i < 5; i++) {
                if (deer_r[i].transform.gameObject.activeInHierarchy)
                {
                    deer_r[i].transform.gameObject.SetActive(false);
                }
            }
        }
        else if (per < 0.4f)
        {
            for (int i = 2; i < 5; i++)
            {
                if (deer_r[i].transform.gameObject.activeInHierarchy)
                {
                    deer_r[i].transform.gameObject.SetActive(false);
                }
            }
        }

        else if (per < 0.6f)
        {
            for (int i = 3; i < 5; i++)
            {
                if (deer_r[i].transform.gameObject.activeInHierarchy)
                {
                    deer_r[i].transform.gameObject.SetActive(false);
                }
            }
        }

        //if there are no deers left

        if (colony_num <= 0 && this.gameObject != null)
        {
            Destroy(this.gameObject);

        }

    }

}

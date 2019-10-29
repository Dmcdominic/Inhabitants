using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deer : MonoBehaviour
{

    public float colony_num = 100f;
    public float tree_effect = 1f;
    private CircleCollider2D cc;
    public float radius = 1f;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CircleCollider2D>();
        this.gameObject.name = "deer";
        
    }

    // Update is called once per frame
    void Update()
    {
        //parameters
        float tree_decrease_rate = -Time.deltaTime * colony_num * 0.0001f;
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
        //if there are no deers left

        if (colony_num <= 0 && this.gameObject != null)
        {
            Destroy(this.gameObject);

        }

    }

}

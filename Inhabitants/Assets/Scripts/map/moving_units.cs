using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class moving_units : MonoBehaviour
{

    public region target_region;

    public player start_owner;

    public Vector3 start_position;

    public int units;

    public TextMeshPro units_num;

    public SpriteRenderer units_color;

    private float movespeed = 1;
    // Start is called before the first frame update
    void Start()
    {
        units_color = GetComponent<SpriteRenderer>();
        units_color.color= player_data.colors[(int)start_owner];
    }

    // Update is called once per frame
    void Update()
    {
        float ds = Time.deltaTime * movespeed;

        units_num.text = units.ToString();
        units_num.color = player_data.colors[(int)start_owner];
        

        //checks if the # of units moving has gone down to zero. Terminate if so.
        if (units <= 0)
        {

            Destroy(this.gameObject);
        }
        else
        {
            //moves the units
            if (Vector3.Distance(transform.position, target_region.centerpoint) > ds)
            {

                Vector3 direction = (Vector3)target_region.centerpoint - transform.position;
                transform.Translate(direction.normalized * ds);

            }
            else
            {

                //check if the target region is friendly, if yes
                if (target_region.Owner == start_owner)
                {

                    target_region.units += units;


                }
                //if no, check if the target region has more or less units
                else
                {

                    if (target_region.units < units)
                    {

                        target_region.Owner = start_owner;
                        target_region.units = units - target_region.units;

                    }
                    else if (target_region.units == units)
                    {//turn the region neutral

                        target_region.Owner = player.none;
                        target_region.units = 0;

                    }
                    else
                    {
                        target_region.units -= units;


                    }


                }

                Destroy(this.gameObject);


            }
        }
    }
}

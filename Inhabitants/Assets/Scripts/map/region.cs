using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class region : MonoBehaviour
{

    // Public fields
    public int area;
    public moving_units movingUnits;

    // Public variables
    public int units
    {
        get { return Mathf.FloorToInt(units_real); }
        set { units_real = value; }
    }
    protected float units_real;

    [HideInInspector]
    public player Owner = player.none;

    // Components
    public TextMeshPro unit_text;


    // Start is called before the first frame update
    void Start()
    {
        units_real = area;
    }

    // Update is called once per frame
    void Update()
    {
        units_real += growth_rate() * Time.deltaTime;
        unit_text.text = units.ToString();
        unit_text.color = player_data.colors[(int)Owner];
    }

    // Determine the growth rate of this region
    public float growth_rate()
    {
        if (Owner == player.none)
        {
            return 0;
        }
        // return units_real / 20f;
        return Mathf.Sqrt(units_real) / 5f;
    }
    public void send_units(region region_target)
    { //takes in region; units/2; prefab
        int unit_to_send = units / 2;

        if (unit_to_send == 0)
        {
            //throw error

        }
        else
        { //instantiate the moving units first

            moving_units x = Instantiate(movingUnits, transform.position, movingUnits.transform.rotation);
            x.target_region = region_target;
            x.start_owner = Owner;
            x.start_position = transform.position;
            x.units = unit_to_send;
            //assign the proper parameters
        }
    }
}

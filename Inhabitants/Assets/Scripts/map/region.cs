using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class region : MonoBehaviour {

    // Public fields
    public int area;

    // Public variables
    public int units {
        get { return Mathf.FloorToInt(units_real); }
        set { units_real = value; }
    }
    protected float units_real;

    [HideInInspector]
    public player Owner = player.none;

    // Components
    public TextMeshPro unit_text;


    // Start is called before the first frame update
    void Start() {
        units_real = area;
    }

    // Update is called once per frame
    void Update() {
        units_real += growth_rate() * Time.deltaTime;
        unit_text.text = units.ToString();
        unit_text.color = player_data.colors[(int)Owner];
    }

    // Determine the growth rate of this region
    public float growth_rate() {
        if (Owner == player.none) {
            return 0;
        }
        // return units_real / 20f;
        return Mathf.Sqrt(units_real) / 5f;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class status_controller : MonoBehaviour
{
    public static status_controller instance;

    // 0-1, currently representing the percentage of occupied cities
    public static float industryLevel;

    public Image trees, air, temperature;

    public float airSpeed = 0.01f, tempSpeed = 0.01f, cityImpact = 0.001f;

    // Higher is "better" for the Earth
    public float treeLevel = 0.5f, airLevel = 0.5f, temperatureLevel = 0.5f;
    
    const float disasterMaxTemp = 0.5f, disasterCheckInterval = 5f;
    const float thermometer_min_fill = 0.33f;

    region[] regions;

    private float disaster_accum;


    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update 
    void Start()
    {
      regions = FindObjectsOfType<region>();
    }

    // Update is called once per frame 
    void Update()
    {
        industryLevel = 0;
        for(int i = 0; i < regions.Length; i++)
        {
            region r = regions[i];
            if(r.City.Policy == policy.industry)
            {
                industryLevel += 1.0f;
            } else if(r.City.Policy == policy.neutral)
            {
                industryLevel += 0.3f;
            }
        }

        industryLevel = industryLevel / (float)regions.Length;

        treeLevel = cell_controller.instance.treeLevel();
        airLevel += airSpeed * Time.deltaTime * (treeLevel - industryLevel * cityImpact);
        airLevel = Mathf.Clamp01(airLevel);
        temperatureLevel += (airLevel - 0.5f) * tempSpeed * Time.deltaTime;
        temperatureLevel = Mathf.Clamp01(temperatureLevel);

        //trees.fillAmount = treeLevel;
        //air.fillAmount = airLevel;
        //temperature.fillAmount = temperatureLevel;
        temperature.fillAmount = ((1f - temperatureLevel) * (1f - thermometer_min_fill)) + thermometer_min_fill;

    //Checks for a disaster once every second
    //Disasters are more likely when the temperature level is low
    for (disaster_accum += Time.deltaTime; disaster_accum >= disasterCheckInterval; disaster_accum -= disasterCheckInterval) {
      if (temperatureLevel < 0.5f) {
        float randResult = Mathf.Pow(Random.Range(0f, 1f), 15);
        if (randResult > temperatureLevel) {
          disaster.setDisaster(true);
        }
      }
    }

  }
}

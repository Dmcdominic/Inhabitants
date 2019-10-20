﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class status_controller : MonoBehaviour
{
    public static status_controller instance;

    public Image trees, air, temperature;

    public float airSpeed = 0.01f, tempSpeed = 0.01f;

    float treeLevel = 0.5f, airLevel = 0.5f, temperatureLevel = 0.5f;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update 
    void Start()
    {

    }

    // Update is called once per frame 
    void Update()
    {
        treeLevel = cell_controller.instance.treeLevel();
        airLevel = Mathf.Clamp(airLevel + (treeLevel - 0.5f) * airSpeed * Time.deltaTime, 0, 1);
        temperatureLevel = Mathf.Clamp(temperatureLevel + (airLevel - 0.5f) * tempSpeed * Time.deltaTime, 0, 1);

        trees.fillAmount = treeLevel;
        air.fillAmount = airLevel;
        temperature.fillAmount = temperatureLevel;
    }
}

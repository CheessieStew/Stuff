using UnityEngine;
using System.Collections;

public class LampBehaviour : MonoBehaviour
{
    public float OffIntensity;
    public float OnIntensity;

    public bool IsOn;
    float Timer;
    Color color;
    Color dimColor;

    //Init
    void Start()
    {
        color = GetComponentInChildren<Light>().color;
        GetComponent<MeshRenderer>().material.SetColor("_SpecColor", color / 4);
        if (IsOn) TurnOn();
        else TurnOff();
    }

    //turns the light on for a while
    public void Flash(float time)
    {
        if (IsOn)
            return;
        Timer = time;
        TurnOn();
    }

    public void TurnOn()
    {
        IsOn = true;
        GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", color);
        foreach(Light light in GetComponentsInChildren<Light>())
        {
            light.intensity = OnIntensity;
        }
        
    }

    public void TurnOff()
    {
        IsOn = false;
        GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", color/16);
        foreach (Light light in GetComponentsInChildren<Light>())
        {
            light.intensity = OffIntensity;
        }
    }

    void Update()
    {
        if (Timer > 0)
        {
            Timer -= Time.deltaTime;
            if (Timer <= 0)
            {
                if (IsOn) TurnOff();
                else TurnOn();
            }
        }
    }
}

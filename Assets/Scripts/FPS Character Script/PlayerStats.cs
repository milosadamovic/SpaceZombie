using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private Image health_Stats;

   

    public void Display_HealthStats(float healthValue)
    {
        healthValue /= 100;

        health_Stats.fillAmount = healthValue;
    }

   

}

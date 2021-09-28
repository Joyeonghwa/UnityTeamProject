using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private int hpGauge;
    public int HPGauge { get { return hpGauge; } }
    private int shieldGauge;
    public int ShieldGauge { get { return shieldGauge; } }

    void Start()
    {
        hpGauge = 100;
        shieldGauge = 0;
    }

    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaseyAttack : MonoBehaviour
{
   
    void Start()
    {
        
    }
    
    void Update()
    {
        MouseL_Attack();
    }

    void MouseL_Attack()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Debug.Log("MoseL");
        }
    }
}

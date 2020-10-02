using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCubeID : MonoBehaviour
{
    public string UpdateID =" "; 
    public string networkID;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerMaterial(Color C)
    {
        Renderer PlayerRender = GetComponent<Renderer>();
        if (PlayerRender)
        {
            PlayerRender.material.SetColor("_Color", C);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartoonShaderScript : MonoBehaviour
{
    [SerializeField]
    private Material mat = null;

    void Update()
    {
        mat.SetVector("_ToonLightDirection", -this.transform.forward);    
    }

}

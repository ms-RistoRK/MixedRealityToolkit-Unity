using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnifiedMaterialManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject backplate, backglow, frontplate;
        Material backplateMaterial, backglowMaterial, frontplateMaterial;
        foreach (Transform child in transform)
        {
            if (child.name.Equals("Backplate"))
            {
                backplate = child.gameObject;
                backplateMaterial = backplate.GetComponent<RawImage>().material;
                backplateMaterial.SetInteger("_SubshaderToRun", 0);
            }
            else if (child.name.Equals("Backglow"))
            {
                backglow = child.gameObject;
                backglowMaterial = backglow.GetComponent<RawImage>().material;
                backglowMaterial.SetInteger("_SubshaderToRun", 1);
            }
            else if (child.name.Equals("Frontplate"))
            {
                frontplate = child.gameObject;
                frontplateMaterial = frontplate.GetComponent<RawImage>().material;
                frontplateMaterial.SetInteger("_SubshaderToRun", 2);
            }
        }

    }

    private void Update()
    {
        //CanvasRenderer canvasRenderer = 
    }
}

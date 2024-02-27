using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.UI;

public class PlateMaterialManager : MonoBehaviour
{
    public enum plateType
    {
        BACKPLATE,
        BACKGLOW,
        FRONTPLATE,
        NONE
    }

    public plateType currentPlateType = plateType.NONE;

    private Material currentPlateMaterial = null;

    private void Start()
    {
        if (name.Equals("Backplate"))
        {
            currentPlateType = plateType.BACKPLATE;
        }
        else if (name.Equals("Backglow"))
        {
            currentPlateType = plateType.BACKGLOW;
        }
        else if (name.Equals("Frontplate"))
        {
            currentPlateType = plateType.FRONTPLATE;
        }

        currentPlateMaterial = GetComponent<RawImage>().material;

        //MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        //propertyBlock.SetInteger("_SubshaderToRun", (int)currentPlateType);
        //CanvasRenderer canvasRenderer = GetComponent<CanvasRenderer>();
        ////Because plates us RawImage that use CanvasRenderer
        //canvasRenderer.SetPropertyBlock(propertyBlock);
    }

    void Update()
    {
        switch (currentPlateType)
        {
            case plateType.BACKPLATE:
                currentPlateMaterial.SetInteger("_SubshaderToRun", 0);
                break;
            case plateType.BACKGLOW:
                currentPlateMaterial.SetInteger("_SubshaderToRun", 1);
                break;
            case plateType.FRONTPLATE:
                currentPlateMaterial.SetInteger("_SubshaderToRun", 2);
                break;
        }
    }
}

using UnityEngine;
using System.Collections;
using MixedReality.Toolkit.UX;
using UnityEngine.UI;


namespace TMPro.Examples
{
    
    public class TestScript : MonoBehaviour
    {
        [SerializeField]
        private GameObject testButton;

        void Start()
        {
            var testButtonVisualizer = testButton.GetComponent<StateVisualizer>();
            var childrenRawImages = testButtonVisualizer.GetComponentsInChildren<RawImage>();
            var backPlateRawImage = childrenRawImages[0];
            backPlateRawImage.color = new Color32(0, 255, 0, 255);
        }


        void Update()
        {
        }

    }
}

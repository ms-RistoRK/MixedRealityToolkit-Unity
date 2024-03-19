using UnityEngine;
using System.Collections;
using MixedReality.Toolkit.UX;
using UnityEngine.UI;


namespace TMPro.Examples
{
    
    public class SimpleScript : MonoBehaviour
    {

        private TextMeshPro m_textMeshPro;
        //private TMP_FontAsset m_FontAsset;

        private const string label = "The <#0050FF>count is: </color>{0:2}";
        private float m_frame;

        [SerializeField]
        private GameObject testButton;

        void Start()
        {
            // Add new TextMesh Pro Component
            m_textMeshPro = gameObject.AddComponent<TextMeshPro>();

            m_textMeshPro.autoSizeTextContainer = true;

            // Set various font settings.
            m_textMeshPro.fontSize = 48;

            m_textMeshPro.alignment = TextAlignmentOptions.Center;
            
            m_textMeshPro.enableWordWrapping = false;

            var testButtonVisualizer = testButton.GetComponent<StateVisualizer>();
            var childrenRawImages = testButtonVisualizer.GetComponentsInChildren<RawImage>();
            var backPlateRawImage = childrenRawImages[0];
            backPlateRawImage.color = new Color32(0, 255, 0, 255);
        }


        void Update()
        {
            m_textMeshPro.SetText(label, m_frame % 1000);
            m_frame += 1 * Time.deltaTime;
        }

    }
}

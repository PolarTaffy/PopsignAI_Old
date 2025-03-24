using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResizeCapturedWindow : MonoBehaviour
{
    [SerializeField] GameObject screen;
    // Start is called before the first frame update
    void Update()
    {
        Vector2 size = screen.GetComponent<RawImage>().rectTransform.sizeDelta;
        GetComponent<Image>().rectTransform.sizeDelta = new Vector2(size[0], size[1]);
    }

}

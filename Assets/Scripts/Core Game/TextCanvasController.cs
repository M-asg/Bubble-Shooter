using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextCanvasController : MonoBehaviour
{
    TMP_Text text;
    private void Awake()
    {
        text = this.transform.GetChild(0).transform.gameObject.GetComponent<TMP_Text>();
    }

    public void SetTextValue(float value)
    {
        text.text = value.ToString();
    }

    public void DestroyItSelf()
    {
        Destroy(this.gameObject);
    }

}

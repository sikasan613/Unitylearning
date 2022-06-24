using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextHighlight : MonoBehaviour
{
    private float num = Mathf.PI;

    // Update is called once per frame
    void Update()
    {
        TextMeshProUGUI tmPro = gameObject.GetComponent<TextMeshProUGUI>();
        Material material = tmPro.fontMaterial;
        material.SetFloat("_OutlineWidth", Mathf.Abs(Mathf.Sin(num)) * 2 / 5);
        num += Time.deltaTime * 2;
    }
}

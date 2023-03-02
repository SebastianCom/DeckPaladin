using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class CustomCursor : MonoBehaviour
{

    Vector3 ScreenPos;
    Vector3 MousePos;

    private void Start()
    {

    }
    void Update()
    {
        if (Application.isFocused)
            Cursor.visible = false;
        else
            Cursor.visible = true;

        ScreenPos = Input.mousePosition;

        ScreenPos.z = Camera.main.nearClipPlane + 2.0f;

        MousePos = Camera.main.ScreenToWorldPoint(ScreenPos);

        transform.position = MousePos;
    }
}

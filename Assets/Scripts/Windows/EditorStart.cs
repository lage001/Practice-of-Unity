using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorStart : MonoBehaviour
{
    void Update()
    {
        // 按键打开窗口
        if (Input.GetKey(KeyCode.A))
        {
            MenuWindow.OpenWindow();
        }
        if (Input.GetKey(KeyCode.B))
        {
            EditorWindow.OpenWindow();
        }
    }
}

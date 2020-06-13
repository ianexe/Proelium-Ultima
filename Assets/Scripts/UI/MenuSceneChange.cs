using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MenuSceneChange : MonoBehaviour
{
    public GameObject to_load;
    public Button to_select;

    public void ChangeScene()
    {
        if (to_load != null)
            to_load.SetActive(true);
        if (to_select != null)
            to_select.Select();
        transform.parent.gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SidebarIcon : MonoBehaviour {

    public GameObject sidebar;
    public GameObject browser;
    public string _name {
        get {
            return __name;
        } set {
            __name = value;
            GetComponentInChildren<Text>().text = __name;
        }
    }
    public string path;
    public bool isDevice;
    private string __name;
    private bool activated = false;

    public void Activate(bool go = true) {
        Debug.Log("Clicked");
        if (!activated) {
            Debug.Log("Deactivating all");
            sidebar.GetComponent<Sidebar>().Deactivate();
            Debug.Log("Activating self");
            GetComponent<Image>().color = new Color32(64, 128, 224, 216);
            GetComponentInChildren<Text>().color = new Color32(255, 255, 255, 255);
            if (go) {
                Debug.Log("Going to " + path);
                browser.GetComponent<Browser>().Goto(path);
            }
            activated = true;
        }
    }

    public void Deactivate() {
        if (activated) {
            GetComponent<Image>().color = new Color32(64, 128, 224, 0);
            GetComponentInChildren<Text>().color = new Color32(0, 0, 0, 255);
            activated = false;
        }
    }

    void Start() {
        sidebar = transform.parent.parent.gameObject;
        GetComponentInChildren<Text>().text = _name;
    }

}

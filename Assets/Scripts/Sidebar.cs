using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sidebar : MonoBehaviour {

    public Transform sidebarIconPrefab;
    public Transform favorites;
    public Transform devices;
    public GameObject browser;
    private List<GameObject> icons = new List<GameObject>();

    public void AddIcon(string name, string path, bool isDevice) {
        if (path.EndsWith("/") && path.Length > 4 && path != "/") path = path.Remove(path.Length - 1);
        GameObject icon = Instantiate(sidebarIconPrefab, isDevice ? devices : favorites).gameObject;
        icon.GetComponent<SidebarIcon>()._name = name;
        icon.GetComponent<SidebarIcon>().path = path;
        icon.GetComponent<SidebarIcon>().isDevice = isDevice;
        icon.GetComponent<SidebarIcon>().browser = browser;
        icons.Add(icon);
        Save();
    }

    public void Deactivate() {
        foreach (GameObject icon in icons) icon.GetComponent<SidebarIcon>().Deactivate();
    }

    public void CheckForPath(string path) {
        if (path.EndsWith("/") && path.Length > 4 && path != "/") path = path.Remove(path.Length - 1);
        foreach (GameObject icon in icons) {
            if (icon.GetComponent<SidebarIcon>().path == path) icon.GetComponent<SidebarIcon>().Activate(false);
            else icon.GetComponent<SidebarIcon>().Deactivate();
        }
    }

	// Use this for initialization
	void Start () {
        string[] prefs = PlayerPrefs.GetString("SidebarIcons").Split('|');
        string[] favoritestr = prefs[0].Split(';');
        string[] devicestr = prefs[1].Split(';');
        foreach (string item in favoritestr) {
            string[] i = item.Split('=');
            AddIcon(i[0], i[1], false);
        }
        foreach (string item in devicestr) {
            string[] i = item.Split('=');
            AddIcon(i[0], i[1], true);
        }
    }

    void Save() {
        string favoritestr = "";
        string devicestr = "";
        foreach (GameObject icon in icons) {
            if (icon.GetComponent<SidebarIcon>().isDevice) devicestr += (devicestr == "" ? "" : ";") + icon.GetComponent<SidebarIcon>()._name + "=" + icon.GetComponent<SidebarIcon>().path;
            else favoritestr += (favoritestr == "" ? "" : ";") + icon.GetComponent<SidebarIcon>()._name + "=" + icon.GetComponent<SidebarIcon>().path;
        }
        PlayerPrefs.SetString("SidebarIcons", favoritestr + "|" + devicestr);
        PlayerPrefs.Save();
    }

    void OnApplicationQuit() {
        Save();
    }

}

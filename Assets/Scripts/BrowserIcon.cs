using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrowserIcon : MonoBehaviour {

    public BrowserSortable info;
    public GameObject icon;
    public GameObject title;
    public GameObject size;
    public GameObject dateModified;
    public Sprite iconSprite;
    public string basePath;
    public bool isClicked {
        get {
            return _isClicked;
        } set {
            _isClicked = value;
            UpdateIcon();
        }
    }
    private System.DateTime lastClick;
    private bool _isClicked;

    public void UpdateIcon() {
        if (isClicked) {
            GetComponent<Image>().color = new Color32(64, 128, 224, 216);
            title.GetComponent<Text>().color = new Color32(255, 255, 255, 255);
            if (size != null) size.GetComponent<Text>().color = new Color32(255, 255, 255, 255);
            if (dateModified != null) dateModified.GetComponent<Text>().color = new Color32(255, 255, 255, 255);
        } else {
            GetComponent<Image>().color = new Color32(255, 255, 255, 0);
            title.GetComponent<Text>().color = new Color32(0, 0, 0, 255);
            if (size != null) size.GetComponent<Text>().color = new Color32(0, 0, 0, 255);
            if (dateModified != null) dateModified.GetComponent<Text>().color = new Color32(0, 0, 0, 255);
        }
        icon.GetComponent<Image>().sprite = iconSprite;
        title.GetComponent<Text>().text = info.Name;
        if (size != null) {
            if (info.IsFolder) size.GetComponent<Text>().text = "--";
            else size.GetComponent<Text>().text = FileClassifier.GetFileSize(info.Size);
        }
        if (dateModified != null) dateModified.GetComponent<Text>().text = info.ModificationDate.ToString();
    }

    public void OnClick() {
        if (!isClicked) {
            transform.parent.gameObject.GetComponent<Browser>().UnclickAll();
            isClicked = true;
        } else if (isClicked && System.DateTime.Now <= lastClick.Add(new System.TimeSpan(0, 0, 0, 0, 333))) {

            if (info.IsFolder)
                transform.parent.gameObject.GetComponent<Browser>().Goto(basePath + (basePath.EndsWith("/") ? "" : "/") + info.Name);
            else if (System.Environment.OSVersion.Platform != System.PlatformID.Unix)
                System.Diagnostics.Process.Start(basePath + (basePath.EndsWith("/") ? "" : "/") + info.Name);
        }
        lastClick = System.DateTime.Now;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathBar : MonoBehaviour {

    public GameObject browser;
    public GameObject backButton;
    public GameObject forwardButton;
    public GameObject pathBox;
    public GameObject statusBar;
    public GameObject sidebar;
    string lastURL = "null";
    Stack<string> history = new Stack<string>(20);
    Stack<string> forwardHistory = new Stack<string>(20);

    private void ToggleBackButton(bool value) {
        if (value) {
            backButton.GetComponent<Button>().enabled = true;
            backButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        } else { 
            backButton.GetComponent<Button>().enabled = false;
            backButton.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
        }
    }

    private void ToggleForwardButton(bool value) {
        if (value) {
            forwardButton.GetComponent<Button>().enabled = true;
            forwardButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        } else {
            forwardButton.GetComponent<Button>().enabled = false;
            forwardButton.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
        }
    }

    public void Back() {
        if (history.Count > 0) {
            string url = history.Pop();
            forwardHistory.Push(lastURL);
            lastURL = url;
            ToggleForwardButton(true);
            browser.GetComponent<Browser>().Goto(url, false);
            statusBar.GetComponent<StatusBar>().UpdateText(url);
            sidebar.GetComponent<Sidebar>().CheckForPath(url);
            pathBox.GetComponent<InputField>().text = url;
            ToggleBackButton(history.Count > 0);
        }
    }

    public void Forward() {
        if (forwardHistory.Count > 0) {
            string url = forwardHistory.Pop();
            history.Push(lastURL);
            lastURL = url;
            ToggleBackButton(true);
            browser.GetComponent<Browser>().Goto(url, false);
            statusBar.GetComponent<StatusBar>().UpdateText(url);
            sidebar.GetComponent<Sidebar>().CheckForPath(url);
            pathBox.GetComponent<InputField>().text = url;
            ToggleForwardButton(forwardHistory.Count > 0);
        }
    }

    public void Go() {
        if (!System.IO.Directory.Exists(pathBox.GetComponent<InputField>().text)) return;
        browser.GetComponent<Browser>().Goto(pathBox.GetComponent<InputField>().text);
    }

    public void DidChangeURL(string url) {
        Debug.Log("Navigated to " + url);
        forwardHistory.Clear();
        if (lastURL != "null") history.Push(lastURL);
        lastURL = url;
        ToggleBackButton(history.Count > 0);
        pathBox.GetComponent<InputField>().text = url;
        statusBar.GetComponent<StatusBar>().UpdateText(url);
        sidebar.GetComponent<Sidebar>().CheckForPath(url);
    }

    private void Start() {
        ToggleBackButton(false);
        ToggleForwardButton(false);
    }

}

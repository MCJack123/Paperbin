using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public struct BrowserSortable {
    public string Name;
    public string Extension;
    public bool IsFolder;
    public long Size;
    public System.DateTime ModificationDate;

    public BrowserSortable(DirectoryInfo dir) {
        Name = dir.Name;
        Extension = dir.Extension;
        IsFolder = true;
        Size = long.MaxValue;
        ModificationDate = dir.LastWriteTime;
    }

    public BrowserSortable(FileInfo file) {
        Name = file.Name;
        Extension = file.Extension;
        IsFolder = false;
        Size = file.Length;
        ModificationDate = file.LastWriteTime;
    }
}

public enum SortType {
    ByName,
    ByType,
    BySize,
    ByModificationDate,
    ByNameWindows
}

public class Browser : MonoBehaviour {

    public Transform fileIconPrefab;
    public GameObject pathBar;
    public GameObject scrollView;
    public SpriteAtlas iconSprites;
    private static bool started = false;
    private static List<SortType> typemap = new List<SortType>() {
        SortType.ByName,
        SortType.ByType,
        SortType.BySize,
        SortType.ByModificationDate,
        SortType.ByNameWindows
    };
    private SortType sortType;
    //private string currentPath;
    private List<GameObject> icons = new List<GameObject>();

    public static class SortEntries {
        private static int CompareExtensions(BrowserSortable lhs, BrowserSortable rhs) {
            if (lhs.IsFolder && rhs.IsFolder) return lhs.Name.CompareTo(rhs.Name);
            else if (lhs.IsFolder && !rhs.IsFolder) return -1;
            else if (!lhs.IsFolder && rhs.IsFolder) return 1;
            else return lhs.Extension.CompareTo(rhs.Extension);
        }
        private static int CompareNamesWindows(BrowserSortable lhs, BrowserSortable rhs) {
            if (lhs.IsFolder ^ rhs.IsFolder) return lhs.IsFolder ? -1 : 1;
            else return lhs.Name.CompareTo(rhs.Name);
        }
        public static void ByName(List<BrowserSortable> list) {
            list.Sort((lhs, rhs) => lhs.Name.CompareTo(rhs.Name));
        }
        public static void ByType(List<BrowserSortable> list) {
            list.Sort(CompareExtensions);
        }
        public static void BySize(List<BrowserSortable> list) {
            list.Sort((lhs, rhs) => rhs.Size.CompareTo(lhs.Size));
        }
        public static void ByModificationDate(List<BrowserSortable> list) {
            list.Sort((lhs, rhs) => rhs.ModificationDate.CompareTo(lhs.ModificationDate));
        }
        public static void ByNameWindows(List<BrowserSortable> list) {
            list.Sort(CompareNamesWindows);
        }
        public static void WithType(SortType type, List<BrowserSortable> list) {
            switch (type) {
                case SortType.ByName:
                    ByName(list);
                    break;
                case SortType.ByType:
                    ByType(list);
                    break;
                case SortType.BySize:
                    BySize(list);
                    break;
                case SortType.ByModificationDate:
                    ByModificationDate(list);
                    break;
                case SortType.ByNameWindows:
                    ByNameWindows(list);
                    break;
            }
        }
    }

    public void Goto(string url, bool changeURL = true) {
        Debug.Log("Navigating to " + url);
        DirectoryInfo info = new DirectoryInfo(url);
        try {
            info.GetFiles();
        } catch (System.UnauthorizedAccessException) {
            Debug.LogWarning("You do not have permission to access this directory.");
            return;
        }
        //Debug.Log("Updating path bar");
        if (changeURL) pathBar.GetComponent<PathBar>().DidChangeURL(url);
        //Debug.Log("Clearing icons");
        foreach (GameObject i in icons) Destroy(i);
        icons.Clear();
        List<BrowserSortable> iconlist = new List<BrowserSortable>();
        //Debug.Log("Retrieving files");
        foreach (FileInfo file in info.GetFiles()) iconlist.Add(new BrowserSortable(file));
        //Debug.Log("Retrieving directories");
        foreach (DirectoryInfo file in info.GetDirectories()) iconlist.Add(new BrowserSortable(file));
        //Debug.Log("Sorting");
        SortEntries.WithType(sortType, iconlist);
        //Debug.Log("Creating entries");
        foreach (BrowserSortable icon in iconlist) {
            //Debug.Log("Creating " + icon.Name);
            GameObject obj = Instantiate(fileIconPrefab, transform).gameObject;
            obj.GetComponent<BrowserIcon>().info = icon;
            if (icon.IsFolder) obj.GetComponent<BrowserIcon>().iconSprite = iconSprites.GetSprite("Folder");
            else obj.GetComponent<BrowserIcon>().iconSprite = iconSprites.GetSprite(FileClassifier.FileTypeNames[FileClassifier.GetFileType(icon.Extension)]);
            obj.GetComponent<BrowserIcon>().basePath = url;
            obj.GetComponent<BrowserIcon>().UpdateIcon();
            icons.Add(obj);
        }
        scrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
        //currentPath = url;
    }

    public void UnclickAll() {
        foreach (GameObject icon in icons) icon.GetComponent<BrowserIcon>().isClicked = false;
    }

	// Use this for initialization
	void Start () {
        sortType = typemap[PlayerPrefs.GetInt("SortType")];
        if (PlayerPrefs.GetInt("ViewType") == 1 && !started) { started = true; SceneSwitcher.SwitchScene(); }
        started = true;
    }
	
}

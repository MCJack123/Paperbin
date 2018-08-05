using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using System.IO;

public class StatusBar : MonoBehaviour {

	public void UpdateText(string url) {
        DirectoryInfo d = new DirectoryInfo(url);
        int count = d.GetFiles().Length + d.GetDirectories().Length;
        string dfout = "";
        if (System.Environment.OSVersion.Platform == System.PlatformID.Unix || System.Environment.OSVersion.Platform == System.PlatformID.MacOSX) {
            Process df = new Process();
            df.StartInfo.FileName = "/bin/bash";
            df.StartInfo.Arguments = "-c \"df --human --output=avail '" + url + "' | grep -v Avail\"";
            df.StartInfo.RedirectStandardOutput = true;
            df.StartInfo.UseShellExecute = false;
            df.Start();
            dfout = df.StandardOutput.ReadToEnd();
            df.WaitForExit();
        }
        GetComponent<Text>().text = count + " items" + (dfout != "" ? "," + dfout + "B available" : "");
    }

}

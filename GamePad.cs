using HidSharp;
using HidSharp.Utility;
using System.Diagnostics;

namespace Gamepad;

public class GamePad
{
    #region Fields
    private readonly DeviceList list = DeviceList.Local;
    private readonly Thread readingGamePadThread = new(new ThreadStart(ReadingGamepad));

    private readonly List<HidDevice> devices = [];
    private HidDevice device = null!;
    private static HidStream stream = null!;
    private static bool listeningGamepad = false;
    #endregion

    public GamePad()
    {
        HidSharpDiagnostics.EnableTracing = true;
        HidSharpDiagnostics.PerformStrictChecks = true;

        list.Changed += (sender, e) => ResetDevices();
        ResetDevices();
    }

    #region public methods
    public bool StartListening()
    {
        if (stream == null) return false;
        listeningGamepad = true;
        readingGamePadThread.Start();
        return true;
    }

    public static void StopListening()
    {
        listeningGamepad = false;
    }
    #endregion


    #region Private methods
    private void ResetDevices()
    {
        devices.Clear();
        foreach (HidDevice hid in list.GetHidDevices())
        {
            try
            {
                string manufacturer = hid.GetManufacturer();
                devices.Add(hid);
                if (manufacturer == "FrSky")
                {
                    device = hid;
                    if (!device.TryOpen(out stream)) throw new Exception("device opening failed.");
                }
            }
            catch { }
        }
    }

    private static void ReadingGamepad()
    {
        while (listeningGamepad)
        {
            var dataFromGamepad = stream.Read();
            string hex = BitConverter.ToString(dataFromGamepad).Replace("-", string.Empty);
            Debug.WriteLine(hex);

            Thread.Sleep(2000);
        }
    }
    #endregion
}

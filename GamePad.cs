using CommunityToolkit.Mvvm.Messaging;
using SDL2;
using System.Diagnostics;

namespace Gamepad;

public class GamePad
{
    #region Fields
    private readonly Thread readingGamePadThread = new(new ThreadStart(ReadingGamepad));
    private static IntPtr myJoystick;

    private static bool listeningGamepad = false;

    private static short[] Axes;
    #endregion

    public GamePad()
    {
        _ = SDL.SDL_Init(SDL.SDL_INIT_JOYSTICK);
        myJoystick = SDL.SDL_JoystickOpen(0);
    }

    #region public methods
    public bool StartListening()
    {
        if (myJoystick == IntPtr.Zero) return false;
        listeningGamepad = true;
        readingGamePadThread.Start();
        return true;
    }

    public static void StopListening()
    {
        listeningGamepad = false;
        SDL.SDL_JoystickClose(myJoystick);
    }

    public static string GetJoystickName()
    {
        string joystickName = string.Empty;
        if (myJoystick != IntPtr.Zero) joystickName = SDL.SDL_JoystickName(myJoystick);
        return joystickName;
    }

    public static short GetAxes(int axes) => Axes[axes];
    #endregion


    #region Private methods
    private static void ReadingGamepad()
    {
        int numAxes = SDL.SDL_JoystickNumAxes(myJoystick);
        Axes = new short[numAxes];

        int numButtons = SDL.SDL_JoystickNumButtons(myJoystick);

        while (listeningGamepad)
        {
            SDL.SDL_PumpEvents();

            for (int i = 0; i < numAxes; i++)
            {
                Axes[i] = SDL.SDL_JoystickGetAxis(myJoystick, i);
            }

            // Loop over each buttons
            //for (int i = 0; i < numButtons; i++)
            //{
            //    // Get the value of the axis
            //    byte buttonValue = SDL.SDL_JoystickGetButton(myJoystick, i);

            //    // Print the value of the axis
            //    Debug.WriteLine("Button " + i + " value: " + buttonValue);
            //}

            _ = WeakReferenceMessenger.Default.Send(Axes);

            Thread.Sleep(100);
        }
    }


    #endregion
}

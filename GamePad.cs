using SDL2;

namespace Gamepad;

public class GamePad
{
    #region Fields
    private readonly Thread readingGamePadThread = new(new ThreadStart(ReadingGamepad));
    private static IntPtr myJoystick;

    private static bool listeningGamepad = false;
    #endregion

    public GamePad()
    {
        ResetDevices();

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

    #endregion


    #region Private methods
    private static void ResetDevices()
    {
        _ = SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING);
        myJoystick = SDL.SDL_JoystickOpen(0);
    }

    private static void ReadingGamepad()
    {
        while (listeningGamepad)
        {
            

            Thread.Sleep(2000);
        }
    }


    #endregion
}

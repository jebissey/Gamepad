using CommunityToolkit.Mvvm.Messaging;
using SDL2;

namespace Gamepad;

public class GamePad
{
    #region Fields
    private readonly Thread readingGamePadThread = new(new ThreadStart(ReadingGamepad));
    private const int readingGamePadSleep = 100;
    private static IntPtr joystick;
    private static bool listeningGamepad = false;
    private static short[] axes = null!;
    private static int numAxes;
    private static byte[] buttons = null!;
    private static int numButtons;
    #endregion

    public GamePad()
    {
        _ = SDL.SDL_Init(SDL.SDL_INIT_JOYSTICK);
        joystick = SDL.SDL_JoystickOpen(0);
    }

    #region public methods
    public bool StartListening()
    {
        if (joystick == IntPtr.Zero) return false;

        numAxes = SDL.SDL_JoystickNumAxes(joystick);
        axes = new short[numAxes];

        numButtons = SDL.SDL_JoystickNumButtons(joystick);
        buttons = new byte[numButtons];

        listeningGamepad = true;
        readingGamePadThread.Start();
        return true;
    }

    public static void StopListening()
    {
        listeningGamepad = false;
    }

    public static string GetJoystickName()
    {
        string joystickName = string.Empty;
        if (joystick != IntPtr.Zero) joystickName = SDL.SDL_JoystickName(joystick);
        return joystickName;
    }

    public static short GetAxis(int axis) => axes != null && axis < axes.Length ? axes[axis] : short.MinValue;
    public static short GetButton(int button) => buttons != null && button < buttons.Length ? buttons[button] : byte.MinValue;
    #endregion


    #region Private methods
    private static void ReadingGamepad()
    {
        while (listeningGamepad)
        {
            SDL.SDL_PumpEvents();

            bool newAxesValues = false;
            for (int i = 0; i < numAxes; i++)
            {
                short currentValue = SDL.SDL_JoystickGetAxis(joystick, i);
                if (currentValue != axes[i])
                {
                    axes[i] = currentValue;
                    newAxesValues = true;
                }
            }
            if(newAxesValues) _ = WeakReferenceMessenger.Default.Send(axes);

            bool newButtonsValues = false;
            for (int i = 0; i < numButtons; i++)
            {
                byte currentValue = SDL.SDL_JoystickGetButton(joystick, i);
                if (currentValue != buttons[i])
                {
                    buttons[i] = currentValue;
                    newButtonsValues = true;
                }
            }
            if (newButtonsValues) _ = WeakReferenceMessenger.Default.Send(buttons);

            Thread.Sleep(readingGamePadSleep);
        }
        SDL.SDL_JoystickClose(joystick);
    }
    #endregion
}

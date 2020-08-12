using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput.Native;
using WindowsInput;
using System.Net.Http.Headers;
using NAudio.Wave;

namespace Unary.CSGOBot
{
    enum GameState : byte
    {
        Clear,
        Chat,
        Menu,
    }

    class ButtonChecker
    {
        private bool Pressed = false;
        private Func<bool> IsPressed = null;

        public ButtonChecker(Func<bool> NewIsPressed)
        {
            IsPressed = NewIsPressed;
        }

        public bool IsUnpressed()
        {
            if (IsPressed())
            {
                if(!Pressed)
                {
                    Pressed = true;
                }

                return false;
            }
            else
            {
                if(Pressed)
                {
                    Pressed = false;
                    return true;
                }

                return false;
            }
        }
    }

    class Program
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        static void Main(string[] args)
        {
            /*
            Console.WriteLine("Choose the output device: ");

            for (int n = 0; n < WaveOut.DeviceCount; n++)
            {
                var caps = WaveOut.GetCapabilities(n);
                Console.WriteLine($"{n}: {caps.ProductName}");
            }

            int DeviceNumber = int.Parse(Console.ReadLine());
            
            using (var audioFile = new AudioFileReader("Test.mp3"))
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.DeviceNumber = DeviceNumber;
                outputDevice.Init(audioFile);
                outputDevice.Play();
                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(1000);
                }
            }
            */

            InputSimulator Test = new InputSimulator();
            Thread.Sleep(3000);

            Process p = Process.GetProcessesByName("csgo").FirstOrDefault();
            IntPtr CSGOWindow = p.MainWindowHandle;

            GameState CurrentState = GameState.Clear;

            ButtonChecker EscapeButton = new ButtonChecker(() => { return Test.InputDeviceState.IsKeyDown(VirtualKeyCode.ESCAPE); });
            ButtonChecker YButton = new ButtonChecker(() => { return Test.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_Y); });
            ButtonChecker ReturnButton = new ButtonChecker(() => { return Test.InputDeviceState.IsKeyDown(VirtualKeyCode.RETURN); });

            while (true)
            {
                if(EscapeButton.IsUnpressed())
                {
                    if(CSGOWindow == GetForegroundWindow())
                    {
                        if (CurrentState == GameState.Chat || CurrentState == GameState.Menu)
                        {
                            CurrentState = GameState.Clear;
                        }
                        else
                        {
                            CurrentState = GameState.Menu;
                        }
                    }
                }
                else if(YButton.IsUnpressed())
                {
                    if (CSGOWindow == GetForegroundWindow() && CurrentState == GameState.Clear)
                    {
                        CurrentState = GameState.Chat;
                    }
                }
                else if(ReturnButton.IsUnpressed())
                {
                    if (CSGOWindow == GetForegroundWindow() && CurrentState == GameState.Chat)
                    {
                        CurrentState = GameState.Clear;
                    }
                }

                Console.WriteLine("CurrentState: " + CurrentState);
            }
            
            
            /*
            Process p = Process.GetProcessesByName("csgo").FirstOrDefault();
            if (p != null)
            {
                IntPtr OriginalWindow = GetForegroundWindow();
                SetForegroundWindow(p.MainWindowHandle);
                InputSimulator Test = new InputSimulator();

                for (int i = 0; i < 100000; ++i)
                {
                    Test.Mouse.MoveMouseToPositionOnVirtualDesktop(0, 0);
                    Test.Mouse.MoveMouseToPositionOnVirtualDesktop(150, 0);
                }
                
                SetForegroundWindow(OriginalWindow);
            }
            */
        }
    }
}

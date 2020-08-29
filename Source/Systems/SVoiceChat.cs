using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Unary.CSGOBot.Abstract;

namespace Unary.CSGOBot.Systems
{
    public class SVoiceChat : ISystem
    {
        public override void Init()
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
        }

        public override void Clear()
        {
            
        }

        public override void Poll()
        {
            
        }
    }
}

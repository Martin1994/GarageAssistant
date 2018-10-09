using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace GarageAssistant
{
    public class LED
    {
        private readonly GpioPin pin;

        private int blinkInterval = 1000;

        private readonly Task blinkWorker;

        private readonly ManualResetEvent blinkEvent = new ManualResetEvent(false);

        public LED(GpioPin pin)
        {
            this.pin = pin;
            pin.PinMode = GpioPinDriveMode.Output;

            blinkWorker = Task.Run((Action)BlinkWorker);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TurnOn()
        {
            blinkEvent.Reset();
            pin.Write(true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TurnOff()
        {
            blinkEvent.Reset();
            pin.Write(false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Blink(int interval)
        {
            blinkInterval = interval;
            blinkEvent.Set();
        }

        private void BlinkWorker()
        {
            bool on = true;
            while (blinkEvent.WaitOne())
            {
                pin.Write(on);
                on = !on;
                Thread.Sleep(blinkInterval);
            }
        }
    }
}

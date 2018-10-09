using System;
using System.Threading;
using Unosquare.RaspberryIO;

namespace GarageAssistant
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Launching GarageAssistant...");
            LED green = new LED(Pi.Gpio.GetGpioPinByBcmPinNumber(20));
            LED red = new LED(Pi.Gpio.GetGpioPinByBcmPinNumber(21));
            UltrasoundSensor sensor = new UltrasoundSensor(
                Pi.Gpio.GetGpioPinByBcmPinNumber(2),
                Pi.Gpio.GetGpioPinByBcmPinNumber(3)
            );

            Console.CancelKeyPress += (sender, e) =>
            {
                Console.WriteLine("Terminating...");
                green.TurnOff();
                red.TurnOff();
            };

            Daemon daemon = new Daemon(
                statusLED: green,
                distanceLED: red,
                ultrasoundSensor: sensor
            );
            daemon.Run();
        }
    }
}

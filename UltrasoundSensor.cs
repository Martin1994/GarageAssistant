using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace GarageAssistant
{
    public class UltrasoundSensor
    {
        // HC-SR04 requires a 10μs pulse to trigger its measurement
        private static readonly long SENSOR_TRIGGER_TICKS = Stopwatch.Frequency / 100000;
        public const double MAX_MEASURE_DISTANCE = 4;
        private static readonly long MAX_MEASURE_TICKS = Stopwatch.Frequency;

        private const double SPEED_OF_SOUND = 343;

        private readonly GpioPin trigger;
        private readonly GpioPin echo;

        public UltrasoundSensor(GpioPin trigger, GpioPin echo)
        {
            this.trigger = trigger;
            this.echo = echo;

            trigger.PinMode = GpioPinDriveMode.Output;
            echo.PinMode = GpioPinDriveMode.Input;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Measure()
        {
            Stopwatch sw = new Stopwatch();

            // Send a measurement
            sw.Start();
            trigger.Write(true);
            while (sw.ElapsedTicks < SENSOR_TRIGGER_TICKS);
            trigger.Write(false);

            // Get distance by the time interval of high voltage on ECHO
            sw.Reset();
            sw.Start();
            while (!echo.Read())
            {
                if (sw.ElapsedTicks > MAX_MEASURE_TICKS) {
                    Console.WriteLine("Elapsed: {0}, Threshold: {1}.", sw.ElapsedTicks, MAX_MEASURE_TICKS);
                    return MAX_MEASURE_DISTANCE;
                }
            }
            sw.Reset();
            sw.Start();
            while (echo.Read())
            {
                if (sw.ElapsedTicks > MAX_MEASURE_TICKS) {
                    Console.WriteLine("Elapsed: {0}, Threshold: {1}!", sw.ElapsedTicks, MAX_MEASURE_TICKS);
                    return MAX_MEASURE_DISTANCE;
                }
            }
            return sw.ElapsedTicks / 2 * SPEED_OF_SOUND / Stopwatch.Frequency;
        }
    }
}

using System;
using System.Diagnostics;
using System.Threading;

namespace GarageAssistant
{
    public class Daemon
    {
        const double ACTIVATION_DELTA_DISTANCE = 0.01;
        const int ACTIVE_MEASURE_INTERVAL = 200;
        const int INACTIVE_MEASURE_INTERVAL = 1000;
        const double MEASURE_INTERVAL_GROWTH_RATE = 1.08;

        const double MAX_WARNING_DISTANCE = 1;

        const double MAX_DISTANCE_BLINK_INTERVAL = 1000;
        const int MIN_STATUS_BLINK_INTERVAL = 100;
    
        private readonly LED distanceLED;
        private readonly LED statusLED;
        private readonly UltrasoundSensor sensor;

        private double distance = UltrasoundSensor.MAX_MEASURE_DISTANCE;
        private int measureInterval = INACTIVE_MEASURE_INTERVAL;

        public Daemon(LED statusLED, LED distanceLED, UltrasoundSensor ultrasoundSensor)
        {
            this.distanceLED = distanceLED;
            this.statusLED = statusLED;
            sensor = ultrasoundSensor;
        }

        public void Run()
        {
            Stopwatch sw = new Stopwatch();
            while (true)
            {
                sw.Restart();
                statusLED.TurnOn();
                Measure();
                if (measureInterval < INACTIVE_MEASURE_INTERVAL)
                {
                    Signal();
                }
                else
                {
                    distanceLED.TurnOff();
                }
                // Console.WriteLine("Measured distance: {0:000.000}m.", distance);
                if (sw.ElapsedMilliseconds < MIN_STATUS_BLINK_INTERVAL)
                {
                    Thread.Sleep(Math.Max(MIN_STATUS_BLINK_INTERVAL - (int)sw.ElapsedMilliseconds, 0));
                }
                statusLED.TurnOff();
                Thread.Sleep(Math.Max(measureInterval - (int)sw.ElapsedMilliseconds, 0));
            }
        }

        private void Measure()
        {
            double distance = sensor.Measure();
            if (distance < UltrasoundSensor.MAX_MEASURE_DISTANCE)
            {
                UpdateDistance(distance);
            }
            else
            {
                UpdateDistance(UltrasoundSensor.MAX_MEASURE_DISTANCE);
            }
        }

        private void UpdateDistance(double distance)
        {
            if (Math.Abs(this.distance - distance) < ACTIVATION_DELTA_DISTANCE)
            {
                measureInterval = (int)(measureInterval * MEASURE_INTERVAL_GROWTH_RATE);
                if (measureInterval > INACTIVE_MEASURE_INTERVAL)
                {
                    measureInterval = INACTIVE_MEASURE_INTERVAL;
                }
            }
            else
            {
                measureInterval = ACTIVE_MEASURE_INTERVAL;
            }
            this.distance = distance;
        }

        private void Signal()
        {
            if (distance < MAX_WARNING_DISTANCE)
            {
                double blinkInterval = Math.Tan(distance * Math.PI * 0.5) * 200;
                blinkInterval = Math.Min(blinkInterval, MAX_DISTANCE_BLINK_INTERVAL);
                distanceLED.Blink((int)blinkInterval);
            }
            else
            {
                distanceLED.TurnOff();
            }
        }
    }
}

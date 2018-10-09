# GarageAssistant
This is a simple program running on Raspberry Pi to detect the distance between
a car the the wall when parking in the garage.

There are two output LEDs:
* When the status LED flashes once, the distance sensor measures once.
* The closer the mesuring object is, the quicker the distance LED flashes.

# Hardware
1. Raspberry Pi with 4 GPIO pins available,
2. HC-SR04 ultrasound distance sensor,
3. An LED to show running status, and
4. An LED to show distance.

# Runtime
.NET core 2.1.4 runtime for arm32.

# Deploy instruction
1. `dotnet publish`
2. Copy `bin/${CONFIGURATION}/netcoreapp2.0/linux-arm/publish` to your
Raspberry Pi.
3. Run `./GarageAssistant` in `publish` directory. (Make sure you have .NET
core 2.1 runtime on your Raspberry Pi)

# Debug instruction
See https://github.com/OmniSharp/omnisharp-vscode/wiki/Remote-Debugging-On-Linux-Arm.

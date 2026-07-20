# plcView

A Windows Forms application written in C# (.NET Framework 4.7.2) to collect, monitor, and playback PLC data using the Mitsubishi MC Protocol (3E Frame).

👉 **[日本語のREADMEはこちら (Japanese README_JP.md)](./README_JP.md)**

## Features

- **MC Protocol (3E Frame) Support**: Connects to PLCs via TCP/IP.
- **Flexible Data Collection**:
  - Configure up to 10 points (data blocks) with customizable device type, start address, and word size.
  - Scan intervals from 0ms (fastest/approx. 1ms wait) up to 5 minutes.
- **High-Performance Persistence**:
  - Dual format logging: CSV (horizontal layout) or Binary format.
  - Automatic file splitting at 100MB to prevent storage locks.
  - Reduced disk I/O overhead using cached stream writers.
- **Interactive Playback**:
  - Load binary history files and scroll through frames using a timeline trackbar.
  - Playback speeds of 1x, 2x, 5x, and 10x.
- **Optimized Rendering**:
  - Grid flickering is prevented via Double Buffering.
  - Ultra-fast UI refresh rate by updating cell values directly instead of recreating rows.

## System Requirements

- Windows 10/11
- .NET Framework 4.7.2
- A target PLC configured for MC Protocol 3E Frame TCP communication.

## Getting Started

1. **Configure PLC Connection**: Enter the IP address, Port, and Timeout of the PLC.
2. **Setup Points**: Define the points (up to 10) you want to monitor by selecting the Device Type (D, W, M, ZR, SW, etc.), start address, and word count.
3. **Start Collection**: Click **Start** to begin polling and logging. Data is automatically saved in the output directory.
4. **Playback History**: Go to the History tab, load a `.dat` binary file, and use the player controls to review past PLC states.

## Repository Structure

- `plcView/Config/`: Setting serialize manager (`ProjectSettings.cs`).
- `plcView/Plc/`: MC Protocol Client (`McProtocolClient.cs`) and Address parser (`DeviceConverter.cs`).
- `plcView/Data/`: Stream logger (`DataLogger.cs`) and Binary index reader (`HistoryReader.cs`).
- `plcView/MainForm.cs`: UI controls and collection thread dispatch loop.

## License

This project is open-source.

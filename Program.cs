using CommandLine;
using DdmLibrary.Monitor;

// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace DDM_HDRPeak1000;

internal static class Program
{
    
    private class Options
    {
        [Option('i', "index", Required = false, HelpText = "Index of the monitor.", Default = (byte)0)]
        public byte Index { get; set; }
    }

    [Verb("off", HelpText = "Turn off HDR Peak 1000. Toggling off turns SmartHDR off.")]
    private class OffOptions : Options
    {
        
    }
    
    [Verb("on", HelpText = "Switch to HDR Peak 1000.")]
    private class OnOptions : Options
    {
        
    }
    
    public static int Main(string[] args)
    {
        return Parser.Default.ParseArguments<OffOptions, OnOptions>(args)
            .MapResult(
                (OffOptions options) => WithOptions(options, false),
                (OnOptions options) => WithOptions(options, true),
                _ => 1);
    }

    private static int WithOptions(Options options, bool toggle)
    {
        try
        {
            if (!OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763, 0))
            {
                throw new PlatformNotSupportedException("Only Windows 10 build 17763 and up is supported.");
            }

            Console.WriteLine($"Toggling {(toggle ? "on" : "off")}");
            byte monIndex = options.Index;
            Console.WriteLine("Using index: " + monIndex);

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            ClassMonitorControllerBase monController = ClassMonitorControllerBase.GetInstance();
            Console.WriteLine("Initialising monitor connection...");
            monController.initialMonitor();
            Console.WriteLine("Connecting to monitor...");
            ClassMonitorControllerBase.ConnectStatus connectStatus = monController.ConnMonitor(token);
            if (connectStatus != ClassMonitorControllerBase.ConnectStatus.Success)
            {
                throw new InvalidOperationException("Couldn't connect to any Dell compatible monitor.");
            }

            ClassMonitorInfo monInfo = ClassMonitorInfo.GetInstance();
            monInfo.monitorInfos = monController.MonitorInfos;
            monInfo.iCurrentMonitorSelectIndex = monIndex;

            if (monController.MonitorInfos == null || monController.MonitorInfos.Length == 0)
            {
                throw new InvalidOperationException("Couldn't get information about any Dell compatible monitor.");
            }

            if (!monController.MonitorInfos[monIndex].ColorInfos.lsSHDR.Contains(70)) // VCPF4.HDRPeak1000 => 0x46 => 70
            {
                throw new InvalidOperationException("This monitor doesn't have the capability to use HDR Peak 1000. <VCPF4.HDRPeak1000, 0x46, 70> not found.");
            }

            Console.WriteLine("Sending to monitor...");
            //TargetJob job = TargetJob.GetSetVcp;
            const VcpTypes vcpType = VcpTypes.GamingWidgetsControls;
            //DdmLibrary.Utility.ColorPresetModeStruct.VCPF4
            //VCPF4.Off => 64
            //VCPF4.HDRPeak1000 => 70
            int vcpf4Value = toggle ? 70 : 64;
            
            int a = 0;
            int b = 0;
            bool result = ClassMonitorControllerBase.GetInstance().VcpSetCmd(monIndex, GetSetMode.Set, vcpType, ref a, ref b, vcpf4Value, 0);
            Console.WriteLine("Waiting 5 seconds for threads to finish...");
            Thread.Sleep(5000);

            if (!result)
            {
                Console.WriteLine("Something happened while sending the command to the monitor.");
            }
            return result ? 0 : 1;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.ToString());
            return 1;
        }
    }
}
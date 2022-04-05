using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime;
using System.Runtime.Versioning;
using Serilog;
using Baltic.Core.Extensions;

namespace Baltic.Core.Utils
{
    public static class SystemInfo
    {
        public static string MachineName => Environment.MachineName;
        public static string NodeId => GetNodeId();

        public static string RuntimeVersion => (Assembly.GetEntryAssembly() ?? throw new InvalidOperationException()).GetCustomAttribute<TargetFrameworkAttribute>().FrameworkName;
        public static string OsNameAndVersion => System.Runtime.InteropServices.RuntimeInformation.OSDescription;
        public static bool IsServerGC => GCSettings.IsServerGC;
        public static GCLargeObjectHeapCompactionMode LargeObjectHeapCompactionMode => GCSettings.LargeObjectHeapCompactionMode;
        public static GCLatencyMode LatencyMode => GCSettings.LatencyMode;
        public static string ContentRootPath => Directory.GetCurrentDirectory();
        public static string ExecutingAssemblyName => Assembly.GetEntryAssembly()?.GetName().Name;
        public static bool Windows => Environment.OSVersion.Platform == PlatformID.Win32NT;
        public static bool Unix => Environment.OSVersion.Platform == PlatformID.Unix;


        private static string InternalNodeId { get; set; }
        private static DateTimeOffset StartDateTime { get; } = DateTimeOffset.UtcNow;
        public static string UpTime => GetUpTime();

        /// <summary>
        /// Finds the MAC address of the first NIC with maximum speed
        /// </summary>
        /// <returns>The MAC address</returns>
        public static string GetMacAddress()
        {
            const int minMacAddressLength = 12;

            var macAddress = new string('0', minMacAddressLength);
            long maxSpeed = -1;

            if (Windows)
            {
                foreach (var nic1 in NetworkInterface.GetAllNetworkInterfaces())
                {
                    Log.Debug("Found MAC Address: {address}, Type: {type}", nic1.GetPhysicalAddress(),
                        nic1.NetworkInterfaceType);

                    var tempMac = nic1.GetPhysicalAddress().ToString();
                    if (nic1.Speed > maxSpeed && !string.IsNullOrEmpty(tempMac) &&
                        tempMac.Length >= minMacAddressLength)
                    {
                        Log.Debug("New Max Speed = {speed}, MAC: {mac}", nic1.Speed, tempMac);
                        maxSpeed = nic1.Speed;
                        macAddress = tempMac;
                    }
                }
                return macAddress.Substring(0, 12);
            }

            if (Unix)
            {
                var nic2 = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault();
                if (nic2 != null)
                {
                    Log.Debug("Found MAC Address: {address}, Type: {type}", nic2.GetPhysicalAddress(),
                        nic2.NetworkInterfaceType);
                    macAddress = nic2.GetPhysicalAddress().ToString();
                }
                return macAddress.Substring(0, 12);
            }

            return macAddress;
        }

        private static string GetUpTime()
        {
            TimeSpan difference = DateTimeOffset.UtcNow - StartDateTime;
            var days = (int)difference.TotalDays;
            var s = days > 2?"days":"day";
            var dif = difference.Subtract(new TimeSpan(days, 0, 0, 0));
            return $"up {days} {s}, {dif.Hours:D2}:{dif.Minutes:D2}";
        }

        public static string GetNodeId(int id = 0)
        {
            if (string.IsNullOrEmpty(InternalNodeId)) 
            {
                var machineName = id==0?Environment.MachineName:$"{Environment.MachineName}-{id:X6}";
                var machineNameHashCode = machineName.GetDeterministicHashCode();
                var highMachineNameHashCode = (int)(machineNameHashCode >> 32);
                var lowMachineNameHashCode = (int)(machineNameHashCode & ((1L << 32) - 1));
                InternalNodeId =  $"{GetMacAddress()}-{highMachineNameHashCode:X8}-{lowMachineNameHashCode:X8}";
            }
            return InternalNodeId;
        }
        
        public static string GetIpAddress()
        {
            var ipAddress = "0.0.0.0";

            if (NetworkInterface.GetIsNetworkAvailable())
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                var address = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
                
                if (address != null)
                {
                    ipAddress = address.ToString();
                }
            }
            return ipAddress;
        }
        
        public static string GetHostName()
        {
            return Dns.GetHostName();
        }
       
    }
}

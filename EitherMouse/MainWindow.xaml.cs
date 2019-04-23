using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft;

namespace EitherMouse
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /*Device(string name, int sensitivity, int doubleClickSpeed, int scrollSpeed)
        {
            this.Name = name;
            this.Sensitivity = sensitivity;
            this.DoubleClickSpeed = doubleClickSpeed;
            this.ScrollSpeed = scrollSpeed;
        }*/

        List<Device> devices = new List<Device>();

        FileManager fileManager = new FileManager();

        Device curDevice = new Device();

        public const UInt32 spi_setMouseSpeed = 0x0071;
        public const UInt32 spi_setMouseDoubleclickSpeed = 0x0020;
        public const UInt32 spi_setMouseScrollSpeed = 0x0069;

        public const UInt32 spi_getMouseSpeed = 0x0070;
        public const UInt32 spi_getMouseScrollSpeed = 0x0068;

        [DllImport("User32.dll")]
        static extern Boolean SystemParametersInfo(
            UInt32 uiAction,
            UInt32 uiParam,
            UInt32 pvParam,
            UInt32 fWinIni
            );

        public MainWindow()
        {
            InitializeComponent();
            devices = fileManager.LoadProfiles();
            loadDevices();
        }

        void loadDevices()
        {
            if (devices == null || devices.Count == 0)
            {
                devices = new List<Device>();
                devices.Add(new Device());
            }

            deviceSelection.Items.Clear();

            foreach (Device device in devices)
            {
                deviceSelection.Items.Add(new ComboBoxItem() { Content = device.Name });
            }

            deviceSelection.SelectedIndex = devices.Count - 1;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Device newDevice = new Device(deviceName.Text.ToString(), Convert.ToUInt32(sensitivityInput.Value), Convert.ToUInt32(doubleClickSpeedInput.Value), Convert.ToUInt32(scrollSpeedInput.Value));

            devices.Add(newDevice);
            curDevice = newDevice;

            sensitivityInput.Value = curDevice.Sensitivity;
            scrollSpeedInput.Value = curDevice.ScrollSpeed;
            doubleClickSpeedInput.Value = curDevice.DoubleClickSpeed;

            loadDevices();

            deviceName.Text = "";
        }

        void setDevice()
        {
            if (deviceSelection.SelectedIndex != -1)
            {
                curDevice = devices[deviceSelection.SelectedIndex];

                devices[deviceSelection.SelectedIndex] = curDevice;

                loadDevice(curDevice);
            }
        }

        void updateDevice()
        {
            if (sensitivityInput != null && doubleClickSpeedInput != null && scrollSpeedInput != null)
            {
                curDevice.Sensitivity = UInt32.Parse(sensitivityInput.Value.ToString());
                curDevice.DoubleClickSpeed = UInt32.Parse(doubleClickSpeedInput.Value.ToString());
                curDevice.ScrollSpeed = UInt32.Parse(scrollSpeedInput.Value.ToString());

                if (deviceSelection.SelectedIndex != -1)
                {
                    devices[deviceSelection.SelectedIndex] = curDevice;

                    loadDevice(curDevice);
                }
            }
        }

        private void SetDevice_Click(object sender, RoutedEventArgs e)
        {
            updateDevice();
        }

        void loadDevice(Device curDevice)
        {
            SystemParametersInfo(

                spi_setMouseSpeed,
                0,
                curDevice.Sensitivity,
                0

                );
            SystemParametersInfo(

                spi_setMouseDoubleclickSpeed,
                curDevice.DoubleClickSpeed,
                0,
                0

                );
            SystemParametersInfo(

                spi_setMouseScrollSpeed,
                curDevice.ScrollSpeed,
                0,
                0

                );
        }

        private void selectDevice(object sender, SelectionChangedEventArgs e)
        {
            if (deviceSelection.SelectedIndex != -1)
            {
                curDevice = devices[deviceSelection.SelectedIndex];

                sensitivityInput.Value = curDevice.Sensitivity;
                doubleClickSpeedInput.Value = curDevice.DoubleClickSpeed;
                scrollSpeedInput.Value = curDevice.ScrollSpeed;

                setDevice();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            fileManager.SaveProfiles(devices);
        }
    }
}

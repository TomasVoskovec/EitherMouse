using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
using Newtonsoft.Json;

namespace EitherMouse
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Device> devices = new List<Device>();
        List<Device> devicesFromDb = new List<Device>();

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

            fileManager.SaveProfiles(devices);
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

        private void Sync_Click(object sender, RoutedEventArgs e)
        {
            synchronizeDevices(devices);
        }

        async void synchronizeDevices(List<Device> localDevices)
        {
            devicesFromDb = await GET_AllDevices();

            foreach (Device dbDevice in devicesFromDb)
            {
                bool deviceExist = false;

                foreach (Device localDevice in devices)
                {
                    if (dbDevice.Id == localDevice.Id)
                    {
                        deviceExist = true;

                        await POST_UpdateDevice(localDevice);
                    }
                }

                if (!deviceExist)
                {
                    devices.Add(dbDevice);
                }
            }

            loadDevices();

            for (int i = 0; i < devices.Count; i++)
            {
                if (devices[i].Id == -1)
                {
                    HttpResponseMessage x = await POST_Device(devices[i]);

                    string newId = await x.Content.ReadAsStringAsync();

                    devices[i].Id = Convert.ToInt32(newId);
                }
            }

            fileManager.SaveProfiles(devices);
        }

        public async Task<List<Device>> GET_AllDevices()
        {
            HttpClient httpClient = new HttpClient();

            HttpResponseMessage response = await httpClient.GetAsync("https://voskoto16.sps-prosek.cz/Api/selectAllDevices.php");

            string jsonContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<Device>>(jsonContent);
        }

        public async Task<HttpResponseMessage> POST_Device(Device device)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://voskoto16.sps-prosek.cz/Api/insertDevice.php");

            List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>();

            keyValues.Add(new KeyValuePair<string, string>("User", "application"));
            keyValues.Add(new KeyValuePair<string, string>("Name", device.Name));
            keyValues.Add(new KeyValuePair<string, string>("Sensitivity", device.Sensitivity.ToString()));
            keyValues.Add(new KeyValuePair<string, string>("DoubleClickSpeed", device.DoubleClickSpeed.ToString()));
            keyValues.Add(new KeyValuePair<string, string>("ScrollSpeed", device.ScrollSpeed.ToString()));

            request.Content = new FormUrlEncodedContent(keyValues);

            HttpResponseMessage responseMessage = await client.SendAsync(request);

            return responseMessage;
        }

        public async Task<HttpResponseMessage> POST_UpdateDevice(Device device)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://voskoto16.sps-prosek.cz/Api/insertDevice.php");

            List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>();

            keyValues.Add(new KeyValuePair<string, string>("User", "application"));
            keyValues.Add(new KeyValuePair<string, string>("Id", device.Id.ToString()));
            keyValues.Add(new KeyValuePair<string, string>("Name", device.Name));
            keyValues.Add(new KeyValuePair<string, string>("Sensitivity", device.Sensitivity.ToString()));
            keyValues.Add(new KeyValuePair<string, string>("DoubleClickSpeed", device.DoubleClickSpeed.ToString()));
            keyValues.Add(new KeyValuePair<string, string>("ScrollSpeed", device.ScrollSpeed.ToString()));

            request.Content = new FormUrlEncodedContent(keyValues);

            HttpResponseMessage responseMessage = await client.SendAsync(request);

            return responseMessage;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            fileManager.SaveProfiles(devices);
        }
    }
}

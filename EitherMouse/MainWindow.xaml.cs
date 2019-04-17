using System;
using System.Collections.Generic;
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
            loadDevices();
        }

        void loadDevices()
        {
            int i = 0;
            foreach (Device device in devices)
            {
                deviceSelection.Items.Add(new ComboBoxItem() { Content = devices[0].Name });
                i++;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UInt32 curSensitivity = 0;
            SystemParametersInfo(spi_getMouseSpeed, 0, curSensitivity, 0);

            UInt32 curScrollSpeed = 0;
            SystemParametersInfo(spi_getMouseScrollSpeed, 0, curScrollSpeed, 0);

            Device newDevice = new Device(deviceName.Text.ToString(), curSensitivity, 500, spi_getMouseScrollSpeed);

            devices.Add(newDevice);

            loadDevices();
        }

        private void SetDevice_Click(object sender, RoutedEventArgs e)
        {
            if (deviceSelection.SelectedIndex != -1)
            {
                Device curDevice = devices[deviceSelection.SelectedIndex];

                curDevice.Sensitivity = UInt32.Parse(sensitivityInput.Value.ToString());
                curDevice.DoubleClickSpeed = UInt32.Parse(doubleClickSpeedInput.Value.ToString());
                curDevice.ScrollSpeed = UInt32.Parse(scrollSpeedInput.Value.ToString());

                SystemParametersInfo(

                    spi_setMouseSpeed,
                    0,
                    curDevice.Sensitivity,
                    0

                    );
                SystemParametersInfo(

                    spi_setMouseDoubleclickSpeed,
                    curDevice.ScrollSpeed,
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
            else
            {
                MessageBox.Show("At first select device");
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        List<string> devices = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            loadDevices();
        }

        void loadDevices()
        {
            int i = 0;
            foreach (string device in devices)
            {
                deviceSelection.Items.Add(new ComboBoxItem() { Content = devices[0] });
                i++;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            devices.Add(deviceName.Text.ToString());

            loadDevices();
        }
    }
}

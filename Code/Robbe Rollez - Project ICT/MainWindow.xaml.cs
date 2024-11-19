﻿using System.IO.Ports;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Robbe_Rollez___Project_ICT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort _serialPort;
        string? comPort;

        public MainWindow()
        {
            InitializeComponent();

            _serialPort = new SerialPort();


            cmbComPorts.Items.Add("None");

            // Alle beschikbare poortnamen opvragen en toevoegen aan de combobox.
            foreach (string s in SerialPort.GetPortNames())
            {
                cmbComPorts.Items.Add(s);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if ((_serialPort != null) && (_serialPort.IsOpen))
            {
                _serialPort.Write("0");
                _serialPort.Dispose();
            }
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (_serialPort != null)
            {
                // Controle of de seriële poort al open is.
                if (_serialPort.IsOpen)
                {
                    // Seriële poort sluiten
                    _serialPort.Close();
                }
                // Controle of de poortnaam geselecteerd is.
                if (comPort != "None")
                {
                    // Seriële poort instellen.
                    _serialPort.PortName = comPort;
                    _serialPort.BaudRate = 9600;
                    _serialPort.Open();
                    // Arduino laten weten dat connectie gelukt is.
                    _serialPort.Write("R");
                    MessageBox.Show("Connectie gelukt");

                    cmbComPorts.IsEnabled = false;
                    btnDisconnect.IsEnabled = true;
                    btnClearLCD.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show("Selecteer een COM-poort", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void cmbComPorts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cmbComPorts.SelectedIndex != -1)
            {
                if (cmbComPorts.SelectedItem.ToString() != "None")
                {
                    comPort = cmbComPorts.SelectedItem.ToString();
                }
            } else
            {
                comPort = "";
            }

            btnConnect.IsEnabled = true;
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            // Controle of de seriële poort open is.
            if (_serialPort.IsOpen)
            {
                // 0 schrijven naar microcontroller.
                _serialPort.Write("0");
                // Seriële poort sluiten.
                _serialPort.Close();

                MessageBox.Show("Disconnected");

                cmbComPorts.IsEnabled = true;
                btnClearLCD.IsEnabled = false;
                btnConnect.IsEnabled = false;
                btnDisconnect.IsEnabled = false;
                cmbComPorts.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Poort is niet open.");
            }
        }

        private void cmbMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            contentControl.Content = null;

            if (cmbMode.SelectedItem is ComboBoxItem selectedItem)
            {
                switch (selectedItem.Content.ToString())
                {
                    case "Binaire teller":
                        contentControl.Content = new BinaryCounterView(_serialPort);
                        if (_serialPort.IsOpen)
                        {
                            _serialPort.Write("A");
                        }
                        break;

                    case "Rekenmachine":
                        contentControl.Content = new CalculatorView(_serialPort);
                        if (_serialPort.IsOpen)
                        {
                            _serialPort.Write("B");
                        }
                        break;

                    case "HEX, BIN, DEC Omzetter":
                        contentControl.Content = new ConverterView();
                        if (_serialPort.IsOpen)
                        {
                            _serialPort.Write("C");
                        }
                        break;
                    default:
                        MessageBox.Show("Onbekende mode geselecteerd.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;

                }
            }
        }

        private void btnClearLCD_Click(object sender, RoutedEventArgs e)
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Write("-1");
                contentControl.Content = new MainWindow();
                cmbMode.SelectedIndex = -1;
                cmbComPorts.SelectedIndex = -1;
            }
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            cmbMode.SelectedIndex = -1;
            contentControl.Content = new Settings(_serialPort);
        }
    }
}
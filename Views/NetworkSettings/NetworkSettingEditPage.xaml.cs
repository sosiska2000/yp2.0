using System;
using System.Windows;
using System.Windows.Controls;
using EquipmentManagement.Client.Models;
using EquipmentManagement.Client.Services;

namespace EquipmentManagement.Client.Views.NetworkSettings
{
    public partial class NetworkSettingEditPage : Page
    {
        private readonly Frame _mainFrame;
        private readonly ApiService _apiService;
        private readonly NetworkSetting _currentSetting;

        public NetworkSettingEditPage(Frame mainFrame, NetworkSetting setting = null)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
            _apiService = new ApiService();
            _currentSetting = setting ?? new NetworkSetting();

            if (setting != null)
            {
                TitleText.Text = "Редактирование сетевых настроек";
                EquipmentIdBox.Text = setting.EquipmentId.ToString();
                IpAddressBox.Text = setting.IpAddress;
                SubnetMaskBox.Text = setting.SubnetMask;
                GatewayBox.Text = setting.Gateway;
                DnsServersBox.Text = setting.DnsServers;
            }

            CancelButton.Click += CancelButton_Click;
            SaveButton.Click += SaveButton_Click;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _mainFrame.GoBack();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EquipmentIdBox.Text))
            {
                MessageBox.Show("ID оборудования обязательно");
                return;
            }

            if (string.IsNullOrWhiteSpace(IpAddressBox.Text))
            {
                MessageBox.Show("IP адрес обязателен");
                return;
            }

            if (!int.TryParse(EquipmentIdBox.Text, out int equipmentId))
            {
                MessageBox.Show("ID оборудования должно быть числом");
                return;
            }

            _currentSetting.EquipmentId = equipmentId;
            _currentSetting.IpAddress = IpAddressBox.Text;
            _currentSetting.SubnetMask = SubnetMaskBox.Text;
            _currentSetting.Gateway = GatewayBox.Text;
            _currentSetting.DnsServers = DnsServersBox.Text;

            try
            {
                if (_currentSetting.Id == 0)
                    await _apiService.AddNetworkSettingAsync(_currentSetting);
                else
                    await _apiService.UpdateNetworkSettingAsync(_currentSetting);

                _mainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }
    }
}
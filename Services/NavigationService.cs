using System;
using System.Windows.Controls;

namespace EquipmentManagement.Client.Services
{
    public class NavigationService
    {
        private readonly Frame _mainFrame;

        public NavigationService(Frame mainFrame)
        {
            _mainFrame = mainFrame;
        }

        public void NavigateTo(Page page)
        {
            _mainFrame.Navigate(page);
        }

        public void GoBack()
        {
            if (_mainFrame.CanGoBack)
                _mainFrame.GoBack();
        }

        public void GoForward()
        {
            if (_mainFrame.CanGoForward)
                _mainFrame.GoForward();
        }
    }
}
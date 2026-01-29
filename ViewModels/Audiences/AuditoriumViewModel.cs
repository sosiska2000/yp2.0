using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EquipmentManagement.Client.ViewModel.Audiences
{
    public class AuditoriumViewModel : INotifyPropertyChanged
    {
        private int _id;
        private string _name = string.Empty;
        private string _shortName = string.Empty;
        private UserViewModel? _responsibleUser;
        private UserViewModel? _tempResponsibleUser;
        private int _equipmentCount;

        public int Id
        {
            get => _id;
            set => SetField(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        public string ShortName
        {
            get => _shortName;
            set => SetField(ref _shortName, value);
        }

        public UserViewModel? ResponsibleUser
        {
            get => _responsibleUser;
            set => SetField(ref _responsibleUser, value);
        }

        public UserViewModel? TempResponsibleUser
        {
            get => _tempResponsibleUser;
            set => SetField(ref _tempResponsibleUser, value);
        }

        public int EquipmentCount
        {
            get => _equipmentCount;
            set => SetField(ref _equipmentCount, value);
        }

        // Для отображения в таблице
        public string ResponsibleUserName => ResponsibleUser?.FullName ?? "Не назначен";
        public string TempResponsibleUserName => TempResponsibleUser?.FullName ?? "Не назначен";

        // Для DataGrid сортировки
        public string SortName => Name.ToLower();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
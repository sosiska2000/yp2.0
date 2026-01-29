using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EquipmentManagement.Client.ViewModel.Audiences
{
    public class UserViewModel : INotifyPropertyChanged
    {
        private int _id;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string? _middleName;

        public int Id
        {
            get => _id;
            set => SetField(ref _id, value);
        }

        public string FirstName
        {
            get => _firstName;
            set
            {
                if (SetField(ref _firstName, value))
                {
                    OnPropertyChanged(nameof(FullName));
                }
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                if (SetField(ref _lastName, value))
                {
                    OnPropertyChanged(nameof(FullName));
                }
            }
        }

        public string? MiddleName
        {
            get => _middleName;
            set
            {
                if (SetField(ref _middleName, value))
                {
                    OnPropertyChanged(nameof(FullName));
                }
            }
        }

        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();

        // Для ComboBox
        public override string ToString() => FullName;

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
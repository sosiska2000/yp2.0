using EquipmentManagement.Client.Models;
using EquipmentManagement.Client.Models.DTO;
using EquipmentManagement.Client.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EquipmentManagement.Client.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private ObservableCollection<Auditories> _auditories;
        private ObservableCollection<Oborudovanie> _equipment;
        private bool _isLoading;

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainViewModel(ApiService apiService)
        {
            _apiService = apiService;
            _auditories = new ObservableCollection<Auditories>();
            _equipment = new ObservableCollection<Oborudovanie>();

            LoadDataCommand = new RelayCommand(async () => await LoadData());
            LoadEquipmentCommand = new RelayCommand(async () => await LoadEquipment());
        }

        public ObservableCollection<Auditories> Auditories
        {
            get => _auditories;
            set { _auditories = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Oborudovanie> Equipment
        {
            get => _equipment;
            set { _equipment = value; OnPropertyChanged(); }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public ICommand LoadDataCommand { get; }
        public ICommand LoadEquipmentCommand { get; }

        private async Task LoadData()
        {
            try
            {
                IsLoading = true;

                // Загружаем пользователей
                var users = await _apiService.Polzovateli.GetPolzovateli();

                // Загружаем аудитории (пример)
                // var auditories = await _apiService.Auditorii.GetAllAuditorii();
                // Auditories.Clear();
                // foreach (var a in auditories) Auditories.Add(a);
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                System.Windows.MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadEquipment()
        {
            try
            {
                IsLoading = true;

                // Получаем DTO из API
                var equipmentDtos = await _apiService.Oborudovanie.GetOborudovanie();

                // Конвертируем DTO в локальные модели
                var equipment = equipmentDtos.Select(dto => new Oborudovanie
                {
                    Id = dto.Id,
                    Name = dto.Nazvanie ?? string.Empty,
                    InventNumber = dto.InventarnyiNomer ?? string.Empty,
                    PriceObor = dto.Stoimost.ToString("F2"),
                    IdResponUser = dto.OtvetstvennyiPolzovatelId ?? 0,
                    IdTimeResponUser = dto.VremennoOtvetstvennyiPolzovatelId ?? 0,
                    IdClassroom = dto.AuditoriaId ?? 0,
                    IdNapravObor = dto.NapravlenieId ?? 0,
                    IdStatusObor = dto.StatusId ?? 0,
                    IdModelObor = dto.VidModeliId ?? 0,
                    Comments = dto.Kommentarii ?? string.Empty,
                    Photo = dto.Photo
                }).ToList();

                // Очищаем и добавляем в ObservableCollection
                Equipment.Clear();
                foreach (var item in equipment)
                {
                    Equipment.Add(item);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка загрузки оборудования: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    // Простая реализация RelayCommand
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object? parameter) => _execute();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using HospitalApp.Models;
using HospitalApp.Services;
using ReactiveUI;

namespace HospitalApp.ViewModels
{
    public class DoctorPageViewModel : ViewModelBase
    {
        private readonly ApiService _apiService;
        private readonly SignalRService _signalRService;

        // Doctor List Properties
        private ObservableCollection<Doctor> _doctors;
        public ObservableCollection<Doctor> Doctors
        {
            get => _doctors;
            set => this.RaiseAndSetIfChanged(ref _doctors, value);
        }

        // Selected Doctor and Current Doctor for editing
        private Doctor _selectedDoctor;
        public Doctor SelectedDoctor
        {
            get => _selectedDoctor;
            set => this.RaiseAndSetIfChanged(ref _selectedDoctor, value);
        }

        private Doctor _currentDoctor;
        public Doctor CurrentDoctor
        {
            get => _currentDoctor;
            set => this.RaiseAndSetIfChanged(ref _currentDoctor, value);
        }

        // UI State Properties
        private bool _isEditMode;
        public bool IsEditMode
        {
            get => _isEditMode;
            set => this.RaiseAndSetIfChanged(ref _isEditMode, value);
        }

        private string _formTitle;
        public string FormTitle
        {
            get => _formTitle;
            set => this.RaiseAndSetIfChanged(ref _formTitle, value);
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set 
            {
                this.RaiseAndSetIfChanged(ref _searchText, value);
                LoadDoctorsAsync();
            }
        }

        // Pagination Properties
        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set => this.RaiseAndSetIfChanged(ref _currentPage, value);
        }

        private int _totalPages = 1;
        public int TotalPages
        {
            get => _totalPages;
            set => this.RaiseAndSetIfChanged(ref _totalPages, value);
        }

        // Commands
        public ICommand AddDoctorCommand { get; }
        public ICommand EditDoctorCommand { get; }
        public ICommand DeleteDoctorCommand { get; }
        public ICommand SaveDoctorCommand { get; }
        public ICommand CancelEditCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }

        public DoctorPageViewModel(ApiService apiService, SignalRService signalRService)
        {
            _apiService = apiService;
            _signalRService = signalRService;
            
            // Initialize collections
            Doctors = new ObservableCollection<Doctor>();
            
            // Setup commands
            AddDoctorCommand = ReactiveCommand.Create(AddDoctor);
            EditDoctorCommand = ReactiveCommand.Create<Doctor>(EditDoctor);
            DeleteDoctorCommand = ReactiveCommand.Create<Doctor>(DeleteDoctor);
            SaveDoctorCommand = ReactiveCommand.CreateFromTask(SaveDoctorAsync);
            CancelEditCommand = ReactiveCommand.Create(CancelEdit);
            NextPageCommand = ReactiveCommand.Create(NextPage);
            PreviousPageCommand = ReactiveCommand.Create(PreviousPage);
            
            // Setup SignalR event handlers
            _signalRService.DoctorAdded += OnDoctorAdded;
            _signalRService.DoctorUpdated += OnDoctorUpdated;
            _signalRService.DoctorDeleted += OnDoctorDeleted;
            
            // Initial load
            LoadDoctorsAsync();
        }

        private void AddDoctor()
        {
            CurrentDoctor = new Doctor();
            FormTitle = "Add New Doctor";
            IsEditMode = true;
        }

        private void EditDoctor(Doctor doctor)
        {
            CurrentDoctor = doctor.Clone();
            FormTitle = "Edit Doctor";
            IsEditMode = true;
        }

        private void DeleteDoctor(Doctor doctor)
        {
            // Confirm deletion
            var result = MessageBox.Avalonia.MessageBoxManager
                .GetMessageBoxStandardWindow("Confirm Delete", $"Are you sure you want to delete {doctor.Name}?",
                    MessageBox.Avalonia.Enums.ButtonEnum.YesNo)
                .ShowDialog();
            
            if (result == MessageBox.Avalonia.Enums.ButtonResult.Yes)
            {
                DeleteDoctorAsync(doctor);
            }
        }

        private async Task DeleteDoctorAsync(Doctor doctor)
        {
            try
            {
                await _apiService.DeleteDoctorAsync(doctor.Id);
                Doctors.Remove(doctor);
            }
            catch (Exception ex)
            {
                // Handle error
                await ShowErrorMessage($"Error deleting doctor: {ex.Message}");
            }
        }

        private async Task SaveDoctorAsync()
        {
            try
            {
                if (CurrentDoctor.Id == 0)
                {
                    // Add new doctor
                    var addedDoctor = await _apiService.AddDoctorAsync(CurrentDoctor);
                    Doctors.Add(addedDoctor);
                }
                else
                {
                    // Update existing doctor
                    await _apiService.UpdateDoctorAsync(CurrentDoctor);
                    
                    // Find and update in the collection
                    var index = Doctors.IndexOf(Doctors.First(d => d.Id == CurrentDoctor.Id));
                    if (index >= 0)
                    {
                        Doctors[index] = CurrentDoctor;
                    }
                }
                
                IsEditMode = false;
            }
            catch (Exception ex)
            {
                // Handle error
                await ShowErrorMessage($"Error saving doctor: {ex.Message}");
            }
        }

        private void CancelEdit()
        {
            IsEditMode = false;
        }

        private void NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                LoadDoctorsAsync();
            }
        }

        private void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                LoadDoctorsAsync();
            }
        }

        private async Task LoadDoctorsAsync()
        {
            try
            {
                var response = await _apiService.GetDoctorsAsync(CurrentPage, 10, SearchText);
                Doctors = new ObservableCollection<Doctor>(response.Items);
                TotalPages = response.TotalPages;
            }
            catch (Exception ex)
            {
                // Handle error
                await ShowErrorMessage($"Error loading doctors: {ex.Message}");
            }
        }

        // SignalR event handlers
        private void OnDoctorAdded(Doctor doctor)
        {
            // Add to collection if we're on the first page or refresh the list
            if (CurrentPage == 1)
            {
                Doctors.Insert(0, doctor);
                if (Doctors.Count > 10)
                {
                    Doctors.RemoveAt(Doctors.Count - 1);
                }
            }
            else
            {
                LoadDoctorsAsync();
            }
        }

        private void OnDoctorUpdated(Doctor doctor)
        {
            // Update in our collection if present
            var existingDoctor = Doctors.FirstOrDefault(d => d.Id == doctor.Id);
            if (existingDoctor != null)
            {
                var index = Doctors.IndexOf(existingDoctor);
                Doctors[index] = doctor;
            }
        }

        private void OnDoctorDeleted(int doctorId)
        {
            // Remove from our collection if present
            var existingDoctor = Doctors.FirstOrDefault(d => d.Id == doctorId);
            if (existingDoctor != null)
            {
                Doctors.Remove(existingDoctor);
            }
        }

        private async Task ShowErrorMessage(string message)
        {
            await MessageBox.Avalonia.MessageBoxManager
                .GetMessageBoxStandardWindow("Error", message)
                .ShowDialog();
        }
    }
}
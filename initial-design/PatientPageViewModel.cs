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
    public class PatientPageViewModel : ViewModelBase
    {
        private readonly ApiService _apiService;
        private readonly SignalRService _signalRService;

        // Patient List Properties
        private ObservableCollection<Patient> _patients;
        public ObservableCollection<Patient> Patients
        {
            get => _patients;
            set => this.RaiseAndSetIfChanged(ref _patients, value);
        }

        // Selected Patient and Current Patient for editing
        private Patient _selectedPatient;
        public Patient SelectedPatient
        {
            get => _selectedPatient;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedPatient, value);
                if (value != null && IsMedicalRecordsVisible)
                {
                    LoadMedicalRecordsAsync(value.Id);
                }
            }
        }

        private Patient _currentPatient;
        public Patient CurrentPatient
        {
            get => _currentPatient;
            set => this.RaiseAndSetIfChanged(ref _currentPatient, value);
        }

        // Medical Records
        private ObservableCollection<MedicalRecord> _medicalRecords;
        public ObservableCollection<MedicalRecord> MedicalRecords
        {
            get => _medicalRecords;
            set => this.RaiseAndSetIfChanged(ref _medicalRecords, value);
        }

        // UI State Properties
        private bool _isEditMode;
        public bool IsEditMode
        {
            get => _isEditMode;
            set => this.RaiseAndSetIfChanged(ref _isEditMode, value);
        }

        private bool _isMedicalRecordsVisible;
        public bool IsMedicalRecordsVisible
        {
            get => _isMedicalRecordsVisible;
            set => this.RaiseAndSetIfChanged(ref _isMedicalRecordsVisible, value);
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
                LoadPatientsAsync();
            }
        }

        // Dropdown options
        public List<string> GenderOptions { get; } = new List<string> { "Male", "Female", "Other", "Prefer not to say" };

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
        public ICommand AddPatientCommand { get; }
        public ICommand EditPatientCommand { get; }
        public ICommand DeletePatientCommand { get; }
        public ICommand SavePatientCommand { get; }
        public ICommand CancelEditCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand ViewMedicalRecordsCommand { get; }
        public ICommand AddMedicalRecordCommand { get; }
        public ICommand EditMedicalRecordCommand { get; }
        public ICommand DeleteMedicalRecordCommand { get; }

        public PatientPageViewModel(ApiService apiService, SignalRService signalRService)
        {
            _apiService = apiService;
            _signalRService = signalRService;
            
            // Initialize collections
            Patients = new ObservableCollection<Patient>();
            MedicalRecords = new ObservableCollection<MedicalRecord>();
            
            // Setup commands
            AddPatientCommand = ReactiveCommand.Create(AddPatient);
            EditPatientCommand = ReactiveCommand.Create<Patient>(EditPatient);
            DeletePatientCommand = ReactiveCommand.Create<Patient>(DeletePatient);
            SavePatientCommand = ReactiveCommand.CreateFromTask(SavePatientAsync);
            CancelEditCommand = ReactiveCommand.Create(CancelEdit);
            NextPageCommand = ReactiveCommand.Create(NextPage);
            PreviousPageCommand = ReactiveCommand.Create(PreviousPage);
            ViewMedicalRecordsCommand = ReactiveCommand.Create<Patient>(ViewMedicalRecords);
            AddMedicalRecordCommand = ReactiveCommand.Create(AddMedicalRecord);
            EditMedicalRecordCommand = ReactiveCommand.Create<MedicalRecord>(EditMedicalRecord);
            DeleteMedicalRecordCommand = ReactiveCommand.Create<MedicalRecord>(DeleteMedicalRecord);
            
            // Setup SignalR event handlers
            _signalRService.PatientAdded += OnPatientAdded;
            _signalRService.PatientUpdated += OnPatientUpdated;
            _signalRService.PatientDeleted += OnPatientDeleted;
            _signalRService.MedicalRecordAdded += OnMedicalRecordAdded;
            _signalRService.MedicalRecordUpdated += OnMedicalRecordUpdated;
            _signalRService.MedicalRecordDeleted += OnMedicalRecordDeleted;
            
            // Initial load
            LoadPatientsAsync();
        }

        private void AddPatient()
        {
            CurrentPatient = new Patient { DateOfBirth = DateTime.Now.AddYears(-30) };
            FormTitle = "Add New Patient";
            IsEditMode = true;
            IsMedicalRecordsVisible = false;
        }

        private void EditPatient(Patient patient)
        {
            CurrentPatient = patient.Clone();
            FormTitle = "Edit Patient";
            IsEditMode = true;
            IsMedicalRecordsVisible = false;
        }

        private void DeletePatient(Patient patient)
        {
            // Confirm deletion
            var result = MessageBox.Avalonia.MessageBoxManager
                .GetMessageBoxStandardWindow("Confirm Delete", $"Are you sure you want to delete {patient.Name}?",
                    MessageBox.Avalonia.Enums.ButtonEnum.YesNo)
                .ShowDialog();
            
            if (result == MessageBox.Avalonia.Enums.ButtonResult.Yes)
            {
                DeletePatientAsync(patient);
            }
        }

        private async Task DeletePatientAsync(Patient patient)
        {
            try
            {
                await _apiService.DeletePatientAsync(patient.Id);
                Patients.Remove(patient);
            }
            catch (Exception ex)
            {
                // Handle error
                await ShowErrorMessage($"Error deleting patient: {ex.Message}");
            }
        }

        private async Task SavePatientAsync()
        {
            try
            {
                if (CurrentPatient.Id == 0)
                {
                    // Add new patient
                    var addedPatient = await _apiService.AddPatientAsync(CurrentPatient);
                    Patients.Add(addedPatient);
                }
                else
                {
                    // Update existing patient
                    await _apiService.UpdatePatientAsync(CurrentPatient);
                    
                    // Find and update in the collection
                    var index = Patients.IndexOf(Patients.First(p => p.Id == CurrentPatient.Id));
                    if (index >= 0)
                    {
                        Patients[index] = CurrentPatient;
                    }
                }
                
                IsEditMode = false;
            }
            catch (Exception ex)
            {
                // Handle error
                await ShowErrorMessage($"Error saving patient: {ex.Message}");
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
                LoadPatientsAsync();
            }
        }

        private void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                LoadPatientsAsync();
            }
        }

        private void ViewMedicalRecords(Patient patient)
        {
            SelectedPatient = patient;
            IsMedicalRecordsVisible = true;
            IsEditMode = false;
            LoadMedicalRecordsAsync(patient.Id);
        }

        private void AddMedicalRecord()
        {
            // Navigate to medical record form or show dialog
            // This could open a new view or dialog for adding a medical record
            // For now, we'll just show a placeholder message
            ShowInfoMessage("Add Medical Record form would open here");
        }

        private void EditMedicalRecord(MedicalRecord record)
        {
            // Navigate to medical record form or show dialog
            ShowInfoMessage("Edit Medical Record form would open here");
        }

        private void DeleteMedical
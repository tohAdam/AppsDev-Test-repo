using System;
using Avalonia;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using HospitalApp.Models;
using Avalonia.Controls;
using HospitalApp.Views.HelperWindows;
using Avalonia.Controls.ApplicationLifetimes;

namespace HospitalApp.ViewModels
{
    public class DoctorPageViewModel : ViewModelBase
    {
        private string _searchText = string.Empty;
        private ObservableCollection<Doctor> _filteredDoctors;
        private Doctor _selectedDoctor;
        private bool _isEditing;

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Doctor> Doctors { get; } = new()
        {
            new Doctor
            {
                Id = 1,
                Name = "Dr. Sarah Johnson",
                Email = "sarah.johnson@hospital.com",
                Username = "sjohnson",
                Password = "********",
                Sex = "Female",
                ContactNumber = "555-123-4567",
                Birthday = new DateTime(1985, 5, 15),
                Role = "Doctor",
                specialization = "Cardiology",
                is_available = 1
            },
            new Doctor
            {
                Id = 2,
                Name = "Dr. Michael Chen",
                Email = "michael.chen@hospital.com",
                Username = "mchen",
                Password = "********",
                Sex = "Male",
                ContactNumber = "555-234-5678",
                Birthday = new DateTime(1978, 10, 3),
                Role = "Doctor",
                specialization = "Neurology",
                is_available = 1
            },
            new Doctor
            {
                Id = 3,
                Name = "Dr. Emily Rodriguez",
                Email = "emily.rodriguez@hospital.com",
                Username = "erodriguez",
                Password = "********",
                Sex = "Female",
                ContactNumber = "555-345-6789",
                Birthday = new DateTime(1990, 3, 22),
                Role = "Doctor",
                specialization = "Pediatrics",
                is_available = 0
            },
            new Doctor
            {
                Id = 4,
                Name = "Dr. James Williams",
                Email = "james.williams@hospital.com",
                Username = "jwilliams",
                Password = "********",
                Sex = "Male",
                ContactNumber = "555-456-7890",
                Birthday = new DateTime(1972, 7, 11),
                Role = "Doctor",
                specialization = "Orthopedics",
                is_available = 1
            },
            new Doctor
            {
                Id = 5,
                Name = "Dr. Lisa Patel",
                Email = "lisa.patel@hospital.com",
                Username = "lpatel",
                Password = "********",
                Sex = "Female",
                ContactNumber = "555-567-8901",
                Birthday = new DateTime(1987, 12, 5),
                Role = "Doctor",
                specialization = "Dermatology",
                is_available = 1
            },
        };

        // Commands (no implementation)
        public ICommand AddDoctorCommand { get; }
        public ICommand EditDoctorCommand { get; }
        public ICommand ContactDoctorCommand { get; }

        public DoctorPageViewModel()
        {
            _filteredDoctors = new ObservableCollection<Doctor>(Doctors);
            _isEditing = false;

            AddDoctorCommand = new RelayCommand(_ => { });
            EditDoctorCommand = new RelayCommand(_ => { }, _ => SelectedDoctor != null);
            ContactDoctorCommand = new RelayCommand(_ => { }, _ => SelectedDoctor != null);
        }

        public ObservableCollection<Doctor> FilteredDoctors
        {
            get => _filteredDoctors;
            private set
            {
                _filteredDoctors = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    FilterDoctors();
                }
            }
        }

        public Doctor SelectedDoctor
        {
            get => _selectedDoctor;
            set
            {
                _selectedDoctor = value;
                OnPropertyChanged();
                IsEditing = false;
                ((RelayCommand)EditDoctorCommand)?.RaiseCanExecuteChanged();
                ((RelayCommand)ContactDoctorCommand)?.RaiseCanExecuteChanged();
            }
        }

        private void FilterDoctors()
        {
            if (string.IsNullOrWhiteSpace(_searchText))
            {
                FilteredDoctors = new ObservableCollection<Doctor>(Doctors);
            }
            else
            {
                var filtered = Doctors
                    .Where(d => d.Name.ToLower().Contains(_searchText.ToLower()) ||
                                d.specialization.ToLower().Contains(_searchText.ToLower()))
                    .ToList();

                FilteredDoctors = new ObservableCollection<Doctor>(filtered);
            }
        }
    }
}

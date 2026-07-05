using Castor.database.tab_medis;
using Castor.gui.commities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;


namespace Castor.gui.find
{
    public class PatientSearchViewModel : INotifyPropertyChanged
    {
        private readonly MedisContext _context; // замени на имя своего контекста

        public PatientSearchViewModel(MedisContext context)
        {
            _context = context;
            SearchCommand = new RelayCommand(ExecuteSearch);
        }

        private string? _searchFio;
        private string? _searchSnils;
        private ObservableCollection<patient> _patients = new();
        private patient? _selectedPatient;
        private ObservableCollection<visit> _visits = new();

        public string? SearchFio
        {
            get => _searchFio;
            set { _searchFio = value; OnPropertyChanged(); }
        }

        public string? SearchSnils
        {
            get => _searchSnils;
            set { _searchSnils = value; OnPropertyChanged(); }
        }

        public ObservableCollection<patient> Patients
        {
            get => _patients;
            set { _patients = value; OnPropertyChanged(); }
        }

        public patient? SelectedPatient
        {
            get => _selectedPatient;
            set
            {
                _selectedPatient = value;
                OnPropertyChanged();
                LoadVisits();
            }
        }

        public ObservableCollection<visit> Visits
        {
            get => _visits;
            set { _visits = value; OnPropertyChanged(); }
        }

        public ICommand SearchCommand { get; }

        private void ExecuteSearch()
        {
            var query = _context.Set<patient>().AsQueryable();

            bool hasFio = !string.IsNullOrWhiteSpace(SearchFio);
            bool hasSnils = !string.IsNullOrWhiteSpace(SearchSnils);

            if (!hasFio && !hasSnils)
            {
                Patients = new ObservableCollection<patient>(query.Take(50).ToList());
                return;
            }

            // Разбиваем ФИО на слова, убираем пустые элементы
            string[]? fioParts = null;
            if (hasFio)
            {
                fioParts = SearchFio
                    .Trim()
                    .Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            }

            if (fioParts != null && fioParts.Length > 0)
            {
                // 1-е слово — фамилия
                string lastNameTerm = fioParts[0];
                query = query.Where(p =>
                    !string.IsNullOrEmpty(p.lastname) &&
                    p.lastname.Trim().Contains(lastNameTerm));

                // 2-е слово — имя (если есть)
                if (fioParts.Length >= 2)
                {
                    string firstNameTerm = fioParts[1];
                    query = query.Where(p =>
                        !string.IsNullOrEmpty(p.firstname) &&
                        p.firstname.Trim().Contains(firstNameTerm));
                }

                // 3-е слово — отчество (если есть)
                if (fioParts.Length >= 3)
                {
                    string secondNameTerm = fioParts[2];
                    query = query.Where(p =>
                        !string.IsNullOrEmpty(p.secondname) &&
                        p.secondname.Trim().Contains(secondNameTerm));
                }
            }

            if (hasSnils)
            {
                string snilsTerm = SearchSnils.Trim();
                query = query.Where(p => !string.IsNullOrEmpty(p.snils) && p.snils.Contains(snilsTerm));
            }

            Patients = new ObservableCollection<patient>(query.Take(200).ToList());
        }

        private void LoadVisits()
        {
            if (SelectedPatient == null)
            {
                Visits.Clear();
                return;
            }

            var visits = _context.Set<visit>()
                .Where(v => v.patientid == SelectedPatient.keyid)
                .OrderByDescending(v => v.dat)
                .Take(100)
                .ToList();

            Visits = new ObservableCollection<visit>(visits);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
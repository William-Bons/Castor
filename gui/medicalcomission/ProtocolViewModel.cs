using Castor.database.tables;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;

namespace Castor.gui.medicalcomission
{
    public class ProtocolViewModel : INotifyPropertyChanged
    {
        public ProtocolViewModel()
        {
            Members = new ObservableCollection<CommissionMember>();
            AgendaItems = new ObservableCollection<ProtocolAgendaItem>();
            SaveCommand = new RelayCommand(Save);
            AddMemberCommand = new RelayCommand(AddMember);
            RemoveMemberCommand = new RelayCommand(RemoveMember);
            SetChairmanCommand = new RelayCommand(() => SetRole(CommissionMemberRole.Chairman));
            SetSecretaryCommand = new RelayCommand(() => SetRole(CommissionMemberRole.Secretary));
        }

        public ProtocolViewModel(MedicalCommissionProtocol protocol)
        {
            _protocol = protocol;
            Members = new ObservableCollection<CommissionMember>(protocol.Members);

            SaveCommand = new RelayCommand(Save);
            AddMemberCommand = new RelayCommand(AddMember);
            RemoveMemberCommand = new RelayCommand(RemoveMember);
            SetChairmanCommand = new RelayCommand(() => SetRole(CommissionMemberRole.Chairman));
            SetSecretaryCommand = new RelayCommand(() => SetRole(CommissionMemberRole.Secretary));
        }

        private string _protocolNumber;
        public string ProtocolNumber
        {
            get => _protocolNumber;
            set { _protocolNumber = value; OnPropertyChanged(); }
        }

        private DateTime _protocolDate = DateTime.Today;
        public DateTime ProtocolDate
        {
            get => _protocolDate;
            set { _protocolDate = value; OnPropertyChanged(); }
        }

        private TimeSpan? _protocolTime;
        public TimeSpan? ProtocolTime
        {
            get => _protocolTime;
            set { _protocolTime = value; OnPropertyChanged(); }
        }

        public string Location { get; set; }
        public string CommissionType { get; set; }
        public List<string> RolesList { get; set; }

        public ObservableCollection<CommissionMember> Members { get; }
        public ObservableCollection<ProtocolAgendaItem> AgendaItems { get; }

        public int VotesFor { get; set; }
        public int VotesAgainst { get; set; }
        public int VotesAbstained { get; set; }

        public string DecisionText { get; set; }
        public string SpecialOpinionText { get; set; }
        public string SpecialOpinionAuthorName { get; set; }
        public string SpecialOpinionAuthorPosition { get; set; }

        public ICommand SaveCommand { get; }

        private void Save()
        {
            // Здесь будет логика сохранения в БД через EF
            // var protocol = new MedicalCommissionProtocol { ... }
        }

#region Members
        private readonly MedicalCommissionProtocol _protocol;
        
        private CommissionMember _selectedMember;
        public CommissionMember SelectedMember
        {
            get => _selectedMember;
            set { _selectedMember = value; OnPropertyChanged(); }
        }

        public ICommand AddMemberCommand { get; }
        public ICommand RemoveMemberCommand { get; }
        public ICommand SetChairmanCommand { get; }
        public ICommand SetSecretaryCommand { get; }

        

        private void AddMember()
        {
            System.Diagnostics.Debug.WriteLine("[AddMember] Вызов метода. Текущее количество членов: " + Members.Count);

            var newMember = new CommissionMember
            {
                FullName = "",
                Position = "",
                Role = CommissionMemberRole.Member
            };
            Members.Add(newMember);
            _protocol.Members.Add(newMember); // синхронизация с сущностью EF
            System.Diagnostics.Debug.WriteLine("[AddMember] После добавления: " + Members.Count);
        }

        private bool CanRemoveMember() => SelectedMember != null;

        private void RemoveMember()
        {
            if (SelectedMember == null) return;
            Members.Remove(SelectedMember);
            _protocol.Members.Remove(SelectedMember);
            SelectedMember = null;
        }

        private void SetRole(CommissionMemberRole role)
        {
            if (SelectedMember == null) return;
            SelectedMember.Role = role;
            OnPropertyChanged(nameof(Members));
        }
#endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // Простая реализация RelayCommand
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();
        public void Execute(object parameter) => _execute();

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    public class HasValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

}
using Castor.database;
using Castor.database.tables;
using Castor.gui.common;
using Castor.gui.dialogs;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Castor.gui.force
{
    public class ForcedViewModel : INotifyPropertyChanged
    {
        private Forced? _selectedItem;
        private Movebook? _movebook;

        // Данные пациента (только для отображения вверху формы)
        public string? PatientInfo => $"Пациент: #{_movebook?.Fio ?? string.Empty} | Визит: #{SelectedItem?.Visitid ?? 0}";
        public ObservableCollection<Forced> RootTreeItem { get; private set; } = new();
        public Forced? SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanEdit));
                OnPropertyChanged(nameof(IsNew));
            }
        }
        public bool CanEdit => SelectedItem != null;
        public Visibility IsNew => (SelectedItem?.Id ?? 0) == 0 ? Visibility.Visible : Visibility.Collapsed;
        public ICommand? RefreshCommand { get; }
        public ICommand? AddCommand { get; }
        public ICommand? SetBaseCommand { get; }
        public ICommand? SaveCommand { get; }

        public ForcedViewModel(Movebook mb)
        {
            if (mb == null) return;

            _movebook = mb;
            RefreshCommand = new RelayCommand(LoadTree);
            AddCommand = new RelayCommand(AddItem);
            SetBaseCommand = new RelayCommand(SetBaseFlag);
            SaveCommand = new RelayCommand(Save);

            LoadTree();
        }

        private void Save()
        {
            if(RootTreeItem.Count()>0)
            {
                using CastorContext context = new CastorContext();

                context.Update(RootTreeItem.FirstOrDefault());

                if (RootTreeItem.FirstOrDefault().AllForces != null)
                {
                    // ессли Id == 0 то запись добавлена, если Id уже есть то отредактирована
                    foreach (var item in RootTreeItem.FirstOrDefault().AllForces)
                    {
                        if(item.Id>0) context.Update(item);
                        else context.Add(item);
                    }
                }
                context.SaveChanges();
            }
        }

        private void SetBaseFlag()
        {
            int calcMonth = SelectedItem?.Start.Month ?? 1;
            ReportSettingsWindow rsw = new ReportSettingsWindow();
            rsw.SelectedRange = calcMonth;
            if (rsw.ShowDialog()==true)
            {

                if (RootTreeItem.Count() > 0 && SelectedItem != null)
                {
                    RootTreeItem.FirstOrDefault().MonthFlag = rsw.SelectedRange;
                    if (RootTreeItem.FirstOrDefault()?.AllForces?.Count > 0)
                    {
                        foreach (var item in RootTreeItem.FirstOrDefault().AllForces)
                        {
                            item.MonthFlag = rsw.SelectedRange;
                        }
                    }
                    LoadTree();
                    OnPropertyChanged(nameof(SelectedItem));
                    OnPropertyChanged(nameof(RootTreeItem));
                }
            }
        }

        private void AddItem()
        {
            if(_movebook!=null)
            {
                // добавление последующих
                var _rootForced = RootTreeItem.FirstOrDefault();
                SelectedItem = new Forced()
                {
                    Movebookid = _movebook.Id,
                    Patientid = _movebook.Patientid ?? 0,
                    Visitid = _movebook.Visitid ?? 0,
                    RootId = _rootForced?.Id ?? 0,
                    Start = _rootForced?.End ?? DateOnly.FromDateTime(DateTime.Now),
                    Courtname = _rootForced?.Courtname,
                    Section = _rootForced?.Section,
                    Type = _rootForced?.Type 
                };
                if (_rootForced.AllForces == null) _rootForced.AllForces = new List<Forced>();
                _rootForced?.AllForces?.Add(SelectedItem);
                LoadTree();
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        /// <summary>
        /// Загружает Forceds из Movebook или добавляет первого нового
        /// </summary>
        public void LoadTree()
        {
            RootTreeItem.Clear();

            // получает Root FORCED для пациента в ввиде списка только раз
            if (RootTreeItem.Count()==0 && _movebook!=null)
            {
                RootTreeItem.Add(_movebook.Forceds
                   .Where(f => f.RootId == null)
                   .ToList().FirstOrDefault() ?? new Forced()
                   {
                       Movebookid = _movebook.Id,
                       Patientid = _movebook.Patientid ?? 0,
                       Visitid = _movebook.Visitid ?? 0,
                       Start = _movebook.Datein ?? DateOnly.FromDateTime(DateTime.Now),
                       MonthFlag = 1
                   });
                    
            }

            OnPropertyChanged(nameof(PatientInfo));
            OnPropertyChanged(nameof(RootTreeItem));
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

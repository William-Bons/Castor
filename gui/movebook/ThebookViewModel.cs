using Castor.database;
using Castor.database.tab_medis;
using Castor.database.tables;
using Castor.gui.commities;
using Castor.gui.common;
using Castor.gui.dialogs;
using Castor.gui.force;
using Castor.Properties;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Castor.gui.movebook
{
    public class ThebookViewModel : INotifyPropertyChanged
    {
        public ThebookViewModel()
        {
            DisorderPatientCommand = new RelayCommandAsync(DisorderPatient);
            DeleteCommand = new RelayCommand(RemovePatient);
            FilterCommand = new RelayCommand(execute: param => PrepareFilter(param));
            EditCommand = new RelayCommandAsync(EditMovebookItemWindowAsync);
            ShowVisitsCommand = new RelayCommand(ShowAllVisistsForPatient);
            ShowDsCommand = new RelayCommand(ShowAllDiagnosis);
        }

        public Movebook? Selected {  get; set; }
        public ICollection<Movebook>? LoadedData { get; private set; } = new List<Movebook>();
        public MoveFilter FilterPeriod { get; set; } = new();
        public ICommand DisorderPatientCommand {  get; set; }
        public ICommand FilterCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; }
        public ICommand? ShowVisitsCommand { get; }
        public ICommand? ShowDsCommand { get; }

        // Это свойство привязано к IsChecked ToggleButton (и тем самым закрывает Popup)
        public bool IsMenuOpen
        {
            get => _isMenuOpen;
            set
            {
                _isMenuOpen = value;
                OnPropertyChanged(); // INotifyPropertyChanged
            }
        }
        private bool _isMenuOpen;

        private async Task PrepareFilter(object? param)
        {
            switch (param ?? string.Empty)
            {
                case "C":
                    FilterPeriod.IsHideClosedCards = !FilterPeriod.IsHideClosedCards;
                    await Load();
                    break;

                case "D":
                    FilterPeriod.IsHideDisordered = !FilterPeriod.IsHideDisordered;
                    await Load();
                    break;

                case "P":
                    FilterPeriod = SelectDatePeriod.Show();
                    await Load();
                    break;
            }
            IsMenuOpen = false;
        }


        /// <summary>
        /// Загружает данные из базы в LoadedData при установленных флагах фильтрует ЗАКРЫТЫЕ и ВЫПИСАННЫЕ
        /// </summary>
        public async Task Load()
        {
            using CastorContext context = new CastorContext();

            try
            {
                if (FilterPeriod?.DatesSet ?? false)
                {
                    LoadedData = context.Movebooks
                        .Where(x => (x.Datein >= DateOnly.FromDateTime(FilterPeriod.Start) && x.Datein <= DateOnly.FromDateTime(FilterPeriod.End)) ||
                        (x.Dateout >= DateOnly.FromDateTime(FilterPeriod.Start) && x.Dateout <= DateOnly.FromDateTime(FilterPeriod.End)))
                        .Include(x => x.Forceds)
                        .Include(x => x.Commities)
                        .ToList();
                }
                else
                {
                    LoadedData = context.Movebooks
                        .Include(x => x.Forceds)
                        .Include(x => x.Commities)
                        .ToList();
                    ;
                }

                // hide closed cards!
                if (FilterPeriod?.IsHideClosedCards ?? false)
                {
                    LoadedData = LoadedData.Where(l => !l.Closed).ToList();
                }

                // hide disordered cards
                if (FilterPeriod?.IsHideDisordered ?? false)
                {
                    LoadedData = LoadedData.Where(l => !l.Dateout.HasValue).ToList();
                }
            }
            catch (Exception ex) 
            { 
                Debug.WriteLine(ex.Message);
            }

            OnPropertyChanged(nameof(LoadedData));

        }

        public async Task Save()
        {
            using CastorContext context = new CastorContext();
            try
            {
                //todo ??? LoadedData.Where(x => x.Dateout == DateOnly.MinValue).ToList().ForEach(x => x.Dateout = null);
                context.UpdateRange(LoadedData ?? new List<Movebook>());
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public async Task DisorderPatient()
        {
            new MovebookEdit(Selected, 1).ShowDialog();
            await Load();
        }

        public void RemovePatient()
        {
            if (MessageBox.Show("Удалить строку из базы данных?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes && Selected is Movebook _mbItem)
            {
                using CastorContext context = new CastorContext();
                context.Movebooks.Remove(_mbItem);
                context.SaveChanges();
                Task.Run(async () => await Load());
            }
        }

        public void OpenPMMWindow()
        {
            if (Selected is Movebook _mbItem)
            {
                new ForcedTreeWindow(Selected).ShowDialog();
                Task.Run(async () => await Load());
            }
        }

        internal void OpenCommityWindow()
        {
            new CommityForm(Selected).ShowDialog();
        }
        internal async Task EditMovebookItemWindowAsync()
        {
            new MovebookEdit(Selected,0).ShowDialog();
            await Load();
        }

        private void ShowAllDiagnosis()
        {
            try
            {
                if (Selected is Movebook _row)
                {
                    using MedisContext medis = new MedisContext();
                    ICollection<patdiag> dss = medis.patdiag
                        .Where(p => p.patientid == _row.Patientid)
                        .Include(p => p.Diagnos)
                        .ToList();

                    DataGrid dataGrid = new DataGrid() { Language = XmlLanguage.GetLanguage("ru-RU") };
                    dataGrid.ItemsSource = dss;
                    Window window = new Window();
                    window.Content = dataGrid;
                    window.Show();
                }
            }
            catch (Exception ex)
            {
                Message.ShowPopup(ex.Message);
            }
        }

        /// <summary>
        /// отбирает все визиты пациента в указанное учреждение. Всключая закрытые истории и поступления в п/п
        /// </summary>
        private void ShowAllVisistsForPatient()
        {
            try
            {
                if (Selected is Movebook _row)
                {
                    using MedisContext medis = new MedisContext();

                    ICollection<visit> dss = medis.visit
                        .Where(v => v.patientid == _row.Patientid)
                        .Include(v => v.Dep)
                        //.Where(w => w.Dep.rootid == Settings.Default.RootDepartmentId)
                        .ToList();

                    DataGrid dataGrid = new DataGrid() { Language = XmlLanguage.GetLanguage("ru-RU") };
                    dataGrid.ItemsSource = dss;
                    Window window = new Window();
                    window.Content = dataGrid;
                    window.Show();
                }
            }
            catch (Exception ex)
            {
                Message.ShowPopup(ex.Message);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


    }
}

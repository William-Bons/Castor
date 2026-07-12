using Castor.database;
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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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
        }

        public Movebook? Selected {  get; set; }
        public ICollection<Movebook>? LoadedData { get; private set; } = new List<Movebook>();
        public MoveFilter FilterPeriod { get; set; } = new();
        public ICommand DisorderPatientCommand {  get; set; }
        public ICommand FilterCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; }

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
            catch { }

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
                Task.Run(() => Load());
            }
        }

        public void OpenPMMWindow()
        {
            new ForcedTreeWindow(Selected).ShowDialog();
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

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


    }
}

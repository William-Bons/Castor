using Castor.database;
using Castor.database.tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Castor.gui.force
{
    public class MonthsPanelViewModel
    {
        public MonthsPanelViewModel()
        {
            RefreshAsync();
        }

        // --- Свойства для биндинга (ровно те, что в XAML) ---
        public ObservableCollection<Forced> PatientsMonth1 { get; private set; } = new();
        public ObservableCollection<Forced> PatientsMonth2 { get; private set; } = new();
        public ObservableCollection<Forced> PatientsMonth3 { get; private set; } = new();
        public ObservableCollection<Forced> PatientsMonth4 { get; private set; } = new();
        public ObservableCollection<Forced> PatientsMonth5 { get; private set; } = new();
        public ObservableCollection<Forced> PatientsMonth6 { get; private set; } = new();
        public ObservableCollection<Forced> PatientsMonth7 { get; private set; } = new();
        public ObservableCollection<Forced> PatientsMonth8 { get; private set; } = new();   
        public ObservableCollection<Forced> PatientsMonth9 { get; private set; } = new();
        public ObservableCollection<Forced> PatientsMonth10 { get; private set; } = new();
        public ObservableCollection<Forced> PatientsMonth11 { get; private set; } = new();
        public ObservableCollection<Forced> PatientsMonth12 { get; private set; } = new();

        /// <summary>
        /// Загружает данные по месяцам и раскладывает по коллекциям.
        /// Вызывать асинхронно после загрузки страницы (например, в Page.Loaded).
        /// </summary>
        public void RefreshAsync()
        {
            try
            {
                // Очищаем все коллекции перед новой загрузкой
                ClearAllMonths();

                using CastorContext _context = new CastorContext();
                var data = _context.Forced
                    .Include(f => f.RootForced)
                    .Include(f => f.Movebook)           // Важно: подгружаем связанного пациента
                    .Where(f => f.RootId==null)         // Только записи ROOT
                    .ToList();



                foreach (Forced item in data)
                {
                    string prop0 = $"PatientsMonth{item.Month[0]}";
                    string prop1 = $"PatientsMonth{item.Month[1]}";

                    (GetType().GetProperty(prop0).GetValue(this) as ObservableCollection<Forced>)?
                        .Add(item);

                    (GetType().GetProperty(prop1).GetValue(this) as ObservableCollection<Forced>)?
                        .Add(item);

                    OnPropertyChanged(prop0);
                    OnPropertyChanged(prop1);
                }
                ;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void ClearAllMonths()
        {
            PatientsMonth1.Clear(); PatientsMonth2.Clear(); PatientsMonth3.Clear();
            PatientsMonth4.Clear(); PatientsMonth5.Clear(); PatientsMonth6.Clear();
            PatientsMonth7.Clear(); PatientsMonth8.Clear(); PatientsMonth9.Clear();
            PatientsMonth10.Clear(); PatientsMonth11.Clear(); PatientsMonth12.Clear();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

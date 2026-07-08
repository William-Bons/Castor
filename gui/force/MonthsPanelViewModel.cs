using Castor.database;
using Castor.database.tab_medis;
using Castor.database.tables;
using Castor.Properties;
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
        public ObservableCollection<CombinedRow> PatientsMonth1 { get; private set; } = new();
        public ObservableCollection<CombinedRow> PatientsMonth2 { get; private set; } = new();
        public ObservableCollection<CombinedRow> PatientsMonth3 { get; private set; } = new();
        public ObservableCollection<CombinedRow> PatientsMonth4 { get; private set; } = new();
        public ObservableCollection<CombinedRow> PatientsMonth5 { get; private set; } = new();
        public ObservableCollection<CombinedRow> PatientsMonth6 { get; private set; } = new();
        public ObservableCollection<CombinedRow> PatientsMonth7 { get; private set; } = new();
        public ObservableCollection<CombinedRow> PatientsMonth8 { get; private set; } = new();   
        public ObservableCollection<CombinedRow> PatientsMonth9 { get; private set; } = new();
        public ObservableCollection<CombinedRow> PatientsMonth10 { get; private set; } = new();
        public ObservableCollection<CombinedRow> PatientsMonth11 { get; private set; } = new();
        public ObservableCollection<CombinedRow> PatientsMonth12 { get; private set; } = new();

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


                // список визитов пациентов нваходящихся в настоящее время в отделении исключая номера историй уже заруженных в базу (по castorIds)
                using MedisContext medisContext = new MedisContext();
                var __DataRowSource = medisContext.visit
                    .Where(v => v.depid == Settings.Default.LastSelectedDepId && !v.dat1.HasValue) // Только находящиеся в отделении
                    .Include(v => v.Doctor)
                    .Include(v => v.Patient)
                    .ToList()
                    .Select(v => (pid: v.Patient.num, doc: v.Doctor.text))
                    .ToList();

                var result = data.Join(
                    __DataRowSource,
                    a => a.Patientid,
                    b => b.pid,
                    (a, b) => new CombinedRow
                    {
                        Key = a.Patientid,
                        Force = a,
                        Doctor = b.doc ?? string.Empty
                    });

                foreach (CombinedRow item in result)
                {
                    string prop0 = $"PatientsMonth{item.Force.Month[0]}";
                    string prop1 = $"PatientsMonth{item.Force.Month[1]}";

                    (GetType().GetProperty(prop0).GetValue(this) as ObservableCollection<CombinedRow>)?
                        .Add(item);

                    (GetType().GetProperty(prop1).GetValue(this) as ObservableCollection<CombinedRow>)?
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

    public class CombinedRow
    {
        public long Key { get; set; } // PatientID
        public Forced? Force { get; set; } // Pat name
        public string? Doctor { get; set; } // Doc name
    }
}

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
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Castor.gui.force
{
    public class MonthsPanelViewModel
    {
        public MonthsPanelViewModel()
        {
            RefreshAsync();
        }

        // --- Свойства для биндинга (ровно те, что в XAML) ---
        public ObservableCollection<CombinedRow> PatientsMonth0 { get; private set; } = new(); // ошибочно загруженные
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
                var ForcesList = _context.Forced
                    .Include(f => f.RootForced)
                    .Include(f => f.Movebook)           // Важно: подгружаем связанного пациента
                    .Include(f => f.AllForces)
                    .Where(f => f.RootId == null)         // Только записи ROOT
                    .ToList();


                // список визитов пациентов нваходящихся в настоящее время в отделении исключая номера историй уже заруженных в базу (по castorIds)
                using MedisContext medisContext = new MedisContext();
                var visitsNDoctors = medisContext.visit
                    .Where(v => v.depid == Settings.Default.LastSelectedDepId && !v.dat1.HasValue) // Только находящиеся в отделении
                    .Include(v => v.Doctor)
                    .Include(v => v.Patient)
                    .ToList()
                    .Select(v => (pid: v.Patient.num, doc: v.Doctor.text, docid:v.Doctor.keyid))
                    .ToList();

                // объединение двух массивов данных ForcesList и visitsNDoctors ключи объединения ForcesList.Patientid, visitsNDoctors.pid результат в масив объектов CombinedRow
                IEnumerable<CombinedRow> result = ForcesList.Join(
                    visitsNDoctors,
                    _force => _force.Patientid,     // PK1
                    _visit => _visit.pid,           // PK2
                    (f, v) => new CombinedRow
                    {
                        Key = f.Patientid,
                        Force = f,
                        Doctor = v.doc ?? string.Empty,
                        Color = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString(_context.Users.Where(u => u.DocdepId == v.docid).Select(u => u.Color).First() ?? "#000000")
                            )
                    });

                foreach (CombinedRow item in result)
                {
                    string prop0 = $"PatientsMonth{item.Force.Month?[0] ?? 1/*0*/}";
                    string prop1 = $"PatientsMonth{item.Force.Month?[1] ?? 1/*0*/}";

                    (GetType().GetProperty(prop0).GetValue(this) as ObservableCollection<CombinedRow>)?
                        .Add(item);

                    (GetType().GetProperty(prop1).GetValue(this) as ObservableCollection<CombinedRow>)?
                        .Add(item);

                    OnPropertyChanged(prop0);
                    OnPropertyChanged(prop1);
                }
                ;
            }
            catch (Exception ex)
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
        public Brush? Color { get; set; } = Brushes.Black;
    }



}

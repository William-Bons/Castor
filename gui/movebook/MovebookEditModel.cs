using Castor.database;
using Castor.database.tab_medis;
using Castor.database.tables;
using Castor.gui.common;
using Castor.Properties;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Castor.gui.movebook
{
    internal class MovebookEditModel : INotifyPropertyChanged, IConsoleMessage
    {
        private string? _originalJson;

        public MovebookEditModel(object? scaff)
        {

            if (scaff is Movebook)
            {
                Movebook = (Movebook)scaff;

                // Сериализуем оригинал в JSON (один раз при открытии формы)
                var options = new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    PropertyNameCaseInsensitive = true
                };
                _originalJson = JsonSerializer.Serialize(scaff, options);
            }

            if (scaff is visit v && v.num>0)    MoveVisitToBook(v);

            SaveCommand = new RelayCommand(Save);
            SelectDiagnosis = new RelayCommand(execute: param => SelectDiagnosFromList(param));
            SelectPatient = new RelayCommand(SelectPatientFromMedis);
            CancelCommand = new RelayCommand(CancelChanges);
        }

        

        public Movebook Movebook { get; private set; } = new Movebook();
        public visit Visit { get; private set; } = new visit();
        public ICommand SaveCommand { get; }
        public ICommand SelectDiagnosis { get; }
        public ICommand SelectPatient { get;  }
        public ICommand CancelCommand { get; }

        private void Save()
        {
            //todo if (!Validate())


            CastorContext castor = new CastorContext();
            castor.Update(Movebook);
            castor.SaveChanges();
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        private void SelectPatientFromMedis()
        {
            using (MedisContext medisContext = new MedisContext())
            {
                try
                {
                    /* Запрос к Медис*/
                    ICollection<dep>? depList = medisContext.dep // отделения
                        .Where(d => d.keyid == Settings.Default.LastSelectedDepId) // где номер отделения = сохраненному в Settings
                        .Include(d => d.Visits.Where(v => !v.dat1.HasValue || (DateTime.Today.ToUniversalTime() - v.dat1).Value.Days < 8)) // все невыписанные (!v.dat1.HasValue) и выписанные менее 8 дней назад
                        .ThenInclude(v => v.Doctor)  // привязка доктора
                        .Include(d => d.Visits.Where(v => !v.dat1.HasValue || (DateTime.Today.ToUniversalTime() - v.dat1).Value.Days < 8))
                        .ThenInclude(v => v.Patient)  // привязка пациента
                        .ThenInclude(p => p.Diagnoses) // привязка диагнозов пациента
                        .ThenInclude(d => d.Diagnos)  // привязка к диагнозам пациента дианоза из мкб
                        .ToList();

                    var a = new SelectObjectFromEnumerable(depList.First().Visits, PlacementMode.Center, "dat", "Patient.fullname", "dat1");

                    a.Selected += (a) =>
                    {
                        MoveVisitToBook((visit)a);
                    };


                }
                catch (Exception ex)
                {
                    ConsoleMessage?.Invoke(ex.Message);
                }
            }
        }

        private void MoveVisitToBook(visit visit)
        {
            try
            {
                Visit = visit;

                using (MedisContext medisContext = new MedisContext())
                {
                    /* выборка из Медис: выбор диагноза верхнего уровня группы */
                    var DS = medisContext.diagnos
                        .Where(d => d.keyid == Visit.Patient.CurrentDs.Diagnos.rootid)?.First();

                    Movebook.Dsin = DS.code;
                }

                Movebook.Ordered = 0;
                Movebook.Card_Id = Visit.num;
                Movebook.Patientid = Visit.Patient?.num;
                Movebook.Fio = Visit.Patient?.fullname ?? string.Empty;
                Movebook.Birthdate = DateOnly.FromDateTime(Visit.Patient.birthdate.Value);
                Movebook.Datein = DateOnly.FromDateTime(Visit.dat.Value);
                Movebook.Visitid = Visit?.keyid;
                Movebook.Dateout = DateOnly.FromDateTime(Visit.dat1.Value);
                Movebook.Outto = Visit.dat1.HasValue ? 0 : null;
                Movebook.Dsout = Visit.dat1.HasValue ? Movebook.Dsin : null;
            }
            catch { }

            OnPropertyChanged(nameof(Visit));
            OnPropertyChanged(nameof(Movebook));
        }

        /// <summary>
        /// выводит попап с диагнозами установленными на визит. при выборе заполняет соответсвующее параметру поле Movebook
        /// </summary>
        /// <param name="param">Название поля Movebook</param>
        private void SelectDiagnosFromList(object? param)
        {
            try
            {
                if(Visit == null || Visit.num == 0)
                {
                    using MedisContext medis = new MedisContext();
                    Visit = medis.visit
                        .Where(v => v.keyid == Movebook.Visitid)
                        .Include(v => v.Patient)
                        .ThenInclude(p => p.Diagnoses)
                        .ThenInclude(d => d.Diagnos)
                        .ToList()
                        .First();
                }

                var a = new SelectObjectFromEnumerable(Visit.Patient.Diagnoses.TakeLast(10), PlacementMode.MousePoint, "dat", "Diagnos.code", "Diagnos.text");
                a.Selected += (sel) =>
                {
                    using (MedisContext medisContext = new MedisContext())
                    {
                        var DS = medisContext.diagnos
                            .Where(d => d.keyid == (sel as patdiag).Diagnos.rootid)?.First();

                        if (param?.ToString() == "Dsout") Movebook.Dsout = DS?.code ?? string.Empty;
                        else if (param?.ToString() == "Dsin") Movebook.Dsin = DS?.code ?? string.Empty;

                        OnPropertyChanged(nameof(Movebook));
                    }
                };
            }
            catch { }
        }

        public void PrepareDisorder()
        {
            //PRELOAD
            try
            {
                using (MedisContext medis = new MedisContext())
                {
                    visit visit = medis.visit
                        .Where(v => v.keyid == Movebook.Visitid)
                        .Include(v => v.Patient)
                        .ThenInclude(v => v.Diagnoses).First();

                    if (visit != null && visit.Patient.CurrentDs.diagid != null && visit.dat1.HasValue)
                    {
                        var DS = medis.diagnos
                                .Where(d => d.keyid == visit.Patient.CurrentDs.diagid)?.First();

                        var RD = medis.diagnos
                            .Where(d => d.keyid == DS.rootid)?.First();

                        Movebook.Dateout = DateOnly.FromDateTime(visit.dat1.Value);
                        Movebook.Dsout = RD.code;
                    }
                    else
                    {
                        Movebook.Dateout = DateOnly.FromDateTime(DateTime.Today);
                        Movebook.Dsout = Movebook.Dsin;
                    }
                }

                Movebook.Outto = 0;
            }
            catch (Exception ex)
            {
                Message.ShowPopup(ex.Message);
            }
        }
        private void CancelChanges()
        {
            // откат изменний при нажатии отмены. работает только так
            var rest = JsonSerializer.Deserialize<Movebook>(_originalJson ?? string.Empty);
            Movebook.Datein = rest.Datein;
            Movebook.Dateout = rest.Dateout;
            Movebook.Ordered = rest.Ordered;
            Movebook.Dsin = rest.Dsin;
            Movebook.Dsout = rest.Dsout;
            Movebook.Outto = rest.Outto;
            Movebook.City = rest.City;
            Movebook.First = rest.First;
            Movebook.Second = rest.Second;
            Movebook.Early = rest.Early;
            Movebook.Date_Lastout = rest.Date_Lastout;
            Movebook.Closed = rest.Closed;
            Movebook.Deceased = rest.Deceased;

            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        //  событие, которое слушает окно
        public event EventHandler? RequestClose;
        public event PropertyChangedEventHandler? PropertyChanged;
        public event ConsoleMessageHandler? ConsoleMessage;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }

    public class FCodeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var s = value as string;
            if (string.IsNullOrWhiteSpace(s))
                return new ValidationResult(false, "Диагноз обязателен.");

            if (!s.Trim().StartsWith("F", StringComparison.InvariantCultureIgnoreCase))
                return new ValidationResult(false, "Должен начинаться с F.");

            return ValidationResult.ValidResult;
        }
    }
}

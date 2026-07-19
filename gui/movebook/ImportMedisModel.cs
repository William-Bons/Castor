using Castor.database;
using Castor.database.tab_medis;
using Castor.database.tables;
using Castor.gui.common;
using Castor.gui.dialogs;
using Castor.Properties;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Castor.gui.movebook
{
    public class ImportMedisModel : INotifyPropertyChanged
    {
        public MoveFilter ControlDate { get; set; } = new(30); // Инициализация даты в течение последнего месяца
        public ObservableCollection<MoveRow> DataRowSource { get; private set; } = new ObservableCollection<MoveRow>();
        public object SelectedDataItem { get; set; } = null!;
        public ICommand? FilterCommand { get; }
        public ICommand? SaveCommand { get; }
        public string StatusMessage { get; set; } = "Ready";
        public string Title => $"Список пациентов отделения из БД [{ControlDate.Start:d} - {ControlDate.End:d}]";

        public ImportMedisModel()
        {
            FilterCommand = new RelayCommand(SetFilterPeriod);
            SaveCommand = new RelayCommand(Save);
            
            Task.Run(async () => await Load());
        }

        private void SetFilterPeriod()
        {
            ControlDate = SelectDatePeriod.Show();
            Task.Run(async () => await Load());
        }


        /// <summary>
        /// Загружает визиты пациентов находящихся в отделении, в Root - поступление в п/п, оттуда - дата начала госпитализации
        /// </summary>
        public async Task Load()
        {
            try
            {
                // контексты
                using CastorContext castorContext = new CastorContext();
                using MedisContext medisContext = new MedisContext();


                // список номеров и/б уже загруженных в базу Кастор
                PlayMessage("Получение номеров загруженных");
                List<long> castorIds = castorContext.Movebooks
                    .Select(x => x.Card_Id)
                    .ToList();


                // список визитов пациентов нваходящихся в настоящее время в отделении исключая номера историй уже заруженных в базу (по castorIds)
                PlayMessage("Получение данных их МЕДИС");
                List<visit> __DataRowSource = medisContext.visit
                    .Where(v => v.depid == Settings.Default.LastSelectedDepId) // Только находящиеся в отделении
                    .Include(v => v.Root) // чаще всего это поступление в п/п
                    .Where(v => (v.Root.dat >= ControlDate.Start.ToUniversalTime() && v.Root.dat <= ControlDate.End.ToUniversalTime())
                           || (v.dat1.HasValue && v.dat1 <= ControlDate.End.ToUniversalTime() && v.dat1 >= ControlDate.Start.ToUniversalTime()))
                    .Include(v => v.Patient)  // привязка пациента
                    .ThenInclude(p => p.Diagnoses.Where(d => d.Diagnos.code.StartsWith("F"))) // диагнозы пациента
                    .ThenInclude(d => d.Diagnos)
                    .ToList()
                    .ExceptBy(castorIds, v => v.num)
                    .ToList();

                // для каждого полученного визита создается запись в movebook
                PlayMessage("Создание записей Movebook");
                var newRows = new List<MoveRow>();
                foreach (visit vis in __DataRowSource)
                {
                    try
                    {
                        Movebook movebook = new Movebook();
                        movebook.Fio = vis.Patient?.fullname ?? string.Empty;
                        movebook.Datein = DateOnly.FromDateTime(vis.order_dat.Value); //vis.dat.HasValue ? DateOnly.FromDateTime(vis.dat.Value) : null;
                        movebook.Ordered = 0;
                        movebook.Card_Id = vis.num;
                        movebook.Patientid = vis?.Patient?.num;
                        movebook.Birthdate = DateOnly.FromDateTime(vis?.Patient?.birthdate ?? DateTime.MinValue);
                        movebook.Visitid = vis?.keyid;
                        movebook.Dsin = vis?.Patient?.CurrentDs?.Diagnos?.code;

                        if (vis.dat1.HasValue)
                        {
                            movebook.Dateout = DateOnly.FromDateTime(vis.dat1.Value);
                            diagnos? dsOut = medisContext.diagnos
                                .Where(d => d.keyid == vis.Patient.CurrentDs.diagid)?.First();
                            movebook.Dsout = dsOut?.code;
                        }

                        // проверка повторности поступления - поиск уже закрытых карт для пациента в этом учреждении
                        movebook.Second = medisContext.visit
                            .Include(v => v.Patient).Where(v => v.Patient.num == vis.Patient.num)
                            .Include(v => v.Dep)
                            .Where(v => v.Dep.rootid == Settings.Default.RootDepartmentId && v.dat1.HasValue && v.vistype == 102 && v.num != vis.num) // root=ДПБ и дата выписки заполнена и поступление в п/п и номер истории не равен загружаемому
                            .Count() > 0
                            ;


                        newRows.Add(new MoveRow
                        {
                            Movebook = movebook,
                            Visit = vis,
                            IsSelected = false
                        });
                    }
                    catch { }

                }

                // Обновление коллекции в UI-потоке
                Application.Current.Dispatcher.Invoke(() =>
                {
                    DataRowSource.Clear();
                    foreach (var row in newRows)
                    {
                        DataRowSource.Add(row);
                    }
                });
                PlayMessage($"Готово  {DataRowSource.Count()} строк");

                OnPropertyChanged(nameof(Title));
            }
            catch (Exception ex)
            {
                Message.ShowPopup(ex.Message);
            }
        }

        /// <summary>
        /// Импорт выбранных визитов в базу Castor
        /// </summary>
        private void Save()
        {
            try
            {
                var _a = DataRowSource
                    .Where(d => d.IsSelected == true)
                    .Select(d => d.Movebook)
                    .ToList();

                using CastorContext castorContext = new CastorContext();
                castorContext.Movebooks.AddRange(_a);
                castorContext.SaveChanges();
                Task.Run(async () => await Load());
                PlayMessage("Выбранные записи сохранены");
            }
            catch (Exception ex)
            {
                Message.ShowPopup(ex.Message);
            }
        }


    

        private void PlayMessage(string message)
        {
            StatusMessage = message;
            OnPropertyChanged(nameof(StatusMessage));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class MoveRow
    {
        public bool? IsSelected { get; set; }
        public Movebook? Movebook { get; set; }
        public visit? Visit { get; set; }
    }
}

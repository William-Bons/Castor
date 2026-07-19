using Castor.database;
using Castor.database.tab_medis;
using Castor.database.tables;
using Castor.gui.common;
using Castor.Properties;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Castor.gui.movebook
{
    public class Synchronization : INotifyPropertyChanged
    {
        public Synchronization() { }


        public ICollection<Movebook> LoadedMovebooks { get; set; } = new List<Movebook>();

        /// <summary>
        /// Загружает визиты пациентов находящихся в отделении, в Root - поступление в п/п, оттуда - дата начала госпитализации
        /// </summary>
        public async Task LoadExistsFromMedis()
        {
            try
            {
                
                using CastorContext castorContext = new CastorContext();
                using MedisContext medisContext = new MedisContext();

                // список номеров и/б уже загруженных в базу Кастор
                List<long> castorIds = castorContext.Movebooks
                    .Select(x => x.Card_Id)
                    .ToList();
               

                // список визитов пациентов нваходящихся в настоящее время в отделении исключая номера историй уже заруженных в базу (по castorIds)
                List<visit> __DataRowSource = medisContext.visit
                    .Where(v => v.depid == Settings.Default.LastSelectedDepId && !v.dat1.HasValue) // Только находящиеся в отделении
                    .Include(v => v.Root) // чаще всего это поступление в п/п
                    .Include(v => v.Patient)  // привязка пациента
                    .ThenInclude(p => p.Diagnoses.Where(d => d.Diagnos.code.StartsWith("F"))) // диагнозы пациента
                    .ThenInclude(d => d.Diagnos)
                    .ToList()
                    .ExceptBy(castorIds, v => v.num)
                    .ToList();

                // для каждого полученного визита создается запись в movebook
                LoadedMovebooks.Clear();
                foreach (visit vis in __DataRowSource)
                {
                    Movebook movebook = new Movebook();
                    movebook.Fio = vis.Patient?.fullname ?? string.Empty;
                    movebook.Datein = DateOnly.FromDateTime(vis.order_dat.Value); //vis.dat.HasValue ? DateOnly.FromDateTime(vis.dat.Value) : null;
                    movebook.Ordered = 0;
                    movebook.Card_Id = vis.num;
                    movebook.Patientid = vis?.Patient?.num;
                    movebook.Birthdate = DateOnly.FromDateTime(vis.Patient.birthdate.Value);
                    movebook.Visitid = vis?.keyid;
                    movebook.Dsin = vis.Patient.CurrentDs.Diagnos.code;

                    // проверка повторности поступления - поиск уже закрытых карт для пациента в этом учреждении
                    movebook.Second = medisContext.visit
                        .Include(v => v.Patient).Where(v => v.Patient.num == vis.Patient.num)
                        .Include(v => v.Dep)
                        .Where(v => v.Dep.rootid == Settings.Default.RootDepartmentId && v.dat1.HasValue && v.vistype == 102 && v.num != vis.num) // root=ДПБ и дата выписки заполнена и поступление в п/п и номер истории не равен загружаемому
                        .Count() > 0
                        ;

                    LoadedMovebooks.Add(movebook);
                }

                // отобранные пациенты загружаются в базу Castor
                castorContext.Movebooks.AddRange(LoadedMovebooks);
                castorContext.SaveChanges();

                OnPropertyChanged(nameof(LoadedMovebooks));

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

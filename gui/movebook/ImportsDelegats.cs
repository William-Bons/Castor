using Castor.database;
using Castor.database.tab_medis;
using Castor.gui.common;
using Castor.gui.find;
using Castor.Properties;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Castor.gui.movebook
{
    internal class ImportsDelegats
    {
        /// <summary>
        /// Импорт из медис поступивших не более 30 дней назад и не импортированных в базу, сравнение по полному фио
        /// нигде не вызывается
        /// </summary>
        private async void ImportNewby(object sender, RoutedEventArgs e)
        {
            List<visit> visits = new List<visit>();

            Func<Task> __check = async () =>
            {
                try
                {
                    IEnumerable<visit> visitsWeek = new List<visit>();
                    IEnumerable<string> fios = new List<string>();

                    using (CastorContext castor = new CastorContext())
                    {
                        // list of Fullnames unordered 
                        fios = castor.Movebooks
                            .Where(m => m.Dateout == null)
                            .Select(m => m.Fio).ToList();
                    }
                    using (MedisContext cc = new MedisContext())
                    {
                        // get visits for last 30 days
                        visitsWeek = cc.visit
                           .Where(v => v.depid == Settings.Default.LastSelectedDepId && (DateTime.Today.ToUniversalTime() - v.dat.Value).Days < 30 && v.dat1 == null)
                           .Include(v => v.Patient)
                           .ThenInclude(p => p.Diagnoses.Where(g => g.Diagnos.code.StartsWith("F"))) // привязка диагнозов пациента
                           .ThenInclude(d => d.Diagnos)  // привязка к диагнозам пациента дианоза из мкб
                           .ToList()
                           .ExceptBy(fios, w => w.Fullname)
                           .ToList();
                    }

                    SelectObjectFromEnumerable soe = new SelectObjectFromEnumerable("Не загруженные", visitsWeek, PlacementMode.Center, "Patient.fullname", "Patient.age", "dat", "dat1", "Patient.CurrentDs.text");
                    soe.Selected += (a) =>
                    {
                        CreateNew createNew = new CreateNew(a);
                        createNew.ShowDialog();
                    };

                }
                catch (Exception ex)
                {
                    Message.ShowPopup(ex.Message);
                }
            };
            await __check();
        }

        /// <summary>
        /// Импорт из медис по списку всех кто находится в отдлении и выписан не более 90 дней назад, исключая повторы, сравнение по visitId
        /// нигде не вызывается
        /// </summary>
        private async void ImportList(object sender, RoutedEventArgs e)
        {
            List<long?> visitIds;
            List<visit> visits = new List<visit>();

            Func<Task> __check = async () =>
            {
                try
                {
                    var __depId = Settings.Default.LastSelectedDepId;

                    // получение списка VisitsIds загруженных в книгу
                    using (CastorContext castorContext = new CastorContext())
                    {
                        visitIds = castorContext.Movebooks.Select(m => m.Visitid).ToList();
                    }

                    // получение списка Visits за исключением загруженных в книгу
                    using (MedisContext medisContext = new MedisContext())
                    {
                        MainWindow.Wait(true);

                        /* Medis выбирает тех кто находится в отделении и выписанных не более 30 дней назад */
                        visits = medisContext.visit
                            .Where(v => v.depid == __depId && (!v.dat1.HasValue || (DateTime.Today.ToUniversalTime() - v.dat1).Value.Days <= 90)) // __depId - номер отдел. из Settings
                            .Include(f => f.Patient)  // привязка пациента
                            .ThenInclude(p => p.Diagnoses.Where(g => g.Diagnos.code.StartsWith("F"))) // привязка диагнозов пациента
                            .ThenInclude(d => d.Diagnos)  // привязка к диагнозам пациента дианоза из мкб
                            .ToList()
                            .ExceptBy(visitIds, v => v.keyid)
                            .ToList();

                        MainWindow.Wait();
                    }

                    SelectObjectFromEnumerable soe = new SelectObjectFromEnumerable("Не загруженные", visits, PlacementMode.Center, "Patient.fullname", "Patient.age", "dat", "dat1", "Patient.CurrentDs.text");
                    soe.Selected += (a) =>
                    {
                        CreateNew createNew = new CreateNew(a);
                        createNew.ShowDialog();
                    };

                }
                catch (Exception ex)
                {
                    MainWindow.Wait();
                    Message.ShowPopup(ex.Message);
                }
            };
            await __check();
        }



         
        /// <summary>
        /// Должен формировать Movebook экземпляр из любого найденного в медис пациента по визиту
        /// нигде не вызывается
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        
        private void LoadPatient(object sender, EventArgs e)
        {
            FindPatient findPatient = new FindPatient();
            if (findPatient.ShowDialog().Value)
            {
                new CreateNew(findPatient.Visit).ShowDialog();
            }
        }
    }
}

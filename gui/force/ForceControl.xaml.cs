using Castor.database;
using Castor.database.tab_medis;
using Castor.database.tables;
using Castor.gui.common;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Castor.gui.force
{
    /// <summary>
    /// Логика взаимодействия для ForceControl.xaml
    /// </summary>
    public partial class ForceControl : Window, INotifyPropertyChanged
    {
        public ForceControl(object? _force)
        {
            using CastorContext context = new CastorContext();

            // редактирование 
            if (_force is Forced forced ) 
            {
                ForcedItem = forced;
                if(ForcedItem.RootId>0)
                    FirstForce = context.Forced
                        .First(f => f.Id == ForcedItem.RootId);
            }
            // добавление нового 
            else if (_force is IEnumerable<Forced> fl ) 
            {
                FirstForce = fl.First(f => f.RootId == 0);

                ForcedItem = new Forced()
                {
                    Patientid = FirstForce.Patientid,
                    Visitid = FirstForce.Visitid,
                    RootId = FirstForce.Id
                };
            }
            // создание нового по ссылке на запись Movebook
            else if (_force is Movebook mb) 
            {
                ForcedItem = new Forced() { Patientid = mb.Patientid.Value, Visitid=mb.Visitid.Value };
            }
            else
            {
                ForcedItem = new Forced();
            }

            // загрузить данные визита и пациента из medis
            Load();

            
            Courts = context.Forced
                    .Select(f => f.Courtname)
                    .Distinct()
                    .ToList();

            InitializeComponent();
            DataContext = this;
            new MoveFocusHelper(FocusPanel, [Key.Return, Key.Enter], null, null);
        }

        public IEnumerable<Forced> ForcesToPatient { get; set; }
        public Forced? FirstForce { get; set; }
        public Forced ForcedItem { get; private set; }
        public visit Visit {  get; private set; }
        public List<string?> Courts { get; set;  }


        public event PropertyChangedEventHandler? PropertyChanged;

        private void Load()
        {
            MainWindow.Wait(true);
            try
            {
                // load Visit datas from MEDIS
                using (MedisContext medis = new MedisContext())
                {
                    Visit = medis.visit
                        .Where(v => v.keyid == ForcedItem.Visitid)
                        .Include(v => v.Patient)
                        .First();
                } 

                //// load all exists forces to patient accordint this visit
                //using(CastorContext  castor = new CastorContext())
                //{
                //    ForcesToPatient = castor.Forced
                //        .Where(f => f.Patientid == ForcedItem.Patientid && f.Visitid==ForcedItem.Visitid);
                //}

                //// load first force if exist
                //if(ForcesToPatient.Any())
                //{
                //    FirstForce = ForcesToPatient
                //        .MinBy(f => f.Start);
                //    ForcedItem.RootId = FirstForce?.Id ?? 0;
                //}

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Visit)));
            }
            catch { }
            MainWindow.Wait();
        }

        private void Write(object sender, RoutedEventArgs e)
        {
            try
            {
                using(CastorContext castor = new CastorContext())
                {
                    castor.Forced.Update(ForcedItem);
                    castor.SaveChanges();
                }
                DialogResult = true;
                Close();
            }
            catch { }
        }
    }
}

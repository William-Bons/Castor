using Castor.database;
using Castor.database.tables;
using Castor.gui;
using Castor.gui.common;
using System.Windows;
using System.Windows.Controls;

namespace Castor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CastorCommonContext DatabaseContext;
        public MainWindow()
        {
            InitializeComponent();
            new MenuLoader(CentralMenu).MenuItemRise += MainWindow_MenuItemRise;
            Console.Print("connection to database...");

            ContentRendered += async (o, e) =>
            {
                await Task.Run(() =>
                {
                    DatabaseContext = new CastorCommonContext();
                });
                Console.Print("Success");
            };
        }

        private void MainWindow_MenuItemRise(MenuItem sender)
        {
            if (sender?.CommandParameter is string _classType &&    // check CommandParameter is string
                Type.GetType(_classType) is Type _type)             // try get Type from string
            {

                object activeCreatedObject =
                    _type.GetConstructor([typeof(CastorCommonContext)]) != null ? Activator.CreateInstance(_type, DatabaseContext) :
                    _type.GetConstructor([typeof(int)]) != null ? Activator.CreateInstance(_type, 0/*Tag??*/) :
                    _type.GetConstructor([typeof(string)]) != null ? Activator.CreateInstance(_type, null /*localTag*/) :
                    _type.GetConstructor([typeof(MainWindow)]) != null ? Activator.CreateInstance(_type, this) :
                    Activator.CreateInstance(_type);

                if (activeCreatedObject is IConsoleMessage obj)
                {
                    obj.ConsoleMessage += (message) => Console.Print($"{message}");
                }

                if (activeCreatedObject is IDialog dlg)
                {
                    dlg.Show();
                }



                if (activeCreatedObject is Window _aw)
                {
                    _aw.ShowDialog();
                }
                //else if (activeCreatedObject is Page)
                //{
                //    frame.Content = activeCreatedObject;
                //}
            }
        }

        private void OpenDb()
        {
            using (CastorCommonContext db = CastorCommonContext.Get())
            {
                //Console.Print(db.Variant);

                // создаем два объекта User
                //User tom = new User { Name = "Tom", Department = 6 };
                //User alice = new User { Name = "Alice", Department = 12 };

                //// добавляем их в бд
                //db.Users.Add(tom);
                //db.Users.Add(alice);
                //db.SaveChanges();
                //Console.Print("Объекты успешно сохранены");

                Medcard m1 = new Medcard { Department = 6, User = new User { Name = "Irma", Department = 12, Rights = 100 }, Person = new Person { FirstName = "Kukukina" } };
                db.MedCards.Add(m1);

                Person p1 = new Person { FirstName = "Romanik", Age = 23 };
                db.Persons.Add(p1);

                Person p2 = new Person { FirstName = "Sologk", Age = 34 };
                db.Persons.Add(p2);

                Person p3 = new Person { FirstName = "Ffeowse", Age = 12 };
                db.Persons.Add(p3);

                db.SaveChanges();

                // получаем объекты из бд и выводим на консоль
                //var users = db.MedCards.ToList();
                //Console.Print("Список объектов:");
                //foreach (Medcard u in users)
                //{
                //    Console.Print($"{u.Id}.{u.User} - {u.Person}");
                //}
            }
        }


    }
}
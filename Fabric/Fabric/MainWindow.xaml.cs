using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace Fabric
{
    public partial class MainWindow : Window
    {
        public MainWindow() { InitializeComponent(); LoadBackUP(); }
        /// <summary> Восстанавливает предыдущее состояние программы </summary>
        void LoadBackUP()
        {
            string file = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\BackUP.txt";
            if (File.Exists(file))
            {
                string[] lines = File.ReadAllLines(file);//Считываем все строки
                if (lines[0].Length < 3) return; //Если файл пуст ( так как JSON вида "[]" является валидным => минимальная нужная нам длина >= 3)
                object[] list = new JavaScriptSerializer().Deserialize<object[]>(lines[0]);
                int index = 0;
                foreach (object raw in list)//Для каждого JSON пользователя
                {
                    Dictionary<string, object> client = (Dictionary<string, object>)raw;//Словарь всех полей для текущего клиента
                    object o = client["listen"];//Берем из словаря список фабрик на которые был подписан пользователь
                    for (int i = 0; i < (o as Array).Length; i++)
                    {
                        foreach (Button bt in new List<Button>(mainGrid.Children.OfType<Button>()))//Возвращаем кнопкам предыдущее состояние
                            if (bt.Name == (o as Array).GetValue(i).ToString())//Ищем пользователя 
                            { Bind_Click(bt, null); /*break;*/ }//Заново подписываем пользователя на фабрику
                    }

                    o = client["myMessages"];//Берем из словаря список сообщений, которые пользователь уже получил 
                    for (int i = 0; i < (o as Array).Length; i++)
                    {
                        Dictionary<string, object> message = (Dictionary<string, object>)(o as Array).GetValue(i);
                        clients[index].myMessages.Add(new message(message["FabricName"].ToString(), message["Message"].ToString()));//Добавляем сообщение в список полученных
                    }
                    clients[index++].showMyMessages();
                }
            }
        }
        Random rand = new Random();
        List<Client> clients = new List<Client>() { new Client(), new Client(), };//Список клиентов
        /// <summary> Подписывает/отписывает клиента на фабрику </summary>
        /// <param name="sender"></param> <param name="e"></param>
        void Bind_Click(object sender, RoutedEventArgs e)
        {
            Button btn = ((Button)sender);
            int index = getClientIndex(btn.Name);
            if (index > -1)
            {
                if (btn.Content.ToString() == "Link")
                {
                    btn.Content = "UnLink";
                    btn.Foreground = Brushes.Red;
                }
                else
                {
                    btn.Content = "Link";
                    btn.Foreground = Brushes.Green;
                }
                clients[index].configureListener(btn.Name, ref rand);
            }
        }
        /// <summary> Получает индекс клиента в списке </summary>
        /// <param name="btnName"></param> <returns></returns>
        int getClientIndex(string btnName)
        {
            if (!int.TryParse(new Regex(@"\d").Match(btnName).Value, out int index)) return -1;//Не удалось распознать

            while (index % 3 != 0) index++;

            index = index / 3 - 1;//Получаем номер клиента

            if (!clients[index].Init) clients[index] = new Client(index, ref mainGrid);
            return index;
        }
        /// <summary> Очистка вывода </summary> <param name="sender"></param> <param name="e"></param>
        void Clear_Click(object sender, RoutedEventArgs e)
        {
            output1.Items.Clear(); output2.Items.Clear();
            foreach (Client cl in clients) cl.myMessages.Clear();
        }
        /// <summary> Сохраняет текущее соостояние программы и при следующем запуске оно будет восстановлено</summary>
        /// <param name="sender"></param> <param name="e"></param>
        void Save(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (Client cl in clients) cl.Lock = true;//Блокируем списки для сериализации(JSON)

            string backUp = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\BackUP.txt";
            File.Create(backUp).Close();//Создаём новый файл
            string jStr = new JavaScriptSerializer().Serialize(clients);
            StreamWriter file = new StreamWriter(backUp);
            file.WriteLine(jStr); file.Close();

            foreach (Client cl in clients) cl.Lock = false;//Убераем блокировку списков после сериализации(JSON)
        }
        void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
    }
}
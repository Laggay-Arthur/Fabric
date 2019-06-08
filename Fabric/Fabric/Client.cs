using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;


namespace Fabric
{
    class Client
    {
        ListBox output;//Для вывода
        Factory factory;//Фабрика создающая сообщения
        public List<string> listen = new List<string>();//Список фабрик которые слушает клиент
        public List<message> myMessages = new List<message>();
        public void showMyMessages()
        {
            foreach (message m in myMessages)
                output.Items.Add(dictionary[m.FabricName] + " : " + m.Message);//Выводим сообщения на форму
        }
        [NonSerialized] public bool Lock = false;
        [NonSerialized] public readonly bool Init = false;
        public Client() { }
        public Client(int index, ref Grid output)
        {
            factory = new Factory();
            timer.Tick += new EventHandler(CheckItems);
            this.output = new List<ListBox>(output.Children.OfType<ListBox>())[index];
            Init = true;
        }
        /// <summary> Добавляем/Удаляем фабрику в список прослушивания </summary> <param name="FabricName"></param>
        public void configureListener(string FabricName, ref Random rand)
        {
            factory.configureListener(FabricName, rand);
            if (listen.Contains(FabricName)) listen.Remove(FabricName);
            else listen.Add(FabricName);

            configureTimer();
        }
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer() { Interval = new TimeSpan(0, 0, 2) };
        /// <summary> Переодически проверяем у брокера сообщения от фабрик </summary>
        void configureTimer()
        {
            if (!timer.IsEnabled) timer.Start();
            else if (timer.IsEnabled && listen.Count == 0) timer.Stop();//Если таймер запущен но клиент не подписан на фабрики
        }
        Dictionary<string, string> dictionary = new Dictionary<string, string>(12) { { "btn1", "Новости" }, { "btn4", "Новости" }, { "btn2", "Погода" }, { "btn5", "Погода" }, { "btn3", "Работа" }, { "btn6", "Работа" } };
        /// <summary> Проверяем есть ли новые сообщения от фабрик на которые подписан клиент </summary>
        /// <param name="sender"></param> <param name="e"></param>
        void CheckItems(object sender, EventArgs e)
        {
            while (Lock)
            {
                int delay = 1000;
                System.Threading.Tasks.Task.Delay(delay);
            }

            foreach (string fabric in listen) foreach (message m in Broker.GetMessage(fabric))
                {
                    output.Items.Add(dictionary[m.FabricName] + " : " + m.Message);//Выводим сообщения на форму
                    myMessages.Add(m);
                }
        }
    }
}
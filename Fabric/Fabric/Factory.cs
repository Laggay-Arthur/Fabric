using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Fabric
{
    class Factory
    {
        public List<string> factory = new List<string>();//Список фабрик на которые подписан клиент
        /// <summary> Генерируем сообщение </summary>
        /// <param name="FabricName"></param> <param name="sleepTime"></param> <returns></returns>
        message msg(string FabricName, int sleepTime) => new message(FabricName, sleepTime.ToString());
        /// <summary> Подписывает/Отписывает клиента на получение сообщений </summary>
        /// <param name="FabricName"></param>
        async public void configureListener(string FabricName, Random rand)
        {
            if (!factory.Contains(FabricName))
            {
                factory.Add(FabricName);

                while (true)
                {
                    if (!factory.Contains(FabricName)) break;//Клиент отписался от фабрики
                    int sleepTime = (rand.Next(0, 6) + rand.Next(0, 4)) * 1000 + (rand.Next(0, 2) + rand.Next(0, 5)) * 100 + (rand.Next(0, 1) + rand.Next(0, 9)) * 10;
                    await Task.Delay(sleepTime);
                    Broker.AddMessage(await Task.Run(() => msg(FabricName, sleepTime)));
                }
            }
            else factory.Remove(FabricName);
        }
    }
}
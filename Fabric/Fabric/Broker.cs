using System.Collections.Generic;


namespace Fabric
{
    public struct message
    {
        public message(string Fabric, string msg)
        {
            FabricName = Fabric;
            Message = msg;
        }
        public string FabricName { get; set; }
        public string Message { get; set; }
    }
    static class Broker
    {
        static List<message> messages = new List<message>();//Список сообщений от фабрик
        /// <summary> Добавляет новое сообщение в список </summary>
        /// <param name="msg"></param>
        static public void AddMessage(message msg) => messages.Add(msg);
        /// <summary> Возвращает список сообщений для фабрики </summary>
        /// <param name="FabricName"></param> <returns></returns>
        static public List<message> GetMessage(string FabricName)
        {
            List<message> AllMessages = new List<message>();

            for (int i = 0; i < messages.Count; i++)
                if (FabricName == messages[i].FabricName)
                {
                    AllMessages.Add(messages[i]);//Если есть сообщения для выбранной фабрики добавляем их в список
                    messages.Remove(messages[i]);//Удаляем полученно сообщение
                }
            return AllMessages;
        }
    }
}
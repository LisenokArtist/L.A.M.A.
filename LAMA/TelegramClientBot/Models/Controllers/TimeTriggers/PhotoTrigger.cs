namespace TelegramClientBot.Models.Controllers.TimeTriggers
{
    public class PhotoTrigger : TimerTriggerBase
    {
        /// <summary>
        /// Задает режим обновления на последовательное, иначе на случайное
        /// </summary>
        public bool Ordered { get; set; } = false;

        public PhotoTrigger(TimeSpan timeTrigger) : base(timeTrigger)
        {

        }
    }
}

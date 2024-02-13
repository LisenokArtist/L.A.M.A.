namespace TelegramClientBot.Models.Controllers.TimeTriggers
{
    public class TimerTriggerBase : IDisposable
    {
        /// <summary>
        /// Темпоральный таймер. Используется если 
        /// необходимо сохранить предыдущие настройки 
        /// временного триггера
        /// </summary>
        internal TimeSpan? TemporalTimeTrigger { get; set; }

        public TimeSpan TimeTrigger { get; set; }
        
        CancellationTokenSource CancellationToken { get; set; } = new CancellationTokenSource();

        Task RunningTask { get; set; }

        public event Action? OnTimeTriggered;

        public bool IsRunning { get; private set; } = false;

        public TimerTriggerBase(TimeSpan timeTrigger)
        {
            TimeTrigger = timeTrigger;
            RunningTask = new Task(async () =>
            {
                while (IsRunning)
                {
                    await Task.Delay(TimeTrigger);
                    OnTimeTriggered?.Invoke();
                }
            }, CancellationToken.Token);
        }

        /// <summary>
        /// Запускает работу триггера
        /// </summary>
        public void Run()
        {
            if (IsRunning) return;

            IsRunning = true;
            RunningTask.Start();
        }

        /// <summary>
        /// Останавливает работу триггера
        /// </summary>
        public void Stop()
        {
            if (!IsRunning) return;

            IsRunning = false;
            CancellationToken.Cancel();
        }
        
        /// <summary>
        /// Выгружает ресурсы триггера
        /// </summary>
        public void Dispose()
        {
            CancellationToken.Cancel();
            CancellationToken.Dispose();
            RunningTask.Dispose();
        }
    }
}
using TelegramClientBot.Models.Extensions;
using TL;

namespace TelegramClientBot.Models.Controllers.TimeTriggers
{
    public class PhotoTrigger : TimerTriggerBase
    {
        /// <summary>
        /// Задает режим обновления на последовательное, иначе на случайное
        /// </summary>
        public bool Ordered { get; set; } = false;

        /// <summary>
        /// Переключает состояние работы триггера
        /// </summary>
        public bool Enabled
        {
            get
            {
                return IsRunning;
            }
            set
            {
                if (value)
                {
                    Run();
                }
                else
                {
                    Stop();
                }
            }
        }

        public PhotoTrigger(TimeSpan timeTrigger) : base(timeTrigger)
        {
            OnTimeTriggered += OnTimeTriggeredEvent;
        }

        private async void OnTimeTriggeredEvent()
        {
            var photosData = await Program.ProgramModel.Client.Photos_GetUserPhotos(Program.ProgramModel.Me);

            if (photosData.photos.Length > 1)
            {
                if (Ordered)
                {
                    var nextPhoto = photosData.photos.SkipWhile(x => x.ID != Program.ProgramModel.Me.photo.photo_id).Skip(1).FirstOrDefault();
                    if (nextPhoto != null)
                    {
                        OnResponse(nextPhoto);
                    }
                }
                else
                {
                    var randomize = photosData.photos.Where(x => x.ID != Program.ProgramModel.Me.photo.photo_id).PickRandom();
                    if (randomize != null)
                    {
                        OnResponse(randomize);
                    }
                }
            }
        }

        private async void OnResponse(PhotoBase photo)
        {
            try
            {
                var response = await Program.ProgramModel.Client.Photos_UpdateProfilePhoto(photo);

                if (response != null)
                {
                    Program.ProgramModel.Me.photo.photo_id = response.photo.ID;
                }

                if (TemporalTimeTrigger != null)
                {
                    TimeTrigger = (TimeSpan)TemporalTimeTrigger;
                    TemporalTimeTrigger = null;
                }
            } 
            catch (TL.RpcException rpc) //FLOOD_WAIT_X
            {
                TemporalTimeTrigger = TimeTrigger;
                TimeTrigger = new TimeSpan(0, 0, rpc.X);
            }
        }
    }
}

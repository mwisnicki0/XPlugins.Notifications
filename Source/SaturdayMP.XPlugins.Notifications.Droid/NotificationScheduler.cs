using System;
using Android.App;
using Android.Content;

namespace SaturdayMP.XPlugins.Notifications.Droid
{
    /// <summary>
    /// Used to schedule local notificaions in Android.
    /// </summary>
    public abstract class NotificationScheduler : INotificationScheduler
    {
        /// <inheritdoc />
        public int Create(string title, string message)
        {
            // Create the intent to be called when the alarm triggers.
            var alarmIntent = new Intent(Application.Context, typeof(NotificationAlarmHandler));
            alarmIntent.PutExtra("title", title);
            alarmIntent.PutExtra("message", message);

            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, 0, alarmIntent, PendingIntentFlags.UpdateCurrent);

            // Figure out the alaram in milliseconds.
            var utcTime = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);
            var epochDif = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;
            var notifyTimeInInMilliseconds = utcTime.AddSeconds(-epochDif).Ticks/10000;

            // Set the notification.
            var alarmManager = Application.Context.GetSystemService(Context.AlarmService) as AlarmManager;
            alarmManager?.Set(AlarmType.RtcWakeup, notifyTimeInInMilliseconds, pendingIntent);

            // TODO: Figure out a way to track the notification number.
            // All done.
            return 0;

        }
    }
}
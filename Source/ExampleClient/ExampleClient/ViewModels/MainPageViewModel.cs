﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ExampleClient.Repositories;
using ExampleClient.Views;
using JetBrains.Annotations;
using SaturdayMP.XPlugins.Notifications;
using Xamarin.Forms;

namespace ExampleClient.ViewModels
{
    internal sealed class MainPageViewModel : INotifyPropertyChanged
    {
        public MainPageViewModelScheduledNotification SelectedScheduledNotification
        {
            get { return null; }
            set
            {
                OnPropertyChanged();

                if (value != null)
                    _navigation.PushAsync(new ScheduledNotificationPage(new Guid(value.Text)));
            }
        }

        #region Fields

        /// <summary>
        ///     The scheduler used to send notifications.
        /// </summary>
        private readonly INotificationScheduler _notificationScheduler;

        /// <summary>
        ///     Used to navigate between pages.
        /// </summary>
        private readonly INavigation _navigation;

        #endregion

        #region Constructors

        /// <summary>
        ///     Create a new view model without any notification.
        /// </summary>
        public MainPageViewModel()
        {
            // So we can send notifications.
            _notificationScheduler = DependencyService.Get<INotificationScheduler>();

            // Commands used to schedule notifications.
            ScheduleNowCommand = new Command(ScheduleNow);

            // No extra info by default.
            IncludeExtraInfo = false;

            _extraInfoOne = "";
            _extraInfoTwo = "";
            _scheduledNotifications = new ObservableCollection<MainPageViewModelScheduledNotification>();
        }

        /// <summary>
        ///     Create a new view model with the notification scheduled and commands
        ///     initialized to the defaults.
        /// </summary>
        public MainPageViewModel([NotNull] INavigation navigation) : this()
        {
            _navigation = navigation;
        }

        #endregion

        #region Schedule Now

        /// <summary>
        ///     Schedule a notification for now.
        /// </summary>
        [UsedImplicitly]
        public ICommand ScheduleNowCommand { get; set; }

        /// <summary>
        ///     Schedule a notification for now.  If there is extra info then include it.
        /// </summary>
        private void ScheduleNow()
        {
            Guid notificationId;

            // Check if the extra info should be included when sending the notification
            // then schedule the notification.
            const string title = "Scheduled Now";
            var message = $"Created: {DateTime.Now:G}, Scheduled: {DateTime.Now:G}";

            if (IncludeExtraInfo)
            {
                var extraInfo = new Dictionary<string, string> {{"ExtraInfoOne", ExtraInfoOne}, {"ExtraInfoTwo", ExtraInfoTwo}};
                notificationId = _notificationScheduler.Create(title, message, extraInfo);
            }
            else
            {
                notificationId = _notificationScheduler.Create(title, message);
            }

            // Keep track of this scheduled notification.
            ScheduledNotificationRepository.NotificationScheduled(notificationId, title, message, DateTime.Now, ExtraInfoOne, ExtraInfoTwo);

            // Add to the list of notifications.
            ScheduledNotifications.Add(new MainPageViewModelScheduledNotification {Text = notificationId.ToString()});
        }

        #endregion

        #region ExtraInfo

        /// <summary>
        ///     See <see cref="IncludeExtraInfo" />.
        /// </summary>
        private bool _includeExtraInfo;

        /// <summary>
        ///     If true then include the extra info in the notification
        ///     to be scheduled.
        /// </summary>
        public bool IncludeExtraInfo
        {
            get { return _includeExtraInfo; }
            set
            {
                if (_includeExtraInfo == value) return;

                _includeExtraInfo = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     See <see cref="ExtraInfoOne" />.
        /// </summary>
        [NotNull] private string _extraInfoOne;

        /// <summary>
        ///     The first piece of extra into to include in the notifiaction.
        /// </summary>
        [NotNull]
        public string ExtraInfoOne
        {
            get { return _extraInfoOne; }
            set
            {
                if (_extraInfoOne == value) return;

                _extraInfoOne = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        ///     See <see cref="ExtraInfoTwo" />.
        /// </summary>
        [NotNull] private string _extraInfoTwo;

        /// <summary>
        ///     The second piece of extra information to be included in the notification.
        /// </summary>
        [NotNull]
        public string ExtraInfoTwo
        {
            get { return _extraInfoTwo; }
            set
            {
                if (_extraInfoTwo == value) return;

                _extraInfoTwo = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Scheduled Notifications

        /// <summary>
        ///     See <see cref="ScheduledNotifications" />.
        /// </summary>
        [NotNull] private ObservableCollection<MainPageViewModelScheduledNotification> _scheduledNotifications;

        /// <summary>
        ///     If true then include the extra info in the notification
        ///     to be scheduled.
        /// </summary>
        [NotNull]
        public ObservableCollection<MainPageViewModelScheduledNotification> ScheduledNotifications
        {
            get { return _scheduledNotifications; }
            set
            {
                if (_scheduledNotifications == value) return;

                _scheduledNotifications = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region  Property Changed Handler

        /// <summary>
        ///     Used to handled view model changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc />
        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
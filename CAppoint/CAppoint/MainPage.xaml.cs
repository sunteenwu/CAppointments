using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Appointments;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CAppoint
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        Windows.ApplicationModel.Appointments.Appointment appointment = new Windows.ApplicationModel.Appointments.Appointment();

        private async void Create_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = null;

            // StartTime
            var date = StartTimeDatePicker.Date;
            var time = StartTimeTimePicker.Time;
            var timeZoneOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
            var startTime = new DateTimeOffset(date.Year, date.Month, date.Day, time.Hours, time.Minutes, 0, timeZoneOffset);
            appointment.StartTime = startTime;

            // Subject
            appointment.Subject = SubjectTextBox.Text;

            if (appointment.Subject.Length > 255)
            {
                errorMessage = "The subject cannot be greater than 255 characters.";
            }

            // Location
            appointment.Location = LocationTextBox.Text;

            if (appointment.Location.Length > 32768)
            {
                errorMessage = "The location cannot be greater than 32,768 characters.";
            }

            // Details
            appointment.Details = DetailsTextBox.Text;

            if (appointment.Details.Length > 1073741823)
            {
                errorMessage = "The details cannot be greater than 1,073,741,823 characters.";
            }

            // Duration
            if (DurationComboBox.SelectedIndex == 0)
            {
                // 30 minute duration is selected
                appointment.Duration = TimeSpan.FromMinutes(30);
            }
            else
            {
                // 1 hour duration is selected
                appointment.Duration = TimeSpan.FromHours(1);
            }

            // All Day
            appointment.AllDay = AllDayCheckBox.IsChecked.Value;

            // Reminder
            if (ReminderCheckBox.IsChecked.Value)
            {
                switch (ReminderComboBox.SelectedIndex)
                {
                    case 0:
                        appointment.Reminder = TimeSpan.FromMinutes(15);
                        break;
                    case 1:
                        appointment.Reminder = TimeSpan.FromHours(1);
                        break;
                    case 2:
                        appointment.Reminder = TimeSpan.FromDays(1);
                        break;
                }
            }

            //Busy Status
            switch (BusyStatusComboBox.SelectedIndex)
            {
                case 0:
                    appointment.BusyStatus = Windows.ApplicationModel.Appointments.AppointmentBusyStatus.Busy;
                    break;
                case 1:
                    appointment.BusyStatus = Windows.ApplicationModel.Appointments.AppointmentBusyStatus.Tentative;
                    break;
                case 2:
                    appointment.BusyStatus = Windows.ApplicationModel.Appointments.AppointmentBusyStatus.Free;
                    break;
                case 3:
                    appointment.BusyStatus = Windows.ApplicationModel.Appointments.AppointmentBusyStatus.OutOfOffice;
                    break;
                case 4:
                    appointment.BusyStatus = Windows.ApplicationModel.Appointments.AppointmentBusyStatus.WorkingElsewhere;
                    break;
            }

            // Sensitivity
            switch (SensitivityComboBox.SelectedIndex)
            {
                case 0:
                    appointment.Sensitivity = Windows.ApplicationModel.Appointments.AppointmentSensitivity.Public;
                    break;
                case 1:
                    appointment.Sensitivity = Windows.ApplicationModel.Appointments.AppointmentSensitivity.Private;
                    break;
            }

            // Uri
            if (UriTextBox.Text.Length > 0)
            {
                try
                {
                    appointment.Uri = new System.Uri(UriTextBox.Text);
                }
                catch (Exception)
                {
                    errorMessage = "The Uri provided is invalid.";
                }
            }

            // Organizer
            // Note: Organizer can only be set if there are no invitees added to this appointment.
            if (OrganizerRadioButton.IsChecked.Value)
            {
                var organizer = new Windows.ApplicationModel.Appointments.AppointmentOrganizer();

                // Organizer Display Name
                organizer.DisplayName = OrganizerDisplayNameTextBox.Text;

                if (organizer.DisplayName.Length > 256)
                {
                    errorMessage = "The organizer display name cannot be greater than 256 characters.";
                }
                else
                {
                    // Organizer Address (e.g. Email Address)
                    organizer.Address = OrganizerAddressTextBox.Text;

                    if (organizer.Address.Length > 321)
                    {
                        errorMessage = "The organizer address cannot be greater than 321 characters.";
                    }
                    else if (organizer.Address.Length == 0)
                    {
                        errorMessage = "The organizer address must be greater than 0 characters.";
                    }
                    else
                    {
                        appointment.Organizer = organizer;
                    }
                }
            }

            // Invitees
            // Note: If the size of the Invitees list is not zero, then an Organizer cannot be set.
            if (InviteeRadioButton.IsChecked.Value)
            {
                var invitee = new Windows.ApplicationModel.Appointments.AppointmentInvitee();

                // Invitee Display Name
                invitee.DisplayName = InviteeDisplayNameTextBox.Text;

                if (invitee.DisplayName.Length > 256)
                {
                    errorMessage = "The invitee display name cannot be greater than 256 characters.";
                }
                else
                {
                    // Invitee Address (e.g. Email Address)
                    invitee.Address = InviteeAddressTextBox.Text;

                    if (invitee.Address.Length > 321)
                    {
                        errorMessage = "The invitee address cannot be greater than 321 characters.";
                    }
                    else if (invitee.Address.Length == 0)
                    {
                        errorMessage = "The invitee address must be greater than 0 characters.";
                    }
                    else
                    {
                        // Invitee Role
                        switch (RoleComboBox.SelectedIndex)
                        {
                            case 0:
                                invitee.Role = Windows.ApplicationModel.Appointments.AppointmentParticipantRole.RequiredAttendee;
                                break;
                            case 1:
                                invitee.Role = Windows.ApplicationModel.Appointments.AppointmentParticipantRole.OptionalAttendee;
                                break;
                            case 2:
                                invitee.Role = Windows.ApplicationModel.Appointments.AppointmentParticipantRole.Resource;
                                break;
                        }

                        // Invitee Response
                        switch (ResponseComboBox.SelectedIndex)
                        {
                            case 0:
                                invitee.Response = Windows.ApplicationModel.Appointments.AppointmentParticipantResponse.None;
                                break;
                            case 1:
                                invitee.Response = Windows.ApplicationModel.Appointments.AppointmentParticipantResponse.Tentative;
                                break;
                            case 2:
                                invitee.Response = Windows.ApplicationModel.Appointments.AppointmentParticipantResponse.Accepted;
                                break;
                            case 3:
                                invitee.Response = Windows.ApplicationModel.Appointments.AppointmentParticipantResponse.Declined;
                                break;
                            case 4:
                                invitee.Response = Windows.ApplicationModel.Appointments.AppointmentParticipantResponse.Unknown;
                                break;
                        }

                        appointment.Invitees.Add(invitee);
                    }
                }
            }


            var recurrence = new Windows.ApplicationModel.Appointments.AppointmentRecurrence();

            // Unit
            switch (UnitComboBox.SelectedIndex)
            {
                case 0:
                    recurrence.Unit = Windows.ApplicationModel.Appointments.AppointmentRecurrenceUnit.Daily;
                    break;
                case 1:
                    recurrence.Unit = Windows.ApplicationModel.Appointments.AppointmentRecurrenceUnit.Weekly;
                    break;
                case 2:
                    recurrence.Unit = Windows.ApplicationModel.Appointments.AppointmentRecurrenceUnit.Monthly;
                    break;
                case 3:
                    recurrence.Unit = Windows.ApplicationModel.Appointments.AppointmentRecurrenceUnit.MonthlyOnDay;
                    break;
                case 4:
                    recurrence.Unit = Windows.ApplicationModel.Appointments.AppointmentRecurrenceUnit.Yearly;
                    break;
                case 5:
                    recurrence.Unit = Windows.ApplicationModel.Appointments.AppointmentRecurrenceUnit.YearlyOnDay;
                    break;
            }

            // Occurrences
            // Note: Occurrences and Until properties are mutually exclusive.
            if (OccurrencesRadioButton.IsChecked.Value)
            {
                recurrence.Occurrences = (uint)OccurrencesSlider.Value;
            }

            // Until
            // Note: Until and Occurrences properties are mutually exclusive.
            if (UntilRadioButton.IsChecked.Value)
            {
                var untildate = UntilDatePicker.Date;
                var untiltime = UntilTimePicker.Time;
                var timeZone  = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);               
                recurrence.Until = new DateTimeOffset(untildate.Year, untildate.Month, untildate.Day, untiltime.Hours, untiltime.Minutes, 0, timeZone);
                //recurrence.Until = UntilDatePicker.Date;
                System.Diagnostics.Debug.WriteLine("User selected recurrence until:"+recurrence.Until);
            }

            // Interval
            recurrence.Interval = (uint)IntervalSlider.Value;

            // Week of the month
            switch (WeekOfMonthComboBox.SelectedIndex)
            {
                case 0:
                    recurrence.WeekOfMonth = Windows.ApplicationModel.Appointments.AppointmentWeekOfMonth.First;
                    break;
                case 1:
                    recurrence.WeekOfMonth = Windows.ApplicationModel.Appointments.AppointmentWeekOfMonth.Second;
                    break;
                case 2:
                    recurrence.WeekOfMonth = Windows.ApplicationModel.Appointments.AppointmentWeekOfMonth.Third;
                    break;
                case 3:
                    recurrence.WeekOfMonth = Windows.ApplicationModel.Appointments.AppointmentWeekOfMonth.Fourth;
                    break;
                case 4:
                    recurrence.WeekOfMonth = Windows.ApplicationModel.Appointments.AppointmentWeekOfMonth.Last;
                    break;
            }

            // Days of the Week
            // Note: For Weekly, MonthlyOnDay or YearlyOnDay recurrence unit values, at least one day must be specified.
            if (SundayCheckBox.IsChecked.Value) { recurrence.DaysOfWeek |= Windows.ApplicationModel.Appointments.AppointmentDaysOfWeek.Sunday; }
            if (MondayCheckBox.IsChecked.Value) { recurrence.DaysOfWeek |= Windows.ApplicationModel.Appointments.AppointmentDaysOfWeek.Monday; }
            if (TuesdayCheckBox.IsChecked.Value) { recurrence.DaysOfWeek |= Windows.ApplicationModel.Appointments.AppointmentDaysOfWeek.Tuesday; }
            if (WednesdayCheckBox.IsChecked.Value) { recurrence.DaysOfWeek |= Windows.ApplicationModel.Appointments.AppointmentDaysOfWeek.Wednesday; }
            if (ThursdayCheckBox.IsChecked.Value) { recurrence.DaysOfWeek |= Windows.ApplicationModel.Appointments.AppointmentDaysOfWeek.Thursday; }
            if (FridayCheckBox.IsChecked.Value) { recurrence.DaysOfWeek |= Windows.ApplicationModel.Appointments.AppointmentDaysOfWeek.Friday; }
            if (SaturdayCheckBox.IsChecked.Value) { recurrence.DaysOfWeek |= Windows.ApplicationModel.Appointments.AppointmentDaysOfWeek.Saturday; }

            if (((recurrence.Unit == Windows.ApplicationModel.Appointments.AppointmentRecurrenceUnit.Weekly) ||
                 (recurrence.Unit == Windows.ApplicationModel.Appointments.AppointmentRecurrenceUnit.MonthlyOnDay) ||
                 (recurrence.Unit == Windows.ApplicationModel.Appointments.AppointmentRecurrenceUnit.YearlyOnDay)) &&
                (recurrence.DaysOfWeek == Windows.ApplicationModel.Appointments.AppointmentDaysOfWeek.None))
            {
                errorMessage = "The recurrence specified is invalid. For Weekly, MonthlyOnDay or YearlyOnDay recurrence unit values, at least one day must be specified.";
            }

            // Month of the year
            recurrence.Month = (uint)MonthSlider.Value;

            // Day of the month
            recurrence.Day = (uint)DaySlider.Value;

            appointment.Recurrence = recurrence;
            if (errorMessage == null)
            {
                //System.Diagnostics.Debug.WriteLine("Create the appoint object successfully");
            }
            else
            {
                await new MessageDialog(errorMessage).ShowAsync();
            }
        }

        /// <summary>
        /// Organizer and Invitee properties are mutually exclusive.
        /// This radio button enables the organizer properties while disabling the invitees.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrganizerRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            OrganizerStackPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            InviteeStackPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        /// <summary>
        /// Organizer and Invitee properties are mutually exclusive.
        /// This radio button enables the invitees properties while disabling the organizer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InviteeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            InviteeStackPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            OrganizerStackPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        /// <summary>
        /// Displays the combo box containing various reminder times.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReminderCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ReminderComboBox.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        /// <summary>
        /// Hides the combo box containing various reminder times.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReminderCheckBox_UnChecked(object sender, RoutedEventArgs e)
        {
            ReminderComboBox.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private async void btnsaveappoint_Click(object sender, RoutedEventArgs e)
        {
            AppointmentCalendar appointmentcalendar;
            AppointmentStore appointmentStore = await AppointmentManager.RequestStoreAsync(AppointmentStoreAccessType.AppCalendarsReadWrite);
            try
            {
                appointmentcalendar = await appointmentStore.GetAppointmentCalendarAsync("b, d, 4d");
            }
            catch
            {
                appointmentcalendar = await appointmentStore.CreateAppointmentCalendarAsync("calendar1");
            }
            await appointmentcalendar.SaveAppointmentAsync(appointment);
            System.Diagnostics.Debug.WriteLine("After saved:"+appointment.Recurrence.Until);

        }

        private async void btngetappoint_Click(object sender, RoutedEventArgs e)
        {
            AppointmentStore appointmentStore = await AppointmentManager.RequestStoreAsync(AppointmentStoreAccessType.AllCalendarsReadOnly);
            AppointmentCalendar appointmentcalendar = await appointmentStore.GetAppointmentCalendarAsync("b, d, 4d");
            //Appointment appointments = await appointmentcalendar.GetAppointmentAsync("c, d, 3");
            Appointment appointments = await appointmentcalendar.GetAppointmentAsync(appointment.LocalId);
            System.Diagnostics.Debug.WriteLine("loaded recurrence.until:"+appointments.Recurrence.Until);

        }
    }
}

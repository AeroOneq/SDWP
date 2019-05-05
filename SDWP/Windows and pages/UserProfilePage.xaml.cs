using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using Microsoft.Win32;
using System.IO;
using System.Windows.Threading;

using ApplicationLib.Models;
using ApplicationLib.Exceptions;
using ApplicationLib.Interfaces;
using ApplicationLib.Factories;
using SDWP.Factories;
using SDWP.Interfaces;

namespace SDWP
{
    /// <summary>
    /// Логика взаимодействия для UserProfilePage.xaml
    /// </summary
    public partial class UserProfilePage : Page, IAccountPage
    {
        #region Services
        private IEmailService<UserInfo> EmailService { get; set; }
        private IUserService<UserInfo> UserService { get; set; }
        private IExceptionHandler ExceptionHandler { get; set; }
        #endregion

        #region Properties
        private int CodeID { get; set; } = -1;
        public Action CloseAccGrid { get; set; }

        private byte[] NewUserPhoto { get; set; } = null;
        private bool IsProfileDataEdititing { get; set; } = false;
        private bool PasswordBoxState { get; set; } = false;

        private PageHeader PageHeader { get; set; }
        #endregion

        #region Constructors and Utility methods
        public UserProfilePage(UserInfo user)
        {
            InitializeComponent();

            InitializeInterfaces();
            UploadUserDataToUI();

            PageHeader = pageHeader;
            PageHeader.OnRefresh = UpdateCommonUserAndRefreshUI;
        }

        private void InitializeInterfaces()
        {
            IServiceAbstractFactory serviceFactory = new ServiceAbstractFactory();
            ISdwpAbstractFactory sdwpAbstractFactory = new SdwpAbstractFactory();

            EmailService = serviceFactory.GetEmailService();
            UserService = serviceFactory.GetUserService();
            ExceptionHandler = sdwpAbstractFactory.GetExceptionHandler(Dispatcher);
        }

        private void UploadUserDataToUI()
        {
            nameTextBox.Text = UserInfo.CurrentUser.Name;
            surnameTextBox.Text = UserInfo.CurrentUser.Surname;
            loginTextBox.Text = UserInfo.CurrentUser.Login;
            birthDateTextBox.Text = UserInfo.CurrentUser.BirthDate.
                ToShortDateString();
            emailTextBox.Text = UserInfo.CurrentUser.Email;
            if (UserInfo.CurrentUser.UserPhoto != null && UserInfo.CurrentUser.UserPhoto.Length > 1)
                SetUserPhotoImageImageBrush(UserInfo.CurrentUser.UserPhoto);
        }
        private void SetUserPhotoImageImageBrush(byte[] imageByteArray)
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                memoryStream.Write(imageByteArray, 0, (int)imageByteArray.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);

                BitmapImage userPhotoSource = new BitmapImage();
                userPhotoSource.BeginInit();
                userPhotoSource.StreamSource = memoryStream;
                userPhotoSource.EndInit();

                userPhotoEllipse.Fill = new ImageBrush(userPhotoSource);
            }
            catch (ArgumentNullException ex) { ExceptionHandler.HandleWithMessageBox(ex); }
            catch (IOException ex) { ExceptionHandler.HandleWithMessageBox(ex); }
            catch (Exception ex) { ExceptionHandler.HandleWithMessageBox(ex); }
        }
        #endregion

        #region Grids animations
        private void ShowEnterCodeGrid()
        {
            ChangeEnablePropertyOfUserDataElemenets(false);
            enterCodeGrid.Visibility = Visibility.Visible;
            enterCodeGrid.Margin = new Thickness(0, -500, 0, 0);
            ThicknessAnimation lowerTheGrid = new ThicknessAnimation
            {
                From = enterCodeGrid.Margin,
                To = new Thickness(0, 140, 0, 0),
                Duration = TimeSpan.FromMilliseconds(300),
                DecelerationRatio = 1,
                SpeedRatio = 0.5,
                FillBehavior = FillBehavior.Stop
            };
            lowerTheGrid.Completed += (sender, e) =>
            {
                enterCodeGrid.Margin = new Thickness(0, 140, 0, 0);
            };
            enterCodeGrid.BeginAnimation(FrameworkElement.MarginProperty, lowerTheGrid);
        }

        private void HideEnterCodeGrid()
        {
            ChangeEnablePropertyOfUserDataElemenets(true);
            ThicknessAnimation lowerTheGrid = new ThicknessAnimation
            {
                From = enterCodeGrid.Margin,
                To = new Thickness(0, -500, 0, 0),
                Duration = TimeSpan.FromMilliseconds(300),
                DecelerationRatio = 1,
                SpeedRatio = 0.5,
                FillBehavior = FillBehavior.Stop
            };
            lowerTheGrid.Completed += (sender, e) =>
            {
                enterCodeGrid.Margin = new Thickness(250, -500, 0, 0);
                enterCodeGrid.Visibility = Visibility.Collapsed;
            };
            enterCodeGrid.BeginAnimation(FrameworkElement.MarginProperty, lowerTheGrid);
        }
        #endregion

        #region Edit profile methods
        private void EditProfileData(object sender, RoutedEventArgs e)
        {
            if (!IsProfileDataEdititing)
            {
                ChangeEnablePropertyOfUserDataElemenets(true);
                ChangeButtonsProperties(true);
                IsProfileDataEdititing = true;
            }
            else
            {
                UploadUserDataToUI();
                ChangeEnablePropertyOfUserDataElemenets(false);
                ChangeButtonsProperties(false);
                IsProfileDataEdititing = false;
            }
        }
        private void ChangeButtonsProperties(bool status)
        {
            if (status)
            {
                editProfileDataBtn.Content = "Отмена";
                updateProfileDataBtn.Background = new SolidColorBrush(Color.FromRgb(
                    255, 69, 0));
            }
            else
            {
                editProfileDataBtn.Content = "Редактировать профиль";
                updateProfileDataBtn.Background = new SolidColorBrush(Color.FromRgb(
                    255, 187, 162));
            }
        }

        private void ChangeEnablePropertyOfUserDataElemenets(bool newEnableValue)
        {
            TextBox[] propertiesTextBoxes = userProfileDataGrid.Children.
                OfType<TextBox>().ToArray();
            Array.ForEach(propertiesTextBoxes, tb =>
            {
                tb.IsEnabled = newEnableValue;
                if (newEnableValue)
                    tb.Style = this.Resources["userProfileTextBoxEnabled"] as Style;
                else
                    tb.Style = this.Resources["userProfileTextBoxDisabled"] as Style;
            });

            if (newEnableValue)
                userPhotoBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(
                    255, 69, 0));
            else
                userPhotoBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(
                    255, 187, 162));

            updateProfileDataBtn.IsEnabled = newEnableValue;
        }
        #endregion

        #region Update profile data methods
        private async void UpdatePassword(object sender, RoutedEventArgs e)
        {
            try
            {
                PageHeader.SwitchOnTopLoader();

                await EmailService.SendChangePassLink(UserInfo.CurrentUser);

                PageHeader.SwitchOffTheLoader();
            }
            catch (Exception ex)
            {
                PageHeader.SwitchOffTheLoader();
                ExceptionHandler.HandleWithMessageBox(ex);
            }
        }
        private async void StartUpdatingProcessAsync(object sender, EventArgs e)
        {
            PageHeader.SwitchOnTopLoader();

            if (CheckIfDataChanged())
            {
                PageHeader.SwitchOffTheLoader();
                SDWPMessageBox.ShowSDWPMessageBox("Статус обновления профиля",
                    "Данные не были изменены", MessageBoxButton.OK);
            }
            else
            {
                try
                {
                    UserInfo newUserInfo = CreateNewUserObject();
                    UserInfo.CheckUserProperties(newUserInfo);

                    if (UserInfo.CurrentUser.Login != newUserInfo.Login)
                        await UserService.CheckLogin(newUserInfo.Login);

                    if (newUserInfo.Email != UserInfo.CurrentUser.Email)
                    {
                        await UserService.CheckEmail(newUserInfo.Email);
                        CodeID = await EmailService.SendCodeEmail(newUserInfo);

                        ShowEnterCodeGrid();
                        StartCodeTimer();
                    }
                    else
                    { 
                        await UserService.UpdateRecord(newUserInfo);

                        OnSuccesfullUpdate();
                    }
                }
                catch (NotAppropriateUserParam ex)
                {
                    PageHeader.SwitchOffTheLoader();
                    ExceptionHandler.HandleWithMessageBox(ex);
                }
                catch (Exception ex)
                {
                    PageHeader.SwitchOffTheLoader();
                    ExceptionHandler.HandleWithMessageBox(ex);
                }
            }

            PageHeader.SwitchOffTheLoader();
        }
            
        private void OnSuccesfullUpdate()
        {
            UpdateCommonUserAndRefreshUI();

            Dispatcher.Invoke(() => PageHeader.SwitchOffTheLoader());
            Dispatcher.Invoke(() => SDWPMessageBox.ShowSDWPMessageBox(
                "Статус обновления профиля", "Профиль успешно обновлен",
                MessageBoxButton.OK));
            Dispatcher.Invoke(() => HideEnterCodeGrid());
        }

        private async void UpdateCommonUserAndRefreshUI()
        {
            UserInfo.CurrentUser = await UserService.GetUserByID(UserInfo.CurrentUser.ID);
            Dispatcher.Invoke(() => UploadUserDataToUI());
        }

        private async void UpdateRecordAfterEmailConfirmationAsync(object sender,
            EventArgs e)
        {
            try
            {
                PageHeader.SwitchOnTopLoader();

                UserInfo newUserInfo = CreateNewUserObject();
                if (CodeID != -1 && await EmailService.CheckCode(CodeID, emailCodeTextBox.Text))
                {
                    await UserService.UpdateRecord(newUserInfo);
                    OnSuccesfullUpdate();
                }
                else
                {
                    PageHeader.SwitchOffTheLoader();
                    SDWPMessageBox.ShowSDWPMessageBox("Ошибка подтверждения e-mail",
                        "Вы ввели неверный код", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleWithMessageBox(ex);
            }
        }

        private void StartCodeTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            TimeSpan currentTimerTime = new TimeSpan(0);

            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += async (sender, e) =>
            {
                timeTillCodeExpireTextBlock.Text = "Время до исчезновения кода " +
                    $"{new TimeSpan(0, 2, 0) - currentTimerTime}";

                currentTimerTime += new TimeSpan(0, 0, 1);
                if (currentTimerTime.Seconds == 0 && currentTimerTime.Minutes == 2)
                {
                    await DeleteCode();

                    timeTillCodeExpireTextBlock.Text = "Код недействителен";
                    timer.Stop();
                }
            };

            timer.Start();
        }
        private async Task DeleteCode()
        {
            try
            {
                await EmailService.DeleteCode(CodeID);
                CodeID = -1;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleWithMessageBox(ex);
            }
        }

        private async void CloseEnterCodeGrid(object sender, MouseButtonEventArgs e)
        {
            emailCodeTextBox.Text = string.Empty;
            await DeleteCode();

            PageHeader.SwitchOffTheLoader();
            HideEnterCodeGrid();
        }

        private bool CheckIfDataChanged()
        {
            return nameTextBox.Text == UserInfo.CurrentUser.Name &&
                   surnameTextBox.Text == UserInfo.CurrentUser.Surname &&
                   loginTextBox.Text == UserInfo.CurrentUser.Login &&
                   emailTextBox.Text == UserInfo.CurrentUser.Email &&
                   birthDateTextBox.Text == UserInfo.CurrentUser.BirthDate.ToShortDateString()
                   && NewUserPhoto == null;
        }
        private UserInfo CreateNewUserObject()
        {
            return new UserInfo
            {
                ID = UserInfo.CurrentUser.ID,
                Login = loginTextBox.Text,
                Name = nameTextBox.Text,
                Surname = surnameTextBox.Text,
                BirthDate = GetNewUserDateTime(),
                Email = emailTextBox.Text,
                UserPhoto = NewUserPhoto ?? UserInfo.CurrentUser.UserPhoto
            };
        }
        private DateTime GetNewUserDateTime()
        {
            string[] dateElements = birthDateTextBox.Text.Split('.');

            if (dateElements.Length != 3 || !int.TryParse(dateElements[0], out int day) ||
                !int.TryParse(dateElements[1], out int month) ||
                !int.TryParse(dateElements[2], out int year) || day < 0 || day > 31 ||
                month < 0 || month > 12 || year < 1900)
                throw new NotAppropriateUserParam("Дата рождения введена неверно");

            return new DateTime(year, month, day);
        }
        #endregion

        #region Event handlers
        private void RepositionElements(object sender, SizeChangedEventArgs e)
        {
            userProfileGrid.Width = this.Width;
            pageScrollViewer.Height = this.Height;
            contentOutterGrid.Height = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        private void UserPhotoMouseEnter(object sender, MouseEventArgs e)
        {
            if (IsProfileDataEdititing)
                userPhotoBorder.BorderBrush = new SolidColorBrush(Colors.Orange);
        }

        private void UserPhotoMouseLeave(object sender, MouseEventArgs e)
        {
            if (IsProfileDataEdititing)
                userPhotoBorder.BorderBrush = new SolidColorBrush(Colors.OrangeRed);
        }

        /// <summary>
        /// Shows a file open dialog to choose a new picture 
        /// </summary>
        private void UserPhotoMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsProfileDataEdititing)
            {
                string imagePath = string.Empty;
                OpenFileDialog chooseImageDialog = new OpenFileDialog
                {
                    Filter = "Фотографии(*.JPG;*.PNG)|*.JPG;*.PNG",
                    Multiselect = false,
                    CheckFileExists = true
                };
                if (chooseImageDialog.ShowDialog() == true)
                    imagePath = chooseImageDialog.FileName;
                if (imagePath != string.Empty)
                {
                    NewUserPhoto = CreateByteRepresentationOfAnImage(imagePath);
                    SetUserPhotoImageImageBrush(NewUserPhoto);
                }
            }
        }

        private byte[] CreateByteRepresentationOfAnImage(string imagePath)
        {
            try
            {
                FileStream fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
                byte[] imageByteArr = new byte[fileStream.Length];
                fileStream.Read(imageByteArr, 0, (int)fileStream.Length);
                fileStream.Close();
                return imageByteArr;
            }
            catch (ArgumentNullException ex)
            {
                ExceptionHandler.HandleWithMessageBox(ex);
                return new byte[0];
            }
            catch (IOException ex)
            {
                ExceptionHandler.HandleWithMessageBox(ex);
                return new byte[0];
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleWithMessageBox(ex);
                return new byte[0];
            }
        }

        private void UserProfileBtnMouseEnter(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            button.Background = new SolidColorBrush(Colors.White);
            button.Foreground = new SolidColorBrush(Colors.OrangeRed);
        }
        private void UserProfileBtnMouseLeave(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            button.Background = new SolidColorBrush(Colors.OrangeRed);
            button.Foreground = new SolidColorBrush(Colors.White);
        }

        private void CancelTextBlockMouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock cancelTextBlock = sender as TextBlock;
            cancelTextBlock.TextDecorations = TextDecorations.Underline;
        }
        private void CancelTextBlockMouseLeave(object sender, MouseEventArgs e)
        {
            TextBlock cancelTextBlock = sender as TextBlock;
            cancelTextBlock.TextDecorations = null;
        }
        #endregion
    }
}

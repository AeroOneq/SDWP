using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ApplicationLib.Database;
using System.Windows.Media.Animation;
using AeroORMFramework;
using Microsoft.Win32;
using System.IO;
using System.Windows.Threading;
using ApplicationLib.Models;
using ApplicationLib.Exceptions;
using ApplicationLib.Services;

namespace SDWP
{
    /// <summary>
    /// Логика взаимодействия для UserProfilePage.xaml
    /// </summary
    public partial class UserProfilePage : Page
    {
        #region Properties
        private byte[] NewUserPhoto { get; set; } = null;
        private Connector Connector { get; } = new Connector(
            DatabaseProperties.ConnectionString);
        private bool IsProfileDataEdititing { get; set; } = false;
        private bool PasswordBoxState { get; set; } = false;
        #endregion

        #region Constructors and Utility methods
        public UserProfilePage(UserInfo user)
        {
            InitializeComponent();
            UploadUserDataToUI();
        }
        private void UploadUserDataToUI()
        {
            nameTextBox.Text = CommonObjects.User.Name;
            surnameTextBox.Text = CommonObjects.User.Surname;
            loginTextBox.Text = CommonObjects.User.Login;
            passwordTextBox.Text = passwordPasswordBox.Password =
                CommonObjects.User.Password;
            birthDateTextBox.Text = CommonObjects.User.BirthDate.
                ToShortDateString();
            emailTextBox.Text = CommonObjects.User.Email;
            if (CommonObjects.User.UserPhoto != null && CommonObjects.User.UserPhoto.Length > 1)
                SetUserPhotoImageImageBrush(CommonObjects.User.UserPhoto);
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
#warning Decompose
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

            PasswordBox[] passwordBoxes = userProfileDataGrid.Children.
                OfType<PasswordBox>().ToArray();
            Array.ForEach(passwordBoxes, pb =>
            {
                pb.IsEnabled = newEnableValue;
                if (newEnableValue)
                    pb.Style = this.Resources["userProfilePasswordBoxEnabled"] as Style;
                else
                    pb.Style = this.Resources["userProfilePasswordBoxDisabled"] as Style;
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
        private async void StartUpdatingProcessAsync(object sender, EventArgs e)
        {
            SwitchOnTopLoader();
            if (CheckIfDataChanged())
            {
                SwitchOffTheLoader();
                SDWPMessageBox.ShowSDWPMessageBox("Статус обновления профиля",
                    "Данные не были изменены", MessageBoxButton.OK);
            }
            else
            {
                try
                {
                    UserInfo newUserInfo = CreateNewUserObject();

                    UserInfo.CheckUserProperties(newUserInfo);

                    if (newUserInfo.Email != CommonObjects.User.Email)
                    {
                        await UserService.GetService.CheckEmail(newUserInfo.Email);
                        await EmailService.GetService.SendCodeEmail(newUserInfo);

                        ShowEnterCodeGrid();
                        StartCodeTimer();
                    }
                    else
                    {
                        if (CommonObjects.User.Login != newUserInfo.Login)
                            await UserService.GetService.CheckLogin(newUserInfo.Login);

                        await UserService.GetService.UpdateRecord(newUserInfo);
                        await EmailService.GetService.ResetCode();

                        OnSuccesfullUpdate();
                    }
                }
                catch (NotAppropriateUserParam ex)
                {
                    await EmailService.GetService.ResetCode();
                    SwitchOffTheLoader();
                    ExceptionHandler.HandleWithMessageBox(ex);
                }
                catch (Exception ex)
                {
                    await EmailService.GetService.ResetCode();
                    SwitchOffTheLoader();
                    ExceptionHandler.HandleWithMessageBox(ex);
                }
            }
            SwitchOffTheLoader();
        }
        private void OnSuccesfullUpdate()
        {
            UpdateCommonUserAndRefreshUI();
            Dispatcher.Invoke(() => SwitchOffTheLoader());
            Dispatcher.Invoke(() => SDWPMessageBox.ShowSDWPMessageBox(
                "Статус обновления профиля", "Профиль умпешно обновлен",
                MessageBoxButton.OK));
            Dispatcher.Invoke(() => HideEnterCodeGrid());
        }

        private async void UpdateCommonUserAndRefreshUI()
        {
            CommonObjects.User = await UserService.GetService.GetUser("ID",
                CommonObjects.User.ID);
            Dispatcher.Invoke(() => UploadUserDataToUI());
        }

        private async void UpdateRecordAfterEmailConfirmationAsync(object sender,
            EventArgs e)
        {
            SwitchOnTopLoader();
            UserInfo newUserInfo = CreateNewUserObject();
            if (!(EmailService.GetService.Code == null) && emailCodeTextBox.Text == EmailService.GetService.Code)
                await UserService.GetService.UpdateRecord(newUserInfo);
            else
            {
                SwitchOffTheLoader();
                SDWPMessageBox.ShowSDWPMessageBox("Ошибка подтверждения e-mail",
                    "Вы ввели неверный код", MessageBoxButton.OK);
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
                    await EmailService.GetService.ResetCode();
                    timeTillCodeExpireTextBlock.Text = "Код недействителен";
                    timer.Stop();
                }
            };

            timer.Start();
        }
        private async void CloseEnterCodeGrid(object sender, MouseButtonEventArgs e)
        {
            emailCodeTextBox.Text = string.Empty;
            await EmailService.GetService.ResetCode();
            HideEnterCodeGrid();
        }

        private bool CheckIfDataChanged()
        {
            return nameTextBox.Text == CommonObjects.User.Name &&
                   surnameTextBox.Text == CommonObjects.User.Surname &&
                   loginTextBox.Text == CommonObjects.User.Login &&
                   passwordTextBox.Text == CommonObjects.User.Password &&
                   emailTextBox.Text == CommonObjects.User.Email &&
                   birthDateTextBox.Text == CommonObjects.User.BirthDate.ToShortDateString()
                   && NewUserPhoto == null;
        }
        private UserInfo CreateNewUserObject()
        {
            return new UserInfo
            {
                ID = CommonObjects.User.ID,
                Login = loginTextBox.Text,
                Password = passwordTextBox.Text,
                Name = nameTextBox.Text,
                Surname = surnameTextBox.Text,
                BirthDate = GetNewUserDateTime(),
                Email = emailTextBox.Text,
                UserPhoto = NewUserPhoto ?? CommonObjects.User.UserPhoto
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
            headerRect.Width = this.Width;
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

        private void RefreshIconMouseEnter(object sender, EventArgs e)
        {
            refreshIconActive.Visibility = Visibility.Visible;
            refreshIconStatic.Visibility = Visibility.Collapsed;
        }
        private void RefreshIconMouseLeave(object sender, EventArgs e)
        {
            refreshIconActive.Visibility = Visibility.Collapsed;
            refreshIconStatic.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Gets an user object from the database, refreshes the ui 
        /// and updates the CommonObjects.User
        /// </summary>
        private async void RefreshUserData(object sender, EventArgs e)
        {
            SwitchOnTopLoader();
            await Task.Run(() => UpdateCommonUserAndRefreshUI());
            SwitchOffTheLoader();
        }

        private void SeePasswordIconMouseDown(object sender, EventArgs e)
        {
            if (!PasswordBoxState)
            {
                passwordTextBox.Visibility = Visibility.Visible;
                passwordPasswordBox.Visibility = Visibility.Collapsed;
                PasswordBoxState = true;
            }
            else
            {
                passwordTextBox.Visibility = Visibility.Collapsed;
                passwordPasswordBox.Visibility = Visibility.Visible;
                PasswordBoxState = false;
            }
        }
        private void SeePasswordIconMouseLeave(object sender, EventArgs e)
        {
            seePasswordActiveIcon.Visibility = Visibility.Collapsed;
            seePasswordStaticIcon.Visibility = Visibility.Visible;
        }
        private void SeePasswordIconMouseEnter(object sender, EventArgs e)
        {
            seePasswordActiveIcon.Visibility = Visibility.Visible;
            seePasswordStaticIcon.Visibility = Visibility.Collapsed;
        }

        private void PasswordTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (passwordTextBox != null && passwordPasswordBox != null)
                passwordPasswordBox.Password = passwordTextBox.Text;
        }
        private void PasswordPasswordBoxTextChanged(object sender, RoutedEventArgs e)
        {
            if (passwordTextBox != null && passwordPasswordBox != null)
                passwordTextBox.Text = passwordPasswordBox.Password;
        }
        #endregion

        #region Top loader opertaions
        private void SwitchOnTopLoader()
        {
            topLoaderGrid.Visibility = Visibility.Visible;
            List<Ellipse> ellipsesList = topLoaderGrid.Children.Cast<Ellipse>().
                ToList();

            ellipsesList[2].BeginAnimation(FrameworkElement.MarginProperty,
                this.Resources["topLoaderThirdEllipseAnimation"] as
                ThicknessAnimationUsingKeyFrames);
            ellipsesList[1].BeginAnimation(FrameworkElement.MarginProperty,
                this.Resources["topLoaderSecondEllipseAnimation"] as
                ThicknessAnimationUsingKeyFrames);
            ellipsesList[0].BeginAnimation(FrameworkElement.MarginProperty,
                this.Resources["topLoaderFirstEllipseAnimation"] as
                ThicknessAnimationUsingKeyFrames);
        }
        private void SwitchOffTheLoader()
        {
            List<Ellipse> ellipsesList = topLoaderGrid.Children.Cast<Ellipse>().
                 ToList();

            ellipsesList[2].BeginAnimation(FrameworkElement.MarginProperty, null);
            ellipsesList[1].BeginAnimation(FrameworkElement.MarginProperty, null);
            ellipsesList[0].BeginAnimation(FrameworkElement.MarginProperty, null);
        }
        #endregion
    }
}

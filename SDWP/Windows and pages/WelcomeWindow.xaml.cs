using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using AeroORMFramework;
using System.Data.SqlTypes;
using System.Net.Mail;

using ApplicationLib.Models;
using ApplicationLib.Exceptions;
using ApplicationLib.Services;
using ApplicationLib.Interfaces;
using ApplicationLib.Factories;

using SDWP.Factories;
using SDWP.Exceptions;
using SDWP.Interfaces;

namespace SDWP
{
    /// <summary>
    /// Логика взаимодействия для WelcomeWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Services
        private IEmailService<UserInfo> EmailService { get; set; }
        private IUserService<UserInfo> UserService { get; set; }
        private IExceptionHandler ExceptionHandler { get; set; }
        #endregion

        #region Properties
        private UserInfo NewUser { get; set; }
        private int CodeID { get; set; } = -1;
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            InitializeInterfaces();

            loginTextBox.Focus();
            //initialize the list of all grids
            WelcomePageRightGridAnimations.RightGridsList = new List<Grid>
            {
                applicationDescriptionGrid,
                remindPassGrid,
                createAnAccountGrid
            };
        }

        private void InitializeInterfaces()
        {
            IServiceAbstractFactory serviceFactory = new ServiceAbstractFactory();
            ISdwpAbstractFactory sdwpAbstractFactory = new SdwpAbstractFactory();

            EmailService = serviceFactory.GetEmailService();
            UserService = serviceFactory.GetUserService();

            ExceptionHandler = sdwpAbstractFactory.GetExceptionHandler(Dispatcher);
        }

        #region Registration process
        /// <summary>
        /// Checks the input data in the registration form, and if everything is alright
        /// redirects user to the enter code grid
        /// </summary>
        private async void GoToEnterTheCodeGrid(object sender, RoutedEventArgs e)
        {
            try
            {
                SwitchOnTheLoader(rightCreateAccLoaderGrid);

                UserInfo newUser = CreateNewUser();
                bool checkingResult = await CheckUserInputDataAsync(newUser);

                if (checkingResult)
                    SendEmailAndGoToCodeGridAsync(newUser);
            }
            catch (NotAppropriateParamException ex)
            {
                HandleAccCreationException(ex);
            }
            catch (InvalidCastException ex)
            {
                HandleAccCreationException(ex);
            }
            catch (Exception ex)
            {
                HandleAccCreationException(ex);
            }
        }

        /// <summary>
        /// Checks all properties of user object and 
        /// availability of login and email
        /// </summary>
        private async Task<bool> CheckUserInputDataAsync(UserInfo newUser)
        {
            try
            {
                UserInfo.CheckUserProperties(newUser);

                await UserService.CheckLogin(newUser.Login);
                await UserService.CheckEmail(newUser.Email);

                return true;
            }
            catch (NotAppropriateUserParam ex)
            {
                HandleAccCreationException(ex);
                return false;
            }
            catch (SqlException ex)
            {
                HandleAccCreationException(ex);
                return false;
            }
            catch (Exception ex)
            {
                HandleAccCreationException(ex);
                return false;
            }
        }

        /// <summary>
        /// Sends an email with a code to confirm this email and then redirects
        /// user to the 'enter code grid'
        /// </summary>
        private async void SendEmailAndGoToCodeGridAsync(UserInfo newUser)
        {
            try
            {
                NewUser = newUser;
                CodeID = await EmailService.SendCodeEmail(NewUser);
            }
            catch (InvalidOperationException ex)
            {
                HandleAccCreationException(ex);
            }
            catch (ArgumentNullException ex)
            {
                HandleAccCreationException(ex);
            }
            catch (Exception ex)
            {
                HandleAccCreationException(ex);
            }
            finally
            {
                SwitchOffTheLoader(rightCreateAccLoaderGrid);
                enterTheEmailCodeGrid.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Checks the code entered by user and if it is correct
        /// creates new account into the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CreateNewAccount(object sender, RoutedEventArgs e)
        {
            try
            {
                (sender as Button).IsEnabled = false;
                string code = codeTextBox.Text;

                await CheckCodeAndCreateAccAsync(code);
            }
            catch (NotAppropriateUserParam ex)
            {
                HandleAccCreationException(ex);
            }
            catch (InvalidCastException ex)
            {
                HandleAccCreationException(ex);
            }
            catch (Exception ex)
            {
                HandleAccCreationException(ex);
            }
            finally
            {
                (sender as Button).IsEnabled = true;
            }
        }

        private async Task CheckCodeAndCreateAccAsync(string code)
        {
            SwitchOnTheLoader(rightCreateAccLoaderGrid);

            if (await EmailService.CheckCode(CodeID, code))
            {
                await UserService.CreateNewAccountAsync(NewUser);
                await DeleteCode();

                SwitchOffTheLoader(rightCreateAccLoaderGrid);

                SDWPMessageBox.ShowSDWPMessageBox("Статус создание аккаунта",
                    "Аккаунт был успешно создан", MessageBoxButton.OK);

                enterTheEmailCodeGrid.Visibility = Visibility.Collapsed;
                WelcomePageRightGridAnimations.HideTheGrid(createAnAccountGrid);
            }
            else
            {
                SwitchOffTheLoader(rightCreateAccLoaderGrid);
                SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Вы ввели неверный код", MessageBoxButton.OK);
            }
        }

        private async Task DeleteCode()
        {
            await EmailService.DeleteCode(CodeID);
            CodeID = -1;
        }

        /// <summary>
        /// Tries to create a new UserInfo object based on the data given by user
        /// </summary>
        private UserInfo CreateNewUser()
        {
            return new UserInfo
            {
                Login = regLoginTextBox.Text,
                Password = regPassTextBox.Password.GetHashCode().ToString(),
                Name = regNameTextBox.Text,
                Surname = regSurnameTextBox.Text,
                BirthDate = GetUserBirthDate(),
                Email = regEmailTextBox.Text
            };
        }

        /// <summary>
        /// Creates a DateTime object with the input year, month and day, 
        /// which are entered by user
        /// </summary>
        private DateTime GetUserBirthDate()
        {
            try
            {
                int year = int.Parse(regYearOfBirthTextBox.Text);
                int month = int.Parse(regMonthOfBirthTextBox.Text);
                int day = int.Parse(regDayOfBirthTextBox.Text);

                if (year >= DateTime.Now.Year || year <= 1900)
                    throw new NotAppropriateUserParam("Неверно введен год рождения. ");
                if (month <= 0 || month > 12)
                    throw new NotAppropriateUserParam("Неверно введен месяц рождения");
                if (day <= 0 || day > DateTime.DaysInMonth(year, month))
                    throw new NotAppropriateParamException("Неверно введен день рождения");

                return new DateTime(year, month, day);
            }
            catch
            {
                throw new NotAppropriateUserParam("Неверно введенна дата рождения");
            }
        }

        /// <summary>
        /// Handles any exception which occures in the process of account creation
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private bool HandleAccCreationException(Exception ex)
        {
            Dispatcher.Invoke(() => SwitchOffTheLoader(rightCreateAccLoaderGrid));
            ExceptionHandler.HandleWithMessageBox(ex);
            return false;
        }
        #endregion

        #region Autharization process
        /// <summary>
        /// Creates the LoginData object basing on the input data and
        /// initializes the process of authorization
        /// </summary>
        private void LoginBtnMouseClick(object sender, EventArgs eArgs)
        {
            try
            {
                (sender as Button).IsEnabled = false;
                LoginData loginData = new LoginData
                {
                    Login = loginTextBox.Text,
                    Password = passwordTextBox.Password
                };

                TryToLoginAsync(loginData);
            }
            finally
            {
                (sender as Button).IsEnabled = true;
            }
        }

        /// <summary>
        /// Handles every exception which can arise in the login process
        /// </summary>
        /// <returns>
        /// An empty UserInfo object
        /// </returns>
        private UserInfo HandleExceptionAndReturnEmptyUser(Exception ex)
        {
            Dispatcher.Invoke(() => SwitchOffTheLoader(leftLoaderGrid));
            ExceptionHandler.HandleWithMessageBox(ex);
            return null;
        }

        /// <summary>
        /// Tries to login with the given login data 
        /// </summary>
        /// <returns>
        /// The user object if the input data is correct,
        /// the empty user object otherwise
        /// </returns>
        private async Task<UserInfo> LoginAndReturnUserObj(LoginData loginData)
        {
            try
            {
                return await UserService.AuthorizeUserAsync(loginData);
            }
            catch (UserNotFoundException ex)
            {
                return HandleExceptionAndReturnEmptyUser(ex);
            }
            catch (SqlException ex)
            {
                return HandleExceptionAndReturnEmptyUser(ex);
            }
            catch (Exception ex)
            {
                return HandleExceptionAndReturnEmptyUser(ex);
            }
        }

        /// <summary>
        /// Tries to login into the system with the given input login data
        /// </summary>
        private async void TryToLoginAsync(LoginData loginData)
        {
            SwitchOnTheLoader(leftLoaderGrid);
            UserInfo user = await LoginAndReturnUserObj(loginData);
            if (user != null)
            {
                SwitchOffTheLoader(leftLoaderGrid);
                Hide();

                UserInfo.CurrentUser = user;
                SDWPMainWindow mainWindow = new SDWPMainWindow(UserInfo.CurrentUser);

                mainWindow.Show();
            }
        }
        #endregion

        #region Hint grid processes
        /// <summary>
        /// Shows the hint grid when the question mark is tapped
        /// </summary>
        private void ShowHintGrid(object sender, EventArgs eArgs)
        {
            Image pressedImage = sender as Image;
            WelcomePageBottomGridAnimation.ShowTheHintGrid(registrationHintGrid);
            inputDataNameTextBlock.Text = GetInputDataName(pressedImage.Name);
            inputDataDescriptionTextBlock.Text = GetInputDataDescription(pressedImage.Name);
        }

        private void CloseTheHintGrid(object sender, EventArgs eArgs)
        {
            WelcomePageBottomGridAnimation.CloseTheHintGrid(registrationHintGrid);
        }

        private string GetInputDataName(string imageName)
        {
            if (imageName.IndexOf("login") > -1)
                return "Логин";
            if (imageName.IndexOf("password") > -1)
                return "Пароль";
            if (imageName.IndexOf("name") > -1)
                return "Имя";
            if (imageName.IndexOf("surN") > -1)
                return "Фамилия";
            if (imageName.IndexOf("bDay") > -1)
                return "Дата рождения";
            if (imageName.IndexOf("email") > -1)
                return "E-mail";
            return string.Empty;
        }

        private string GetInputDataDescription(string imageName)
        {
            if (imageName.IndexOf("login") > -1)
                return "Длина логина должна быть больше 6 и меньше 200 символов, логин должен содержать только " +
                    "буквы английского алфавита или цифры.";
            if (imageName.IndexOf("password") > -1)
                return "Пароль состоит из любых символов, пароль должен быть состоять минимум из 8 символов.";
            if (imageName.IndexOf("name") > -1)
                return "Длина имени должна быть больше одного.";
            if (imageName.IndexOf("surN") > -1)
                return "Длина фамилии должна быть больше двух.";
            if (imageName.IndexOf("bDay") > -1)
                return "В первое поле для ввода введите день рождения, во второе - месяц, в третье - год.";
            if (imageName.IndexOf("email") > -1)
                return "Используйте стандартные правила написания адреса электронной почты";
            return string.Empty;
        }
        #endregion

        #region Welcome button mouse enter/leave events
        /// <summary>
        /// Makes the background white and the text the color of main theme
        /// </summary>
        private void WelcomeBtnMouseEnter(object sender, EventArgs eArgs)
        {
            Button button = sender as Button;
            button.Background = new SolidColorBrush(Colors.White);
            button.Foreground = new SolidColorBrush(Colors.OrangeRed);
        }
        /// <summary>
        /// Makes the background color the color of main theme and text white
        /// </summary>
        private void WelcomeBtnMouseLeave(object sender, EventArgs eArgs)
        {
            Button button = sender as Button;
            button.Background = new SolidColorBrush(Colors.OrangeRed);
            button.Foreground = new SolidColorBrush(Colors.White);
        }
        #endregion

        #region Loader opertaions
        private void SwitchOnTheLoader(Grid loaderGrid)
        {
            loaderGrid.Visibility = Visibility.Visible;
            Rectangle rect3 = loaderGrid.Children[2] as Rectangle;
            Rectangle rect2 = loaderGrid.Children[1] as Rectangle;
            Rectangle rect1 = loaderGrid.Children[0] as Rectangle;
            rect3.BeginAnimation(MarginProperty,
                (ThicknessAnimationUsingKeyFrames)authWindow.Resources["leftLoaderThirdRectAnimation"]);
            rect2.BeginAnimation(MarginProperty,
                (ThicknessAnimationUsingKeyFrames)authWindow.Resources["leftLoaderSecondRectAnimation"]);
            rect1.BeginAnimation(MarginProperty,
                (ThicknessAnimationUsingKeyFrames)authWindow.Resources["leftLoaderFirstRectAnimation"]);
        }
        private void SwitchOffTheLoader(Grid loaderGrid)
        {
            loaderGrid.Visibility = Visibility.Collapsed;
            Rectangle rect3 = loaderGrid.Children[2] as Rectangle;
            Rectangle rect2 = loaderGrid.Children[1] as Rectangle;
            Rectangle rect1 = loaderGrid.Children[0] as Rectangle;
            rect3.BeginAnimation(MarginProperty, null);
            rect2.BeginAnimation(MarginProperty, null);
            rect1.BeginAnimation(MarginProperty, null);
        }
        #endregion

        #region Active text boxes mouse enter/leave events
        /// <summary>
        /// Underlines the text of a textblock
        /// </summary>
        private void ForgotPassTextBlockMouseEnter(object sender, MouseEventArgs eArgs)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.TextDecorations.Add(TextDecorations.Underline);
        }
        /// <summary>
        /// Makes the text of a textblock non-underlined
        /// </summary>
        private void ForgotPassTextBlockMouseLeave(object sender, MouseEventArgs eArgs)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.TextDecorations.Clear();
        }
        #endregion

        #region Remind pass process
        /// <summary>
        /// Handles all exceptions which can arise during remind pass process
        /// </summary>
        private void HandleRemindPassExceptions(Exception ex)
        {
            Dispatcher.Invoke(() => SwitchOffTheLoader(rightRemindPassLoaderGrid));
            ExceptionHandler.HandleWithMessageBox(ex);
        }

        private async void SendNewPassToUser(object sender, RoutedEventArgs e)
        {
            try
            {
                SwitchOnTheLoader(rightRemindPassLoaderGrid);

                string login = remindPassEnterLoginTextBox.Text;
                string email = remindPassEnterEmailTextBox.Text;

                bool remindPassResult = await RemindPassAsync(login, email);

                SwitchOffTheLoader(rightRemindPassLoaderGrid);

                if (remindPassResult)
                {
                    SDWPMessageBox.ShowSDWPMessageBox("Статус обновления пароля",
                        "Письмо было выслано Вам на почту", MessageBoxButton.OK);
                }
            }
            catch (InvalidCastException ex)
            {
                HandleRemindPassExceptions(ex);
            }
            catch (Exception ex)
            {
                HandleRemindPassExceptions(ex);
            }
        }

        /// <summary>
        /// Initializes the process of pass restoration
        /// </summary>
        /// <returns>
        /// True if everything is OK, false otherwise
        /// </returns>
        private async Task<bool> RemindPassAsync(string login, string email)
        {
            try
            {
                await UserService.RemindPassAsync(login, email);
                return true;
            }
            catch (UserNotFoundException ex)
            {
                HandleRemindPassExceptions(ex);
                return false;
            }
            catch (SqlException ex)
            {
                HandleRemindPassExceptions(ex);
                return false;
            }
            catch (Exception ex)
            {
                HandleRemindPassExceptions(ex);
                return false;
            }
        }
        #endregion

        private void ForgotPassTextBlockClick(object sender, EventArgs eArgs)
        {
            WelcomePageRightGridAnimations.ShowTheGrid(remindPassGrid);
        }
        private void CloseTheRemindPassGrid(object sender, MouseButtonEventArgs e)
        {
            WelcomePageRightGridAnimations.HideTheGrid(remindPassGrid);
        }
        private void CloseTheRegistrationGrid(object sender, RoutedEventArgs eArgs)
        {
            WelcomePageRightGridAnimations.HideTheGrid(createAnAccountGrid);
        }
        private void OpenRegistrationGrid(object sender, RoutedEventArgs e)
        {
            enterTheEmailCodeGrid.Visibility = Visibility.Collapsed;
            WelcomePageRightGridAnimations.ShowTheGrid(createAnAccountGrid);
        }

        private void QuestionMarkMouseEnter(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
            List<Image> imageList = ((Grid)image.Parent).Children.OfType<Image>().ToList();
            int imageIndex = imageList.FindIndex(x => x.Name == image.Name);
            image.Visibility = Visibility.Hidden;
            imageList[imageIndex + 1].Visibility = Visibility.Visible;
        }
        private void QuestionMarkMouseLeave(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
            List<Image> imageList = ((Grid)image.Parent).Children.OfType<Image>().ToList();
            int imageIndex = imageList.FindIndex(x => x.Name == image.Name);
            image.Visibility = Visibility.Hidden;
            imageList[imageIndex - 1].Visibility = Visibility.Visible;
        }

        private void CloseHintMouseEnter(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
            List<Image> imageList = ((Grid)image.Parent).Children.OfType<Image>().ToList();
            int imageIndex = imageList.FindIndex(x => x.Name == image.Name);
            image.Visibility = Visibility.Hidden;
            imageList[imageIndex + 1].Visibility = Visibility.Visible;
        }

        private void CloseHintMouseLeave(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
            List<Image> imageList = ((Grid)image.Parent).Children.OfType<Image>().ToList();
            int imageIndex = imageList.FindIndex(x => x.Name == image.Name);
            image.Visibility = Visibility.Hidden;
            imageList[imageIndex - 1].Visibility = Visibility.Visible;
        }
    }
}

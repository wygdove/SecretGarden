using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ServiceModel;
using System.Windows.Ink;
using System.Windows.Media.Animation;
using SecretGardenClient.SecretGardenServiceReference;
using SecretGardenCliet.UControl;

namespace SecretGardenClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, ISecretGardenServiceCallback
    {
        SecretGardenServiceClient client;
        User user;
        bool logined=false;
        Room room;
        Button btn;
        DrawingAttributes inkDA;
        string str = "";

        public MainWindow(string username, string password) 
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
            this.inputUsername.Text = username;
            this.inputPassword.Password = password;
        }

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
        }

        async void myCanvas_StrokeCollected(object sender, Object e)
        {
            StrokeCollectionConverter scc = new StrokeCollectionConverter();
            await client.SendInkAsync(room.id, scc.ConvertToString(myCanvas.Strokes));
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            if (logined) {
                client.Logout(user.name);
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InstanceContext context = new InstanceContext(this);
            client = new SecretGardenServiceClient(context);
            FormLogin.Visibility = Visibility.Visible;
            FormMain.Visibility = Visibility.Collapsed;
            btn = btnblack;
            inkDA = new DrawingAttributes()
            {
                Color = Colors.Black,
                Height = 12,
                Width = 12
            };
            myCanvas.UseCustomCursor = true;
            myCanvas.Cursor = Cursors.Pen;
            myCanvas.DefaultDrawingAttributes = inkDA;
            myCanvas.StrokeCollected += myCanvas_StrokeCollected;
        }

        private  void buttonLogin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ("" == inputUsername.Text || "" == inputPassword.Password) 
            {
                MessageBox.Show("用户名或密码不能为空！");
                return;
            }
            client.Login(inputUsername.Text, inputPassword.Password); 
            buttonLogin.IsEnabled = false; 
        }


        public void CallbackShowLoginStatus(int loginCode, User user)
        {
            if (loginCode == 0) {
                logined = true;
                this.user=user;
                FormLogin.Visibility = Visibility.Collapsed;
                FormRegist.Visibility = Visibility.Collapsed;
                FormMain.Visibility = Visibility.Visible;
                PageHall.Visibility = Visibility.Visible;
                //this.doAnimation(1);
                info_user.Content = user.name;
            }else{
                logined=false;
                FormLogin.Visibility = Visibility.Visible;
                FormRegist.Visibility = Visibility.Collapsed;
                FormMain.Visibility = Visibility.Collapsed;
                MessageBox.Show("登录失败");
                buttonLogin.IsEnabled = true; 
            }
        }

        public void CallbackShowHall(Dictionary<int, Room> rooms)
        {
            Image image = new Image();
            Uri uri = new Uri(string.Format("{0}{1}.png", "\\icons\\", user.icon.ToString()), UriKind.Relative);
            IconShower.Source = new BitmapImage(uri);

            tablesArea.Children.Clear();

            info_user.Content = user.name;

            int numroom = rooms.Count();
            int temp = numroom / 4;
            if (numroom % 4 != 0)
                temp += 1;
            
            int[] roomID = rooms.Keys.ToArray();
            int k = 0;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < temp; j++)
                {
                    RowDefinition rd = new RowDefinition();
                    rd.Height = GridLength.Auto;
                    tablesArea.RowDefinitions.Add(rd);

                    ColumnDefinition cd = new ColumnDefinition();
                    cd.Width = GridLength.Auto;
                    tablesArea.ColumnDefinitions.Add(cd);
                }
            }

            for (int i = 0; i < temp; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Table uc = new Table();
                    
                    uc.Tag = roomID[k];
                    uc.MouseLeftButtonDown += uc_MouseLeftButtonDown;
                    uc.MouseEnter += uc_MouseEnter; 
                    uc.MouseLeave += uc_MouseLeave;
                    try
                    {
                        uc.textblock1.Visibility = Visibility.Collapsed; 
                        uc.textblock2.Visibility = Visibility.Collapsed; 
                        uc.textblock3.Visibility = Visibility.Collapsed; 
                        uc.textblock4.Visibility = Visibility.Collapsed; 
                        uc.textblock5.Visibility = Visibility.Collapsed;
                        uc.textblock1.Text = rooms[roomID[k]].users[0].name;
                        uc.textblock1.Visibility = Visibility.Visible;
                        uc.textblock2.Text = rooms[roomID[k]].users[1].name;
                        uc.textblock2.Visibility = Visibility.Visible;
                        uc.textblock3.Text = rooms[roomID[k]].users[2].name;
                        uc.textblock3.Visibility = Visibility.Visible; 
                        uc.textblock4.Text = rooms[roomID[k]].users[3].name;
                        uc.textblock4.Visibility = Visibility.Visible; 
                        uc.textblock5.Text = rooms[roomID[k]].users[4].name;
                        uc.textblock5.Visibility = Visibility.Visible;
                    }
                    catch { }
                    Grid.SetRow(uc, i);
                    Grid.SetColumn(uc, j);

                    tablesArea.Children.Add(uc);
                    numroom--;
                    if (numroom == 0)
                        break;
                    k++;
                }
            }
        }

        void uc_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        void uc_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void uc_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FormLogin.Visibility = System.Windows.Visibility.Collapsed;
            FormMain.Visibility = System.Windows.Visibility.Visible;
            PageHall.Visibility = System.Windows.Visibility.Collapsed;
            PagePlay.Visibility = System.Windows.Visibility.Visible;
            //this.doAnimation(2);
            client.EnterRoom(user.name, (int)((Table)sender).Tag);
        }

        public void CallbackShowRoom(Room room)
        {
            drawer.Source = new BitmapImage(new Uri(string.Format("{0}{1}.png", "\\icons\\", room.users[0].icon.ToString()), UriKind.Relative));
            RoomUserShow.Items.Clear();
            RoomUserIcons.Items.Clear();
            this.room = room;
            foreach (var u in room.users)
            {
                Image image = new Image();
                Uri uri = new Uri(string.Format("{0}{1}.png", "\\icons\\", u.icon.ToString()), UriKind.Relative);
                image.Source = new BitmapImage(uri);
                image.Width = 25;
                image.Height = 25;
                RoomUserIcons.Items.Add(image);
                RoomUserShow.Items.Add(u.name);
            }
            welcome.Source = new Uri(".\\..\\..\\images\\welcome.gif", UriKind.Relative);

            if (room.users[0].name == user.name)
            {
                myCanvas.IsEnabled = true;
                PenSelector.Visibility = Visibility.Visible;
                send.IsEnabled = false;

                msgBox.IsEnabled = false;

            }
            else
            {
                PenSelector.Visibility = Visibility.Collapsed;
                send.IsEnabled = true;

                msgBox.IsEnabled = true;
                myCanvas.IsEnabled = false;

            }

        }
        public void CallbackShowMessage(string who, string message)
        {
            string msgStr =string.Format( "{0}：{1}",who,message);
            InfoShow.Items.Add(msgStr);
            msgBox.Text = "";
            
        }

        public void CallbackShowInk(string ink)
        {
            StrokeCollectionConverter scc = new StrokeCollectionConverter();
            myCanvas.Strokes = (StrokeCollection)scc.ConvertFromString(ink);
        }

        public void CallbackStartNewTurn(Room room)
        {

            myCanvas.Strokes.Clear();
        }

        public void CallbackShowRegistStatus(int loginCode, User user)
        {
            MessageBox.Show("注册回调");
        }

        private void newRoomButton_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(user.name +" " + roomNameInput.Text);
            //选择要画的图片
            client.RegistRoom(user.name, roomNameInput.Text);
        }

        private void backToHall_Click(object sender, RoutedEventArgs e)
        {
            client.EnterHall(user.name);
            FormLogin.Visibility = Visibility.Collapsed;
            FormRegist.Visibility = Visibility.Collapsed;
            FormMain.Visibility = Visibility.Visible;
            PageHall.Visibility = Visibility.Visible;
            PagePlay.Visibility = Visibility.Collapsed;
        }

        private void send_Click(object sender, RoutedEventArgs e)
        {
            if(""==msgBox.Text)
            {
                MessageBox.Show("请输入...");
                return;
            }
            client.SendMessageAsync(room.id, user.name, msgBox.Text);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Button btnnow = sender as Button;
            btnnow.Content = "√";
            btn.Content = "";
            btn = btnnow;
            ereaser.Content = "橡皮";
            if ( "橡皮"!= btn.Content)
            {
                myCanvas.Cursor = Cursors.Pen;
            }
            else
            {
                myCanvas.Cursor = Cursors.Cross;
            }


            Color c = (Color)ColorConverter.ConvertFromString(btn.Background.ToString());
            inkDA.Color = c;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            string strSize = (string)rb.Content;
            switch (strSize)
            {
                case "大":
                    inkDA.Width = 16;
                    inkDA.Height = 16;
                    break;
                case "中":
                    inkDA.Width = 10;
                    inkDA.Height = 10;
                    break;
                case "小":
                    inkDA.Width = 4;
                    inkDA.Height = 4;
                    break;
                default:
                    break;
            }
        }
        public void CallbackEnterRoom(Room room)
        {
            PageHall.Visibility = Visibility.Collapsed;
            PagePlay.Visibility = Visibility.Visible;
        }

        private void buttonLogin_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void buttonLogin_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        private void msgBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if ("" == msgBox.Text)
                {
                    MessageBox.Show("请输入...");
                    return;
                }
                client.SendMessageAsync(room.id, user.name, msgBox.Text);
            }
        }
 
    }
}

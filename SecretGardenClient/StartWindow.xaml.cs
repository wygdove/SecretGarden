using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SecretGardenClient
{
    /// <summary>
    /// StartWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StartWindow : Window
    {
        private string[] usernames = { "张三", "李四", "王五", "赵六" };
        private string[] passwords = { "zhangsan", "lisi", "wangwu", "zhaoliu" };
        private int ind = 0;

        public StartWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (new MainWindow()).Show();
        }

        private void Button_Click_Test(object sender, RoutedEventArgs e)
        {
            (new MainWindow(usernames[ind], passwords[ind])).Show();
            ind = ((ind + 1) % usernames.Length);
        }
    }
}
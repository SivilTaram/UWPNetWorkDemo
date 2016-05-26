using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace NetWorkSocket.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ClientConnect : Page
    {
        public ClientConnect()
        {
            this.InitializeComponent();
        }

        private void SwitchButton_Clicked(object sender, RoutedEventArgs e)
        {
            //如果此时是要连接的,则Connect,并给出提示消息;否则断开连接
            if(SwitchButton.IsChecked==true)
            {
                //为连接上的按钮
                SwitchButton.FontFamily = new FontFamily("Segoe MDL2 Assets");
                SwitchButton.Content = "\xE840";
                SuggestBox.IsReadOnly = true;
            }
            else
            {
                SwitchButton.Content = "\xE718";
                SuggestBox.IsReadOnly = false;
            }
        }

        private void SuggestBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void SuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {

        }

        //当变为确定状态时,将左侧文本框锁定,并开始连接到服务器
        private void SwitchButton_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

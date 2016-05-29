using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Text;
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
        private MainPage rootPage = MainPage.Current;
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
                SeriveBox.IsReadOnly = true;
                SwitchButton_Checked(sender,e);
            }
            else
            {
                SwitchButton.Content = "\xE718";
                SeriveBox.IsReadOnly = false;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (CoreApplication.Properties.ContainsKey("connected"))
            {
                object value;
                StreamSocketListener client;
                CoreApplication.Properties.TryGetValue("listener", out value);
                client = value as StreamSocketListener;
                //将绑定的端口号填入页面
                SeriveBox.Text = client.Information.LocalPort;
                SwitchButton.Content = "\xE840";
                SwitchButton.IsChecked = true;
            }
        }

        //当变为确定状态时,将左侧文本框锁定,并开始连接到服务器
        private async void SwitchButton_Checked(object sender, RoutedEventArgs e)
        {
            //这里控制不会让SiwtchButton进行额外的Checked
            if (String.IsNullOrEmpty(SeriveBox.Text))
            {
                MessageDialog dialog = new MessageDialog("请输入端口名","提示");
                dialog.Commands.Add(new UICommand(
                    "确认",
                    cmd => { }
                    ));
                await dialog.ShowAsync();
                SwitchButton.IsChecked = false;
                return;
            }
            StreamSocket client = new StreamSocket();
            client.Control.KeepAlive = false;

            CoreApplication.Properties.Add("clientSocket",client);
            try
            {
                rootPage.NotifyUser("连接"+SeriveBox.Text+"中...",NotifyType.StatusMessage);;
                await 
                    client.ConnectAsync(
                    new HostName("localhost"),
                    SeriveBox.Text,
                    SocketProtectionLevel.PlainSocket
                    ) ;
                rootPage.NotifyUser("连接端口"+SeriveBox.Text+"成功!",NotifyType.StatusMessage);
                //如果连接的端口并没有开启服务器端口,那么会await这里一直进行阻塞,就不会将connected加入
                CoreApplication.Properties.Add("connected", null);
            }
            catch (Exception Ex)
            {
                Debug.Write(Ex.ToString());
            }
        }

        //发送消息到绑定的服务端口
        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CoreApplication.Properties.ContainsKey("connected"))
            {
                rootPage.NotifyUser("所连接的服务器端口无效,请重新进行连接!", NotifyType.ErrorMessage);
                //将SwitchButton的状态设置为unchecked.
                SwitchButton_OnUnchecked(sender, e);
                return;
            }

            object outSocket;
            //发送端
            StreamSocket sendSocket = null;
            if (!CoreApplication.Properties.TryGetValue("clientSocket", out outSocket))
            {
                rootPage.NotifyUser("没有要连接的服务器端口,请确认连接后再发送消息!", NotifyType.ErrorMessage);
                return;
            }
            else
            {
                sendSocket = outSocket as StreamSocket;
            }
            //发送端用来写入Stream
            DataWriter sendWriter;
            if (!CoreApplication.Properties.TryGetValue("clientDataWriter", out outSocket))
            {
                sendWriter = new DataWriter(sendSocket.OutputStream);
                CoreApplication.Properties.Add("clientDataWriter", sendWriter);
            }
            else
            {
                sendWriter = outSocket as DataWriter;
            }
            string sendString;
            SendBox.Document.GetText(TextGetOptions.None, out sendString);
            //如果要发送的消息不为空的话,则发送出去
            if (!String.IsNullOrEmpty(sendString))
            {
                //首先前4个字节存放了本次要发送字符串的长度
                sendWriter.WriteUInt32(sendWriter.MeasureString(sendString));
                sendWriter.WriteString(sendString);
                await sendWriter.StoreAsync();
                rootPage.NotifyUser("已经成功发送数据",NotifyType.StatusMessage);
            }
        }

        private void SwitchButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            //当选择Unchecked的状态的时候,要将之前绑定的端口解绑 --- 暂时未定动作,需要后续再考虑
        }
    }
}

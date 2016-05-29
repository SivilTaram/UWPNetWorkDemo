using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using Windows.ApplicationModel.Core;
using Windows.UI.Popups;
using NetWorkSocket.Models;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace NetWorkSocket.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ServerView : Page
    {
        MainPage rootPage = MainPage.Current;

        public ServerView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DisplayText.Text = ServerViewModel.GetTextString();
            if (CoreApplication.Properties.ContainsKey("listener"))
            {
                object value;
                StreamSocketListener listener;
                CoreApplication.Properties.TryGetValue("listener",out value);
                listener = value as StreamSocketListener;
                ServiceTextBox.Text = listener.Information.LocalPort;
                rootPage.NotifyUser("正在监听端口:" + ServiceTextBox.Text, NotifyType.StatusMessage);
            }
        }

        private async void OnConnection(
            StreamSocketListener sender,StreamSocketListenerConnectionReceivedEventArgs args)
        {
            //reader 读取args中携带的Socket信息流
            DataReader reader = new DataReader(args.Socket.InputStream);
            try
            {
                while (true)
                {
                    // Read first 4 bytes (length of the subsequent string).
                    var sizeFieldCount = await reader.LoadAsync(sizeof(uint));
                    if (sizeFieldCount != sizeof(uint))
                    {
                        // The underlying socket was closed before we were able to read the whole data.
                        return;
                    }

                    // 实际通过Socket套接字发送的字符串
                    uint stringLength = reader.ReadUInt32();
                    uint actualStringLength = await reader.LoadAsync(stringLength);
                    if (stringLength != actualStringLength)
                    {
                        //在读取之前就已经关闭了
                        return;
                    }
                    string actualAcceptString = reader.ReadString(actualStringLength);
                    ServerViewModel.AddToText(actualAcceptString);
                    //DisplayText.Text += actualAcceptString + Environment.NewLine;
                    // Display the string on the screen. The event is invoked on a non-UI thread, so we need to marshal
                    // the text back to the UI thread.
                    NotifyUserFromAsyncThread(
                        String.Format("收到数据: \"{0}\"", actualAcceptString),
                        NotifyType.StatusMessage);
                }
            }
            catch (Exception exception)
            {
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }

                NotifyUserFromAsyncThread(
                    "Read stream failed with error: " + exception.Message,
                    NotifyType.ErrorMessage);
            }
        }
        //通过获取rootPage 页面异步调用打印方法，使用Dispatcher与lamda表达式。
        private void NotifyUserFromAsyncThread(string strMessage, NotifyType type)
        {
            var ignore = Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, () => rootPage.NotifyUser(strMessage, type));
        }

        private void ConfrimReplace()
        {

        }

        private async void ListenButton_Click(object sender, RoutedEventArgs e)
        {
            if (CoreApplication.Properties.Keys.Contains("listener"))
            {
                MessageDialog dialog = new MessageDialog("已经设置过一个监听端口了!","消息提示");
                dialog.Commands.Add(
                    new UICommand(
                        "返回",
                        cmd =>
                        {
                            return;
                        }
                        ));
                await dialog.ShowAsync();
                return;
            }
            StreamSocketListener Listener = new StreamSocketListener();
            //绑定监听者的监听事件
            Listener.ConnectionReceived += OnConnection;
            //监听者暂时休眠
            Listener.Control.KeepAlive = false;
            //开始为监听者绑定端口
            CoreApplication.Properties.Add("listener", Listener);
            try
            {
                Debug.Write(ServiceTextBox.Text);
                await Listener.BindServiceNameAsync(ServiceTextBox.Text);
                //绑定完成后通知用户正在监控
                rootPage.NotifyUser("正在监听端口:" + ServiceTextBox.Text, NotifyType.StatusMessage);
                //并且此时无法再更改监听端口Text
                ServiceTextBox.IsReadOnly = true;
            }
            catch (Exception Ex)
            {
                Debug.Write(Ex.ToString());
                rootPage.NotifyUser("监听端口失败", NotifyType.ErrorMessage);
            }
        }
    }
}

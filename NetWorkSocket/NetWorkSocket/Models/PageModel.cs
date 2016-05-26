using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkSocket.Models
{
    class PageModel
    {

        private static ObservableCollection<Obserview> ViewModel;

        static PageModel()
        {
            ViewModel = new ObservableCollection<Obserview>();
            InitModel();
        }

        private static void InitModel()
        {
            ViewModel.Add(new Obserview()
            {
                PageTitle = " 服务端启动",
                ClassType = typeof(NetWorkSocket.Views.ServerView)
            });
            ViewModel.Add(new Obserview()
            {
                PageTitle = " 客户端连接"
            });
            ViewModel.Add(new Obserview()
            {
                PageTitle = " 发送消息"
            });
            ViewModel.Add(new Obserview()
            {
                PageTitle = " 服务端关闭"
            });
        }

        public static ObservableCollection<Obserview> GetViewModel()
        {
            return ViewModel;
        } 
    }
}

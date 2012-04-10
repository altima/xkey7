using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using xkey7.XKey;

namespace xkey7
{
    public partial class MainPage : PhoneApplicationPage
    {
        private const string IP = "10.1.7.83";

        private XKey.Controller _controller;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

        }

        void InitController(string ip)
        {
            if(_controller == null)
            {
                _controller = new Controller(ip);
                _controller.DataLoaded += new EventHandler<DataLoadedEventArgs>(_controller_DataLoaded);
                _controller.Error += new EventHandler<ErrorEventArgs>(_controller_Error);
                _controller.LatestVersionLoaded += new EventHandler<LatestVersionEventArgs>(_controller_LatestVersionLoaded);
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            InitController(IP);
            _controller.LoadData();
        }

        void _controller_LatestVersionLoaded(object sender, LatestVersionEventArgs e)
        {
            
        }

        void _controller_Error(object sender, ErrorEventArgs e)
        {
            
        }
        
        void _controller_DataLoaded(object sender, DataLoadedEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                icGames.ItemsSource = e.DataModel.Games[0].Isos;
                icInfo.ItemsSource = e.DataModel.About;
            });
        }

        void LoadGame(string id)
        {
            InitController(IP);
            _controller.LaunchGame(id);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var iso = button.DataContext as Iso;
            LoadGame(iso.Id);
        }
    }
}
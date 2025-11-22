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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace IslandCaller.App.Views.Windows
{
    /// <summary>
    /// FluentShower.xaml 的交互逻辑
    /// </summary>
    public partial class FluentShower : Window
    {
        public FluentShower(string name)
        {
            var control = new Controls.FluentShower.FluentShowerControl(name)
            {
                Height = 95,
                Margin = new Thickness(20, 0, 25, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            InitializeComponent();
            RootGrid.Children.Add(control);
            Loaded += (s, e) => Onloaded();
        }
        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            WindowHelper.EnableFluentWindow(this, 0x7FFF628B); // 设置模糊 + 圆角
            HwndSource source = (HwndSource)PresentationSource.FromVisual(this);
            source.CompositionTarget.BackgroundColor = Colors.Transparent;
        }

        private void Onloaded()
        {
            var storyboard = new Storyboard();
            var fadeIn = new DoubleAnimation
            {
                From = 0.3,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new ElasticEase { Oscillations = 1, Springiness = 3 }
            };
            Storyboard.SetTarget(fadeIn, this);
            Storyboard.SetTargetProperty(fadeIn, new PropertyPath(Window.OpacityProperty));

            storyboard.Children.Add(fadeIn);
            storyboard.Begin();
        }

    }
}

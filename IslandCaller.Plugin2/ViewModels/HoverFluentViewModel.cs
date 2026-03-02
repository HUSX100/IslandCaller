using ClassIsland.Shared;
using IslandCaller.Models;
using ReactiveUI;

namespace IslandCaller.ViewModels
{
    public class HoverFluentViewModel : ReactiveObject
    {
        private double _windowScalingFactor;
        public double WindowScalingFactor
        {
            get => _windowScalingFactor;
            set => this.RaiseAndSetIfChanged(ref _windowScalingFactor, value);
        }

        private bool _isenabled;
        public bool IsEnabled
        {
            get => _isenabled;
            set => this.RaiseAndSetIfChanged(ref _isenabled, value);
        }

        public string Glyph1 => IsEnabled ? "\uECF8" : "\uED08";
        public string Glyph2 => IsEnabled ? "\uED38" : "\uED08";

        private double _height;
        public double Height
        {
            get => _height;
            set => this.RaiseAndSetIfChanged(ref _height, value);
        }

        private double _width;
        public double Width
        {
            get => _width;
            set => this.RaiseAndSetIfChanged(ref _width, value);
        }

        private double _positionX;
        public double PositionX
        {
            get => _positionX;
            set
            {
                this.RaiseAndSetIfChanged(ref _positionX, value);
                Settings.Instance.Hover.Position.X = value;
            }
        }

        private double _positionY;
        public double PositionY
        {
            get => _positionY;
            set
            {
                this.RaiseAndSetIfChanged(ref _positionY, value);
                Settings.Instance.Hover.Position.Y = value;
            }
        }

        private double _button1Width;
        public double Button1Width
        {
            get => _button1Width;
            set => this.RaiseAndSetIfChanged(ref _button1Width, value);
        }
        private double _button2Width;
        public double Button2Width
        {
            get => _button2Width;
            set => this.RaiseAndSetIfChanged(ref _button2Width, value);
        }
        private double _buttonHeight;
        public double ButtonHeight
        {
            get => _buttonHeight;
            set => this.RaiseAndSetIfChanged(ref _buttonHeight, value);
        }

        public HoverFluentViewModel()
        {
            // 从设置加载初始值
            WindowScalingFactor = Settings.Instance.Hover.ScalingFactor;
            Height = 70 * WindowScalingFactor;
            Width = 163 * WindowScalingFactor;
            PositionX = Settings.Instance.Hover.Position.X;
            PositionY = Settings.Instance.Hover.Position.Y;
            Button1Width = Width * 0.46;
            Button2Width = Width * 0.34;
            ButtonHeight = Height * 0.8;

            // 监听设置变化
            Settings.Instance.Hover.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Settings.Instance.Hover.ScalingFactor))
                {
                    WindowScalingFactor = Settings.Instance.Hover.ScalingFactor;
                    Height = 70 * WindowScalingFactor;
                    Width = 163 * WindowScalingFactor;
                    Button1Width = Width * 0.46;
                    Button2Width = Width * 0.34;
                    ButtonHeight = Height * 0.8;
                }
            };
            var plugin = IAppHost.GetService<Plugin>();
            IsEnabled = plugin.PluginStatus;
            plugin.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(plugin.PluginStatus))
                {
                    IsEnabled = plugin.PluginStatus;
                }
            };
            this.WhenAnyValue(x => x.IsEnabled)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(Glyph1)));
            this.WhenAnyValue(x => x.IsEnabled)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(Glyph2)));
        }
    }
}

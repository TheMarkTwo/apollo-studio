﻿using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace Apollo.Components {
    public class ClearButton: IconButton {
        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

        Path Path;

        protected override IBrush Fill {
            get => Path.Fill;
            set => Path.Fill = value;
        }

        public ClearButton() {
            InitializeComponent();

            Path = this.Get<Path>("Path");

            base.MouseLeave(this, null);
        }
    }
}
﻿using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

using Apollo.Windows;

namespace Apollo.Components {
    public class PreferencesButton: UserControl {
        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

        public PreferencesButton() {
            InitializeComponent();
        }

        private void Clicked(object sender, PointerReleasedEventArgs e) {
            if (e.MouseButton == MouseButton.Left) PreferencesWindow.Create();
        }
    }
}
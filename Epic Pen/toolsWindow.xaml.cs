using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace click_through_wpf
{
    /// <summary>
    /// Interaction logic for toolsWindow.xaml
    /// </summary>
    public partial class ToolsWindow : Window
    {
        InkCanvas inkCanvas;
        public ToolsWindow()
        {
            InitializeComponent();
        }

        public void setInkCanvas(InkCanvas _inkCanvas)
        { inkCanvas = _inkCanvas; }




        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            selectedColourBorder.Background = ((Border)sender).Background;
            inkCanvas.DefaultDrawingAttributes.Color = ((SolidColorBrush)((Border)sender).Background).Color;
        }

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //System.Media.SystemSounds.Asterisk.Play();
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        public event EventHandler CloseButtonClick;

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            onCloseButtonClick();
        }

        void onCloseButtonClick()
        {
            if (CloseButtonClick != null)
                CloseButtonClick.Invoke(new object(), new EventArgs());
        }

        private void resetAllToolBackgrounds()
        {
            foreach (Button i in toolToolBar.Items)
                i.BorderBrush = null;
        }

        public void cursorButton_Click(object sender, RoutedEventArgs e)
        {
            resetAllToolBackgrounds();
            cursorButton.BorderBrush = Brushes.Red;
        }
        public void penButton_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.Cursor = Cursors.Pen;
            inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
            inkCanvas.DefaultDrawingAttributes.IsHighlighter = false;
            setBrushSize();
            resetAllToolBackgrounds();
            penButton.BorderBrush = Brushes.Red;
        }

        public void highlighterButton_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.Cursor = Cursors.Pen;
            inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
            inkCanvas.DefaultDrawingAttributes.IsHighlighter = true;
            setBrushSize();
            resetAllToolBackgrounds();
            highlighterButton.BorderBrush = Brushes.Red;

        }
        
        public void eraserButton_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.Cursor = Cursors.Cross;
            inkCanvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
            setBrushSize();
            resetAllToolBackgrounds();
            eraserButton.BorderBrush = Brushes.Red;     
        }
        
        public void eraseAllButton_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.Strokes.Clear();
        }
        double penSize=3;
        private void penSizeButton_MouseDown(object sender, RoutedEventArgs e)
        {
            penSize = ((Ellipse)((Button)sender).Content).Width;
            setBrushSize();

            foreach (Button i in brushSizeToolBar.Items)
                i.BorderBrush = null;
            ((Button)sender).BorderBrush = Brushes.Red;  
        }

        private void setBrushSize()
        {
            if (inkCanvas.Cursor == Cursors.Cross)
            {
                inkCanvas.DefaultDrawingAttributes.Width = penSize * 5;
                inkCanvas.DefaultDrawingAttributes.Height = penSize * 5;
            }
            else
            {
                inkCanvas.DefaultDrawingAttributes.Width = penSize;
                inkCanvas.DefaultDrawingAttributes.Height = penSize;
            }
        }

        private void clickThroughCheckBox_Checked(object sender, RoutedEventArgs e)
        {

            if ((bool)hideInkCheckBox.IsChecked)
                toolsDockPanel.Height = 0;
            else
                toolsDockPanel.Height = double.NaN;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Height = ActualHeight;
            SizeToContent = System.Windows.SizeToContent.Manual;
        }

    }
}

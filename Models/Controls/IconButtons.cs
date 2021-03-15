using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Button = System.Windows.Controls.Button;

namespace Ark.Models.Controls
{
    class IconButtons : Button
    {
        public ImageSource IconSource
        {
            get
            {
                return (ImageSource)GetValue(IconSourceProperty);
            }
            set
            {
                SetValue(IconSourceProperty, value);
            }
        }

        public static readonly DependencyProperty IconSourceProperty = DependencyProperty.Register(
            "IconSource", typeof(ImageSource), typeof(IconButtons),
            new PropertyMetadata(null));

        public ImageSource HoverIconSource
        {
            get
            {
                return (ImageSource)GetValue(HoverIconSourceProperty);
            }
            set
            {
                SetValue(HoverIconSourceProperty, value);
            }
        }

        public static readonly DependencyProperty HoverIconSourceProperty = DependencyProperty.Register(
            "HoverIconSource", typeof(ImageSource), typeof(IconButtons),
            new PropertyMetadata(null));

        public double IconWidth
        {
            get
            {
                return (double)GetValue(IconWidthProperty);
            }
            set
            {
                SetValue(IconWidthProperty, value);
            }
        }

        public static readonly DependencyProperty IconWidthProperty = DependencyProperty.Register(
            "IconWidth", typeof(double), typeof(IconButtons),
            new PropertyMetadata(0.0));

        public double IconHeight
        {
            get
            {
                return (double)GetValue(IconHeightProperty);
            }
            set
            {
                SetValue(IconHeightProperty, value);
            }
        }

        public static readonly DependencyProperty IconHeightProperty = DependencyProperty.Register(
            "IconHeight", typeof(double), typeof(IconButtons),
            new PropertyMetadata(0.0));

        public Brush HoverBackground
        {
            get
            {
                return (Brush)GetValue(HoverBackgroundProperty);
            }
            set
            {
                SetValue(HoverBackgroundProperty, value);
            }
        }

        public static readonly DependencyProperty HoverBackgroundProperty = DependencyProperty.Register(
            "HoverBackground", typeof(Brush), typeof(IconButtons),
            new PropertyMetadata(null));

        static IconButtons()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconButtons), new FrameworkPropertyMetadata(typeof(IconButtons)));
        }
    }
}
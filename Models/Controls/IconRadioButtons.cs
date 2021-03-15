using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ark.Models.Controls
{
    class IconRadioButtons : RadioButton
    {
        public ImageSource IconSource { get; set; }

        public static readonly DependencyProperty IconSourceProperty = DependencyProperty.Register(
            "IconSource", typeof(ImageSource), typeof(IconRadioButtons), new PropertyMetadata(null));

        public ImageSource HoverIconSource { get; set; }

        public static readonly DependencyProperty HoverIconSourceProperty = DependencyProperty.Register(
            "HoverIconSource", typeof(ImageSource), typeof(IconRadioButtons), new PropertyMetadata(null));

        static IconRadioButtons()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconRadioButtons), new FrameworkPropertyMetadata(typeof(IconRadioButtons)));
        }
    }
}
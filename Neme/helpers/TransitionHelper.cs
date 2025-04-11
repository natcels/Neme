using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows;
using System;
using System.Windows.Controls;

namespace Neme.helpers
{
    public static class TransitionHelper
    {
        public static void SwapWithFade(this Panel container, UIElement newContent, double durationSeconds = 0.3)
        {
            if (container.Children.Count > 0)
            {
                UIElement current = container.Children[0];

                var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(durationSeconds));
                fadeOut.Completed += (s, e) =>
                {
                    container.Children.Clear();

                    newContent.Opacity = 0;
                    container.Children.Add(newContent);

                    var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(durationSeconds));
                    newContent.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                };

                current.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            }
            else
            {
                newContent.Opacity = 0;
                container.Children.Add(newContent);

                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(durationSeconds));
                newContent.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            }
        }
    }
}


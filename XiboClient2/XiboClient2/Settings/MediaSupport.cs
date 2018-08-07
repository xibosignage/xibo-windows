using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using CefSharp.Wpf;

namespace XiboClient2.Settings
{
    public class MediaSupport
    {
        public static DependencyProperty OpacityProperty { get; private set; }

        /// <summary>
        /// Pulls the duration out of the temporary file and sets the media Duration to the same
        /// </summary>
        public static double ReadControlMetaDuration(string _filePath)
        {
            double durationMeta = 0;
            try
            {
                // read the contents of the file
                using (StreamReader reader = new StreamReader(_filePath))
                {
                    string html = reader.ReadToEnd();

                    // Parse out the duration using a regular expression
                    try
                    {
                        Match match = Regex.Match(html, "<!-- DURATION=(.*?) -->");

                        if (match.Success)
                        {
                            // We have a match, so override our duration.
                            durationMeta = Convert.ToInt32(match.Groups[1].Value);
                        }
                    }
                    catch
                    {
                        durationMeta = 0;
                    }
                }
            }
            catch
            {
                durationMeta = 0;
            }
            return durationMeta;
        }

        /// <summary>
        /// Select Animation type , pass animation details 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="dp"></param>
        /// <param name="type"></param>
        /// <param name="direction"></param>
        /// <param name="duration"></param>
        /// <param name="inOut"></param>
        /// <param name="top"></param>
        /// <param name="left"></param>
        public static void MoveAnimation(object item, DependencyProperty dp, string type, string direction, double duration, string inOut, int top, int left)
        {
            switch (type)
            {
                case "fly":
                    FlyAnimation(item, direction, duration, inOut, top, left);
                    break;
                case "fadeIn":
                    FadeIn(item, dp, duration);
                    break;
                case "fadeOut":
                    FadeOut(item, dp, duration);
                    break;
            }
        }

        /// <summary>
        /// Fade in animation
        /// </summary>
        /// <param name="item"></param>
        /// <param name="dp"></param>
        /// <param name="duration"></param>
        private static void FadeIn(object item, DependencyProperty dp, double duration)
        {
            DoubleAnimation doubleAnimationFade = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(duration)
            };
            
            if (item is System.Windows.Controls.Image)
            {
                (item as System.Windows.Controls.Image).BeginAnimation(dp, doubleAnimationFade);
            }
            else if (item is MediaElement)
            {
                (item as MediaElement).BeginAnimation(dp, doubleAnimationFade);
            }
            else if (item is ChromiumWebBrowser)
            {
                (item as ChromiumWebBrowser).BeginAnimation(dp, doubleAnimationFade);
            }
        }

        /// <summary>
        /// FadeOut animation
        /// </summary>
        /// <param name="item"></param>
        /// <param name="direction"></param>
        /// <param name="duration"></param>
        private static void FadeOut(object item, DependencyProperty dp, double duration)
        {
            DoubleAnimation doubleAnimationFade = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(duration)
            };

            if (item is System.Windows.Controls.Image)
            {
                (item as System.Windows.Controls.Image).BeginAnimation(dp, doubleAnimationFade);
            }
            else if (item is MediaElement)
            {
                (item as MediaElement).BeginAnimation(dp, doubleAnimationFade);
            }
            else if (item is ChromiumWebBrowser)
            {
                (item as ChromiumWebBrowser).BeginAnimation(dp, doubleAnimationFade);
            }
        }

        /// <summary>
        /// item moving animation with all directions
        /// </summary>
        /// <param name="item"></param>
        /// <param name="direction"></param>
        /// <param name="duration"></param>
        /// <param name="inOut"></param>
        private static void FlyAnimation(object item, string direction, double duration, string inOut, int top, int left)
        {
            int inValueY = 0;
            int inValueX = 0;

            int endValueX = 0;
            int endValueY = 0;

            int screenHeight = (int)SystemParameters.PrimaryScreenHeight;
            int screenWight = Convert.ToInt32(SystemParameters.PrimaryScreenWidth);

            if (inOut == "in")
            {
                inValueY = screenHeight;
                inValueX = screenWight;
                endValueX = 0;
                endValueY = 0;
            }
            else if (inOut == "out")
            {
                inValueY = 0;
                inValueX = 0;
                endValueX = screenWight;
                endValueY = screenHeight;
            }

            DoubleAnimation doubleAnimationX = new DoubleAnimation();
            DoubleAnimation doubleAnimationY = new DoubleAnimation();
            doubleAnimationX.To = endValueX;
            doubleAnimationY.To = endValueY;
            doubleAnimationX.Duration = TimeSpan.FromMilliseconds(duration);
            doubleAnimationY.Duration = TimeSpan.FromMilliseconds(duration);
            var trans = new TranslateTransform();
            switch (direction)
            {
                case "N":
                    if (inOut == "in")
                    {
                        doubleAnimationY.From = (screenHeight - top);
                    }
                    else
                    {
                        doubleAnimationY.From = top;
                    }

                    trans.BeginAnimation(TranslateTransform.YProperty, doubleAnimationY);
                    break;
                case "NE":
                    if (inOut == "in")
                    {
                        doubleAnimationX.From = (screenWight - left);
                        doubleAnimationY.From = (screenHeight - top);
                    }
                    else
                    {
                        doubleAnimationX.From = left;
                        doubleAnimationY.From = top;
                    }

                    trans.BeginAnimation(TranslateTransform.YProperty, doubleAnimationY);
                    trans.BeginAnimation(TranslateTransform.XProperty, doubleAnimationX);
                    break;
                case "E":
                    if (inOut == "in")
                    {
                        doubleAnimationX.From = -(screenWight - left);
                    }
                    else
                    {
                        if (left == 0)
                        {
                            doubleAnimationX.From = -left;
                        }
                        else
                        {
                            doubleAnimationX.From = -(screenWight - left);
                        }

                    }

                    trans.BeginAnimation(TranslateTransform.XProperty, doubleAnimationX);
                    break;
                case "SE":
                    if (inOut == "in")
                    {
                        doubleAnimationX.From = left;
                        doubleAnimationY.From = -(screenHeight - top);
                    }
                    else
                    {
                        doubleAnimationX.From = (screenWight - left);
                        doubleAnimationY.From = -(screenHeight - top);
                    }
                    trans.BeginAnimation(TranslateTransform.YProperty, doubleAnimationY);
                    trans.BeginAnimation(TranslateTransform.XProperty, doubleAnimationX);
                    break;
                case "S":
                    if (inOut == "in")
                    {
                        doubleAnimationX.From = -(screenHeight - top);
                    }
                    else
                    {
                        doubleAnimationX.From = -top;
                    }

                    trans.BeginAnimation(TranslateTransform.YProperty, doubleAnimationX);
                    break;
                case "SW":
                    if (inOut == "in")
                    {
                        doubleAnimationX.From = (screenWight - left);
                        doubleAnimationY.From = -top;
                    }
                    else
                    {
                        doubleAnimationX.From = left;
                        doubleAnimationY.From = -(screenHeight - left);
                    }

                    trans.BeginAnimation(TranslateTransform.XProperty, doubleAnimationX);
                    trans.BeginAnimation(TranslateTransform.YProperty, doubleAnimationY);
                    break;
                case "W":
                    if (inOut == "in")
                    {
                        doubleAnimationX.From = (screenWight - left);
                    }
                    else
                    {
                        doubleAnimationX.From = -left;
                    }

                    trans.BeginAnimation(TranslateTransform.XProperty, doubleAnimationX);

                    break;
                case "NW":
                    if (inOut == "in")
                    {
                        doubleAnimationX.From = (screenWight - left);
                        doubleAnimationY.From = (screenHeight - top);
                    }
                    else
                    {
                        doubleAnimationX.From = left;
                        doubleAnimationY.From = top;
                    }

                    trans.BeginAnimation(TranslateTransform.XProperty, doubleAnimationX);
                    trans.BeginAnimation(TranslateTransform.YProperty, doubleAnimationY);

                    break;
            }

            if (item is System.Windows.Controls.Image)
            {
                (item as System.Windows.Controls.Image).RenderTransform = trans;
            }
            else if (item is MediaElement)
            {
                (item as MediaElement).RenderTransform = trans;
            }
            else if (item is ChromiumWebBrowser)
            {
                (item as ChromiumWebBrowser).RenderTransform = trans;
            }
        }
    }
}

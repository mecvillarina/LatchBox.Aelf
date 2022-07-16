﻿using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Infrastructure.Settings
{
    public class AppTheme
    {
        private static Typography DefaultTypography = new Typography()
        {
            Default = new Default()
            {
                FontFamily = new[] { "Poppins", "Helvetica", "Arial", "sans-serif" },
                FontSize = ".875rem",
                FontWeight = 400,
                LineHeight = 1.43,
                LetterSpacing = ".01071em"
            },
            H1 = new H1()
            {
                FontFamily = new[] { "Poppins", "Helvetica", "Arial", "sans-serif" },
                FontSize = "6rem",
                FontWeight = 300,
                LineHeight = 1.167,
                LetterSpacing = "-.01562em"
            },
            H2 = new H2()
            {
                FontFamily = new[] { "Poppins", "Helvetica", "Arial", "sans-serif" },
                FontSize = "3.75rem",
                FontWeight = 300,
                LineHeight = 1.2,
                LetterSpacing = "-.00833em"
            },
            H3 = new H3()
            {
                FontFamily = new[] { "Poppins", "Helvetica", "Arial", "sans-serif" },
                FontSize = "3rem",
                FontWeight = 400,
                LineHeight = 1.167,
                LetterSpacing = "0"
            },
            H4 = new H4()
            {
                FontFamily = new[] { "Poppins", "Helvetica", "Arial", "sans-serif" },
                FontSize = "2.125rem",
                FontWeight = 400,
                LineHeight = 1.235,
                LetterSpacing = ".00735em"
            },
            H5 = new H5()
            {
                FontFamily = new[] { "Poppins", "Helvetica", "Arial", "sans-serif" },
                FontSize = "1.5rem",
                FontWeight = 400,
                LineHeight = 1.334,
                LetterSpacing = "0"
            },
            H6 = new H6()
            {
                FontFamily = new[] { "Poppins", "Helvetica", "Arial", "sans-serif" },
                FontSize = "1.25rem",
                FontWeight = 400,
                LineHeight = 1.6,
                LetterSpacing = ".0075em"
            },
            Button = new Button()
            {
                FontFamily = new[] { "Poppins", "Helvetica", "Arial", "sans-serif" },
                FontSize = ".875rem",
                FontWeight = 500,
                LineHeight = 1.75,
                LetterSpacing = ".02857em"
            },
            Body1 = new Body1()
            {
                FontFamily = new[] { "Poppins", "Helvetica", "Arial", "sans-serif" },
                FontSize = "1rem",
                FontWeight = 400,
                LineHeight = 1.5,
                LetterSpacing = ".00938em"
            },
            Body2 = new Body2()
            {
                FontFamily = new[] { "Poppins", "Helvetica", "Arial", "sans-serif" },
                FontSize = ".875rem",
                FontWeight = 400,
                LineHeight = 1.43,
                LetterSpacing = ".01071em"
            },
            Caption = new Caption()
            {
                FontFamily = new[] { "Poppins", "Helvetica", "Arial", "sans-serif" },
                FontSize = ".75rem",
                FontWeight = 400,
                LineHeight = 1.66,
                LetterSpacing = ".03333em"
            },
            Subtitle2 = new Subtitle2()
            {
                FontFamily = new[] { "Poppins", "Helvetica", "Arial", "sans-serif" },
                FontSize = ".875rem",
                FontWeight = 500,
                LineHeight = 1.57,
                LetterSpacing = ".00714em"
            }
        };

        private static LayoutProperties DefaultLayoutProperties = new LayoutProperties()
        {
            DefaultBorderRadius = "4px"
        };

        public static MudTheme GetDefaultTheme()
        {
            var theme = new MudTheme()
            {
                Typography = DefaultTypography,
                LayoutProperties = DefaultLayoutProperties
            };
            //theme.PaletteDark.Black = "#3D4853";
            //theme.PaletteDark.Primary = "#90CB9D";
            //theme.PaletteDark.Secondary = "#C4A311";
            //theme.PaletteDark.Background = "#3D4853";
            //theme.PaletteDark.Surface = "#3D4853";
            //theme.PaletteDark.DrawerBackground = "#3D4853";
            //theme.PaletteDark.AppbarBackground = "#3D4853";
            //theme.PaletteDark.Error = "#E97A7A";

            theme.PaletteDark.Black = "#1d1d1d";
            theme.PaletteDark.White = "#ece0c6";
            theme.PaletteDark.Primary = "#f4583d";
            theme.PaletteDark.TextPrimary = "#ece0c6";
            theme.PaletteDark.Secondary = "#f6d38d";
            //theme.PaletteDark.TextSecondary = "#ece0c6";
            theme.PaletteDark.Background = "#1d1d1d";
            theme.PaletteDark.Surface = "#282828";
            theme.PaletteDark.DarkContrastText= "#ece0c6";
            theme.PaletteDark.DrawerBackground = "#1d1d1d";
            theme.PaletteDark.AppbarBackground = "#1d1d1d";
            theme.PaletteDark.AppbarText = "#ece0c6";
            theme.PaletteDark.Error = "#E97A7A";
            return theme;
        }
    }
}

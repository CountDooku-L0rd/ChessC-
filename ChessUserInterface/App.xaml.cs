﻿using System.Windows;

namespace ChessUserInterface
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DatabaseHelper.InitializeDatabase();
        }

    }

}

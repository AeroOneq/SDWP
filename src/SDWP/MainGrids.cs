﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Data.SqlTypes;
using System.Net.Mail;

namespace SDWP
{
    public class MainGrids
    {
        public Grid TopOptionsGrid { get; set; }
        public Grid MainGrid { get; set; } 
        public Grid UserAccountGrid { get; set; }
    }
}

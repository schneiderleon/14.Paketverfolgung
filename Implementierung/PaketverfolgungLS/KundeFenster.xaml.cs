using Microsoft.Data.SqlClient;
using System;
using System.Windows;

namespace Paketverfolgung;

public partial class KundeFenster : Window
{
    public KundeFenster()
    {
        InitializeComponent();
        TbName.Focus();
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        return; 
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        return;
    }
}

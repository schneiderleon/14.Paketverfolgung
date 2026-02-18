using Microsoft.Data.SqlClient;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Paketverfolgung;

public partial class BestellungBearbeitenFenster : Window
{
    private readonly int _orderId;
    private int _customerId;

    public BestellungBearbeitenFenster(int orderId)
    {
        InitializeComponent();
        _orderId = orderId;
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        return;
    }

    private void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        return;
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        return;
    }
}

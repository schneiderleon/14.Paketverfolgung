using Microsoft.Data.SqlClient;
using System;
using System.Windows;

namespace Paketverfolgung;

public partial class KundeBearbeitenFenster : Window
{
    private readonly int _kundeId;

    public KundeBearbeitenFenster(int kundeId)
    {
        InitializeComponent();
        _kundeId = kundeId;
        Loaded += Window_Loaded;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        LoadCustomer();
        LoadOrders();
    }

    private void LoadCustomer()
    {
        return;
    }

    private void LoadOrders()
    {
        return;
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

using Microsoft.Data.SqlClient;
using System;
using System.Windows;

namespace Paketverfolgung;

public partial class PaketFenster : Window
{
    public PaketFenster()
    {
        InitializeComponent();
        DpOrderDate.SelectedDate = DateTime.Today;
        CbStatus.SelectedIndex = 0;
    }

    private void BtnSearch_Click(object sender, RoutedEventArgs e)
    {
        return;
    }

    private void BtnReset_Click(object sender, RoutedEventArgs e)
    {
        return;
    }

    private void BtnNewOrder_Click(object sender, RoutedEventArgs e)
    {
        return;
    }

    private void BtnNewCustomer_Click(object sender, RoutedEventArgs e)
    {
        return;
    }

    private void BtnEditOrder_Click(object sender, RoutedEventArgs e)
    {
        return;
    }

    private void DgOrders_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        return;
    }

    private void BtnSaveOrder_Click(object sender, RoutedEventArgs e)
    {
        return;
    }

    private void BtnCancelOrder_Click(object sender, RoutedEventArgs e)
    {
        return;
    }

    private void BtnEditCustomer_Click(object sender, RoutedEventArgs e)
    {
        return;
    }

    private void DgCustomers_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        return;
    }

}

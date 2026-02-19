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

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            var customers = Database.GetCustomerLookup();
            CbCustomer.ItemsSource = customers;

            var order = Database.GetOrderById(_orderId);
            if (order is null)
            {
                MessageBox.Show("Diese Bestellung wurde nicht gefunden.", "Info");
                DialogResult = false;
                Close();
                return;
            }

            _customerId = order.CustomerId;

            TbOrderId.Text = order.Id.ToString();
            TbProduct.Text = order.Product;
            DpOrderDate.SelectedDate = order.OrderDate;

            var selected = customers.FirstOrDefault(c => c.Id == order.CustomerId);
            if (selected != null)
                CbCustomer.SelectedItem = selected;

            SelectStatus(order.Status);
        }
        catch (SqlException ex)
        {
            MessageBox.Show(ex.Message, "DB Fehler");
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Fehler");
            Close();
        }
    }

    private void SelectStatus(string status)
    {
        var s = (status ?? "").Trim();

        foreach (var item in CbStatus.Items)
        {
            if (item is ComboBoxItem cbi)
            {
                var text = (cbi.Content?.ToString() ?? "").Trim();
                if (string.Equals(text, s, StringComparison.OrdinalIgnoreCase))
                {
                    CbStatus.SelectedItem = cbi;
                    return;
                }
            }
        }

        CbStatus.SelectedIndex = 0;
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var product = (TbProduct.Text ?? "").Trim();
            if (product.Length == 0)
            {
                MessageBox.Show("Produktname ist Pflicht.", "Fehler");
                return;
            }

            var date = DpOrderDate.SelectedDate ?? DateTime.Today;

            var status = (CbStatus.Text ?? "").Trim();
            if (status.Length == 0)
            {
                MessageBox.Show("Bitte Status wählen.", "Fehler");
                return;
            }

            var rows = Database.UpdateOrder(_orderId, _customerId, date, status, product);
            if (rows == 0)
            {
                MessageBox.Show("Update hat nichts geändert (Bestellung nicht gefunden).", "Info");
                return;
            }

            DialogResult = true;
            Close();
        }
        catch (SqlException ex)
        {
            MessageBox.Show(ex.Message, "DB Fehler");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Fehler");
        }
    }

    private void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            "Willst du diese Bestellung wirklich löschen?",
            "Bestellung löschen",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes)
            return;

        try
        {
            var rows = Database.DeleteOrder(_orderId);
            if (rows == 0)
            {
                MessageBox.Show("Löschen hat nichts geändert (Bestellung nicht gefunden).", "Info");
                return;
            }

            DialogResult = true;
            Close();
        }
        catch (SqlException ex)
        {
            MessageBox.Show(ex.Message, "DB Fehler");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Fehler");
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}

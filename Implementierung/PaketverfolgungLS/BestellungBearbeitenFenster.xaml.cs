using System;
using System.Linq;
using System.Windows;
using Paketverfolgung.Models;

namespace Paketverfolgung;

public partial class BestellungBearbeitenFenster : Window
{
    private readonly int _orderId;
    private int _currentCustomerId;
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
            
            
            _currentCustomerId = order.CustomerId;

            
            TbOrderId.Text = order.Id.ToString();
            TbProduct.Text = order.Product;
            DpOrderDate.SelectedDate = order.OrderDate;

            
            var selected = customers.FirstOrDefault(c => c.Id == order.CustomerId);
            if (selected != null)
                CbCustomer.SelectedItem = selected;

            SelectStatus(order.Status);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Fehler beim Laden der Daten: " + ex.Message, "Fehler");
            Close();
        }
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var product = (TbProduct.Text ?? "").Trim();
            if (product.Length == 0)
            {
                MessageBox.Show("Produktname darf nicht leer sein.", "Eingabefehler");
                return;
            }

            var date = DpOrderDate.SelectedDate ?? DateTime.Today;
            var status = (CbStatus.SelectionBoxItem as string) ?? "in Bearbeitung";

            
            var rows = Database.UpdateOrder(_orderId, _currentCustomerId, date, status, product);

            if (rows == 0)
            {
                MessageBox.Show("Die Bestellung konnte nicht aktualisiert werden.", "Info");
                return;
            }

            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Fehler beim Speichern: " + ex.Message, "Fehler");
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
                MessageBox.Show("Bestellung wurde bereits gelöscht oder nicht gefunden.", "Info");
                return;
            }

            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            ShowErrorMessage("Fehler beim Speichern", ex);
        }
    }
    private void ShowErrorMessage(string title, Exception ex)
    {
        
        string msg = ex.Message;
        if (ex.InnerException != null)
            msg += "\n\nDetails: " + ex.InnerException.Message;

        MessageBox.Show(msg, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private void SelectStatus(string status)
    {
        foreach (System.Windows.Controls.ComboBoxItem item in CbStatus.Items)
        {
            if (item.Content.ToString() == status)
            {
                CbStatus.SelectedItem = item;
                break;
            }
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}

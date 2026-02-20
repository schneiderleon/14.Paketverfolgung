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

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        TryLoadAll();
    }

    private void TryLoadAll()
    {
        try
        {
            DgOrders.ItemsSource = Database.GetOrders();
            DgCustomers.ItemsSource = Database.GetCustomers();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Datenbankverbindung fehlgeschlagen.\n\n" +               
                ex.Message,
                "DB Fehler");
        }
       
    }

    private void BtnSearch_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            DgOrders.ItemsSource = Database.GetOrders(TbSearch.Text);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Fehler");
        }
    }

    private void BtnReset_Click(object sender, RoutedEventArgs e)
    {
        TbSearch.Text = "";
        TryLoadAll();
    }

    private void BtnNewOrder_Click(object sender, RoutedEventArgs e)
    {
        TbCustomerId.Focus();
    }

    private void BtnNewCustomer_Click(object sender, RoutedEventArgs e)
    {
        var win = new KundeFenster();
        win.Owner = this;
        win.ShowDialog();
        TryLoadAll();
    }

    private void BtnEditOrder_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse((TbEditOrderId.Text ?? "").Trim(), out var orderId))
        {
            MessageBox.Show("Bitte eine gültige Bestell ID eingeben (Zahl).", "Fehler");
            return;
        }

        OpenEditOrder(orderId);
    }

    private void DgOrders_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (DgOrders.SelectedItem is OrderRow row)
        {
            OpenEditOrder(row.Id);
        }
    }

    private void OpenEditOrder(int orderId)
    {
        var win = new BestellungBearbeitenFenster(orderId);
        win.Owner = this;

        var ok = win.ShowDialog();
        if (ok == true)
            TryLoadAll();
    }

    private void BtnSaveOrder_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!int.TryParse((TbCustomerId.Text ?? "").Trim(), out var kundeId))
            {
                MessageBox.Show("Kunden ID muss eine Zahl sein.", "Fehler");
                return;
            }

            var produkt = (TbProductName.Text ?? "").Trim();
            var status = (CbStatus.Text ?? "").Trim();
            var date = DpOrderDate.SelectedDate ?? DateTime.Today;

            if (produkt.Length == 0)
            {
                MessageBox.Show("Bitte Produktname eingeben.", "Fehler");
                return;
            }

            if (status.Length == 0)
            {
                MessageBox.Show("Bitte Status wählen.", "Fehler");
                return;
            }

            Database.InsertOrder(kundeId, date, status, produkt);

            TbCustomerId.Text = "";
            TbProductName.Text = "";
            DpOrderDate.SelectedDate = DateTime.Today;
            CbStatus.SelectedIndex = 0;

            TryLoadAll();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "DB Fehler");
        }      
    }

    private void BtnCancelOrder_Click(object sender, RoutedEventArgs e)
    {
        TbCustomerId.Text = "";
        TbProductName.Text = "";
        DpOrderDate.SelectedDate = DateTime.Today;
        CbStatus.SelectedIndex = 0;
    }

    private void BtnEditCustomer_Click(object sender, RoutedEventArgs e)
    {
    if (!int.TryParse((TbEditCustomerId.Text ?? "").Trim(), out var kundeId))
    {
        MessageBox.Show("Bitte eine gültige Kunden ID eingeben (Zahl).", "Fehler");
        return;
    }

    var win = new KundeBearbeitenFenster(kundeId);
    win.Owner = this;

    var ok = win.ShowDialog();
    if (ok == true)
        TryLoadAll();
    }


    private void DgCustomers_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
    if (DgCustomers.SelectedItem is CustomerRow row)
    {
        var win = new KundeBearbeitenFenster(row.Id);
        win.Owner = this;
        var ok = win.ShowDialog();
        if (ok == true)
            TryLoadAll();
    }
    }

}

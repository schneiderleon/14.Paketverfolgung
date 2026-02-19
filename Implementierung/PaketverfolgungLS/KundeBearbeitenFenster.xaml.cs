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
        try
        {
            var c = Database.GetCustomerById(_kundeId);
            if (c is null)
            {
                MessageBox.Show("Dieser Kunde wurde nicht gefunden.", "Info");
                DialogResult = false;
                Close();
                return;
            }

            TbCustomerId.Text = c.Id.ToString();
            TbName.Text = c.Name;
            TbAddress.Text = c.Address;
            TbEmail.Text = c.Email;
            TbPhone.Text = c.Phone;

            TbName.Focus();
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

    private void LoadOrders()
    {
        try
        {
            DgCustomerOrders.ItemsSource = Database.GetOrdersByCustomerId(_kundeId);
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

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var name = (TbName.Text ?? "").Trim();
            if (name.Length == 0)
            {
                MessageBox.Show("Name ist Pflicht.", "Fehler");
                return;
            }

            var addr = (TbAddress.Text ?? "").Trim();
            var email = (TbEmail.Text ?? "").Trim();
            var phone = (TbPhone.Text ?? "").Trim();

            var rows = Database.UpdateCustomer(
                _kundeId,
                name,
                addr.Length == 0 ? null : addr,
                email.Length == 0 ? null : email,
                phone.Length == 0 ? null : phone);

            if (rows == 0)
            {
                MessageBox.Show("Update hat nichts geändert (Kunde nicht gefunden).", "Info");
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
        // Option anbieten: Bestellungen vorher löschen?
        var result = MessageBox.Show(
            "Kunde löschen:\n\n" +
            "Möchtest du vorher ALLE Bestellungen dieses Kunden löschen?\n\n" +
            "Ja  = Bestellungen löschen + Kunde löschen\n" +
            "Nein = Nur Kunde löschen\n" +
            "Abbrechen = nichts tun",
            "Kunde löschen",
            MessageBoxButton.YesNoCancel,
            MessageBoxImage.Warning);

        if (result == MessageBoxResult.Cancel)
            return;

        try
        {
            if (result == MessageBoxResult.Yes)
            {
                // Erst Bestellungen löschen
                Database.DeleteOrdersByCustomerId(_kundeId);
            }

            // Dann Kunde löschen
            var rows = Database.DeleteCustomer(_kundeId);
            if (rows == 0)
            {
                MessageBox.Show("Löschen hat nichts geändert (Kunde nicht gefunden).", "Info");
                return;
            }

            DialogResult = true;
            Close();
        }
        catch (SqlException ex)
        {
            MessageBox.Show(
                "Kunde konnte nicht gelöscht werden.\n\n" +
                "Wenn du 'Nein' gewählt hast, kann es sein, dass noch Bestellungen existieren.\n" +
                "Versuche es nochmal und wähle 'Ja' (Bestellungen vorher löschen).\n\n" +
                ex.Message,
                "DB Fehler");
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

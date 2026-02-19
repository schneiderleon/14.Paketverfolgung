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
        try
        {
            var name = (TbName.Text ?? "").Trim();
            if (name.Length == 0)
            {
                MessageBox.Show("Name ist Pflicht.", "Fehler");
                return;
            }

            var addr = (TbAdresse.Text ?? "").Trim();
            var email = (TbEmail.Text ?? "").Trim();
            var tel = (TbTelefon.Text ?? "").Trim();

            Database.InsertCustomer(
                name,
                addr.Length == 0 ? null : addr,
                email.Length == 0 ? null : email,
                tel.Length == 0 ? null : tel);

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

using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace deceptive_enigma;

public partial class MainWindow : Window
{

    public ConfigProvider config = new();

    public Mapper mapper;
    public MainWindow()
    {
        mapper =  new(config);
        Console.WriteLine("mainwindow constructor");
        InitializeComponent();
    }

    public void EncryptClickHandler(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("encrypt click");
        ErrorOnEmpty(new TextBox[]{message,password});
        string msg = message.Text ?? "";
        string pswrd = password.Text ?? "";
        string encrypted = mapper.Encrypt(msg,pswrd);
        encryptedMessage.Text = encrypted;
    }

    public void DecryptClickHandler(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("decrypt click");
        ErrorOnEmpty(new TextBox[]{encryptedMessage});
        string encrptd = encryptedMessage.Text ?? "";
        string pswrd = password.Text ?? "";
        string msg = mapper.Decrypt(encrptd,pswrd);
        message.Text = msg; 
    }

    private static void ErrorOnEmpty(TextBox[] listoftextboxes) //move validation to another class 
    {
        
        foreach (TextBox textbox in listoftextboxes)
        {
            if (string.IsNullOrEmpty(textbox.Text)) 
                throw new ArgumentException($"{textbox.Name} cannot be left empty.");
        
        }
    }
}



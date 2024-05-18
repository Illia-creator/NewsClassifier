using System;
using System.Data.SqlClient;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Text;
using NewsClassifier.TextHandler;
using Microsoft.ML.Trainers;
using Telegram.Bot;
using Microsoft.Extensions.Configuration;
using NewsClassifier.TelegramBot;
using System.ComponentModel;
using Microsoft.ML.TorchSharp.NasBert;
using Microsoft.ML.TorchSharp;

public partial class Program
{
    static void Main(string[] args)
    {
        Configuration telegramConfiguration = new Configuration();

        telegramConfiguration.StartConnection();
        Console.ReadLine();
    }
   

 

};


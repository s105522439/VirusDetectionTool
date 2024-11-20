using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VirusDetectionTool.Database;
using VirusDetectionTool.Database.DbServices;
using VirusDetectionTool.MVVM.Models;

namespace VirusDetectionTool.MVVM.ViewModels
{
    public class MainViewModel: ObservableObject
    {
        private string selectFolderPath; 
        private string quarantineFolderPath;
        private Dictionary<string, string> VirusFiles;
        private string consoleOutput;
        private string imageAddress; 
        public string ImageAddress { get { return imageAddress; }set { imageAddress = value; OnPropertyChanged(); } }
        public string ConsoleOutput { get { return consoleOutput; } set { consoleOutput = value; OnPropertyChanged(); } }
        public string SelectFolderPath {  get { return selectFolderPath; } set { selectFolderPath = value; OnPropertyChanged(); CheckVirusCommand.NotifyCanExecuteChanged(); } }    
        public string QuarantineFolderPath { get {return quarantineFolderPath; }set { quarantineFolderPath = value; OnPropertyChanged(); QuarantineVirusCommand.NotifyCanExecuteChanged();ClearVirusCommand.NotifyCanExecuteChanged(); } }
        public RelayCommand SelectFolderCommand { get; set; }
        public RelayCommand QuarantineFolderCommand { get; set; }
        public RelayCommand CheckVirusCommand { get; set; }
        public RelayCommand QuarantineVirusCommand { get; set; }
        public RelayCommand ClearVirusCommand { get;set; }
        public MainViewModel() 
        {
            SelectFolderCommand = new RelayCommand(SelectFolder);
            QuarantineFolderCommand=new RelayCommand(QuarantineFolder);
            CheckVirusCommand = new RelayCommand(CheckVirus, () => !string.IsNullOrWhiteSpace(SelectFolderPath));
            QuarantineVirusCommand = new RelayCommand(QuarantineVirus, () => !string.IsNullOrWhiteSpace(QuarantineFolderPath));
            ClearVirusCommand=new RelayCommand(ClearVirus, () => !string.IsNullOrWhiteSpace(QuarantineFolderPath));
            ConsoleOutput=string.Empty;
            ImageAddress = "C:\\Users\\1\\source\\repos\\VirusDetectionTool\\VirusDetectionTool\\Images\\VirusScan.png";
        }
        public void SelectFolder()
        {
            var folderDialog = new OpenFolderDialog
            {
                InitialDirectory = "C:\\"
            };

            if (folderDialog.ShowDialog() == true)
            {
                SelectFolderPath = folderDialog.FolderName;
            }
        }
        public void QuarantineFolder()
        {
            var folderDialog = new OpenFolderDialog
            {
                InitialDirectory = "C:\\"
            };

            if (folderDialog.ShowDialog() == true)
            {
                QuarantineFolderPath = folderDialog.FolderName;
            }
        }
        public async  void  CheckVirus() 
        {
            VirusFiles = new Dictionary<string, string>();
            if (Directory.Exists(selectFolderPath)) 
            {
                string[] files = Directory.GetFiles(selectFolderPath, "*.*", SearchOption.AllDirectories);
                if (files.Length == 0)
                {
                    ConsoleOutput += $"The folder to be checked for viruses is empty and there are no files to scan.\n";
                    return;
                }
                else
                {
                    await Task.Run(async () =>
                    {
                        foreach (string file in files)
                        {
                            string fileHash;
                            var md5 = MD5.Create();
                            using (var stream = File.OpenRead(file))
                            {
                                byte[] hash = md5.ComputeHash(stream);
                                fileHash = BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant();
                            }
                            VirusSignaturesDataService virusSignaturesDataService = new VirusSignaturesDataService();
                            VirusSignature virusSignature = await virusSignaturesDataService.SearchHashValuesAsync(fileHash);
                            if (virusSignature != null)
                            {
                                VirusFiles.Add(file, virusSignature.VirusName);
                                ConsoleOutput += $"Virus files found: {file} Virus type: {virusSignature.VirusName}.\n";
                            }
                        }
                    });
                    if(!VirusFiles.Any())
                    {
                        ConsoleOutput += "No virus files were found.\n";
                        ImageAddress = "C:\\Users\\1\\source\\repos\\VirusDetectionTool\\VirusDetectionTool\\Images\\Safe.png";
                    }
                    else
                    {
                        ImageAddress = "C:\\Users\\1\\source\\repos\\VirusDetectionTool\\VirusDetectionTool\\Images\\UnSafe.png";
                    }
                }
            } 
            else 
            { 
                ConsoleOutput += "The folder you selected to check for viruses does not exist!\n";
                ImageAddress = "C:\\Users\\1\\source\\repos\\VirusDetectionTool\\VirusDetectionTool\\Images\\VirusScan.png";
            }
        }
        public async void QuarantineVirus()
        {
            await Task.Run(() => 
            {
                if (!Directory.Exists(QuarantineFolderPath))
                {
                    ConsoleOutput += "The folder selected as quarantine does not exist!\n";
                    return;
                }
                if (VirusFiles != null && VirusFiles.Count > 0)
                {
                    foreach (var virusFile in VirusFiles)
                    {
                        string sourceFilePath = virusFile.Key;
                        string targetFilePath = Path.Combine(QuarantineFolderPath, Path.GetFileName(sourceFilePath));
                        try
                        {
                            int counter = 1;
                            string fileName = Path.GetFileNameWithoutExtension(sourceFilePath);
                            string fileExtension = Path.GetExtension(sourceFilePath);
                            while (File.Exists(targetFilePath))
                            {
                                targetFilePath = Path.Combine(QuarantineFolderPath, $"{fileName} ({counter}){fileExtension}"); counter++;
                            }
                            File.Move(sourceFilePath, targetFilePath);
                            ConsoleOutput += $"Virus Files: {sourceFilePath} has been successfully moved to the quarantine folder. The file is:{targetFilePath} now.\n";
                            VirusFiles.Remove(sourceFilePath);
                        }
                        catch (Exception ex)
                        {
                            ConsoleOutput += $"Error while quarantining virus file {sourceFilePath} : {ex.Message}.\n";
                            ImageAddress = "C:\\Users\\1\\source\\repos\\VirusDetectionTool\\VirusDetectionTool\\Images\\UnSafe.png";
                        }
                    }
                    if (!VirusFiles.Any())
                    {
                        ConsoleOutput += $"All virus files have been quarantined.\n";
                        ImageAddress = "C:\\Users\\1\\source\\repos\\VirusDetectionTool\\VirusDetectionTool\\Images\\Safe.png";
                    }
                }
                else
                {
                    ConsoleOutput += $"There are no virus files that need to be quarantined.\n";
                }
            });
        }
        public async void ClearVirus()
        {
            await Task.Run(() => 
            {
                if (!Directory.Exists(QuarantineFolderPath))
                {
                    ConsoleOutput += "The folder selected as quarantine does not exist.\n";
                    return;
                }
                List<string> files = Directory.GetFiles(QuarantineFolderPath).ToList();
                List<string> filesRemoved = new List<string>();
                foreach (string file in files)
                {
                    try
                    {
                        File.Delete(file);
                        ConsoleOutput += $"Deleted virus files: {file} \n";
                        filesRemoved.Add(file);
                    }
                    catch (Exception ex)
                    {
                        ConsoleOutput += $"Error while deleting virus file {file} ：{ex.Message}. \n";
                    }
                }
                if (filesRemoved.Count == files.Count)
                {
                    ConsoleOutput += $"All virus files in the quarantine folder have been cleared.\n";
                }
            });
            
        }
    }
}

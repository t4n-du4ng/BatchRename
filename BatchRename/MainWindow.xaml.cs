﻿using Contract;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Forms;
using System.Linq;
using System.Diagnostics;
using MessageBox = System.Windows.Forms.MessageBox;

namespace BatchRename
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BindingList<RunRule> _runRules = new BindingList<RunRule>();

        private BindingList<File> _files = new BindingList<File>();

        private BindingList<File> _folders = new BindingList<File>();

        private enum FileType
        {
            File,
            Folder
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void winMain_Loaded(object sender, RoutedEventArgs e)
        {
            RenameRuleParserFactory.Instance().Register();
            BaseWindowFactory.Instance().Register();
            var items = RenameRuleParserFactory.Instance().RuleParserPrototypes;

            foreach (var item in items)
            {
                var rule = item.Value;

                var button = new System.Windows.Controls.Button()
                {
                    Margin = new Thickness(0, 0, 5, 0),
                    Padding = new Thickness(5, 3, 5, 3),
                    BorderThickness = new Thickness(0),
                    Background = new SolidColorBrush(Colors.Transparent),
                    Content = rule.Title,
                    Tag = rule.Name
                };

                button.Click += btnAddRunRule_Click;
                wpRuleChooser.Children.Add(button);
            }

            lvRunRules.ItemsSource = _runRules;
            lvFiles.ItemsSource = _files;
            lvFolders.ItemsSource = _folders;
        }

        #region project hanler
        private void btnOpenProject_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSaveProject_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSaveAsProject_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCloseProject_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region start batch
        private void btnStartBatch_Click(object sender, RoutedEventArgs e)
        {
            int type = tcTargets.SelectedIndex;

            MessageBoxResult msg = System.Windows.MessageBox.Show(
                "Are you sure you want to make the renaming?",
                "Start renaming",
                MessageBoxButton.YesNo
            );

            if (msg == MessageBoxResult.Yes)
            {
                if (type == (int)FileType.File)
                {
                    Dictionary<string, int> duplications = new Dictionary<string, int>();

                    foreach (var file in _files)
                    {
                        string newIdealName = Path.Combine(Path.GetDirectoryName(file.Path)!, file.NewName);

                        try
                        {
                            System.IO.File.Move(
                                file.Path,
                                newIdealName
                            );
                        }
                        catch (Exception)
                        {
                            if (duplications.ContainsKey(newIdealName))
                            {
                                duplications[newIdealName]++;
                            }
                            else
                            {
                                duplications[newIdealName] = 1;
                            }

                            string newLessCollisionName = $"{Path.GetFileNameWithoutExtension(file.NewName)} ({duplications[newIdealName]}){Path.GetExtension(file.NewName)}";

                            System.IO.File.Move(
                                file.Path,
                                Path.Combine(Path.GetDirectoryName(file.Path)!, newLessCollisionName)
                            );
                        }
                    }

                    _files.Clear();

                    if (Title != "Batch rename")
                    {
                        //SaveProjectHandler();
                    }
                }
                else
                {
                    Dictionary<string, int> duplications = new Dictionary<string, int>();

                    foreach (var folder in _folders)
                    {
                        string newIdealName = Path.Combine(Path.GetDirectoryName(folder.Path)!, folder.NewName);

                        try
                        {
                            Directory.Move(folder.Path, newIdealName);
                        }
                        catch (Exception)
                        {
                            if (duplications.ContainsKey(newIdealName))
                            {
                                duplications[newIdealName]++;
                            }
                            else
                            {
                                duplications[newIdealName] = 1;
                            }

                            string newLessCollisionName = $"{Path.GetFileNameWithoutExtension(folder.NewName)} ({duplications[newIdealName]})";

                            Directory.Move(
                                folder.Path,
                                Path.Combine(Path.GetDirectoryName(folder.Path)!, newLessCollisionName)
                            );
                        }
                    }

                    _folders.Clear();
                    if (Title != "Batch rename")
                    {
                        //SaveProjectHandler();
                    }
                }
            }
        }

        private void btnStartBatchCopy_Click(object sender, RoutedEventArgs e)
        {
            int type = tcTargets.SelectedIndex;
            MessageBoxResult msg = System.Windows.MessageBox.Show(
                "Are you sure you want to make the renaming?",
                "Start renaming",
                MessageBoxButton.YesNo
            );

            if (msg == MessageBoxResult.Yes)
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string directory = folderBrowserDialog.SelectedPath;

                    if (type == (int)FileType.File)
                    {
                        Dictionary<string, int> duplicationsCopyfile = new Dictionary<string, int>();
                        
                        foreach (var file in _files)
                        {
                            string newPath = Path.Combine(directory, file.Name);

                            try
                            {
                                System.IO.File.Copy(
                                    file.Path,
                                    newPath
                                );
                            }
                            catch (Exception)
                            {
                                if (duplicationsCopyfile.ContainsKey(newPath))
                                {
                                    duplicationsCopyfile[newPath]++;
                                }
                                else
                                {
                                    duplicationsCopyfile[newPath] = 1;
                                }

                                string copyNameWithDuplicate = $"{Path.GetFileNameWithoutExtension(file.Name)} ({duplicationsCopyfile[newPath]}) {Path.GetExtension(file.Name)}";

                                newPath = Path.Combine(directory, copyNameWithDuplicate);
                                
                                System.IO.File.Copy(
                                    file.Path,
                                    newPath
                                );
                            }

                            string newIdealName = Path.Combine(directory, file.NewName);
                            Dictionary<string, int> duplicationsNewFileName = new Dictionary<string, int>();

                            try
                            {
                                System.IO.File.Move(
                                    newPath,
                                    newIdealName
                                );
                            }
                            catch (Exception)
                            {
                                if (duplicationsNewFileName.ContainsKey(newIdealName))
                                {
                                    duplicationsNewFileName[newIdealName]++;
                                }
                                else
                                {
                                    duplicationsNewFileName[newIdealName] = 1;
                                }

                                string newLessCollisionName = $"{Path.GetFileNameWithoutExtension(file.NewName)} ({duplicationsNewFileName[newIdealName]}){Path.GetExtension(file.NewName)}";

                                System.IO.File.Move(
                                    newPath,
                                    Path.Combine(Path.GetDirectoryName(newPath)!, newLessCollisionName)
                                );
                            }
                        }

                        _files.Clear();

                        if (Title != "Batch rename")
                        {
                            SaveProjectHandler();
                        }
                    }
                    else
                    {
                        Dictionary<string, int> duplications = new Dictionary<string, int>();

                        foreach (var folder in _folders)
                        {
                            string newPath = Path.Combine(directory, folder.Name);

                            try
                            {
                                Directory.CreateDirectory(newPath);
                            }
                            catch (Exception)
                            {
                                //duplcation folder
                            }

                            string newIdealName = Path.Combine(directory, folder.NewName);

                            try
                            {                                
                                Directory.Move(folder.Path, newIdealName);
                            }
                            catch (Exception)
                            {
                                if (duplications.ContainsKey(newIdealName))
                                {
                                    duplications[newIdealName]++;
                                }
                                else
                                {
                                    duplications[newIdealName] = 1;
                                }

                                string newLessCollisionName = $"{Path.GetFileNameWithoutExtension(folder.NewName)} ({duplications[newIdealName]})";

                                Directory.Move(
                                    folder.Path,
                                    Path.Combine(directory, newLessCollisionName)
                                );
                            }
                        }

                        _folders.Clear();
                        if (Title != "Batch rename")
                        {
                            SaveProjectHandler();
                        }
                    }
                }
            }
        }
        #endregion

        #region preset handler
        private void btnOpenPreset_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSavePreset_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region rule handler

        #region rule helper
        private string ImposeRule(string original)
        {
            string newName = original;

            foreach (var runRule in _runRules)
            {
                if (!string.IsNullOrEmpty(runRule.Command))
                {
                    IRenameRuleParser parser = RenameRuleParserFactory.Instance().GetRuleParser(runRule.Name);
                    IRenameRule rule = parser.Parse(runRule.Command);
                    newName = rule.Rename(newName);
                }
            }

            return newName;
        }

        private void EvokeToUpdateNewName()
        {
            foreach (var file in _files)
            {
                file.NewName = ImposeRule(file.Name);
            }

            foreach (var folder in _folders)
            {
                folder.NewName = ImposeRule(folder.Name);
            }
        }

        private void UpdateOrder()
        {
            for (int i = 0; i < _runRules.Count; i++)
                _runRules[i].Index = i;

            lvRunRules.ItemsSource = null;
            lvRunRules.ItemsSource = _runRules;
        }
        #endregion

        private void btnAddRunRule_Click(object sender, RoutedEventArgs e)
        {
            string selectedTagName = (string)((System.Windows.Controls.Button)sender).Tag;

            IRenameRuleParser parser = RenameRuleParserFactory.Instance().GetRuleParser(selectedTagName);

            _runRules.Add(new RunRule()
            {
                Index = _runRules.Count,
                Name = selectedTagName,
                Title = parser.Title,
                IsPlugAndPlay = parser.IsPlugAndPlay,
                Command = parser.IsPlugAndPlay ? selectedTagName : ""
            });

            EvokeToUpdateNewName();
        }

        private void btnRemoveRunRule_Click(object sender, RoutedEventArgs e)
        {
            if (lvRunRules.SelectedIndex != -1)
            {
                _runRules.RemoveAt(lvRunRules.SelectedIndex);

                UpdateOrder();

                EvokeToUpdateNewName();
            }
        }

        private void btnClearRunRule_Click(object sender, RoutedEventArgs e)
        {
            _runRules.Clear();

            EvokeToUpdateNewName();
        }

        private void btnEditRunRule_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnRemoveRunRuleItself_Click(object sender, RoutedEventArgs e)
        {
            var btnRemove = sender as System.Windows.Controls.Button;
            if(int.TryParse(btnRemove!.Tag.ToString(), out int tag))
            {
                _runRules.RemoveAt(tag);
            }

            UpdateOrder();

            EvokeToUpdateNewName();
        }
        #endregion

        #region file handler
        #region file helper
        private bool IsAdded(string path, int type)
        {
            BindingList<File> browser;
            browser = (type == (int)FileType.File) ? _files : _folders;
            foreach (var item in browser)
            {
                if (item.Path == path)
                {
                    return true;
                }
            }
            return false;
        }
        private string CreateWriterFromRunRules()
        {
            string writer = "Rules\n";

            for (int i = 0; i < _runRules.Count; i++)
            {
                if (!string.IsNullOrEmpty(_runRules[i].Command))
                {
                    writer += _runRules[i].Command + "\n";
                }
            }

            return writer;
        }

        private string CreateWriterFromTargets(int type)
        {
            BindingList<File> targets;
            string writer = "";

            if (type == (int)FileType.File)
            {
                writer = "Files\n";
                targets = _files;
            }
            else
            {
                writer = "Folders\n";
                targets = _folders;
            }

            for (int i = 0; i < targets.Count; i++)
            {
                if (!string.IsNullOrEmpty(targets[i].Path))
                {
                    writer += targets[i].Path + "\n";
                }
            }

            return writer;
        }

        private void SaveProjectFile(string projectName)
        {
            string writer = "";

            writer += CreateWriterFromRunRules();
            writer += CreateWriterFromTargets((int)FileType.File);
            writer += CreateWriterFromTargets((int)FileType.Folder);

            System.IO.File.WriteAllText(projectName, writer);
        }
        #endregion
        private void btnAddFiles_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                for (int i = 0; i < openFileDialog.FileNames.Length; i++)
                {
                    if (!IsAdded(openFileDialog.FileNames[i], (int)FileType.File))
                    {
                        _files.Add(new File()
                        {
                            Name = openFileDialog.SafeFileNames[i],
                            NewName = ImposeRule(openFileDialog.SafeFileNames[i]),
                            Path = openFileDialog.FileNames[i]
                        });
                    }
                }
            }
        }

        private void btnAddFilesInDirectory_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string directory = folderBrowserDialog.SelectedPath;

                var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    if (!IsAdded(file, (int)FileType.File))
                    {
                        _files.Add(new File()
                        {
                            Name = Path.GetFileName(file),
                            NewName = ImposeRule(Path.GetFileName(file)),
                            Path = file
                        });
                    }
                }
            }
        }
        private void btnRemoveFile_Click(object sender, RoutedEventArgs e)
        {
            if (lvFiles.SelectedIndex != -1)
            {
                _files.RemoveAt(lvFiles.SelectedIndex);
            }
        }

        private void btnClearFiles_Click(object sender, RoutedEventArgs e)
        {
            _files.Clear();
        }

        private void lvFiles_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);

                foreach (var file in files)
                {
                    if (!IsAdded(file, (int)FileType.File))
                    {
                        _files.Add(new File()
                        {
                            Name = Path.GetFileName(file),
                            NewName = ImposeRule(Path.GetFileName(file)),
                            Path = file
                        });
                    }
                }
            }
        }
        #endregion

        #region folder handler
        #region folder helper
        private void SaveProjectHandler()
        {
            string projectName = Title.Split(new string[] { "Batch rename - " }, StringSplitOptions.None)[1];

            SaveProjectFile(projectName);
        }

        #endregion
        private void btnAddFolders_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string directory = folderBrowserDialog.SelectedPath;

                if (!IsAdded(directory, (int)FileType.Folder))
                {
                    _folders.Add(new File()
                    {
                        Name = Path.GetFileName(directory),
                        NewName = ImposeRule(Path.GetFileName(directory)),
                        Path = directory
                    });
                }
            }
        }

        private void btnRemoveFolder_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnClearFolders_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lvFolders_Drop(object sender, System.Windows.DragEventArgs e)
        {

        }
        #endregion
    }
}

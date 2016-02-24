using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using InOutEmote;
using InOutTestLib;
using InOutTestLib.INs;
using InOutTestLib.INs.ThalamusINs;
using LogicWebLib;
using LogicWebLib.NodeTypes;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace LogicWeb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _data;
        private LogicFrame _logicFrame;
        private LogicWebLib.LogicWeb _logicWeb;

        public MainWindow()
        {
            InitializeComponent();

            _data = (MainWindowViewModel) this.DataContext;

            var initScript = new InitScriptEmoteEmpathic();
            initScript.Run(out _logicFrame, out _logicWeb);
            //InitializationScript.Run(out _logicFrame, out _logicWeb);
            _logicFrame.NewFrameEvent += _logicFrame_NewFrameEvent;
            _logicFrame.BehaviourExecutionCycleEndedEvent += _logicFrame_BehaviourExecutionCycleEndedEvent;

            UpdateVM();
        }

       

        

        private void LoadInOutLibrary(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException("Can't locate file: "+path);
            var temp = LogicFrame.LoadFromFile(path);
            if (temp == null)
            {
                MessageBox.Show("Can't properly load the InOut library.", "Can't load library", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            else
            {
                if (_logicFrame!=null) _logicFrame.Dispose();
                _logicFrame = temp;
                UpdateVM();
            }
        }

        private void UpdateVM()
        {
            _data.NodesWeb.Clear();
            foreach (var outNode in _logicFrame.Outputs)
            {
                _data.NodesWeb.Add(new NodeViewModel(outNode));
            }
        }


        void _logicFrame_BehaviourExecutionCycleEndedEvent()
        {
            UpdateQueuedBehaviour();
        }

        void _logicFrame_NewFrameEvent(List<InputNode> inputs)
        {
            UpdateQueuedBehaviour();
        }

        private void UpdateQueuedBehaviour()
        {
            Dispatcher.Invoke(() =>
            {
                if (_logicFrame == null) return; // avoids exceptions when this code is executed when loading a library

                //_data.QueuedBehaviour.Clear();
                try
                {
                    foreach (var frame in _logicFrame.Queue)
                    {
                        foreach (var behaviour in frame.Value)
                        {
                            if (!_data.QueuedBehaviour.Any(x => x.BehaviourId == behaviour.Id))
                            {
                                _data.QueuedBehaviour.Insert(0,new QueuedBehaviourVM(behaviour, frame.Key));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception!!! " + ex.Message);
                }
            });
        }

       
        private void LoadInOutLib_Click(object sender, RoutedEventArgs e)
        {
            var openFIle = new CommonOpenFileDialog();
            openFIle.Title = "Select an InOut library for LogicWeb";
            openFIle.Filters.Add(new CommonFileDialogFilter("InOut Library","*.dll"));
            var res = openFIle.ShowDialog();
            if (res == CommonFileDialogResult.Ok)
            {
                LoadInOutLibrary(openFIle.FileName);
            }
        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            _logicFrame.Dispose();
        }

        private void SaveThisWebMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save logic web";
            saveFileDialog.FileName = "myLogicWeb.lweb";
            saveFileDialog.DefaultExt = ".lweb";
            if(saveFileDialog.ShowDialog() == true)
            {
                _logicWeb.Save(saveFileDialog.FileName);
            }
        }

        private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var node = (NodeViewModel) TreeView.SelectedItem;
            node.LogicNode.Active = !node.LogicNode.Active;
        }
    }
}

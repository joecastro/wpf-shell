/**************************************************************************\
    Copyright Microsoft Corporation. All Rights Reserved.
\**************************************************************************/

namespace TaskbarSample
{
    using System;
    using System.Windows;
    using Microsoft.Windows.Shell;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private bool DeferApply
        {
            get
            {
                return _deferApply.IsChecked ?? false;
            }
        }

        private void ThumbButtonInfo_Click(object sender, EventArgs e)
        {
        }

        private void JumpTask_AddToRecent(object sender, RoutedEventArgs e)
        {
            JumpTask jt = GenerateJumpTask();
            JumpList.AddToRecentCategory(jt);
        }

        private void JumpTask_AddToCategory(object sender, RoutedEventArgs e)
        {
            JumpTask jt = GenerateJumpTask();
            var jl = JumpList.GetJumpList(Application.Current);
            jl.JumpItems.Add(jt);
            if (!DeferApply)
            {
                jl.Apply();
            }
        }

        private void JumpPath_AddToRecent(object sender, RoutedEventArgs e)
        {
            JumpPath jp = GenerateJumpPath();
            JumpList.AddToRecentCategory(jp);
        }

        private void JumpPath_AddToCategory(object sender, RoutedEventArgs e)
        {
            JumpPath jp = GenerateJumpPath();
            var jl = JumpList.GetJumpList(Application.Current);
            jl.JumpItems.Add(jp);
            if (!DeferApply)
            {
                jl.Apply();
            }
        }

        private void JumpList_Clear(object sender, RoutedEventArgs e)
        {
            var jl = JumpList.GetJumpList(Application.Current);
            jl.JumpItems.Clear();
            if (!DeferApply)
            {
                jl.Apply();
            }
        }

        private void JumpList_Apply(object sender, RoutedEventArgs e)
        {
            var jl = JumpList.GetJumpList(Application.Current);
            jl.Apply();
        }

        private void UpdateKnownCategories(object sender, RoutedEventArgs e)
        {
            bool showRecent = _showRecentButton.IsChecked ?? false;
            bool showFrequent = _showFrequentButton.IsChecked ?? false;
            var jl = JumpList.GetJumpList(Application.Current);
            jl.ShowRecentCategory = showRecent;
            jl.ShowFrequentCategory = showFrequent;

            if (!DeferApply)
            {
                jl.Apply();
            }
        }

        private JumpPath GenerateJumpPath()
        {
            return new JumpPath
            {
                Path = _jumpPathPathBox.Text,
                CustomCategory = _jumpPathCategoryBox.Text
            };
        }

        private JumpTask GenerateJumpTask()
        {
            var jt = new JumpTask
            {
                ApplicationPath = _jumpTaskAppPathBox.Text,
                IconResourcePath = _jumpTaskIconResourcePathBox.Text,
                Arguments = _jumpTaskArgsBox.Text,
                Title = _jumpTaskTitleBox.Text,
                Description = _jumpTaskDescriptionBox.Text,
                WorkingDirectory = _jumpTaskWorkingDirBox.Text,
                CustomCategory = _jumpTaskCategoryBox.Text,
            };

            int i;
            if (int.TryParse(_jumpTaskIconResourceIndexBox.Text, out i))
            {
                jt.IconResourceIndex = i;
            }

            return jt;
        }

        private void ClearJumpTaskInput()
        {
            _jumpTaskAppPathBox.Text = "";
            _jumpTaskIconResourcePathBox.Text = "";
            _jumpTaskArgsBox.Text = "";
            _jumpTaskTitleBox.Text = "";
            _jumpTaskDescriptionBox.Text = "";
            _jumpTaskWorkingDirBox.Text = "";
            _jumpTaskCategoryBox.Text = "";
        }

        private void JumpTask_ClearInput(object sender, RoutedEventArgs e)
        {
            ClearJumpTaskInput();
        }
    }
}

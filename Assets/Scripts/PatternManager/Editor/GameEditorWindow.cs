﻿using PatternManager.Editor.Common;
using UnityEditor;

namespace PatternManager.Editor
{
    public class GameEditorWindow : EditorWindowSingleton<GameEditorWindow>
    {
        protected override string customTitle => "Game editor";

        public SplitLine m_splitLine;
        public LeftSidebar m_leftSidebar;
        public GridDrawer m_gridDrawer;
        public TopBar m_topBar;
        public TopBarSplitLine m_topBarSplitLine;
        
        [MenuItem("Tools/Editor")]
        private static void ShowWindow()
        {
            var window = instance;
        }

        private void OnEnable()
        {
            m_topBarSplitLine = new TopBarSplitLine();
            m_splitLine = new SplitLine();
            
            m_gridDrawer = new GridDrawer();
            m_leftSidebar = new LeftSidebar();
            m_topBar = new TopBar();
        }

        private void OnGUI()
        {
            m_splitLine.Draw();
            m_gridDrawer.Draw();
            m_leftSidebar.Draw();
            m_topBar.Draw();
            m_topBarSplitLine.Draw();
        }
    }
}
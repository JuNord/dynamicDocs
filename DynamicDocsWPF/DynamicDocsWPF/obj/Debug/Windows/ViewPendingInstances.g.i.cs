﻿#pragma checksum "..\..\..\Windows\ViewPendingInstances.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "64333B73F2D0D77921C8BE9EAA76BCE306527502"
//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

using DynamicDocsWPF.Windows;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace DynamicDocsWPF.Windows {
    
    
    /// <summary>
    /// ViewPendingInstances
    /// </summary>
    public partial class ViewPendingInstances : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 14 "..\..\..\Windows\ViewPendingInstances.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView InstanceList;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\Windows\ViewPendingInstances.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock InfoBlock;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\Windows\ViewPendingInstances.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ContentPresenter ViewHolder;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\Windows\ViewPendingInstances.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnNext;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/DynamicDocsWPF;component/windows/viewpendinginstances.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Windows\ViewPendingInstances.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.InstanceList = ((System.Windows.Controls.ListView)(target));
            
            #line 14 "..\..\..\Windows\ViewPendingInstances.xaml"
            this.InstanceList.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.InstanceList_OnSelectionChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.InfoBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.ViewHolder = ((System.Windows.Controls.ContentPresenter)(target));
            return;
            case 4:
            
            #line 34 "..\..\..\Windows\ViewPendingInstances.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ViewAllInstances_Btn_Back_OnClick);
            
            #line default
            #line hidden
            return;
            case 5:
            this.BtnNext = ((System.Windows.Controls.Button)(target));
            
            #line 40 "..\..\..\Windows\ViewPendingInstances.xaml"
            this.BtnNext.Click += new System.Windows.RoutedEventHandler(this.ViewAllInstances_Btn_Next_OnClick);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}


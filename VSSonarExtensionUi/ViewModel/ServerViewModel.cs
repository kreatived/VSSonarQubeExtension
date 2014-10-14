﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="serverviewmodel.cs" company="Copyright © 2014 Tekla Corporation. Tekla is a Trimble Company">
//     Copyright (C) 2013 [Jorge Costa, Jorge.Costa@tekla.com]
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License
// as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
// You should have received a copy of the GNU Lesser General Public License along with this program; if not, write to the Free
// Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSSonarExtensionUi.ViewModel
{
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using PropertyChanged;
    using VSSonarExtensionUi.Cache;

    [ImplementPropertyChanged]
    public class ServerViewModel : IViewModelBase
    {
        private readonly SonarQubeViewModel sonarQubeViewModel;
        private readonly ModelEditorCache localEditorCache = new ModelEditorCache();

        public ServerViewModel()
        {
            this.IssuesGridView = new IssueGridViewModel();

            this.InitCommanding();
        }

        private void InitCommanding()
        {
            this.EnableCoverageInEditorCommand = new RelayCommand(this.OnEnableCoverageInEditorCommand, () => this.IsRunningInVisualStudio);
            this.DisplaySourceDiffCommand = new RelayCommand(this.OnEnableCoverageInEditorCommand, () => this.IsRunningInVisualStudio);
        }

        private void OnEnableCoverageInEditorCommand()
        {
            
        }

        public ServerViewModel(SonarQubeViewModel sonarQubeViewModel)
        {
            this.sonarQubeViewModel = sonarQubeViewModel;
            this.Header = "Server Analysis";
            this.IssuesGridView = new IssueGridViewModel(sonarQubeViewModel, this, true);
            this.InitCommanding();
        }

        public string Header { get; set; }
        public IssueGridViewModel IssuesGridView { get; private set; }
        public RelayCommand EnableCoverageInEditorCommand { get; private set; }
        public bool IsRunningInVisualStudio { get; private set; }
        public RelayCommand DisplaySourceDiffCommand { get; private set; }
        public bool CoverageInEditorEnabled { get; set; }

        public Dictionary<int, CoverageElement> GetCoverageInEditor(string v)
        {
            throw new NotImplementedException();
        }
    }
}
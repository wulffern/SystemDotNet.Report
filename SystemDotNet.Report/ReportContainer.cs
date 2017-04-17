﻿/*
Copyright (C) 2005 Carsten Wulff

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the

GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/
#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using SystemDotNet.PostProcessing;

#endregion

namespace SystemDotNet.Reporter
{
    public class ReportContainer
    {
        static ReportContainer repc;

        public Report report;

        private ReportContainer(Report report)
        {
            this.report = report;
        }

        public static void NewReport(Report report)
        {
            repc = new ReportContainer(report);
        }

        public static ReportContainer GetInstance()
        {
            if (repc == null)
                throw new CommandException("No report has been loaded");

            return repc;
        }
    }
}
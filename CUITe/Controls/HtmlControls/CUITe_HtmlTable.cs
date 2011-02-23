﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using mshtml;

namespace CUITe.Controls.HtmlControls
{
    public enum CUITe_HtmlTableSearchOptions
    {
        Normal,
        NormalTight,
        Greedy,
        StartsWith,
        EndsWith
    }

    public class CUITe_HtmlTable : CUITe_ControlBase
    {
        private HtmlTable _htmlTable;

        public CUITe_HtmlTable(string sSearchParameters) : base(sSearchParameters) { }

        public void Wrap(HtmlTable control)
        {
            base.Wrap(control);
            this._htmlTable = control;
        }

        public HtmlTable UnWrap()
        {
            return this._htmlTable;
        }

        public int RowCount
        {
            get {
                this._htmlTable.WaitForControlReady();
                return this._htmlTable.Rows.Count; 
            }
        }

        public void FindRowAndClick(int iCol, string sValueToSearch)
        {
            int iRow = FindRow(iCol, sValueToSearch, CUITe_HtmlTableSearchOptions.Normal);
            Mouse.Click(this.GetCell(iRow, iCol));
        }

        public void FindRowAndClick(int iCol, string sValueToSearch, CUITe_HtmlTableSearchOptions option)
        {
            int iRow = FindRow(iCol, sValueToSearch, option);
            Mouse.Click(this.GetCell(iRow, iCol));
        }

        public void FindRowAndDoubleClick(int iCol, string sValueToSearch)
        {
            int iRow = FindRow(iCol, sValueToSearch, CUITe_HtmlTableSearchOptions.Normal);
            Mouse.DoubleClick(this.GetCell(iRow, iCol));
        }

        public void FindRowAndDoubleClick(int iCol, string sValueToSearch, CUITe_HtmlTableSearchOptions option)
        {
            int iRow = FindRow(iCol, sValueToSearch, option);
            Mouse.DoubleClick(this.GetCell(iRow, iCol));
        }

        public void FindCellAndClick(int iRow, int iCol)
        {
            Mouse.Click(this.GetCell(iRow, iCol));
        }

        public void FindCellAndDoubleClick(int iRow, int iCol)
        {
            Mouse.DoubleClick(this.GetCell(iRow, iCol));
        }

        public int FindRow(int iCol, string sValueToSearch, CUITe_HtmlTableSearchOptions option)
        {
            this._htmlTable.WaitForControlReady();
            int iRow = -1;
            int rowCount = -1;
            foreach (HtmlRow cont in this._htmlTable.Rows)
            {
                rowCount++;
                int colCount = -1;
                foreach (HtmlCell cell in cont.Cells)
                {
                    colCount++;
                    bool bSearchOptionResult = false;
                    if (colCount == iCol)
                    {
                        if (option == CUITe_HtmlTableSearchOptions.Normal)
                        {
                            bSearchOptionResult = (sValueToSearch == cell.InnerText);
                        }
                        else if (option == CUITe_HtmlTableSearchOptions.NormalTight)
                        {
                            bSearchOptionResult = (sValueToSearch == cell.InnerText.Trim());
                        }
                        else if (option == CUITe_HtmlTableSearchOptions.StartsWith)
                        {
                            bSearchOptionResult = cell.InnerText.StartsWith(sValueToSearch);
                        }
                        else if (option == CUITe_HtmlTableSearchOptions.EndsWith)
                        {
                            bSearchOptionResult = cell.InnerText.EndsWith(sValueToSearch);
                        }
                        else if (option == CUITe_HtmlTableSearchOptions.Greedy)
                        {
                            bSearchOptionResult = (cell.InnerText.IndexOf(sValueToSearch) > -1);
                        }
                        if (bSearchOptionResult == true)
                        {
                            iRow = rowCount;
                            break;
                        }
                    }
                }
                if (iRow > -1) break;
            }
            return iRow;
        }

        public string GetCellValue(int iRow, int iCol)
        {
            string sResult = "";
            HtmlCell _htmlCell = this.GetCell(iRow, iCol);
            if (_htmlCell != null) sResult = _htmlCell.InnerText;
            return sResult;
        }

        private HtmlCell GetCell(int iRow, int iCol)
        {
            this._htmlTable.WaitForControlReady();
            HtmlCell _htmlCell = null;
            int rowCount = -1;
            foreach (HtmlRow cont in this._htmlTable.Rows)
            {
                rowCount++;
                if (rowCount == iRow)
                {
                    int colCount = -1;
                    foreach (HtmlCell cell in cont.Cells)
                    {
                        colCount++;
                        if (colCount == iCol)
                        {
                            _htmlCell = cell;
                            break;
                        }
                    }
                }
                if (_htmlCell != null)
                {
                    break;
                }
            }
            return _htmlCell;
        }

        private mshtml.IHTMLElement GetEmbeddedCheckBoxNativeElement(mshtml.IHTMLElement parent)
        {
            while (true)
            {
                foreach (mshtml.IHTMLElement ele2 in parent.children)
                {
                    if (ele2.tagName.ToUpper() == "INPUT")
                    {
                        string sType = ele2.getAttribute("type");
                        if (sType.ToLower() == "checkbox")
                        {
                            return ele2;
                        }
                    }
                    else
                    {
                        if (ele2.children != null)
                        {
                            parent = ele2;
                        }
                    }
                }
            }
            return null;
        }

        public CUITe_HtmlCheckBox GetEmbeddedCheckBox(int iRow, int iCol)
        {
            string sSearchProperties = "";
            mshtml.IHTMLElement td = (mshtml.IHTMLElement)GetCell(iRow, iCol).NativeElement;
            mshtml.IHTMLElement check = GetEmbeddedCheckBoxNativeElement(td);
            string sOuterHTML = check.outerHTML.Replace("<", "").Replace(">", "").Trim();
            string[] saTemp = sOuterHTML.Split(' ');
            HtmlCheckBox chk = new HtmlCheckBox(this._htmlTable.Container);
            foreach (string sTemp in saTemp)
            {
                if (sTemp.IndexOf('=') > 0)
                {
                    string[] saKeyValue = sTemp.Split('=');
                    string sValue = saKeyValue[1];
                    if (saKeyValue[0].ToLower() == "name")
                    {
                        sSearchProperties += ";Name=" + sValue;
                        chk.SearchProperties.Add(HtmlControl.PropertyNames.Name, sValue);
                    }
                    if (saKeyValue[0].ToLower() == "id")
                    {
                        sSearchProperties += ";Id=" + sValue;
                        chk.SearchProperties.Add(HtmlControl.PropertyNames.Id, sValue);
                    }
                    if (saKeyValue[0].ToLower() == "class")
                    {
                        sSearchProperties += ";Class=" + sValue;
                        chk.SearchProperties.Add(HtmlControl.PropertyNames.Class, sValue);
                    }
                }
            }

            if (sSearchProperties.Length > 1)
            {
                sSearchProperties = sSearchProperties.Substring(1);
            }
            CUITe_HtmlCheckBox retChk = new CUITe_HtmlCheckBox(sSearchProperties);
            retChk.Wrap(chk);
            return retChk;
        }
    }
}
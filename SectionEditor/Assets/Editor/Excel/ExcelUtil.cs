using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;
using OfficeOpenXml;

namespace Excelutil{

	public class ExcelUtilMain {
		static ExcelUtilMain _instance = new ExcelUtilMain();
		public static ExcelUtilMain Instance{
			get{
				return _instance;
			}
		}

		class ExcelData{
			public Dictionary<string, List<string>> _sheetDic = new Dictionary<string, List<string>>();
		}

		public List<T> Parse<T> (string path) where T : new() {
			var type = typeof(T);
			var result = new List<T> ();
			using (FileStream fs = File.Open (path, FileMode.Open)) {
				using (var package = new ExcelPackage (fs)) {
					var book = package.Workbook;
					var sheets = book.Worksheets;
					foreach (var sheet in sheets) {
						int maxColumn = sheet.Dimension.End.Column;
						int maxRow = sheet.Dimension.End.Row;
						var fields = type.GetFields ();
						Dictionary<string, FieldInfo> fieldDic = new Dictionary<string, FieldInfo> ();
						foreach (var field in fields) {
							fieldDic.Add (field.Name, field);
						}
						List<string> colNameList = new List<string> ();
						colNameList.Add ("");
						for (int col = 1; col <= maxColumn; col++) {
							var key = Convert.ToString (sheet.Cells [2, col].Value);
							if (string.IsNullOrEmpty(key)) {
								break;									
							}
							colNameList.Add(key);
						}
						for (int row = 1; row <= maxRow; row++) {
							var item = new T ();
							for (int col = 1; col < colNameList.Count; col++) {
								if(fieldDic.ContainsKey(colNameList[col])){
									fieldDic [colNameList [col]].SetValue (item, Convert.ToString(sheet.Cells [row, col].Value));
								}
							}
							result.Add (item);
						}
					}
				}
			}
			return result;
		}

		public void Create<T>(string path, List<T> list){
			var type = typeof(T);
			using (FileStream fs = File.Open (path, FileMode.Create)) {
				using (var package = new ExcelPackage (fs)) {
					var book = package.Workbook;
					var sheets = book.Worksheets;
					var sheet = sheets.Add (ToolKit.GetName(path));
					sheet.InsertRow (1, list.Count + 1);
					var fields = type.GetFields ();
//					for (int fi = 0; fi < fields.Length; fi++) {
//						sheet.Cells [1, fi + 1].Value = fields[fi].Name;
//					}
					for(int index = 0; index < list.Count; index++){
						for (int fi = 0; fi < fields.Length; fi++) {
							var item = fields [fi].GetValue (list [index]);
							sheet.Cells [index + 1, fi + 1].Value = Convert.ToString (item);
						}
					}
					package.Save ();
				}
			}
		}
	}
}

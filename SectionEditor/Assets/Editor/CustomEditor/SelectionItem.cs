using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CustomEditor{
	public class SelectionItem{
		int curIndex;
		int lastIndex = -1;
		List<string> selectionList = new List<string>();

		public int CurIndex{
			set{
				lastIndex = curIndex;
				curIndex = value;
			}
			get{ 
				return curIndex;
			}
		}

		public bool Changed{
			get{
				return curIndex != lastIndex;
			}
		}

		public List<string> SelectionList {
			get{
				return  selectionList;
			}
		}

		public void Reset(){
			curIndex = lastIndex = 0;
			selectionList.Clear ();
		}
	}
}
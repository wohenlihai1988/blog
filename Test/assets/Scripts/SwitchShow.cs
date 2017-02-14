using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchShow : MonoBehaviour {
	public List<GameObject> _resList = new List<GameObject>();
	int _index;
	public int _curIndex;

	void Update(){
		if (_index == _curIndex) {
			return;
		}
		for (int i = 0; i < _resList.Count; i++) {
			_resList [i].SetActive (false);
		}
		_resList [_curIndex].SetActive (true);
		_index = _curIndex;
	}
}

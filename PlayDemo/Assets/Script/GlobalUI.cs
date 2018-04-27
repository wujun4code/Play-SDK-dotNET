using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LeanCloud;

public class GlobalUI : MonoBehaviour {
	public GameObject loadingGo = null;

	private static GlobalUI instance = null;

	public static GlobalUI Instantce {
		get {
			return instance;
		}
	}

	void Awake()
	{
		DontDestroyOnLoad(this.gameObject);
		instance = this;
        Play.ToggleLog(true);
	}

	// 屏蔽用户操作
	public void ShowLoading() {
		this.loadingGo.SetActive(true);
	}

	public void HideLoading() {
		this.loadingGo.SetActive(false);
	}
}
